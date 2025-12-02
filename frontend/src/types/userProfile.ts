import type { RentalHistoryDTO } from "./rental.history";

export interface UserProfileDTO {
userId: number;
isVerify?: string | null;
fullName?: string | null;
phoneNumber?: string | null;
email?: string | null;
dob?: string | null;
gender?: string | null;
avatarUrl?: string | null;
emergencyName?: string | null;
emergencyPhone: string;
provinceCode?: number | null;
provinceName?: string | null;
wardCode?: number | null;
wardName?: string | null;
addressDetail?: string | null;
numberCard: string;
placeOfOrigin?: string | null;
placeOfResidence?: string | null;
issuedDate?: string | null;
expiryDate?: string | null;
issuedBy?: string | null;

totalDistanceKm: number;
totalTrips: number;
totalDurationMinutes: number;
totalCo2SavedKg: number;
totalCaloriesBurned: number;

rentalHistories: RentalHistoryDTO[];
createdAt: string;
updatedAt: string;
}

export type UpdateUserProfileBasicDTO = {
fullName?: string | null;
avatarUrl?: string | null;
emergencyName : string;
emergencyPhone: string;
addressDetail?: string | null;
dob?: string | null;
gender?: string | null;
phoneNumber?: string | null;
};