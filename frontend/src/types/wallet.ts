export interface Wallet {
  id: number;
  userId: number;
  balance: number;
  totalDebt: number; 
  status: string;
  createdAt: string;
  updatedAt: string;
}


export interface WalletTransaction {
  id: number;
  walletId: number;
  direction: string; 
  amount: number;
  source: string; 
  balanceAfter: number;
  createdAt: string; 
}