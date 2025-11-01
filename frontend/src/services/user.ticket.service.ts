// src/services/ticketService.ts
import type { ApiResponse } from '../types/api.ts';
import type { UserTicket, PurchaseTicketRequest, UserTicketPlan } from '../types/user.ticket';
import http from './http.ts';

/**
 * Lấy tất cả các vé đang hoạt động (Ready/Active) của người dùng
 * (Interceptor sẽ tự động thêm token)
 */
export const getMyActiveTickets = async (): Promise<ApiResponse<UserTicket[]>> => {
  try {
    const response = await http.get<ApiResponse<UserTicket[]>>(
      '/UserTicket/active' 
    );
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};

/**
 * Mua vé mới
 * (Interceptor sẽ tự động thêm token)
 */
export const purchaseTicket = async (
  payload: PurchaseTicketRequest
): Promise<ApiResponse<UserTicket>> => {
  try {
    const response = await http.post<ApiResponse<UserTicket>>(
      '/UserTicket/purchase',
      payload
    );
    return response.data;
  } catch (error: any) {
    throw error as ApiResponse<null>;
  }
};
/**
 * Lấy danh sách các gói vé đang bán trên thị trường
 * (API này KHÔNG bọc ApiResponse)
 * @param vehicleType 'bike' hoặc 'ebike' (tùy chọn)
 */
export const getTicketMarket = async (vehicleType?: 'bike' | 'ebike'): Promise<UserTicketPlan[]> => {
  try {
    const response = await http.get<UserTicketPlan[]>(
      '/UserTicket/market', 
      { params: { vehicleType } }
    );
    return response.data; 
  } catch (error: any) {
    console.error("Lỗi khi lấy danh sách vé:", error);
    throw new Error(error.message || "Không thể tải danh sách vé.");
  }
};