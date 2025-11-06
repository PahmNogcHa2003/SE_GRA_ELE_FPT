import http from './http'; // <-- Import 'http' chung
import type { 
  CreatePaymentPayload, 
  PaymentUrlResponse, 
  PaymentResult 
} from '../types/payment';

// KHÔNG CẦN 'getAuthHeaders' nữa

/**
 * Tạo URL thanh toán VNPay
 * (KHÔNG CẦN 'token' param nữa)
 */
export const createPaymentUrl = async (
  payload: CreatePaymentPayload,
): Promise<PaymentUrlResponse> => {
  // Interceptor tự động thêm token
  const response = await http.post<PaymentUrlResponse>(
    '/Payments/vnpay/create-url',
    payload
  );
  return response.data;
};

/**
 * Xử lý kết quả trả về từ VNPay
 * (KHÔNG CẦN 'token' param nữa)
 */
export const processVnPayReturn = async (
  queryString: string,
): Promise<PaymentResult> => { 
  // Interceptor tự động thêm token
  const response = await http.get<PaymentResult>(
    `/Payments/vnpay-return?${queryString}`
  );
  return response.data;
};