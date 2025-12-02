// src/http.ts
import axios from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL;

const http = axios.create({
  baseURL,
  // KHÔNG set Content-Type cố định ở đây
});

// ===== BỘ CHẶN YÊU CẦU (Request Interceptor) =====
http.interceptors.request.use(
  (config) => {
    // ---- Gắn token ----
    const token = localStorage.getItem("authToken");
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // ---- Xử lý Content-Type theo loại dữ liệu ----
    if (config.data instanceof FormData) {
      // Nếu là FormData thì để axios tự set multipart/form-data + boundary
      if (config.headers) {
        delete config.headers["Content-Type"];
        delete (config.headers as any)["content-type"];
      }
    } else {
      // Còn lại (body là JSON, text, ...) thì set application/json
      if (config.headers) {
        config.headers["Content-Type"] = "application/json";
      }
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// ===== BỘ CHẶN PHẢN HỒI (Response Interceptor) =====
http.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      const token = localStorage.getItem('authToken');
      
      if (token) {
        console.warn('Token hết hạn hoặc không hợp lệ. Đang đăng xuất...');
        window.dispatchEvent(new Event('auth-error-401'));
      }
    }
    
    return Promise.reject(error);
  }
);

export default http;
