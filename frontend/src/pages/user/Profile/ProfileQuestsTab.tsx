// src/pages/profile/ProfileQuestsTab.tsx
import React from "react";
import {
  App,
  Card,
  Col,
  Row,
  Typography,
  Tag,
  Empty,
  Skeleton,
  Tooltip,
  Progress,
} from "antd";
import {
  ClockCircleOutlined,
  ThunderboltOutlined,
  GiftOutlined,
  CalendarOutlined,
  FlagOutlined,
} from "@ant-design/icons";
import { useQuery } from "@tanstack/react-query";
import type { QuestDTO } from "../../../types/quest";
import { getMyActiveQuests } from "../../../services/quest.service";
import dayjs from "dayjs";

const { Title, Text } = Typography;

const formatCurrency = (value: number) =>
  value.toLocaleString("vi-VN", { style: "currency", currency: "VND" });

const ProfileQuestsTab: React.FC = () => {
  const { message } = App.useApp();

  const {
    data: quests,
    isLoading,
    isError,
  } = useQuery<QuestDTO[]>({
    queryKey: ["myActiveQuests"],
    queryFn: getMyActiveQuests,
  });

  if (isError) {
    message.error("Không tải được danh sách nhiệm vụ.");
  }

  const renderScopeTag = (scope: string) => {
    let color: string = "blue";
    let label = scope;

    const s = scope?.toLowerCase();
    if (s === "daily") {
      color = "green";
      label = "Hàng ngày";
    } else if (s === "weekly") {
      color = "blue";
      label = "Hàng tuần";
    } else if (s === "monthly") {
      color = "purple";
      label = "Hàng tháng";
    } else if (s === "onetime") {
      color = "geekblue";
      label = "Một lần";
    }

    return (
      <Tag color={color} className="font-semibold text-[11px] uppercase">
        {label}
      </Tag>
    );
  };

  const renderTargetText = (q: QuestDTO) => {
    switch (q.questType) {
      case "Distance":
        return `Quãng đường: ${q.currentDistanceKm.toFixed(2)} / ${
          q.targetDistanceKm ?? 0
        } km`;
      case "Trips":
        return `Chuyến đi: ${q.currentTrips} / ${q.targetTrips ?? 0} chuyến`;
      case "Duration":
        return `Thời gian: ${q.currentDurationMinutes} / ${
          q.targetDurationMinutes ?? 0
        } phút`;
      default:
        return q.description ?? "";
    }
  };

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between gap-3">
        <div>
          <Title level={4} className="mb-1!">
            Nhiệm vụ đang diễn ra
          </Title>
          <Text type="secondary">
            Hoàn thành nhiệm vụ để nhận điểm khuyến mãi và nâng cấp cấp độ
            EcoJourney.
          </Text>
        </div>
      </div>

      {/* Content */}
      {isLoading ? (
        <Skeleton active paragraph={{ rows: 6 }} />
      ) : !quests || quests.length === 0 ? (
        <Card className="rounded-2xl shadow-sm border-0">
          <Empty
            description={
              <span>
                Hiện tại bạn chưa có nhiệm vụ nào. Hãy quay lại sau nhé!
              </span>
            }
          />
        </Card>
      ) : (
        <Row gutter={[16, 16]}>
          {quests.map((q) => {
            // % thời gian nhiệm vụ đã trôi qua
            const now = dayjs();
            const start = dayjs(q.startAt);
            const end = dayjs(q.endAt);
            const total = end.diff(start, "second");
            const passed = Math.min(
              Math.max(now.diff(start, "second"), 0),
              total
            );
            const timePercent =
              total > 0 ? Math.round((passed / total) * 100) : 0;

            return (
              <Col xs={24} md={12} key={q.id}>
                <Card
                  className="rounded-2xl shadow-md border border-gray-100 hover:shadow-lg transition-all duration-300 h-full flex flex-col"
                  bodyStyle={{
                    padding: 18,
                    display: "flex",
                    flexDirection: "column",
                    height: "100%",
                  }}
                >
                  {/* Row 1: title + tags */}
                  <div className="flex items-start justify-between gap-3 mb-2">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <FlagOutlined className="text-emerald-500" />
                        <Text className="text-xs text-gray-400 font-mono">
                          {q.code}
                        </Text>
                      </div>
                      <Title level={5} className="mb-1!">
                        {q.title}
                      </Title>
                      <Text type="secondary" className="text-xs md:text-sm">
                        {q.description}
                      </Text>
                    </div>
                    <div className="flex flex-col items-end gap-1 shrink-0">
                      {renderScopeTag(q.scope)}
                      <Tag color="geekblue" className="text-[11px] uppercase">
                        {q.questType}
                      </Tag>
                    </div>
                  </div>

                  {/* Row 2: mục tiêu + thưởng */}
                  <div className="mt-3 space-y-1 text-xs md:text-sm">
                    <div className="flex items-center gap-2 text-emerald-700">
                      <ThunderboltOutlined />
                      <span>{renderTargetText(q)}</span>
                    </div>
                    <div className="flex items-center gap-2 text-amber-600">
                      <GiftOutlined />
                      <span>
                        Phần thưởng:{" "}
                        <b>{formatCurrency(q.promoReward)} điểm khuyến mãi</b>
                      </span>
                    </div>
                  </div>

                  {/* Row 3: thời gian + tiến độ */}
                  <div className="mt-4 space-y-2 text-xs md:text-sm">
                    <div className="flex items-center gap-2 text-gray-500">
                      <CalendarOutlined />
                      <span>
                        Từ <b>{dayjs(q.startAt).format("DD/MM/YYYY")}</b> đến{" "}
                        <b>{dayjs(q.endAt).format("DD/MM/YYYY")}</b>
                      </span>
                    </div>

                    {/* Tiến độ hoàn thành nhiệm vụ */}
                    <Tooltip
                      title={`Tiến độ: ${q.progressPercent.toFixed(1)}%${
                        q.isCompleted ? " (Đã hoàn thành)" : ""
                      }`}
                    >
                      <div className="flex items-center gap-2">
                        <ClockCircleOutlined
                          className={
                            q.isCompleted ? "text-green-500" : "text-sky-500"
                          }
                        />
                        <Progress
                          percent={Math.round(q.progressPercent)}
                          size="small"
                          strokeColor={q.isCompleted ? "#22c55e" : "#0ea5e9"}
                          className="flex-1"
                        />
                      </div>
                    </Tooltip>

                    {/* Tiến độ thời gian nhiệm vụ (phụ) */}
                    <Tooltip title="Tiến độ thời gian nhiệm vụ">
                      <div className="flex items-center gap-2 text-[11px] text-gray-500">
                        <ClockCircleOutlined className="text-gray-400" />
                        <span className="w-20 shrink-0">Thời gian:</span>
                        <Progress
                          percent={timePercent}
                          size="small"
                          strokeColor="#9ca3af"
                          className="flex-1"
                        />
                      </div>
                    </Tooltip>

                    {/* Trạng thái hoàn thành / nhận thưởng */}
                    {q.isCompleted && (
                      <div className="flex items-center gap-2 mt-1 text-green-600 text-xs md:text-sm">
                        <span>🎉 Đã hoàn thành nhiệm vụ</span>
                        {q.rewardClaimedAt && (
                          <Tag
                            color="gold"
                            className="text-[11px] font-semibold"
                          >
                            ĐÃ NHẬN THƯỞNG
                          </Tag>
                        )}
                      </div>
                    )}
                  </div>
                </Card>
              </Col>
            );
          })}
        </Row>
      )}
    </div>
  );
};

export default ProfileQuestsTab;
