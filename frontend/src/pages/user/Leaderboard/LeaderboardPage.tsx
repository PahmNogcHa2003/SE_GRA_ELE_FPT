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
  Badge,
  Progress,
  Space,
  theme,
  Divider,
} from "antd";
import { useQuery } from "@tanstack/react-query";
import {
  CrownOutlined,
  TrophyOutlined,
  UserOutlined,
  FireOutlined,
  StarOutlined,
  RiseOutlined,
  CalendarOutlined,
  EnvironmentOutlined,
  ClockCircleOutlined,
  CarOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import { useAuth } from "../../../features/auth/context/authContext";
import { getLeaderboard } from "../../../services/rentalStats.service";
import type { LeaderboardEntryDTO } from "../../../types/rental.history";

const { Title, Text } = Typography;
const { useToken } = theme;

type PeriodKey = "week" | "month" | "lifetime";

/* ======================= */
/* ===== TIME HELPERS ==== */
/* ======================= */

const getISOWeekInfo = (date = new Date()) => {
  const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
  const dayNum = d.getUTCDay() || 7;
  d.setUTCDate(d.getUTCDate() + 4 - dayNum);

  const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
  const weekNo = Math.ceil((((d.getTime() - yearStart.getTime()) / 86400000) + 1) / 7);

  const weekStart = new Date(d);
  weekStart.setUTCDate(d.getUTCDate() - 3);

  const weekEnd = new Date(weekStart);
  weekEnd.setUTCDate(weekStart.getUTCDate() + 6);

  return {
    week: weekNo,
    year: d.getUTCFullYear(),
    start: weekStart,
    end: weekEnd,
  };
};

const formatDate = (date: Date) =>
  date.toLocaleDateString("vi-VN", { day: "2-digit", month: "2-digit" });

const getPeriodDisplayText = (period: PeriodKey) => {
  const now = new Date();

  if (period === "week") {
    const w = getISOWeekInfo(now);
    return `Tuần ${w.week} (${formatDate(w.start)} – ${formatDate(w.end)})`;
  }

  if (period === "month") {
    return `Tháng ${now.getMonth() + 1}/${now.getFullYear()}`;
  }

  return "Toàn bộ thời gian";
};

/* ======================= */
/* ===== FORMATTERS ====== */
/* ======================= */

const formatDuration = (totalMinutes?: number | null) => {
  const m = totalMinutes ?? 0;
  const h = Math.floor(m / 60);
  const mins = m % 60;
  return h > 0 ? `${h} giờ ${mins} phút` : `${mins} phút`;
};

const formatNumber = (value?: number | null, digits = 1) =>
  (value ?? 0).toLocaleString("vi-VN", {
    minimumFractionDigits: digits,
    maximumFractionDigits: digits,
  });

const getMedalColor = (rank: number) => {
  switch (rank) {
    case 1: return { bg: "linear-gradient(135deg, #FFD700 0%, #FFEC8B 100%)", border: "#FFD700", text: "#B8860B" };
    case 2: return { bg: "linear-gradient(135deg, #C0C0C0 0%, #E0E0E0 100%)", border: "#C0C0C0", text: "#707070" };
    case 3: return { bg: "linear-gradient(135deg, #CD7F32 0%, #E6B87F 100%)", border: "#CD7F32", text: "#8B4513" };
    default: return { bg: "#ffffff", border: "#e8e8e8", text: "#595959" };
  }
};

const getRankIcon = (rank: number, size = 24) => {
  const style = { fontSize: size };
  switch (rank) {
    case 1: return <CrownOutlined style={{ ...style, color: "#FFD700" }} />;
    case 2: return <TrophyOutlined style={{ ...style, color: "#C0C0C0" }} />;
    case 3: return <TrophyOutlined style={{ ...style, color: "#CD7F32" }} />;
    default: return <StarOutlined style={{ ...style, color: "#8c8c8c" }} />;
  }
};

/* ======================= */
/* ===== COMPONENT ======= */
/* ======================= */

const LeaderboardPage: React.FC = () => {
  const { message } = App.useApp();
  const { user } = useAuth();
  const { token } = useToken();
  const [period, setPeriod] = useState<PeriodKey>("lifetime");

  const { data: leaderboard, isLoading, isError } = useQuery({
    queryKey: ["leaderboard", period],
    queryFn: () => getLeaderboard(period, 20),
  });

  const topThree = useMemo(
    () => leaderboard?.slice(0, 3) ?? [],
    [leaderboard]
  );

  const userRank = useMemo(() => {
    if (!user || !leaderboard) return null;
    return leaderboard.find(entry => entry.userId === user.userId);
  }, [user, leaderboard]);

  const columns = [
    {
      title: "HẠNG",
      dataIndex: "rank",
      width: 80,
      align: "center" as const,
      render: (_: any, r: LeaderboardEntryDTO) => (
        <div className="flex flex-col items-center">
          {getRankIcon(r.rank)}
          <Text strong style={{ fontSize: 16, color: getMedalColor(r.rank).text }}>
            {r.rank}
          </Text>
        </div>
      ),
    },
    {
      title: "NGƯỜI DÙNG",
      dataIndex: "fullName",
      render: (_: any, r: LeaderboardEntryDTO) => (
        <div className="flex items-center gap-3">
          <Badge 
            count={r.rank <= 3 ? r.rank : 0} 
            style={{ 
              backgroundColor: getMedalColor(r.rank).border,
              boxShadow: `0 0 0 2px ${token.colorBgContainer}`
            }}
          >
            <Avatar 
              size={42} 
              src={r.avatarUrl} 
              icon={!r.avatarUrl && <UserOutlined />}
              style={{ 
                border: `2px solid ${getMedalColor(r.rank).border}`,
                boxShadow: r.rank <= 3 ? `0 4px 12px ${getMedalColor(r.rank).border}40` : 'none'
              }}
            />
          </Badge>
          <div>
            <div className="flex items-center gap-2">
              <Text strong style={{ fontSize: 15 }}>
                {r.fullName || "Người dùng ẩn danh"}
              </Text>
              {user && r.userId === user.userId && (
                <Tag icon={<CheckCircleOutlined />} color="green">
                  Bạn
                </Tag>
              )}
            </div>
            <Text type="secondary" style={{ fontSize: 12 }}>
              @{r.fullName || "user"}
            </Text>
          </div>
        </div>
      ),
    },
    {
      title: "QUÃNG ĐƯỜNG",
      dataIndex: "totalDistanceKm",
      align: "right" as const,
      render: (v: number, r: LeaderboardEntryDTO) => (
        <div className="flex flex-col items-end">
          <div className="flex items-center gap-1">
            <EnvironmentOutlined style={{ color: token.colorPrimary }} />
            <Text strong style={{ fontSize: 15 }}>
              {formatNumber(v)} km
            </Text>
          </div>
          <Progress 
            percent={Math.min((v / (leaderboard?.[0]?.totalDistanceKm || 1)) * 100, 100)} 
            size="small" 
            showInfo={false}
            strokeColor={getMedalColor(r.rank).border}
            trailColor="#f0f0f0"
          />
        </div>
      ),
    },
    {
      title: "THỜI GIAN",
      dataIndex: "totalDurationMinutes",
      align: "right" as const,
      render: (v: number) => (
        <div className="flex items-center justify-end gap-1">
          <ClockCircleOutlined style={{ color: token.colorPrimary }} />
          <Text>{formatDuration(v)}</Text>
        </div>
      ),
    },
    {
      title: "SỐ CHUYẾN",
      dataIndex: "totalTrips",
      align: "right" as const,
      render: (v: number) => (
        <div className="flex items-center justify-end gap-1">
          <CarOutlined style={{ color: token.colorPrimary }} />
          <Text strong>{v}</Text>
        </div>
      ),
    },
    // ĐÃ LOẠI BỎ CỘT ĐIỂM
  ];

  if (isError) {
    message.error("Không tải được bảng xếp hạng");
  }

  return (
    <div 
      className="min-h-screen py-8"
      style={{
        background: "linear-gradient(135deg, #f0f9ff 0%, #e6fffa 50%, #f0f9ff 100%)",
        backgroundAttachment: "fixed",
      }}
    >
      <div className="max-w-7xl mx-auto px-4">
        {/* HEADER */}
        <Card 
          className="rounded-2xl mb-8"
          style={{
            background: "linear-gradient(135deg, #ffffff 0%, #f8fafc 100%)",
            border: `1px solid ${token.colorBorderSecondary}`,
            boxShadow: "0 8px 32px rgba(0, 100, 80, 0.08)",
          }}
        >
          <Row justify="space-between" align="middle" wrap={false}>
            <Col flex="auto">
              <div className="flex items-center gap-3 mb-2">
                <div style={{
                  width: 48,
                  height: 48,
                  borderRadius: 12,
                  background: "linear-gradient(135deg, #10b981 0%, #059669 100%)",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  boxShadow: "0 4px 12px rgba(16, 185, 129, 0.3)"
                }}>
                  <TrophyOutlined style={{ fontSize: 24, color: "white" }} />
                </div>
                <div>
                  <Title level={3} className="mb-0" style={{ color: token.colorPrimary }}>
                    🏆 Bảng Xếp Hạng EcoJourney
                  </Title>
                  <Text type="secondary">
                    <CalendarOutlined className="mr-1" />
                    {getPeriodDisplayText(period)}
                  </Text>
                </div>
              </div>
            </Col>
            <Col>
              <Segmented
                value={period}
                onChange={(v) => setPeriod(v as PeriodKey)}
                options={[
                  { 
                    label: (
                      <span className="flex items-center gap-1">
                        <FireOutlined /> Tuần
                      </span>
                    ), 
                    value: "week" 
                  },
                  { 
                    label: (
                      <span className="flex items-center gap-1">
                        <RiseOutlined /> Tháng
                      </span>
                    ), 
                    value: "month" 
                  },
                  { 
                    label: (
                      <span className="flex items-center gap-1">
                        <TrophyOutlined /> Tích lũy
                      </span>
                    ), 
                    value: "lifetime" 
                  },
                ]}
                size="large"
                style={{ 
                  background: token.colorBgContainer,
                  boxShadow: `0 2px 8px ${token.colorBorder}`,
                  borderRadius: 12,
                }}
              />
            </Col>
          </Row>
        </Card>

        {/* BODY */}
        {isLoading ? (
          <Card style={{ borderRadius: 16 }}>
            <Skeleton active avatar paragraph={{ rows: 8 }} />
          </Card>
        ) : !leaderboard?.length ? (
          <Card style={{ borderRadius: 16 }}>
            <Empty 
              description="Chưa có dữ liệu xếp hạng" 
              imageStyle={{ height: 120 }}
            />
          </Card>
        ) : (
          <>
            {/* TOP 3 PODIUM */}
            <div className="relative mb-8">
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6 items-end">
                {/* Second Place */}
                <div className="order-2 md:order-1" style={{ marginTop: 60 }}>
                  <Card
                    className="rounded-2xl text-center"
                    style={{
                      background: "linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%)",
                      border: `2px solid #C0C0C0`,
                      boxShadow: "0 8px 32px rgba(192, 192, 192, 0.2)",
                    }}
                  >
                    <div className="relative mb-4">
                      <div style={{
                        position: "absolute",
                        top: -48,
                        left: "50%",
                        transform: "translateX(-50%)",
                        width: 72,
                        height: 72,
                        background: "linear-gradient(135deg, #C0C0C0 0%, #E0E0E0 100%)",
                        borderRadius: "50%",
                        border: "4px solid white",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        boxShadow: "0 4px 12px rgba(192, 192, 192, 0.4)"
                      }}>
                        <Text strong style={{ fontSize: 32, color: "white" }}>2</Text>
                      </div>
                      <Avatar
                        size={80}
                        src={topThree[1]?.avatarUrl}
                        icon={!topThree[1]?.avatarUrl && <UserOutlined />}
                        style={{ marginTop: 24 }}
                      />
                    </div>
                    <Title level={4} style={{ color: "#707070", marginBottom: 4 }}>
                      {topThree[1]?.fullName || "Người dùng"}
                    </Title>
                    <div className="space-y-2">
                      <div className="flex justify-between">
                        <Text type="secondary">Quãng đường:</Text>
                        <Text strong>{formatNumber(topThree[1]?.totalDistanceKm)} km</Text>
                      </div>
                      <div className="flex justify-between">
                        <Text type="secondary">Số chuyến:</Text>
                        <Text strong>{topThree[1]?.totalTrips}</Text>
                      </div>
                    </div>
                  </Card>
                </div>

                {/* First Place */}
                <div className="order-1 md:order-2">
                  <Card
                    className="rounded-2xl text-center"
                    style={{
                      background: "linear-gradient(135deg, #fff9db 0%, #ffec99 100%)",
                      border: `2px solid #FFD700`,
                      boxShadow: "0 12px 48px rgba(255, 215, 0, 0.3)",
                      transform: "translateY(-20px)",
                    }}
                  >
                    <div className="relative mb-6">
                      <CrownOutlined style={{
                        position: "absolute",
                        top: -60,
                        left: "50%",
                        transform: "translateX(-50%)",
                        fontSize: 48,
                        color: "#FFD700",
                        zIndex: 1,
                        filter: "drop-shadow(0 4px 12px rgba(255, 215, 0, 0.5))"
                      }} />
                      <div style={{
                        position: "absolute",
                        top: -36,
                        left: "50%",
                        transform: "translateX(-50%)",
                        width: 88,
                        height: 88,
                        background: "linear-gradient(135deg, #FFD700 0%, #FFEC8B 100%)",
                        borderRadius: "50%",
                        border: "4px solid white",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        boxShadow: "0 8px 24px rgba(255, 215, 0, 0.4)"
                      }}>
                        <Text strong style={{ fontSize: 40, color: "white" }}>1</Text>
                      </div>
                      <Avatar
                        size={96}
                        src={topThree[0]?.avatarUrl}
                        icon={!topThree[0]?.avatarUrl && <UserOutlined />}
                        style={{ marginTop: 40 }}
                      />
                    </div>
                    <Title level={3} style={{ color: "#B8860B", marginBottom: 8 }}>
                      {topThree[0]?.fullName || "Người dùng"}
                    </Title>
                    <div className="space-y-3">
                      <div className="flex justify-between">
                        <Text type="secondary">Quãng đường:</Text>
                        <Text strong style={{ color: "#B8860B" }}>
                          {formatNumber(topThree[0]?.totalDistanceKm)} km
                        </Text>
                      </div>
                      <div className="flex justify-between">
                        <Text type="secondary">Số chuyến:</Text>
                        <Text strong style={{ color: "#B8860B" }}>
                          {topThree[0]?.totalTrips}
                        </Text>
                      </div>
                      <div className="flex justify-between">
                        <Text type="secondary">Thời gian:</Text>
                        <Text>{formatDuration(topThree[0]?.totalDurationMinutes)}</Text>
                      </div>
                    </div>
                  </Card>
                </div>

                {/* Third Place */}
                <div className="order-3 md:order-3" style={{ marginTop: 60 }}>
                  <Card
                    className="rounded-2xl text-center"
                    style={{
                      background: "linear-gradient(135deg, #ffedd5 0%, #fed7aa 100%)",
                      border: `2px solid #CD7F32`,
                      boxShadow: "0 8px 32px rgba(205, 127, 50, 0.2)",
                    }}
                  >
                    <div className="relative mb-4">
                      <div style={{
                        position: "absolute",
                        top: -48,
                        left: "50%",
                        transform: "translateX(-50%)",
                        width: 72,
                        height: 72,
                        background: "linear-gradient(135deg, #CD7F32 0%, #E6B87F 100%)",
                        borderRadius: "50%",
                        border: "4px solid white",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        boxShadow: "0 4px 12px rgba(205, 127, 50, 0.4)"
                      }}>
                        <Text strong style={{ fontSize: 32, color: "white" }}>3</Text>
                      </div>
                      <Avatar
                        size={80}
                        src={topThree[2]?.avatarUrl}
                        icon={!topThree[2]?.avatarUrl && <UserOutlined />}
                        style={{ marginTop: 24 }}
                      />
                    </div>
                    <Title level={4} style={{ color: "#8B4513", marginBottom: 4 }}>
                      {topThree[2]?.fullName || "Người dùng"}
                    </Title>
                    <div className="space-y-2">
                      <div className="flex justify-between">
                        <Text type="secondary">Quãng đường:</Text>
                        <Text strong>{formatNumber(topThree[2]?.totalDistanceKm)} km</Text>
                      </div>
                      <div className="flex justify-between">
                        <Text type="secondary">Số chuyến:</Text>
                        <Text strong>{topThree[2]?.totalTrips}</Text>
                      </div>
                    </div>
                  </Card>
                </div>
              </div>
            </div>

            {/* USER RANK CARD (if not in top 3) */}
            {userRank && userRank.rank > 3 && (
              <Card
                className="rounded-2xl mb-6"
                style={{
                  background: "linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%)",
                  border: `2px solid ${token.colorPrimary}`,
                }}
              >
                <Row align="middle" justify="space-between">
                  <Col>
                    <div className="flex items-center gap-3">
                      <Avatar 
                        size={48} 
                        src={user?.avatarUrl} 
                        icon={!user?.avatarUrl && <UserOutlined />}
                        style={{ border: `2px solid ${token.colorPrimary}` }}
                      />
                      <div>
                        <div className="flex items-center gap-2">
                          <Text strong style={{ fontSize: 16 }}>
                            Vị trí của bạn
                          </Text>
                          <Tag color="green" icon={<CheckCircleOutlined />}>
                            Hạng {userRank.rank}
                          </Tag>
                        </div>
                        <Text type="secondary">
                          {userRank.fullName || user?.email}
                        </Text>
                      </div>
                    </div>
                  </Col>
                  <Col>
                    <Space size="large">
                      <div className="text-center">
                        <Text type="secondary">Quãng đường</Text>
                        <div className="flex items-center gap-1">
                          <EnvironmentOutlined />
                          <Text strong>{formatNumber(userRank.totalDistanceKm)} km</Text>
                        </div>
                      </div>
                      <Divider type="vertical" />
                      <div className="text-center">
                        <Text type="secondary">Số chuyến</Text>
                        <div className="flex items-center gap-1">
                          <CarOutlined />
                          <Text strong>{userRank.totalTrips}</Text>
                        </div>
                      </div>
                      <Divider type="vertical" />
                      <div className="text-center">
                        <Text type="secondary">Thời gian</Text>
                        <div className="flex items-center gap-1">
                          <ClockCircleOutlined />
                          <Text>{formatDuration(userRank.totalDurationMinutes)}</Text>
                        </div>
                      </div>
                    </Space>
                  </Col>
                </Row>
              </Card>
            )}

            {/* LEADERBOARD TABLE */}
            <Card
              className="rounded-2xl"
              style={{
                border: `1px solid ${token.colorBorderSecondary}`,
                boxShadow: "0 4px 24px rgba(0, 0, 0, 0.05)",
              }}
            >
              <div className="mb-6">
                <Title level={5} style={{ color: token.colorPrimary, marginBottom: 4 }}>
                  📊 Bảng Xếp Hạng Chi Tiết
                </Title>
                <Text type="secondary">
                  Hiển thị {leaderboard.length} người dùng hàng đầu
                </Text>
              </div>
              
              <Table
                rowKey="userId"
                columns={columns}
                dataSource={leaderboard}
                pagination={false}
                rowClassName={(r) =>
                  user && r.userId === user.userId
                    ? "ant-table-row-selected"
                    : ""
                }
                onRow={(record) => ({
                  style: {
                    background: record.rank === userRank?.rank 
                      ? token.colorPrimaryBg 
                      : 'transparent',
                    transition: 'all 0.3s',
                  },
                })}
              />
            </Card>
          </>
        )}
      </div>
    </div>
  );
};

export default LeaderboardPage;