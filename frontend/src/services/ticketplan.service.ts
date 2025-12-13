import { httpAdmin } from './http';
import type { ApiResponse ,PagedResult} from '../types/api';
import type { 
  TicketPlan, 
  CreateTicketPlan, 
  UpdateTicketPlan,

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

export const getPagedTicketPlans = (params: PagedRequestParams) => {
  return httpAdmin.get<ApiResponse<PagedResult<TicketPlan>>>('/TicketPlan', { params });
};
export const createTicketPlan = (data: CreateTicketPlan) => {
  return httpAdmin.post<ApiResponse<TicketPlan>>('/TicketPlan', data);
};
export const updateTicketPlan = (id: number, data: UpdateTicketPlan) => {
  return httpAdmin.put<ApiResponse<TicketPlan>>(`/TicketPlan/${id}`, data);
};
export const deleteTicketPlan = (id: number) => {
  return httpAdmin.delete<ApiResponse<object>>(`/TicketPlan/${id}`);
};