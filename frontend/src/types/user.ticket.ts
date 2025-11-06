export type ActivationMode = 'IMMEDIATE' | 'ON_FIRST_USE';

export interface UserTicket {
id: number;
  planPriceId: number;
  planName: string;
  vehicleType?: string | null;
  serialCode?: string | null;
  purchasedPrice?: number | null;
  status: "Ready" | "Active" | "Expired" | "Used";
  activatedAt?: string | null;
  validFrom?: string | null;
  validTo?: string | null;
  expiresAt?: string | null;
  activationDeadline?: string | null;
  remainingMinutes?: number | null;
  remainingRides?: number | null;
  createdAt: string;
  activationMode?: ActivationMode;
}

export interface PurchaseTicketRequest {
  planPriceId: number;
}

export interface UserTicketPlanPrice {
  id: number;
  vehicleType?: string;
  price: number;
  validityDays?: number;
  durationLimitMinutes?: number;
  dailyFreeDurationMinutes?: number;
  overageFeePer15Min?: number;
  activationMode: ActivationMode;
  activationWindowDays?: number;
}

export interface UserTicketPlan {
  id: number;
  code?: string;
  name?: string;
  type?: string;
  description?: string;
  prices: UserTicketPlanPrice[];
}