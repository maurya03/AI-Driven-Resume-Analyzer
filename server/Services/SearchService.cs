using Dapper;
using Npgsql;
using System.Text.Json;
namespace ResumeAI.Services;
public class SearchService
{
    private readonly IConfiguration _config;
    public SearchService(IConfiguration config) { _config = config; }

    private NpgsqlConnection GetConn() => new NpgsqlConnection(_config.GetConnectionString("Postgres"));

    public async Task UpsertEmbeddingAsync(int candidateId, string embJson)
    {
        // embJson is JSON array string of doubles
        var vals = JsonSerializer.Deserialize<double[]>(embJson)!;
        using var conn = GetConn();
        await conn.OpenAsync();
        // ensure extension and table (id, candidate_id int, embedding vector)
        var ensure = @"CREATE EXTENSION IF NOT EXISTS vector; CREATE TABLE IF NOT EXISTS resume_embeddings (id serial PRIMARY KEY, candidate_id int UNIQUE, embedding vector);";
        await conn.ExecuteAsync(ensure);
        // upsert vector
        var upsert = @"INSERT INTO resume_embeddings (candidate_id, embedding) VALUES (@cid, @vec) ON CONFLICT (candidate_id) DO UPDATE SET embedding = EXCLUDED.embedding;";
        // Npgsql supports parameter mapping for vector via byte[]? We'll send as SQL literal for demo - WARNING: not safe for production.
        var vecSql = "ARRAY[" + string.Join(',', vals.Select(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture))) + "]::vector";
        var sql = $"INSERT INTO resume_embeddings (candidate_id, embedding) VALUES ({candidateId}, {vecSql}) ON CONFLICT (candidate_id) DO UPDATE SET embedding = EXCLUDED.embedding;";
        await conn.ExecuteAsync(sql);
    }

    public async Task<List<object>> SemanticSearchAsync(string query, int topK = 10)
    {
        // generate embedding for query via OpenAI
        var aiKey = _config["OPENAI_API_KEY"];
        if (string.IsNullOrEmpty(aiKey)) throw new Exception("OPENAI_API_KEY not set");
        var client = new HttpClient(); client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", aiKey);
        var payload = new { input = query, model = "text-embedding-3-small" };
        var resp = await client.PostAsJsonAsync("https://api.openai.com/v1/embeddings", payload);
        resp.EnsureSuccessStatusCode();
        using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
        var qvec = doc.RootElement.GetProperty("data")[0].GetProperty("embedding").EnumerateArray().Select(x=>x.GetDouble()).ToArray();

        using var conn = GetConn();
        await conn.OpenAsync();
        // ensure resume_embeddings exists
        var ensure = @"CREATE EXTENSION IF NOT EXISTS vector; CREATE TABLE IF NOT EXISTS resume_embeddings (id serial PRIMARY KEY, candidate_id int UNIQUE, embedding vector);";
        await conn.ExecuteAsync(ensure);
        // Build vector literal
        var vecSql = "ARRAY[" + string.Join(',', qvec.Select(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture))) + "]::vector";
        var sql = $@"SELECT re.candidate_id, 1 - (re.embedding <=> {vecSql}) as similarity FROM resume_embeddings re ORDER BY re.embedding <=> {vecSql} LIMIT {topK};";
        var rows = await conn.QueryAsync(sql);
        var ids = rows.Select(r => (int)r.candidate_id).ToArray();
        if (!ids.Any()) return new List<object>();
        // fetch candidates via separate connection using EF is complex; we'll use raw Npgsql
        var idList = string.Join(',', ids);
        var sql2 = $"SELECT id, candidate_name, email, experience_years, resume_file_path, raw_text, ai_summary, ai_skills, ai_score FROM candidates WHERE id IN ({idList});";
        var candRows = await conn.QueryAsync(sql2);
        // order by ids sequence
        var ordered = ids.Select(id => candRows.First(r => (int)r.id == id)).ToList<object>();
        return ordered;
    }
}
