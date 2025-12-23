// src/services/voucher.service.ts
import { httpUser } from './http';
import type { ApiResponse } from '../types/api';
import type { AvailableVoucherDTO } from "../types/voucher";

const BASE_URL = '/voucher';

export const getAvailableVouchers = async (planPrice: number): Promise<AvailableVoucherDTO[]> => {
  const res = await httpUser.get<ApiResponse<AvailableVoucherDTO[]>>(`${BASE_URL}/available`, {
    params: { planPrice }
  });
  return res.data.data ?? [];
};