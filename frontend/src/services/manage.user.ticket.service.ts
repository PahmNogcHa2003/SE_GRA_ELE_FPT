import { httpAdmin } from './http';
import type { ApiResponse ,PagedResult} from '../types/api';
import type { 
  ManageUserTicket

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

export const getPagedUserTickets = (params: PagedRequestParams) => {
  return httpAdmin.get<ApiResponse<PagedResult<ManageUserTicket>>>('/UserTicket', { params });
};
export const voidUserTicket = (id: number, reason: string) => {
  return httpAdmin.post<ApiResponse<ManageUserTicket>>(`/UserTicket/${id}/void`, { reason });
};