export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api/v1'
export const EXPORT_BASE_URL = import.meta.env.VITE_API_BASE_URL
  ? import.meta.env.VITE_API_BASE_URL.replace('/api/v1', '/api/export')
  : '/api/export'
