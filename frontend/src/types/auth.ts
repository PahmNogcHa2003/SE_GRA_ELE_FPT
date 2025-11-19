// src/types/auth.ts

/**
 * Dữ liệu gửi đi khi đăng nhập
 * Khớp với LoginDTO.cs
 */
export interface LoginPayload {
  email: string;
  password: string;
  deviceId: string; 
  pushToken: string;
  platform: string;
}

/**
 * Dữ liệu nhận về trong 'data' khi đăng nhập thành công
 * Khớp với AuthResponseDTO.cs
 */
export interface AuthResponseData {
  token: string;
  roles : string[];
}
export interface User {
  userId: number; 
  email: string;
  fullName: string;
  avatarUrl?: string | null; 
  createdDate: string;
  dob: string;
  gender: string;
  addressDetail: string;
  roles: string[];
}