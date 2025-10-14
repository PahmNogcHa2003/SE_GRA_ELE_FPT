import type  { ApiResponse,  PagedResult } from './api';
export interface StationDTO {
    id: number;
    name : string;
    location: string;
    capacity: number;
    lat : number;
    lng : number;
    isActive: boolean;
    image?: string;
}
export interface GetStationsParams {
  page?: number;
  pageSize?: number;
  search?: string;
  sortOrder?: string;
  filterField?: string; 
  filterValue?: string;  
}
export type StationApiResponse = ApiResponse<StationDTO>;
export type StationPagedApiResponse = ApiResponse<PagedResult<StationDTO>>;