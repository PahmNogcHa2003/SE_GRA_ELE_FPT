import http from "./http";
import type {UserProfileDTO, UpdateUserProfileBasicDTO } from "../types/userProfile";
import type { ApiResponse } from "../types/api";


export const getMyProfile = async (): Promise<UserProfileDTO> => {
const res = await http.get<ApiResponse<UserProfileDTO>>("/UserProfiles/me");
if (!res.data.success || !res.data.data) {
throw new Error(res.data.message || "Không thể lấy thông tin hồ sơ");
}
return res.data.data;
};


export const updateMyProfile = async (
payload: UpdateUserProfileBasicDTO
): Promise<UserProfileDTO> => {
const res = await http.put<ApiResponse<UserProfileDTO>>("/UserProfiles/me", payload);
if (!res.data.success || !res.data.data) {
throw new Error(res.data.message || "Cập nhật thất bại");
}
return res.data.data;
};