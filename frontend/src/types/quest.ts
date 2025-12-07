// src/types/quest.ts
export interface QuestDTO {
id: number;
code: string;
title: string;
description?: string;
  /** "Distance" | "Trips" | "Duration" */
questType: string;
  /** "Daily" | "Weekly" | "Monthly" | "OneTime" */
scope: string;

targetDistanceKm?: number | null;
targetTrips?: number | null;
targetDurationMinutes?: number | null;

promoReward: number;

startAt: string;
endAt: string;
updatedAt?: string | null;
status: string;

  // ==== Tiến độ của user hiện tại ====
currentDistanceKm: number;
currentTrips: number;
currentDurationMinutes: number;

isCompleted: boolean;
completedAt?: string | null;
rewardClaimedAt?: string | null;

  /** % hoàn thành (0–100) */
progressPercent: number;
}
