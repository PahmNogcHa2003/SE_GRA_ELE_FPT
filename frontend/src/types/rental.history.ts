// src/types/rental.history.ts

// Map với C# RentalHistoryDTO
export interface RentalHistoryDTO {
rentalId: number;

startTimeUtc: string;
endTimeUtc?: string | null;
startTimeVn: string;
endTimeVn?: string | null;

startStationName?: string | null;
endStationName?: string | null;
vehicleCode?: string | null;
vehicleType?: string | null;

userTicketId?: number | null;
ticketPlanName?: string | null;
ticketType?: string | null;
ticketVehicleType?: string | null;

durationMinutes?: number | null;
distanceKm?: number | null;
co2SavedKg?: number | null;
caloriesBurned?: number | null;
overusedMinutes?: number | null;
overusedFee?: number | null;
isOvertime: boolean;

status: string; 
}

export interface RentalStatsSummaryDTO {
totalDistanceKm: number;
totalTrips: number;
totalDurationMinutes: number;
totalCo2SavedKg: number;
totalCaloriesBurned: number;
}


export interface LeaderboardEntryDTO {
  userId: number;
  fullName?: string | null;
  avatarUrl?: string | null;
  totalDistanceKm: number;
  totalDurationMinutes: number;
  totalTrips: number;
  rank: number;

  weekNumber?: number | null;
  month?: number | null;
  year?: number | null;
}
