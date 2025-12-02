// src/services/news.service.ts

import http from './http';
import type { ApiResponse, PagedResult } from '../types/api';
import type { NewsDTO, GetNewsParams } from '../types/news';

const BASE_URL = '/news';

export const getNews = (params: GetNewsParams) => {
  return http.get<ApiResponse<PagedResult<NewsDTO>>>(BASE_URL, { params });
};

// Lấy 1 tin
export const getNewsById = (id: number) => {
  return http.get<ApiResponse<NewsDTO>>(`${BASE_URL}/${id}`);
};

export const createNews = (data: Omit<NewsDTO, 'id' | 'createdAt' | 'tags'>) => {
  return http.post<ApiResponse<NewsDTO>>(BASE_URL, data);
};

export const updateNews = (id: number, data: Omit<NewsDTO, 'createdAt' | 'tags'>) => {
  return http.put<ApiResponse<null>>(`${BASE_URL}/${id}`, data);
};

export const deleteNews = (id: number) => {
  return http.delete<ApiResponse<null>>(`${BASE_URL}/${id}`);
};

export const getRelatedNews = (newsId?: number, limit = 5) => {
  return http.get<ApiResponse<NewsDTO[]>>(`${BASE_URL}/related`, {
    params: {newsId, limit },
  });
};