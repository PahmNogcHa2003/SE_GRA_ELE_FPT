export type VoucherStatus = 'Active' | 'Inactive' | 'Expired' | 'Disabled';

export interface VoucherDTO {
  id: number;
  code: string;
  description : string;
  isPercentage: boolean;
  value: number;
  maxDiscountAmount?: number;
  minOrderAmount?: number;
  startDate: string;
  endDate: string;
  usageLimit?: number;
  usagePerUser?: number;
  status: VoucherStatus;
  usageCount: number;
}

export interface VoucherCreateDTO {
  code: string;
  description : string;
  isPercentage: boolean;
  value: number;
  maxDiscountAmount?: number;
  minOrderAmount?: number;
  startDate: string;
  endDate: string;
  usageLimit?: number;
  usagePerUser?: number;
  status?: VoucherStatus;
}

export interface VoucherUpdateDTO extends VoucherCreateDTO {}

export interface VoucherFilterDTO {
  page: number;
  pageSize: number;
  searchQuery?: string;
  filterField?: string;
  filterValue?: string;
  sortOrder?: string;
}
