// src/services/tag.service.ts

import { httpAdmin, httpUser } from './http';
import type { ApiResponse, PagedResult } from '../types/api';
import type { TagDTO, GetTagsParams } from '../types/tag';

const BASE_URL = '/tags';

export const getTags = (params: GetTagsParams) => {
  return httpUser.get<ApiResponse<PagedResult<TagDTO>>>(BASE_URL, { params });
};

export const getTagsAdmin = (params: GetTagsParams) => {
  return httpAdmin.get<ApiResponse<PagedResult<TagDTO>>>(BASE_URL, { params });
};

export const createTag = (data: Omit<TagDTO, 'id'>) => {
  return httpAdmin.post<ApiResponse<TagDTO>>(BASE_URL, data);
};

export const updateTag = (id: number, data: TagDTO) => {
  return httpAdmin.put<ApiResponse<null>>(`${BASE_URL}/${id}`, data);
};

export const deleteTag = (id: number) => {
  return httpAdmin.delete<ApiResponse<null>>(`${BASE_URL}/${id}`);
};