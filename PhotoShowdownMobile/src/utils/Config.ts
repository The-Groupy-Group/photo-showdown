// src/utils/Config.ts

const NGROK_URL = "https://transcendentalistic-ardith-couth.ngrok-free.dev"; 

export const API_BASE = `${NGROK_URL}/api`;

export const WS_URL = `${NGROK_URL.replace('https', 'wss')}/api/ws`;

export const IMAGE_BASE = NGROK_URL;