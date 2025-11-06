import http from './http';
import type { ApiResponse ,PagedResult} from '../types/api';
import type { 
  TicketPlanPrice,
  CreateTicketPlanPrice,
  UpdateTicketPlanPrice,

} from '../types/manage.ticket';

// ===== API Params (Cho phân trang, lọc...) =====
export interface PagedRequestParams {
  page?: number;
  pageSize?: number;
  searchQuery?: string;
  filterField?: string;
  filterValue?: string;
  sortOrder?: string;
}

export const getPagedTicketPlanPrices = (params: PagedRequestParams) => {
  return http.get<ApiResponse<PagedResult<TicketPlanPrice>>>('/TicketPlanPrice', { params });
};
export const createTicketPlanPrice = (data: CreateTicketPlanPrice) => {
  return http.post<ApiResponse<TicketPlanPrice>>('/TicketPlanPrice', data);
};
export const updateTicketPlanPrice = (id: number, data: UpdateTicketPlanPrice) => {
  return http.put<ApiResponse<TicketPlanPrice>>(`/TicketPlanPrice/${id}`, data);
};
export const deleteTicketPlanPrice = (id: number) => {
  return http.delete<ApiResponse<object>>(`/TicketPlanPrice/${id}`);
};