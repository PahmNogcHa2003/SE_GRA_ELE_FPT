// src/types/quest.ts
export interface QuestDTO {
id: number;
code: string;
title: string;
description?: string;
questType: string;
scope: string;

targetDistanceKm?: number | null;
targetTrips?: number | null;
targetDurationMinutes?: number | null;

promoReward: number;

startAt: string;
endAt: string;
updatedAt?: string | null;
status: string;

currentDistanceKm: number;
currentTrips: number;
currentDurationMinutes: number;

isCompleted: boolean;
completedAt?: string | null;
rewardClaimedAt?: string | null;

progressPercent: number;
}

export interface QuestCompletedDTO {
  questId: number; 
  code: string;
  title: string;
  
  finalDistanceKm: number;
  finalTrips: number;
  finalDurationMinutes: number;
  
  rewardValue: number; 
  
  completedAt: string;
  rewardClaimedAt?: string;
}
