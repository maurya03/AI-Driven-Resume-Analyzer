#!/usr/bin/env bash
set -e

OPENAI_KEY="${OPENAI_API_KEY:-}"
if [ -z "$OPENAI_KEY" ]; then
  echo "❌ Please set OPENAI_API_KEY in your environment before running."
  exit 1
fi

echo "🔄 Ensuring docker-compose uses pgvector image..."
sed -i.bak 's|image: postgres:15|image: ankane/pgvector:pg15|' docker-compose.yml

echo "📦 Building frontend..."
cd client
npm install
npm run build
cd ..

echo "🐘 Starting Postgres..."
docker-compose up -d postgres

echo "⏳ Waiting for Postgres to be ready..."
until docker exec $(docker ps --filter "ancestor=ankane/pgvector:pg15" --format "{{.Names}}" | head -n1)   pg_isready -U resumeuser -d resumedb > /dev/null 2>&1; do
  sleep 2
done

echo "🗄 Creating candidates table..."
docker exec -i $(docker ps --filter "ancestor=ankane/pgvector:pg15" --format "{{.Names}}" | head -n1)   psql -U resumeuser -d resumedb -c "CREATE TABLE IF NOT EXISTS candidates (id serial PRIMARY KEY, candidate_name text NOT NULL, email text NOT NULL, experience_years numeric, resume_file_path text, raw_text text, ai_summary text, ai_skills text, ai_score double precision);"

echo "🚀 Starting backend + nginx..."
OPENAI_API_KEY="$OPENAI_KEY" docker-compose up --build server nginx
