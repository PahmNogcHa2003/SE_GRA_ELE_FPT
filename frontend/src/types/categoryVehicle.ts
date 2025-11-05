// src/types/categoryVehicle.ts

import type { ApiResponse, PagedResult } from "./api";


// DTO cho một Loại xe, khớp với C# CategoriesVehicleDTO
export interface CategoryVehicleDTO {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
}

// Các tham số để lấy danh sách Loại xe, có phân trang, tìm kiếm, lọc...
export interface GetCategoriesVehicleParams {
  page?: number;
  pageSize?: number;
  search?: string;
  filterField?: string;
  filterValue?: string;
  sortOrder?: string;
}
export type CategoriesVehiclesApiResponse = ApiResponse<CategoryVehicleDTO>;
export type CategoriesVehiclesPagedApiResponse = ApiResponse<PagedResult<CategoryVehicleDTO>>;