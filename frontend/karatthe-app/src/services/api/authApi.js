import createApiClient from './baseApi';
import { ENDPOINTS } from '../../config/api';

const authApi = createApiClient();

export const authService = {
  login: async (credentials) => {
    const response = await authApi.post(ENDPOINTS.AUTH.LOGIN, credentials);
    return response.data;
  },

  register: async (userData) => {
    const response = await authApi.post(ENDPOINTS.AUTH.REGISTER, userData);
    return response.data;
  },

  logout: async () => {
    localStorage.removeItem('user');
    return Promise.resolve();
  }
};