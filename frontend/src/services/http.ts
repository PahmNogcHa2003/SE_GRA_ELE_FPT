// src/http.ts
import axios from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL;

const http = axios.create({
  baseURL,
  // THÊM ĐOẠN NÀY ĐỂ VƯỢT QUA MÀN HÌNH CẢNH BÁO CỦA NGROK
  headers: {
    "ngrok-skip-browser-warning": "true", // Giá trị nào cũng được
    "Access-Control-Allow-Origin": "*", // Đôi khi cần thêm cái này ở client để chắc chắn
  },
});

// ===== BỘ CHẶN YÊU CẦU (Request Interceptor) =====
http.interceptors.request.use(
  (config) => {
    // ---- Gắn token ----
    const token = localStorage.getItem("authToken");
    
    // Đảm bảo headers luôn tồn tại để tránh lỗi undefined
    if (!config.headers) {
        config.headers = {} as any;
    }

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // Đảm bảo header bypass ngrok luôn được gửi đi (phòng trường hợp bị ghi đè)
    config.headers["ngrok-skip-browser-warning"] = "true";

    // ---- Xử lý Content-Type theo loại dữ liệu ----
    if (config.data instanceof FormData) {
      // Nếu là FormData thì để axios tự set multipart/form-data + boundary
      delete config.headers["Content-Type"];
      delete (config.headers as any)["content-type"];
    } else {
      // Còn lại (body là JSON, text, ...) thì set application/json
      config.headers["Content-Type"] = "application/json";
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
    // Log lỗi ra để dễ debug xem Ngrok trả về cái gì
    console.error("Lỗi API:", error);

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