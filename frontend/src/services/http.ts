// src/http.ts
import axios, { type AxiosInstance, type InternalAxiosRequestConfig, AxiosError } from "axios";

// Fallback an toàn
const USER_BASE = (import.meta.env.VITE_USER_API_URL as string) || "https://localhost:7160/api";
const ADMIN_BASE = (import.meta.env.VITE_ADMIN_API_URL as string) || "https://localhost:7240/api";

function createHttpInstance(baseURL: string): AxiosInstance {
  const instance = axios.create({
    baseURL,
    headers: {
      "ngrok-skip-browser-warning": "true",
      "Content-Type": "application/json", 
    },
    timeout: 30000, 
  });

  // Request interceptor
  instance.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      const token = localStorage.getItem("authToken");
      
      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
      }

      // Xử lý FormData: Để browser tự động set Content-Type và boundary
      if (config.data instanceof FormData && config.headers) {
        delete config.headers["Content-Type"];
      }

      return config;
    },
    (error) => Promise.reject(error)
  );

  // Response interceptor
  instance.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
      // Chỉ log lỗi 500 hoặc lỗi hệ thống, tránh log rác 400 (Bad Request)
      if (!error.response || error.response.status >= 500) {
        console.error("System/Network Error:", error);
      }

      if (error.response?.status === 401) {
        const token = localStorage.getItem("authToken");
        if (token) {
          // Xóa token ngay để tránh loop vô hạn nếu component tự retry
          localStorage.removeItem("authToken"); 
          localStorage.removeItem("userRole"); // Xóa thêm các state liên quan nếu cần
          
          console.warn("Session expired. Dispatching auth-error-401...");
          window.dispatchEvent(new Event("auth-error-401"));
          
          // Optional: Force reload hoặc redirect ngay tại đây nếu muốn gắt gao hơn
          // window.location.href = "/login";
        }
      }

      return Promise.reject(error);
    }
  );

  return instance;
}

export const httpUser = createHttpInstance(USER_BASE);
export const httpAdmin = createHttpInstance(ADMIN_BASE);

export default { httpUser, httpAdmin };