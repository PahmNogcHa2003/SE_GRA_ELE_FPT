import http from './http'; // <-- Import 'http' chung
import type { LoginPayload, AuthResponseData, User } from '../types/auth';
import type { ApiResponse } from '../types/api';

/**
 * Gọi API đăng nhập
 */
export const loginApi = async (
  payload: LoginPayload
): Promise<ApiResponse<AuthResponseData>> => {
  try {
    // 'login' là request duy nhất KHÔNG cần token
    // Interceptor sẽ không tìm thấy token và gửi request bình thường
    const response = await http.post<ApiResponse<AuthResponseData>>(
      '/Auth/login', 
      payload
    );
    return response.data;
  } catch (error: any) {
    // Interceptor sẽ ném 'data' của lỗi
    throw error as ApiResponse<null>; 
  }
};

/**
 * Lấy thông tin user hiện tại (gọi API /me)
 * (KHÔNG CẦN 'token' param nữa)
 */
export const getMeApi = async (): Promise<ApiResponse<User>> => {
  try {
    // Interceptor sẽ tự động đính kèm token
    const response = await http.get<ApiResponse<User>>('/Auth/me');
    return response.data;
  } catch (error: any) {
    // Interceptor sẽ ném 'data' của lỗi
    throw error as ApiResponse<null>; 
  }
};