import axios from 'axios';
const api = axios.create({ baseURL: import.meta.env.VITE_API_BASE_URL || 'https://ai-driven-resume-analyzer-1.onrender.com' });
export default api;
