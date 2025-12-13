// src/types/manage.transaction.ts

export interface TransactionsDTO {
  id: number;
  userId: number;
  transactionType: 'ORDER' | 'WALLET'; // "ORDER" | "WALLET"
  
  // Order specific
  orderNo?: string;
  orderType?: string; // TicketPurchase, Topup...
  currency?: string;
  status?: string; // Pending, Paid, Cancelled...
  paidAt?: string;

  // Wallet specific
  direction?: 'In' | 'Out';
  source?: string; // Topup, RideFee...
  balanceAfter?: number;

  // Common
  amount: number;
  createdAt: string;
}

export interface TransactionQueryParams {
  page: number;
  pageSize: number;
  userId?: number;
  transactionType?: 'ORDER' | 'WALLET'; 
  orderType?: string;
  direction?: string;
  status?: string;
  from?: string; // ISO Date
  to?: string;   // ISO Date
}