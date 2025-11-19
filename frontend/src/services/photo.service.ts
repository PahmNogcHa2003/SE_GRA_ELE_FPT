// src/services/profile.service.ts hoặc photo.service.ts
import http from "./http";
import type { ApiResponse } from "../types/api";
import type { UserProfileDTO } from "../types/userProfile";

export const uploadAvatar = async (file: File): Promise<UserProfileDTO> => {
  const form = new FormData();
  form.append("file", file); 

  const res = await http.post<ApiResponse<UserProfileDTO>>(
    "/UserProfiles/me/avatar",
    form
  );

  const apiRes = res.data;
  if (!apiRes.success || !apiRes.data) {
    throw new Error(apiRes.message || "Upload avatar thất bại");
  }
  return apiRes.data;
};
