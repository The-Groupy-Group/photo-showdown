// src/services/usersService.ts
import httpClient from './api';

const login = async (data: any) => {
    return await httpClient.post('/Users/Login', data);
};

const register = async (data: any) => {
    return await httpClient.post('/Users/Register', data);
};

export default {
    login,
    register
};