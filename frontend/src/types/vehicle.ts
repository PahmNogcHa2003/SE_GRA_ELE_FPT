// src/types/vehicle.ts

import type { ApiResponse, PagedResult } from "./api";

// DTO cho Xe, khớp với C# VehicleDTO
export interface VehicleDTO {
  id: number;
  categoryId: number;
  bikeCode: string;
  batteryLevel?: number;
  chargingStatus?: boolean;
  status: string; // "Available", "InUse", "Maintenance", etc.
  stationId?: number;
  createdAt: string; // Hoặc Date
}

// Tham số để lấy danh sách Xe
export interface GetVehiclesParams {
  page?: number;
  pageSize?: number;
  search?: string;
  filterField?: string;
  filterValue?: string;
  sortOrder?: string;
}
export type VehiclesApiResponse = ApiResponse<VehicleDTO>;
export type VehiclesPagedApiResponse = ApiResponse<PagedResult<VehicleDTO>>;