import { httpAdmin, httpUser } from './http'; // <-- Import 'http' chung
import type {ChangePasswordPayload, LoginPayload, AuthResponseData, User } from '../types/auth';
import type { ApiResponse } from '../types/api';

/**
 * Gọi API đăng nhập
 */
export const loginApiAdmin = async (payload: LoginPayload): Promise<AuthResponseData> => {
  try {
  const res = await httpAdmin.post<AuthResponseData>('/Auth/Admin/login', payload);
  return res.data;
    } catch (error: any) {
    throw error as ApiResponse<null>; 
  }
};

export const changePasswordAdmin = async (payload: ChangePasswordPayload): Promise<AuthResponseData> => {
  try {
    const response = await httpAdmin.post<AuthResponseData>('/Auth/Admin/change-password', payload);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

export const getMeApiAdmin = async (): Promise<ApiResponse<User>> => {
  try {
    const response = await httpAdmin.get<ApiResponse<User>>('/Auth/Admin/me');
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>; 
  }
};

export const getMeApi = async (): Promise<ApiResponse<User>> => {
  try {
    const response = await httpUser.get<ApiResponse<User>>('/Auth/me');
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>; 
  }
};

export const loginApi = async (payload: LoginPayload): Promise<AuthResponseData> => {
  try {
  const res = await httpUser.post<AuthResponseData>('/Auth/login', payload);
  return res.data;
    } catch (error: any) {
    throw error as ApiResponse<null>; 
  }
};

export const changePassword = async (payload: ChangePasswordPayload): Promise<AuthResponseData> => {
  try {
    const response = await httpUser.post<AuthResponseData>('/Auth/change-password', payload);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};
