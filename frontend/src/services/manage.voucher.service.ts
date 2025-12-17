import { httpAdmin } from './http';
import type { ApiResponse, PagedResult } from '../types/api';
import type {
  VoucherDTO,
  VoucherCreateDTO,
  VoucherUpdateDTO,
  VoucherFilterDTO
} from '../types/manage.voucher';

const BASE_URL = '/Staff/Vouchers';

export const getPagedVouchers = async (
  params: VoucherFilterDTO
): Promise<ApiResponse<PagedResult<VoucherDTO>>> => {
  const res = await httpAdmin.get(BASE_URL, { params });
  return res.data;
};

export const createVoucher = async (
  data: VoucherCreateDTO
): Promise<ApiResponse<VoucherDTO>> => {
  const res = await httpAdmin.post(BASE_URL, data);
  return res.data;
};

export const updateVoucher = async (
  id: number,
  data: VoucherUpdateDTO
): Promise<ApiResponse<VoucherDTO>> => {
  const res = await httpAdmin.put(`${BASE_URL}/${id}`, data);
  return res.data;
};

export const toggleVoucherStatus = async (
  id: number
): Promise<ApiResponse<null>> => {
  const res = await httpAdmin.patch(`${BASE_URL}/${id}/toggle-status`);
  return res.data;
};
