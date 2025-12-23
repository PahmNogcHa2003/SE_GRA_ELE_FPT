// src/services/news.service.ts

import { httpAdmin, httpUser } from './http';
import type { ApiResponse, PagedResult } from '../types/api';
import type { NewsDTO, GetNewsParams } from '../types/news';

const BASE_URL = '/news';

// ===== API Calls User =====
export const getNews = (params: GetNewsParams) => {
  return httpUser.get<ApiResponse<PagedResult<NewsDTO>>>(BASE_URL, { params });
};

export const getNewsById = (slug: string, id: number) => {
  return httpUser.get<ApiResponse<NewsDTO>>(`${BASE_URL}/${slug}/${id}`);
};

export const getRelatedNews = (newsId?: number, limit = 5) => {
  return httpUser.get<ApiResponse<NewsDTO[]>>(`${BASE_URL}/related`, {
    params: {newsId, limit },
  });
};
// ===== API Calls Admin =====
export const getNewsAdmin = (params: GetNewsParams) => {
  return httpAdmin.get<ApiResponse<PagedResult<NewsDTO>>>(BASE_URL, { params });
};

export const getNewsByIdAdmin = (id: number) => {
  return httpAdmin.get<ApiResponse<NewsDTO>>(`${BASE_URL}/${id}`);
};

export const createNews = (data: Omit<NewsDTO, 'id' | 'createdAt' | 'tags'>) => {
  return httpAdmin.post<ApiResponse<NewsDTO>>(BASE_URL, data);
};

export const updateNews = (id: number, data: Omit<NewsDTO, 'createdAt' | 'tags'>) => {
  return httpAdmin.put<ApiResponse<null>>(`${BASE_URL}/${id}`, data);
};

export const deleteNews = (id: number) => {
  return httpAdmin.delete<ApiResponse<null>>(`${BASE_URL}/${id}`);
};

