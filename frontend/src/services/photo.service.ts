// src/services/profile.service.ts hoặc photo.service.ts
import { httpAdmin, httpUser } from "./http";
import type { ApiResponse } from "../types/api";
import type { UserProfileDTO } from "../types/userProfile";
import type { StationDTO } from "../types/station";
import type { NewsDTO } from "../types/news";

// Hàm upload ảnh đại diện người dùng
export const uploadAvatar = async (file: File): Promise<UserProfileDTO> => {
  const form = new FormData();
  form.append("file", file); 

  const res = await httpUser.post<ApiResponse<UserProfileDTO>>(
    "/UserProfiles/me/avatar",
    form
  );

  const apiRes = res.data;
  if (!apiRes.success || !apiRes.data) {
    throw new Error(apiRes.message || "Upload avatar thất bại");
  }
  return apiRes.data;
};
// Hàm upload hình trạm xe
export const uploadImageStation = async (id: number,file: File): Promise<StationDTO> => {
  const form = new FormData();
  form.append("file", file); 

  const res = await httpAdmin.post<ApiResponse<StationDTO>>(
    `/Stations/${id}/image`,
    form
  );
  const apiRes = res.data;
  if (!apiRes.success || !apiRes.data) {
    throw new Error(apiRes.message || "Upload hình trạm thất bại");
  }
  return apiRes.data;
};
// Hàm upload banner cho tin tức
export const uploadBannerNews = async (id : number,file: File): Promise<NewsDTO> => {
  const form = new FormData();
  form.append("file", file);
  const res = await httpAdmin.post<ApiResponse<NewsDTO>>(
    `/News/${id}/banner`,
    form
  );
  const apiRes = res.data;
  if (!apiRes.success || !apiRes.data) {
    throw new Error(apiRes.message || "Upload banner thất bại");
  }
  return apiRes.data;
}