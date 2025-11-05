
export interface CreatePaymentPayload {
  amount: number;
}

export interface PaymentUrlResponse {
  paymentUrl: string;
}

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
  transaction?: {
    id: number;
    amount: number;
    direction: string;
    source: string;
    balanceAfter: number;
    createdAt: string;
  };
}