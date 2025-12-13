// src/services/manage.transaction.service.ts
import { httpAdmin } from './http'; 
import type { ApiResponse, PagedResult } from '../types/api';
import type { TransactionsDTO, TransactionQueryParams } from '../types/manage.transaction';

const BASE_URL = '/transactions';

export const getPagedTransactions = async (params: TransactionQueryParams): Promise<ApiResponse<PagedResult<TransactionsDTO>>> => {
  try {
    const response = await httpAdmin.get<ApiResponse<PagedResult<TransactionsDTO>>>(BASE_URL, { params });
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};