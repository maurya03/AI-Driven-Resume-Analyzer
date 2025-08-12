#!/usr/bin/env bash
set -e

if [ -z "$1" ]; then
  echo "Usage: $0 <backend_api_url>"
  exit 1
fi

API_URL="$1"

echo "🔧 Setting VITE_API_BASE_URL to $API_URL in netlify.toml"
sed -i.bak "s|VITE_API_BASE_URL = .*|VITE_API_BASE_URL = \"$API_URL\"|g" netlify.toml

echo "✅ Updated netlify.toml"
echo "📤 Committing changes to Git (if repo)"
if [ -d .git ]; then
  git add netlify.toml
  git commit -m "Set VITE_API_BASE_URL to $API_URL"
  echo "Now push to your GitHub repo so Netlify will redeploy."
else
  echo "⚠️ Not a git repo, skipping commit."
fi
