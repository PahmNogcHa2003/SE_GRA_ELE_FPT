// Kiểu dữ liệu chung cho tất cả các API response từ backend
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

// Kiểu dữ liệu chung cho kết quả trả về có phân trang
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}