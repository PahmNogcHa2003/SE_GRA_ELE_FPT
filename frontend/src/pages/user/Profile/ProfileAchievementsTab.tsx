    // src/pages/profile/ProfileAchievementsTab.tsx
    import React, { useMemo } from "react";
    import {
    Card,
    Col,
    Row,
    Typography,
    Statistic,
    Tooltip,
    Empty,
    List,
    Tag,
    Skeleton,
    App,
    } from "antd";
    import {
    EnvironmentOutlined,
    ClockCircleOutlined,
    FireOutlined,
    CloudOutlined,
    ThunderboltOutlined,
    TrophyFilled,
    } from "@ant-design/icons";
    import { useQuery } from "@tanstack/react-query";
    import type { UserProfileDTO } from "../../../types/userProfile";
    import type { RentalHistoryDTO } from "../../../types/rental.history";
    import { getMyRentalHistory } from "../../../services/rentalStats.service";
    import dayjs from "dayjs";

    const { Title, Text } = Typography;

    interface ProfileAchievementsTabProps {
    profile: UserProfileDTO | null;
    }

    // ====== Helpers ======
    const formatDuration = (totalMinutes: number | undefined | null) => {
    const m = totalMinutes ?? 0;
    const hours = Math.floor(m / 60);
    const mins = m % 60;
    if (hours <= 0) return `${mins} phút`;
    return `${hours} giờ ${mins} phút`;
    };

    const formatNumber = (value: number | undefined | null, fractionDigits = 1) => {
    const num = value ?? 0;
    return num.toLocaleString("vi-VN", {
        minimumFractionDigits: fractionDigits,
        maximumFractionDigits: fractionDigits,
    });
    };

    const ProfileAchievementsTab: React.FC<ProfileAchievementsTabProps> = ({
    profile,
    }) => {
    const { message } = App.useApp();

    // 1. Logic Cấp độ (Thêm style background trực tiếp)
    const achievementLevel = useMemo(() => {
        const d = profile?.totalDistanceKm ?? 0;
        if (d >= 200)
        return {
            label: "Eco Master",
            style: { background: "linear-gradient(to right, #fbbf24, #d97706)" }, // Vàng -> Cam đậm
        };
        if (d >= 100)
        return {
            label: "Eco Rider",
            style: { background: "linear-gradient(to right, #34d399, #059669)" }, // Xanh ngọc -> Xanh lá đậm
        };
        if (d >= 20)
        return {
            label: "Eco Explorer",
            style: { background: "linear-gradient(to right, #60a5fa, #4f46e5)" }, // Xanh dương -> Tím xanh
        };
        return {
        label: "Newbie",
        style: { background: "linear-gradient(to right, #9ca3af, #4b5563)" }, // Xám
        };
    }, [profile?.totalDistanceKm]);

    // 2. Query Lịch sử
    const {
        data: history,
        isLoading: loadingHistory,
        isError: isErrorHistory,
    } = useQuery<RentalHistoryDTO[]>({
        queryKey: ["myRentalHistory"],
        queryFn: getMyRentalHistory,
    });

    if (isErrorHistory) {
        message.error("Không tải được lịch sử chuyến đi");
    }

    // 3. Component Card Thống kê (SỬA LẠI: Dùng style background trực tiếp để tránh bị đè)
    const StatCard = ({ title, value, icon, background, suffix, tooltip }: any) => (
        <Card
        bordered={false}
        className="rounded-2xl shadow-lg transform hover:-translate-y-1 transition-all duration-300 text-white border-0 overflow-hidden"
        style={{
            background: background, // Ép buộc nhận màu nền này
        }}
        bodyStyle={{ padding: "20px" }}
        >
        <div className="flex items-center justify-between mb-2 opacity-90">
            <span className="text-sm font-medium text-white/90">{title}</span>
            <span className="text-lg bg-white/20 p-2 rounded-full backdrop-blur-sm">
            {icon}
            </span>
        </div>
        <Tooltip title={tooltip}>
            <Statistic
            value={value}
            suffix={
                <span className="text-sm text-white/80 ml-1">{suffix}</span>
            }
            valueStyle={{ color: "white", fontWeight: 700, fontSize: "28px" }}
            />
        </Tooltip>
        {/* Icon nền mờ trang trí */}
        <div className="absolute -bottom-4 -right-4 text-white opacity-10 text-8xl pointer-events-none">
            {icon}
        </div>
        </Card>
    );

    return (
        <div className="space-y-6">
        {/* ================= PHẦN 1: BANNER CẤP ĐỘ ================= */}
        <div
            className="rounded-3xl p-6 text-white shadow-xl relative overflow-hidden"
            style={achievementLevel.style} // Dùng style trực tiếp
        >
            <div className="relative z-10 flex items-center justify-between">
            <div>
                <Text className="text-white/90 text-sm uppercase tracking-wider font-semibold">
                Cấp độ hiện tại
                </Text>
                <Title
                level={2}
                className="text-white! m-0! mt-1! flex items-center gap-3"
                >
                <TrophyFilled /> {achievementLevel.label}
                </Title>
                <Text className="text-white/80 mt-2 block max-w-lg">
                {achievementLevel.label === "Newbie"
                    ? "Chào mừng bạn! Hãy bắt đầu chuyến đi đầu tiên."
                    : "Bạn đang làm rất tốt! Hãy tiếp tục hành trình xanh."}
                </Text>
            </div>
            <div className="hidden md:block">
                <div className="w-24 h-24 bg-white/20 rounded-full flex flex-col items-center justify-center backdrop-blur-md shadow-inner border border-white/30">
                <span className="text-2xl font-bold">
                    {formatNumber(profile?.totalDistanceKm, 0)}
                </span>
                <span className="text-[10px] uppercase">km</span>
                </div>
            </div>
            </div>
            <div className="absolute top-0 right-0 w-64 h-64 bg-white opacity-10 rounded-full blur-3xl -translate-y-1/2 translate-x-1/2"></div>
        </div>

        {/* ================= PHẦN 2: GRID THỐNG KÊ (ĐÃ FIX MÀU) ================= */}
        <Row gutter={[16, 16]}>
            <Col xs={24} sm={12} md={8}>
            <StatCard
                title="Tổng quãng đường"
                value={formatNumber(profile?.totalDistanceKm, 1)}
                suffix="km"
                icon={<ThunderboltOutlined />}
                background="linear-gradient(135deg, #34d399 0%, #0d9488 100%)" // Emerald -> Teal
            />
            </Col>
            <Col xs={24} sm={12} md={8}>
            <StatCard
                title="CO₂ Tiết kiệm"
                value={formatNumber(profile?.totalCo2SavedKg, 2)}
                suffix="kg"
                icon={<CloudOutlined />}
                tooltip="Lượng khí thải giảm được so với xe máy"
                background="linear-gradient(135deg, #4ade80 0%, #15803d 100%)" // Green -> Dark Green
            />
            </Col>
            <Col xs={24} sm={12} md={8}>
            <StatCard
                title="Calo tiêu thụ"
                value={formatNumber(profile?.totalCaloriesBurned, 0)}
                suffix="kcal"
                icon={<FireOutlined />}
                background="linear-gradient(135deg, #fb923c 0%, #dc2626 100%)" // Orange -> Red
            />
            </Col>
            <Col xs={24} sm={12} md={12}>
            <StatCard
                title="Tổng thời gian"
                value={
                formatDuration(profile?.totalDurationMinutes).split(" ")[0]
                }
                suffix={formatDuration(profile?.totalDurationMinutes).substring(
                formatDuration(profile?.totalDurationMinutes).indexOf(" ")
                )}
                icon={<ClockCircleOutlined />}
                background="linear-gradient(135deg, #60a5fa 0%, #4338ca 100%)" // Blue -> Indigo
            />
            </Col>
            <Col xs={24} sm={12} md={12}>
            <StatCard
                title="Số chuyến đi"
                value={profile?.totalTrips ?? 0}
                suffix="chuyến"
                icon={<EnvironmentOutlined />}
                background="linear-gradient(135deg, #c084fc 0%, #9333ea 100%)" // Purple -> Violet
            />
            </Col>
        </Row>

        {/* ================= PHẦN 3: LỊCH SỬ CHUYẾN ĐI (GIỮ NGUYÊN) ================= */}
        <Card
            className="shadow-md rounded-2xl border-0"
            title={
            <span className="text-lg font-bold text-gray-800">
                Lịch sử hành trình
            </span>
            }
            headStyle={{ borderBottom: "1px solid #f0f0f0" }}
        >
            {loadingHistory ? (
            <Skeleton active paragraph={{ rows: 6 }} />
            ) : !history || history.length === 0 ? (
            <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description="Bạn chưa có chuyến đi nào."
            />
            ) : (
            <List
                itemLayout="vertical"
                dataSource={history}
                className="max-h-[500px] overflow-y-auto pr-2 custom-scrollbar"
                renderItem={(rental) => (
                <List.Item className="px-0! py-4! border-b border-gray-100 hover:bg-gray-50/50 transition-colors rounded-lg">
                    <div className="flex flex-col gap-2 text-xs md:text-sm w-full">
                    
                    {/* Dòng 1: Trạm + Trạng thái */}
                    <div className="flex flex-wrap items-center justify-between gap-2">
                        <div className="flex items-center gap-2">
                            <div className="w-8 h-8 rounded-full bg-emerald-100 flex items-center justify-center text-emerald-600 shrink-0">
                                <EnvironmentOutlined />
                            </div>
                            <span className="font-semibold text-gray-800 text-sm">
                            {rental.startStationName || "Trạm bắt đầu"}{" "}
                            <span className="mx-1 text-gray-400">→</span>
                            {rental.endStationName || "Chưa kết thúc"}
                            </span>
                        </div>
                        <Tag
                        color={
                            rental.status === "End"
                            ? "green"
                            : rental.status === "Cancelled"
                            ? "red"
                            : rental.status === "Ongoing"
                            ? "blue"
                            : "default"
                        }
                        className="uppercase font-bold text-[10px]"
                        >
                        {rental.status}
                        </Tag>
                    </div>

                    {/* Dòng 2: Thời gian + Vé */}
                    <div className="pl-10 flex flex-wrap justify-between gap-2 text-[11px] text-gray-500">
                        <div className="space-x-3">
                            <span>
                            Bắt đầu:{" "}
                            <strong className="text-gray-700">
                                {dayjs(rental.startTimeVn)
                                .utc()
                                .format("HH:mm [ngày] DD/MM/YYYY")}
                            </strong>
                            </span>
                            {rental.endTimeVn && (
                            <span>
                                Kết thúc:{" "}
                                <strong className="text-gray-700">
                                {dayjs(rental.endTimeVn)
                                    .utc()
                                    .format("HH:mm [ngày] DD/MM/YYYY")}
                                </strong>
                            </span>
                            )}
                        </div>
                        
                        {rental.ticketPlanName && (
                        <span className="bg-emerald-50 text-emerald-700 px-2 py-0.5 rounded border border-emerald-100">
                            Vé: <strong>{rental.ticketPlanName}</strong>{" "}
                            {rental.ticketType && `(${rental.ticketType})`}
                        </span>
                        )}
                    </div>

                    {/* Dòng 3: Số liệu km / phút / overtime */}
                    <div className="pl-10 flex flex-wrap gap-4 text-[11px] text-gray-600 mt-1">
                        <span className="flex items-center gap-1">
                        <ThunderboltOutlined className="text-amber-500"/>
                        Quãng đường:{" "}
                        <strong>
                            {formatNumber(rental.distanceKm ?? 0, 1)} km
                        </strong>
                        </span>
                        <span className="flex items-center gap-1">
                        <ClockCircleOutlined className="text-blue-500"/>
                        Thời lượng:{" "}
                        <strong>
                            {formatDuration(rental.durationMinutes ?? 0)}
                        </strong>
                        </span>

                        {rental.isOvertime && (
                        <span className="text-red-500 bg-red-50 px-2 rounded flex items-center gap-1 border border-red-100">
                            ⚠️ Quá thời gian
                            {rental.overusedMinutes
                            ? ` +${rental.overusedMinutes} phút`
                            : ""}
                            {rental.overusedFee
                            ? ` • Phí thêm: ${formatNumber(
                                rental.overusedFee,
                                0
                                )}đ`
                            : ""}
                        </span>
                        )}
                        {rental.vehicleType && (
                        <span className="bg-amber-50 text-amber-700 px-2 py-0.5 rounded border border-amber-100 flex items-center gap-1">
                            Loại xe: <strong>{rental.vehicleType}</strong>
                        </span>
                        )}
                    </div>
                    </div>
                </List.Item>
                )}
            />
            )}
        </Card>
        </div>
    );
    };

    export default ProfileAchievementsTab;