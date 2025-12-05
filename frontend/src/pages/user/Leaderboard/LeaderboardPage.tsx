    // src/pages/leaderboard/LeaderboardPage.tsx
    import React, { useMemo, useState } from "react";
    import {
    App,
    Card,
    Col,
    Row,
    Typography,
    Segmented,
    Table,
    Tag,
    Avatar,
    Skeleton,
    Empty,
    } from "antd";
    import { useQuery } from "@tanstack/react-query";
    import {
    CrownOutlined,
    TrophyOutlined,
    FireOutlined,
    UserOutlined,
    } from "@ant-design/icons";
    import { useAuth } from "../../../features/auth/context/authContext";
    import { getLeaderboard } from "../../../services/rentalStats.service";
    import type { LeaderboardEntryDTO } from "../../../types/rental.history";

    const { Title, Text } = Typography;

    type PeriodKey = "week" | "month" | "lifetime";

    const periodOptions: { label: string; value: PeriodKey; desc: string }[] = [
    { label: "7 ngày", value: "week", desc: "7 ngày gần nhất" },
    { label: "30 ngày", value: "month", desc: "30 ngày gần nhất" },
    { label: "Tích lũy", value: "lifetime", desc: "Toàn bộ thời gian sử dụng" },
    ];

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

    // styling riêng cho top 1 / 2 / 3
    const getTopCardClass = (rank: number) => {
    if (rank === 1)
        return "rounded-2xl shadow-md border border-amber-300 bg-amber-50";
    if (rank === 2)
        return "rounded-2xl shadow-sm border border-slate-300 bg-slate-50";
    if (rank === 3)
        return "rounded-2xl shadow-sm border border-orange-200 bg-orange-50";
    return "rounded-2xl shadow-sm border border-gray-100 bg-white";
    };

    const LeaderboardPage: React.FC = () => {
    const { message } = App.useApp();
    const { user } = useAuth(); // chỉ để highlight row của chính mình, không bắt buộc đăng nhập
    const [period, setPeriod] = useState<PeriodKey>("lifetime");

    // Chỉ gọi leaderboard – public, không cần login
    const {
        data: leaderboard,
        isLoading: loadingLeaderboard,
        isError: isErrorLeaderboard,
    } = useQuery({
        queryKey: ["leaderboard", period],
        queryFn: () => getLeaderboard(period, 20),
    });

    const isLoading = loadingLeaderboard;

    const topThree = useMemo(() => {
        if (!leaderboard || leaderboard.length === 0) return [];
        return leaderboard.slice(0, 3);
    }, [leaderboard]);

    const columns = [
        {
        title: "#",
        dataIndex: "rank",
        key: "rank",
        width: 70,
        render: (_: any, record: LeaderboardEntryDTO) => {
            if (record.rank === 1) {
            return <CrownOutlined style={{ color: "#f59e0b", fontSize: 18 }} />;
            }
            if (record.rank === 2) {
            return <TrophyOutlined style={{ color: "#9ca3af", fontSize: 18 }} />;
            }
            if (record.rank === 3) {
            return <TrophyOutlined style={{ color: "#b45309", fontSize: 18 }} />;
            }
            return <span>{record.rank}</span>;
        },
        },
        {
        title: "Người dùng",
        dataIndex: "fullName",
        key: "fullName",
        render: (_: any, record: LeaderboardEntryDTO) => (
            <div className="flex items-center gap-2">
            <Avatar
                size="small"
                src={record.avatarUrl}
                icon={!record.avatarUrl && <UserOutlined />}
            />
            <span>{record.fullName || "Người dùng ẩn danh"}</span>
            </div>
        ),
        },
        {
        title: "Tổng quãng đường",
        dataIndex: "totalDistanceKm",
        key: "totalDistanceKm",
        align: "right" as const,
        render: (v: number) => `${formatNumber(v, 1)} km`,
        },
        {
        title: "Thời gian",
        dataIndex: "totalDurationMinutes",
        key: "totalDurationMinutes",
        align: "right" as const,
        render: (v: number) => formatDuration(v),
        },
        {
        title: "Số chuyến",
        dataIndex: "totalTrips",
        key: "totalTrips",
        align: "right" as const,
        },
    ];

    if (isErrorLeaderboard) {
        message.error("Không tải được bảng xếp hạng");
    }

    return (
        <div className="bg-emerald-50 py-8 min-h-screen">
        <div className="container mx-auto px-4 max-w-7xl">
            {/* HEADER */}
            <Card className="shadow-md rounded-2xl border-0 bg-white/80 backdrop-blur mb-6">
            <Row gutter={[24, 16]} align="middle">
                <Col xs={24}>
                <div className="flex flex-wrap items-center justify-between gap-4">
                    <div className="flex items-center gap-3">
                    <TrophyOutlined
                        className="text-eco-green"
                        style={{ fontSize: 28 }}
                    />
                    <div>
                        <Title level={3} className="mb-0!">
                        Bảng xếp hạng EcoJourney
                        </Title>
                        <Text type="secondary">
                        Thành tích đạp xe của tất cả người dùng trong từng giai đoạn.
                        </Text>
                    </div>
                    </div>

                    <div className="text-right">
                    <Text className="mr-2 text-sm text-gray-500">
                        Khoảng thời gian:
                    </Text>
                    <Segmented
                        size="middle"
                        value={period}
                        onChange={(value) => setPeriod(value as PeriodKey)}
                        options={periodOptions.map((p) => ({
                        label: p.label,
                        value: p.value,
                        }))}
                    />
                    <div className="mt-1">
                        <Text type="secondary" className="text-xs">
                        {periodOptions.find((x) => x.value === period)?.desc}
                        </Text>
                    </div>
                    </div>
                </div>
                </Col>
            </Row>
            </Card>

            {/* BODY: TOP 3 + TABLE */}
            {isLoading ? (
            <Card className="shadow-sm rounded-2xl border border-gray-100">
                <Skeleton active paragraph={{ rows: 6 }} />
            </Card>
            ) : !leaderboard || leaderboard.length === 0 ? (
            <Card className="shadow-sm rounded-2xl border border-gray-100">
                <Empty
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description="Chưa có dữ liệu xếp hạng cho khoảng thời gian này."
                />
            </Card>
            ) : (
            <>
                {/* TOP 3 */}
                <Row gutter={[16, 16]} className="mb-4 mt-4">
                {topThree.map((entry) => (
                    <Col xs={24} md={8} key={entry.userId}>
                    <Card className={getTopCardClass(entry.rank)}>
                        <div className="flex items-start justify-between mb-3">
                        <div className="flex items-center gap-3">
                            <Avatar
                            size={48}
                            src={entry.avatarUrl}
                            icon={!entry.avatarUrl && <UserOutlined />}
                            />
                            <div>
                            <Text strong className="block">
                                {entry.fullName || `Người dùng #${entry.userId}`}
                            </Text>
                            </div>
                        </div>

                        <div className="flex flex-col items-end gap-1">
                            {entry.rank === 1 && (
                            <CrownOutlined
                                style={{ fontSize: 26, color: "#f59e0b" }}
                            />
                            )}
                            {entry.rank === 2 && (
                            <TrophyOutlined
                                style={{ fontSize: 24, color: "#9ca3af" }}
                            />
                            )}
                            {entry.rank === 3 && (
                            <FireOutlined
                                style={{ fontSize: 24, color: "#ea580c" }}
                            />
                            )}
                            <Tag color="gold" className="mt-1">
                            TOP {entry.rank}
                            </Tag>
                        </div>
                        </div>

                        <Row gutter={[8, 8]}>
                        <Col span={12}>
                            <Text type="secondary" className="text-xs">
                            Quãng đường
                            </Text>
                            <div className="font-semibold">
                            {formatNumber(entry.totalDistanceKm, 1)} km
                            </div>
                        </Col>
                        <Col span={12}>
                            <Text type="secondary" className="text-xs">
                            Thời gian
                            </Text>
                            <div className="font-semibold">
                            {formatDuration(entry.totalDurationMinutes)}
                            </div>
                        </Col>
                        <Col span={12}>
                            <Text type="secondary" className="text-xs">
                            Số chuyến
                            </Text>
                            <div className="font-semibold">
                            {entry.totalTrips}
                            </div>
                        </Col>
                        </Row>
                    </Card>
                    </Col>
                ))}
                </Row>

                {/* TABLE full leaderboard */}
                <Card className="shadow-sm rounded-2xl border border-gray-100">
                <Title level={5}>Bảng xếp hạng chi tiết</Title>
                <Table
                    rowKey={(record: LeaderboardEntryDTO) => record.userId}
                    columns={columns}
                    dataSource={leaderboard}
                    pagination={false}
                    rowClassName={(record) =>
                    user && record.userId === user.userId
                        ? "bg-emerald-50"
                        : ""
                    }
                    className="mt-3"
                />
                </Card>
            </>
            )}
        </div>
        </div>
    );
    };

    export default LeaderboardPage;
