// src/services/manage.quest.service.ts
import { httpAdmin } from './http'; // Sử dụng instance http của bạn
import type { ApiResponse, PagedResult } from '../types/api';
import type { QuestDTO, QuestCreateDTO, QuestUpdateDTO, QuestFilterDTO } from '../types/manage.quest';

const BASE_URL = '/quests';

export const getPagedQuests = async (params: QuestFilterDTO): Promise<ApiResponse<PagedResult<QuestDTO>>> => {
  try {
    const response = await httpAdmin.get<ApiResponse<PagedResult<QuestDTO>>>(BASE_URL, { params });
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

export const createQuest = async (data: QuestCreateDTO): Promise<ApiResponse<QuestDTO>> => {
  try {
    const response = await httpAdmin.post<ApiResponse<QuestDTO>>(BASE_URL, data);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

export const updateQuest = async (id: number, data: QuestUpdateDTO): Promise<ApiResponse<null>> => {
  try {
    const response = await httpAdmin.put<ApiResponse<null>>(`${BASE_URL}/${id}`, data);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

export const toggleQuestStatus = async (id: number): Promise<ApiResponse<null>> => {
  try {
    const response = await httpAdmin.patch<ApiResponse<null>>(`${BASE_URL}/${id}/toggle`);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};