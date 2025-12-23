// src/types/payment.ts

export interface CreatePaymentPayload {
  amount: number;
}

export interface PaymentUrlResponse {
  paymentUrl: string;
}

// 1. Định nghĩa chi tiết cho giao dịch Ví chính
export interface WalletTransactionDetail {
  id: number;
  amount: number;
  direction: string;
  source: string;
  balanceAfter: number;
  createdAt: string;
}

// 2. Định nghĩa chi tiết cho giao dịch Ví khuyến mãi (Promo)
export interface PromoTransactionDetail {
  id: number;
  amount: number;
  direction: string;
  source: string;
  promoAfter: number; // Backend trả về PromoAfter
  createdAt: string;
}

// 3. Cập nhật PaymentResult để chứa cả 2 loại trên
export interface PaymentResult {
  isSuccess: boolean;
  message: string;
  rspCode: string;
  order?: {
    id: number;
    orderNo: string;
    total: number;
    status: string;
    paidAt?: string;
    createdAt?: string;
  };
  // Cấu trúc mới khớp với Backend C#
  transaction?: {
    wallet?: WalletTransactionDetail;
    promo?: PromoTransactionDetail | null; // Có thể null nếu không có khuyến mãi
  };
}