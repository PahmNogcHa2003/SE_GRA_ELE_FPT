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

// 3. [MỚI] Tách interface transaction ra để dễ tái sử dụng
export interface PaymentTransactionData {
  wallet?: WalletTransactionDetail;
  promo?: PromoTransactionDetail | null;
}

// 4. Cập nhật PaymentResult
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
  // Sử dụng interface đã tách ở trên
  transaction?: PaymentTransactionData;
}