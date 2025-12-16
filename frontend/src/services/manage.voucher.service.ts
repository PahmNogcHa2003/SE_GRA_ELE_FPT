// src/services/manage.voucher.service.ts
import { httpAdmin } from './http'; // Sử dụng instance axios của bạn
import type { ApiResponse, PagedResult } from '../types/api';
import type { VoucherDTO, CreateVoucherDTO, UpdateVoucherDTO, VoucherFilterDTO } from '../types/manage.voucher';

const BASE_URL = '/Voucher';

export const getPagedVouchers = async (params: VoucherFilterDTO): Promise<ApiResponse<PagedResult<VoucherDTO>>> => {
  try {
    const response = await httpAdmin.get<ApiResponse<PagedResult<VoucherDTO>>>(BASE_URL, { params });
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

export const createVoucher = async (data: CreateVoucherDTO): Promise<ApiResponse<VoucherDTO>> => {
  try {
    const response = await httpAdmin.post<ApiResponse<VoucherDTO>>(BASE_URL, data);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

export const updateVoucher = async (id: number, data: UpdateVoucherDTO): Promise<ApiResponse<VoucherDTO>> => {
  try {
    const response = await httpAdmin.put<ApiResponse<VoucherDTO>>(`${BASE_URL}/${id}`, data);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

export const deleteVoucher = async (id: number): Promise<ApiResponse<object>> => {
  try {
    const response = await httpAdmin.delete<ApiResponse<object>>(`${BASE_URL}/${id}`);
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};