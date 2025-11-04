import type { ApiResponse, PagedResult } from "./api";

export interface TagDTO {
  id: number;
  name: string;
}

export interface GetTagsParams {
  page?: number;
  pageSize?: number;
  search?: string;
  sortOrder?: string;
}
export type TagsApiResponse = ApiResponse<TagDTO>;
export type TagsPagedApiResponse = ApiResponse<PagedResult<TagDTO>>;