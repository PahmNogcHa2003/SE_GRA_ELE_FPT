// Khớp với TicketPlanDTO.cs
export interface TicketPlan {
  id: number;
  code?: string;
  type?: string;
  name?: string;
  description?: string;
  isActive: boolean;
}
// Khớp với CreateTicketPlanDTO.cs
export interface CreateTicketPlan {
  code: string;
  type: string;
  name: string;
  description?: string;
  isActive: boolean;
}
// Khớp với UpdateTicketPlanDTO.cs
export interface UpdateTicketPlan extends CreateTicketPlan {}

// Khớp với TicketPlanPriceDTO.cs
export interface TicketPlanPrice {
  id: number;
  planId: number;
  planName: string;
  vehicleType: string;
  price: number;
  durationLimitMinutes?: number;
  dailyFreeDurationMinutes?: number;
  validityDays?: number;
  overageFeePer15Min?: number;
  isActive: boolean;
  validFrom?: string;
  validTo?: string;
}
// Khớp với CreateTicketPlanPriceDTO.cs
export interface CreateTicketPlanPrice {
  planId: number;
  vehicleType?: string;
  price: number;
  durationLimitMinutes?: number;
  dailyFreeDurationMinutes?: number;
  validityDays?: number;
  overageFeePer15Min?: number;
  isActive: boolean;
  validFrom?: string;
  validTo?: string;
}
// Khớp với UpdateTicketPlanPriceDTO.cs
export interface UpdateTicketPlanPrice {
  vehicleType?: string;
  price?: number;
  durationLimitMinutes?: number;
  dailyFreeDurationMinutes?: number;
  validityDays?: number;
  overageFeePer15Min?: number;
  isActive?: boolean;
  validFrom?: string;
  validTo?: string;
}

// Khớp với ManageUserTicketDTO.cs
export interface ManageUserTicket {
  id: number;
  userId: number;
  userEmail: string;
  planPriceId: number;
  planName: string;
  serialCode?: string;
  purchasedPrice?: number;
  status: string;
  createdAt: string;
  activatedAt?: string;
  expiresAt?: string;
  remainingMinutes?: number;
}