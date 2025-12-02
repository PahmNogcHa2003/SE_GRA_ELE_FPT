// src/services/tag.service.ts

import http from './http';
import type { ApiResponse, PagedResult } from '../types/api';
import type { TagDTO, GetTagsParams } from '../types/tag';

const BASE_URL = '/tags';

export const getTags = (params: GetTagsParams) => {
  return http.get<ApiResponse<PagedResult<TagDTO>>>(BASE_URL, { params });
};

export const createTag = (data: Omit<TagDTO, 'id'>) => {
  return http.post<ApiResponse<TagDTO>>(BASE_URL, data);
};

export const updateTag = (id: number, data: TagDTO) => {
  return http.put<ApiResponse<null>>(`${BASE_URL}/${id}`, data);
};

export const deleteTag = (id: number) => {
  return http.delete<ApiResponse<null>>(`${BASE_URL}/${id}`);
};