// src/http.ts
import axios from 'axios';

const baseURL = import.meta.env.VITE_API_BASE_URL;

const http = axios.create({
  baseURL: baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// ===== BỘ CHẶN YÊU CẦU (Request Interceptor) =====
// Tự động thêm token vào MỌI yêu cầu
http.interceptors.request.use(
  (config) => {
    // Lấy token từ localStorage (AuthContext đã lưu nó ở đó)
    const token = localStorage.getItem('authToken'); 
    
    if (token) {
      // Nếu có token, đính nó vào header Authorization
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    // Xử lý lỗi của yêu cầu
    return Promise.reject(error);
  }
);

// ===== BỘ CHẶN PHẢN HỒI (Response Interceptor) =====
// Xử lý lỗi tập trung, đặc biệt là lỗi 401 (Unauthorized)
http.interceptors.response.use(
  (response) => {
    // Bất kỳ mã trạng thái nào nằm trong 2xx đều qua đây
    return response;
  },
  (error) => {
    // Bất kỳ mã trạng thái nào ngoài 2xx đều vào đây
    
    // Xử lý lỗi 401 (Token hết hạn / không hợp lệ)
    if (error.response && error.response.status === 401) {
      // Token không còn hợp lệ, xóa nó đi
      localStorage.removeItem('authToken');
      
      // Gửi một sự kiện (event) để AuthContext có thể "nghe" thấy
      // và tự động cập nhật state (logout user)
      window.dispatchEvent(new Event('auth-error-401'));
      window.location.replace('/'); 
      // Bạn cũng có thể reload trang
      // window.location.href = '/login'; 
    }
    
    // Ném lại phần 'data' của lỗi (thường chứa ApiResponse)
    // để react-query có thể bắt được (onError)
    if (error.response && error.response.data) {
      return Promise.reject(error.response.data);
    }
    
    return Promise.reject(error);
  }
);


export default http;