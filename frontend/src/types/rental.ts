// src/types/rental.ts

export interface RentalListDTO {
  id: number;
  userId: number;
  userFullName?: string;
  bikeCode?: string;
  vehicleType?: string;
  startStationName?: string;
  endStationName?: string;
  startTimeUtc: string;
  endTimeUtc?: string;
  durationMinutes?: number;
  overusedFee?: number;
  status: string;
}

export interface RentalHistoryItemDTO {
  id: number;
  actionType: string;
  timestampUtc: string;
  description?: string;
  distanceKm?: number;
}

export interface RentalDetailDTO extends RentalListDTO {
  userEmail?: string;
  userPhone?: string;
  distanceKm?: number;
  
  // Ticket info
  userTicketId?: number;
  ticketPlanName?: string;
  ticketType?: string;
  ticketPlanPrice?: number;

  // Overuse
  overusedMinutes?: number;

  // Overtime Debt (Nợ quá giờ)
  overtimeOrderId?: number;
  overtimeOrderNo?: string;
  overtimeOrderStatus?: string;
  overtimeDebtAmount?: number;
  overtimeDebtRemaining?: number;
  overtimeDebtStatus?: string;

  createdAt: string;
  history: RentalHistoryItemDTO[];
}

export interface RentalFilterDTO {
  page: number;
  pageSize: number;
  status?: string;
  keyword?: string; 
  fromStartTimeUtc?: string;
  toStartTimeUtc?: string;
  fromEndTimeUtc? : string;
  toEndTimeUtc?: string;
}