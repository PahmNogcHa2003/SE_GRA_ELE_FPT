import { httpAdmin } from './http'; // Instance axios của bạn
import type { ApiResponse } from '../types/api'; // Type ApiResponse chung
import type { 
  DashboardStatisticsDTO, 
  RevenueStatisticsDTO, 
  RentalStatisticsDTO, 
  UserStatisticsDTO 
} from '../types/dashboard'; // Các type đã định nghĩa ở bước trước

const BASE_URL = '/Dashboard';

// Helper để tạo params
const createParams = (fromDate?: string, toDate?: string) => ({
  fromDate,
  toDate
});

export const dashboardService = {
  /**
   * GET /api/Dashboard
   * Lấy toàn bộ thống kê (Dùng cho màn hình chính)
   */
  getAllStatistics: async (fromDate?: string, toDate?: string) => {
    const res = await httpAdmin.get<ApiResponse<DashboardStatisticsDTO>>(BASE_URL, {
      params: createParams(fromDate, toDate)
    });
    return res.data;
  },

  /**
   * GET /api/Dashboard/revenue
   * Lấy chi tiết doanh thu (Dùng nếu muốn reload riêng phần tiền)
   */
  getRevenueStatistics: async (fromDate?: string, toDate?: string) => {
    const res = await httpAdmin.get<ApiResponse<RevenueStatisticsDTO>>(`${BASE_URL}/revenue`, {
      params: createParams(fromDate, toDate)
    });
    return res.data;
  },

  /**
   * GET /api/Dashboard/rentals
   * Lấy chi tiết lượt thuê
   */
  getRentalStatistics: async (fromDate?: string, toDate?: string) => {
    const res = await httpAdmin.get<ApiResponse<RentalStatisticsDTO>>(`${BASE_URL}/rentals`, {
      params: createParams(fromDate, toDate)
    });
    return res.data;
  },

  /**
   * GET /api/Dashboard/users
   * Lấy chi tiết người dùng
   */
  getUserStatistics: async (fromDate?: string, toDate?: string) => {
    const res = await httpAdmin.get<ApiResponse<UserStatisticsDTO>>(`${BASE_URL}/users`, {
      params: createParams(fromDate, toDate)
    });
    return res.data;
  }
};