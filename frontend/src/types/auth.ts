// src/types/auth.ts
export interface LoginPayload {
  email: string;
  password: string;
  deviceId: string; 
  pushToken: string;
  platform: string;
}
export interface AuthResponseData {
  isSuccess: boolean;
  message: string;
  token?: string;
  roles?: string[];
}
export interface ChangePasswordPayload {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
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