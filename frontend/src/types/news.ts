// src/types/news.ts
import type { ApiResponse, PagedResult } from "./api";
export interface NewsDTO {
  id: number;
  title: string;
  slug: string;
  bannerImage: string;
  content: string;
  authorId: number; // Tạm thời chỉ lưu ID
  createdAt: string; // Hoặc Date
  isActive: boolean;
  tagIds: number[];
  tags?: { id: number; name: string }[]; // Để hiển thị trong bảng
}

export interface GetNewsParams {
  page?: number;
  pageSize?: number;
  search?: string;
  filterField?: string;
  filterValue?: string;
  sortOrder?: string;
}
export type NewsApiResponse = ApiResponse<NewsDTO>;
export type NewsPagedApiResponse = ApiResponse<PagedResult<NewsDTO>>;