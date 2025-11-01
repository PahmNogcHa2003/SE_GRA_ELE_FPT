// src/services/categoryVehicle.service.ts

import http from './http'; // Giả sử bạn có file http.ts để quản lý instance của axios
import type { ApiResponse, PagedResult } from '../types/api';
import type { CategoryVehicleDTO, GetCategoriesVehicleParams } from '../types/categoryVehicle';

const BASE_URL = '/categoriesvehicles';

// Lấy danh sách loại xe có phân trang
export const getCategories = (params: GetCategoriesVehicleParams) => {
  return http.get<ApiResponse<PagedResult<CategoryVehicleDTO>>>(BASE_URL, { params });
};

// Tạo loại xe mới
export const createCategory = (data: Omit<CategoryVehicleDTO, 'id'>) => {
  return http.post<ApiResponse<CategoryVehicleDTO>>(BASE_URL, data);
};

// Cập nhật loại xe
export const updateCategory = (id: number, data: CategoryVehicleDTO) => {
  return http.put<ApiResponse<null>>(`${BASE_URL}/${id}`, data);
};

// Xóa loại xe
export const deleteCategory = (id: number) => {
  return http.delete<ApiResponse<null>>(`${BASE_URL}/${id}`);
};