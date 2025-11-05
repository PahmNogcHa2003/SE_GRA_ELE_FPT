// src/services/vehicle.service.ts

import http from './http';
import type { ApiResponse, PagedResult } from '../types/api';
import type { VehicleDTO, GetVehiclesParams } from '../types/vehicle';

const BASE_URL = '/vehicles';

export const getVehicles = (params: GetVehiclesParams) => {
  return http.get<ApiResponse<PagedResult<VehicleDTO>>>(BASE_URL, { params });
};

export const createVehicle = (data: Omit<VehicleDTO, 'id' | 'createdAt'>) => {
  return http.post<ApiResponse<VehicleDTO>>(BASE_URL, data);
};

export const updateVehicle = (id: number, data: VehicleDTO) => {
  return http.put<ApiResponse<null>>(`${BASE_URL}/${id}`, data);
};

export const deleteVehicle = (id: number) => {
  return http.delete<ApiResponse<null>>(`${BASE_URL}/${id}`);
};