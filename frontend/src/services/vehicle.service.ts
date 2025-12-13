// src/services/vehicle.service.ts

import { httpAdmin } from './http';
import type { ApiResponse, PagedResult } from '../types/api';
import type { VehicleDTO, GetVehiclesParams } from '../types/vehicle';

const BASE_URL = '/vehicles';

export const getVehicles = (params: GetVehiclesParams) => {
  return httpAdmin.get<ApiResponse<PagedResult<VehicleDTO>>>(BASE_URL, { params });
};

export const createVehicle = (data: Omit<VehicleDTO, 'id' | 'createdAt'>) => {
  return httpAdmin.post<ApiResponse<VehicleDTO>>(BASE_URL, data);
};

export const updateVehicle = (id: number, data: VehicleDTO) => {
  return httpAdmin.put<ApiResponse<null>>(`${BASE_URL}/${id}`, data);
};

export const deleteVehicle = (id: number) => {
  return httpAdmin.delete<ApiResponse<null>>(`${BASE_URL}/${id}`);
};