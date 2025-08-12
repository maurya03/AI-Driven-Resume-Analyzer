# Resume AI Web App

## 🚀 One-Click Deploy

### Backend (Render)
[![Deploy to Render](https://render.com/images/deploy-to-render-button.svg)](https://render.com/deploy)

1. Click the button above to deploy backend & database to Render.
2. Once deployed, copy the backend URL.

### Frontend (Netlify)
[![Deploy to Netlify](https://www.netlify.com/img/deploy/button.svg)](https://app.netlify.com/start)

1. Click the button above.
2. Set `VITE_API_BASE_URL` in Netlify environment variables to your backend Render URL.
3. Deploy site.

---

## Local Development

Linux/macOS/WSL:
```bash
export OPENAI_API_KEY="sk-xxx"
./run-local.sh
```

Windows PowerShell:
```powershell
$env:OPENAI_API_KEY = "sk-xxx"
.un-local.ps1
```

Visit `http://localhost:3000` after running.
