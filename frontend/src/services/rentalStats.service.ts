    // src/services/rentalStats.service.ts
    import { httpUser } from "./http";
    import type {
    RentalStatsSummaryDTO,
    LeaderboardEntryDTO,
    RentalHistoryDTO,
    } from "../types/rental.history";
    import type { ApiResponse } from "../types/api";

    // 📊 Lấy thống kê tóm tắt của người dùng hiện tại
    export const getMyRentalStatsSummary = async (): Promise<RentalStatsSummaryDTO> => {
    const res = await httpUser.get<ApiResponse<RentalStatsSummaryDTO>>(
        "/Rentals/stats/summary"
    );
    if (!res.data.success) {
        throw new Error(res.data.message || "Không thể lấy thống kê người dùng");
    }
    return res.data.data!;
    };

    // 🏆 Lấy bảng xếp hạng theo period
    export const getLeaderboard = async (
    period: string = "lifetime",
    topN: number = 10
    ): Promise<LeaderboardEntryDTO[]> => {
    const res = await httpUser.get<ApiResponse<LeaderboardEntryDTO[]>>(
        "/Leaderboard",
        {
        params: { period, topN },
        }
    );
    if (!res.data.success) {
        throw new Error(res.data.message || "Không thể lấy bảng xếp hạng");
    }
    return res.data.data || [];
    };
    
    export const getMyRentalHistory = async (): Promise<RentalHistoryDTO[]> => {
    const res = await httpUser.get<ApiResponse<RentalHistoryDTO[]>>(
        "/Rentals/history"
    );
    if (!res.data.success) {
        throw new Error(res.data.message || "Không thể lấy lịch sử chuyến đi");
    }
    return res.data.data || [];
};
