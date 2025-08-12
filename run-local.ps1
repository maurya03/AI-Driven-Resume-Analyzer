if (-not $env:OPENAI_API_KEY) {
    Write-Error "Please set OPENAI_API_KEY before running."
    exit 1
}

Write-Host "🔄 Ensuring docker-compose uses pgvector image..."
(Get-Content docker-compose.yml) -replace 'image:\s*postgres:15', 'image: ankane/pgvector:pg15' | Set-Content docker-compose.yml

Write-Host "📦 Building frontend..."
Set-Location client
npm install
npm run build
Set-Location ..

Write-Host "🐘 Starting Postgres..."
docker-compose up -d postgres

Write-Host "⏳ Waiting for Postgres to be ready..."
do {
    Start-Sleep -Seconds 2
    $ready = docker exec $(docker ps --filter "ancestor=ankane/pgvector:pg15" --format "{{.Names}}" | Select-Object -First 1) pg_isready -U resumeuser -d resumedb
} until ($ready -match "accepting connections")

Write-Host "🗄 Creating candidates table..."
docker exec -i $(docker ps --filter "ancestor=ankane/pgvector:pg15" --format "{{.Names}}" | Select-Object -First 1) `
    psql -U resumeuser -d resumedb -c "CREATE TABLE IF NOT EXISTS candidates (id serial PRIMARY KEY, candidate_name text NOT NULL, email text NOT NULL, experience_years numeric, resume_file_path text, raw_text text, ai_summary text, ai_skills text, ai_score double precision);"

Write-Host "🚀 Starting backend + nginx..."
docker-compose up --build server nginx
