export const API_CONFIG = {
  // In dev we use the Vite proxy: call relative '/api' paths and Vite forwards to backend
  AUTH_SERVICE: '/api',
  BASE_URL: '/api'
};

export const ENDPOINTS = {
  AUTH: {
    LOGIN: '/auth/login',
    REGISTER: '/auth/register',
    REFRESH: '/auth/refresh'
  },
  ADMIN: {
    USERS: '/admin/users',
    DASHBOARD: '/admin/dashboard'
  }
};