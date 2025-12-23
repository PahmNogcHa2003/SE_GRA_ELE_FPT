import { httpUser } from "./http";
import type { ApiResponse } from "../types/api";
import type { QuestCompletedDTO, QuestDTO } from "../types/quest"; 

export const getMyActiveQuests = async (): Promise<QuestDTO[]> => {
const res = await httpUser.get<ApiResponse<QuestDTO[]>>('/quest/active');
return res.data.data ?? [];
};

export const getMyQuestCompleted = async (): Promise<QuestCompletedDTO[]> => {
  const res = await httpUser.get<ApiResponse<QuestCompletedDTO[]>>('/quest/completed');
  return res.data.data ?? [];
}