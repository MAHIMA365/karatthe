import axios from 'axios';
import { API_CONFIG } from '../../config/api';

const createApiClient = (baseURL = API_CONFIG.BASE_URL) => {
  const client = axios.create({
    baseURL,
    withCredentials: true,
    headers: {
      'Content-Type': 'application/json'
    }
  });

  client.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        localStorage.removeItem('user');
        window.location.href = '/login';
      }
      return Promise.reject(error);
    }
  );

  return client;
};

export default createApiClient;