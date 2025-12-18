// src/services/matchesService.ts
import httpClient from './api';

const getAuthHeader = (token: string) => ({
    headers: { Authorization: `Bearer ${token}` }
});

const getCurrentMatch = async (token: string) => {
    return await httpClient.get('/Matches/GetCurrentMatch', getAuthHeader(token));
};

const getMatchById = async (matchId: number, token: string) => {
    return await httpClient.get(`/Matches/GetMatchById/${matchId}`, getAuthHeader(token));
};

const getAllMatches = async (token: string) => {
    return await httpClient.get('/Matches/GetAllMatches', getAuthHeader(token));
};

const createNewMatch = async (token: string) => {
    return await httpClient.post('/Matches/CreateNewMatch', {}, getAuthHeader(token));
};

const joinMatch = async (matchId: string, token: string) => {
    return await httpClient.post(`/Matches/JoinMatch/${matchId}`, {}, getAuthHeader(token));
};

const leaveMatch = async (matchId: number | string, token: string) => {
    return await httpClient.delete(`/Matches/LeaveMatch/${matchId}`, getAuthHeader(token));
};

const startMatch = async (config: any, token: string) => {
    return await httpClient.post('/Matches/StartMatch', config, getAuthHeader(token));
};

const selectPictureForRound = async (payload: any, token: string) => {
    return await httpClient.post('/Matches/SelectPictureForRound', payload, getAuthHeader(token));
};

const voteForPicture = async (payload: any, token: string) => {
    return await httpClient.post('/Matches/VoteForSelectedPicture', payload, getAuthHeader(token));
};

export default {
    getCurrentMatch,
    getMatchById,
    getAllMatches,
    createNewMatch,
    joinMatch,
    leaveMatch,
    startMatch,
    selectPictureForRound,
    voteForPicture
};