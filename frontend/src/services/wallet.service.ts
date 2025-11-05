import http from './http'; 
import axios from 'axios'; // <-- 1. Import axios để kiểm tra lỗi
import type { ApiResponse,PagedResult } from '../types/api';
import type { Wallet, WalletTransaction } from '../types/wallet';

/**
 * Lấy thông tin ví
 * (SỬA: Bắt lỗi 404)
 */
export const getWallet = async (): Promise<ApiResponse<Wallet | null>> => {
  try {
    const response = await http.get<ApiResponse<Wallet>>('/wallets');
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 404) {
      return { success: true, message: 'Wallet not found', data: null };
    }
    throw error;
  }
};

/**
 * Lấy lịch sử giao dịch (phân trang)
 * (SỬA: Bắt lỗi 404)
 */
export const getWalletTransactions = async (
  page = 1,
  pageSize = 10
): Promise<ApiResponse<PagedResult<WalletTransaction> | null>> => {
  try {
    const response = await http.get<ApiResponse<PagedResult<WalletTransaction>>>(
      '/wallet/transactions',
      { params: { page, pageSize, sortOrder: 'createdAt_desc' } }
    );
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 404) {
      return { success: true, message: 'No transactions found', data: null };
    }
    throw error;
  }
};