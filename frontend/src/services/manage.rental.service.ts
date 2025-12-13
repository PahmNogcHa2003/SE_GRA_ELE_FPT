import type { ApiResponse, PagedResult } from '../types/api';
import type { RentalListDTO, RentalDetailDTO, RentalFilterDTO } from '../types/rental';
import { httpAdmin } from './http';

const BASE_URL = '/rental';

export const getPagedRentals = async (params: RentalFilterDTO): Promise<ApiResponse<PagedResult<RentalListDTO>>> => {
  try {
    const response = await httpAdmin.get<ApiResponse<PagedResult<RentalListDTO>>>(BASE_URL, { params });
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

export const getRentalDetail = async (id: number): Promise<ApiResponse<RentalDetailDTO>> => {
  try {
    const response = await httpAdmin.get<ApiResponse<RentalDetailDTO>>(`${BASE_URL}/${id}`);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};