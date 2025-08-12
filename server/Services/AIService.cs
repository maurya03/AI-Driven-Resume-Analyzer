using System.Net.Http.Headers;
using System.Text.Json;
namespace ResumeAI.Services;
public class AIService
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpFactory;
    public AIService(IConfiguration config, IHttpClientFactory httpFactory) { _config = config; _httpFactory = httpFactory; }

    public async Task<string> GetEmbeddingAsync(string input)
    {
        var apiKey = _config["OPENAI_API_KEY"];
        if (string.IsNullOrEmpty(apiKey)) throw new Exception("OPENAI_API_KEY not set");
        var client = _httpFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        var req = new { input = input, model = "text-embedding-3-small" };
        var resp = await client.PostAsJsonAsync("https://api.openai.com/v1/embeddings", req);
        resp.EnsureSuccessStatusCode();
        using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
        var arr = doc.RootElement.GetProperty("data")[0].GetProperty("embedding").EnumerateArray().Select(x=>x.GetDouble()).ToArray();
        // return JSON array string for storing easily
        return JsonSerializer.Serialize(arr);
    }

    public async Task<string> GetCompletionAsync(string prompt)
    {
        var apiKey = _config["OPENAI_API_KEY"];
        if (string.IsNullOrEmpty(apiKey)) throw new Exception("OPENAI_API_KEY not set");
        var client = _httpFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        var payload = new { model = "gpt-4o-mini", messages = new[] { new { role = "user", content = prompt } }, temperature = 0.2 };
        var resp = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", payload);
        resp.EnsureSuccessStatusCode();
        using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
        var txt = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        return txt ?? string.Empty;
    }
}
