import createApiClient from './baseApi';
import { ENDPOINTS } from '../../config/api';

const adminApi = createApiClient();

export const adminService = {
  getUsers: async () => {
    const response = await adminApi.get(ENDPOINTS.ADMIN.USERS);
    return response.data;
  },

  getDashboard: async () => {
    const response = await adminApi.get(ENDPOINTS.ADMIN.DASHBOARD);
    return response.data;
  },

  deleteUser: async (userId) => {
    const response = await adminApi.delete(`${ENDPOINTS.ADMIN.USERS}/${userId}`);
    return response.data;
  }
};