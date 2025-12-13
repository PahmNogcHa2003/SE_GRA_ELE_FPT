// src/types/manage.quest.ts

export type QuestType = 'Distance' | 'Trips' | 'Duration';
export type QuestScope = 'Daily' | 'Weekly' | 'Monthly';
export type QuestStatus = 'Active' | 'Inactive';

export interface QuestDTO {
  id: number;
  code: string;
  title: string;
  description?: string;
  questType: QuestType;
  scope: QuestScope;
  
  // Targets
  targetDistanceKm?: number;
  targetTrips?: number;
  targetDurationMinutes?: number;
  
  promoReward: number;
  
  // Dates
  startAt: string;
  endAt: string;
  updatedAt?: string;
  status: QuestStatus;
}

export interface QuestCreateDTO {
  code: string;
  title: string;
  description?: string;
  questType: QuestType;
  scope: QuestScope;
  targetDistanceKm?: number;
  targetTrips?: number;
  targetDurationMinutes?: number;
  promoReward: number;
  startAt: string;
  endAt: string;
  status?: QuestStatus;
}

export interface QuestUpdateDTO extends QuestCreateDTO {}

export interface QuestFilterDTO {
  page: number;
  pageSize: number;
  search?: string;
  filterField?: string; 
  filterValue?: string;
  sort?: string;
}