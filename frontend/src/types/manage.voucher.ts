export interface VoucherDTO {
  id: number;
  code: string;
  isPercentage: boolean;
  value: number;
  maxDiscountAmount?: number;
  minOrderAmount?: number;
  startDate: string;
  endDate: string;
  usageLimit?: number;
  usagePerUser?: number;
  status: string; 
}

export interface CreateVoucherDTO {
  code: string;
  isPercentage: boolean;
  value: number;
  maxDiscountAmount?: number;
  minOrderAmount?: number;
  startDate: string;
  endDate: string;
  usageLimit?: number;
  usagePerUser?: number;
  status?: string;
}

export interface UpdateVoucherDTO extends CreateVoucherDTO {}

export interface VoucherFilterDTO {
  page: number;
  pageSize: number;
  searchQuery?: string;
  filterField?: string;
  filterValue?: string;
  sortOrder?: string;
}