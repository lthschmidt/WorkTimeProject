import axios from 'axios';

const api = axios.create({
    baseURL: process.env.VUE_APP_API_URL, // Vue CLI использует process.env
    headers: { 'Content-Type': 'application/json' }
});

export default api;
