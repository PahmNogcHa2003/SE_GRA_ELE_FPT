import { httpAdmin } from './http';
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
  return httpAdmin.get<ApiResponse<PagedResult<TicketPlanPrice>>>('/TicketPlanPrice', { params });
};
export const createTicketPlanPrice = (data: CreateTicketPlanPrice) => {
  return httpAdmin.post<ApiResponse<TicketPlanPrice>>('/TicketPlanPrice', data);
};
export const updateTicketPlanPrice = (id: number, data: UpdateTicketPlanPrice) => {
  return httpAdmin.put<ApiResponse<TicketPlanPrice>>(`/TicketPlanPrice/${id}`, data);
};
export const deleteTicketPlanPrice = (id: number) => {
  return httpAdmin.delete<ApiResponse<object>>(`/TicketPlanPrice/${id}`);
};