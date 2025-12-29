// src/config.ts


export const IP_ADDRESS = process.env.EXPO_PUBLIC_IP_ADDRESS || '10.0.0.2';
export const PORT = process.env.EXPO_PUBLIC_PORT || '8080';

export const API_BASE_URL = `http://${IP_ADDRESS}:${PORT}/api`;