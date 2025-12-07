import http from "./http";
import type { ApiResponse } from "../types/api";
import type { QuestDTO } from "../types/quest"; 

/** Lấy danh sách nhiệm vụ đang hoạt động của user */
export const getMyActiveQuests = async (): Promise<QuestDTO[]> => {
const res = await http.get<ApiResponse<QuestDTO[]>>('/quest/active');
return res.data.data ?? [];
};