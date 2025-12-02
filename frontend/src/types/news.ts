// src/types/news.ts
import type { ApiResponse, PagedResult } from './api';

export interface NewsDTO {
  id: number;
  title: string;
  slug: string;
  banner?: string | null;
  content?: string | null;
  status: string;
  publishedAt?: string | null;
  publishedBy?: number | null;
  scheduledAt?: string | null;
  createdAt: string;
  userId: number;
  tagIds: number[];
  tagNames?: string[];
  updatedAt: string;
}

// Param gọi API lấy danh sách News (tuỳ theo controller của bạn)
export interface GetNewsParams {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  filterField?: string;
  filterValue?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  status?: string;
}

export type NewsApiResponse = ApiResponse<NewsDTO>;
export type NewsPagedApiResponse = ApiResponse<PagedResult<NewsDTO>>;
