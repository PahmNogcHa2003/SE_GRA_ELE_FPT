import { httpUser } from './http'; 
import type { 
  CreatePaymentPayload, 
  PaymentUrlResponse, 
  PaymentResult 
} from '../types/payment';

/**
 * Tạo URL thanh toán VNPay
 * (KHÔNG CẦN 'token' param nữa)
 */
export const createPaymentUrl = async (
  payload: CreatePaymentPayload,
): Promise<PaymentUrlResponse> => {
  // Interceptor tự động thêm token
  const response = await httpUser.post<PaymentUrlResponse>(
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
  const response = await httpUser.get<PaymentResult>(
    `/Payments/vnpay-return?${queryString}`
  );
  console.log(queryString);
  return response.data;
};