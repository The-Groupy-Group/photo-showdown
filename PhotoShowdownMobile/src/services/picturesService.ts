// src/services/picturesService.ts
import httpClient from './api';

const getAuthHeader = (token: string) => ({
    headers: { Authorization: `Bearer ${token}` }
});

const getMyPictures = async (token: string) => {
    return await httpClient.get('/Pictures/GetMyPictures', getAuthHeader(token));
};

const uploadPicture = async (formData: FormData, token: string) => {
    return await httpClient.post('/Pictures/UploadPicture', formData, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'multipart/form-data'
        }
    });
};

const deletePicture = async (pictureId: number, token: string) => {
    return await httpClient.delete(`/Pictures/DeletePicture/${pictureId}`, getAuthHeader(token));
};

export default {
    getMyPictures,
    uploadPicture,
    deletePicture
};