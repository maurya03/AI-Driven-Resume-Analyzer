Resume AI - Complete Starter (pgvector + Postgres + .NET 8 + Vite React)
---------------------------------------------------------------------
What you need to run:
- Docker & Docker Compose
- An OpenAI API key (set OPENAI_API_KEY environment variable)

Quick local run (Linux/Mac/WSL):
1. Build frontend static files:
   cd client && npm install && npm run build
2. From repo root, run:
   OPENAI_API_KEY=your_key_here docker-compose up --build

The frontend will be served at http://localhost:3000 and backend at http://localhost:5000/api
Notes:
- This template stores resume text and AI fields in Postgres candidates table, and embeddings in resume_embeddings (pgvector).
- For production: secure secrets, validate uploads, handle large files, use streaming and background jobs for AI calls.
