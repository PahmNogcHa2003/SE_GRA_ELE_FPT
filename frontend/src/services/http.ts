import axios from 'axios';

const baseURL = import.meta.env.VITE_API_BASE_URL;

const http = axios.create({
  baseURL: baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Bạn có thể thêm các interceptors để xử lý token, lỗi... ở đây
// http.interceptors.request.use(...)
// http.interceptors.response.use(...)

export default http;