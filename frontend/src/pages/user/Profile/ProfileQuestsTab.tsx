// src/pages/profile/ProfileQuestsTab.tsx
import React from "react";
import {
  Card,
  Col,
  Row,
  Typography,
  Tag,
  Empty,
  Skeleton,
  Tooltip,
  Progress,
  Tabs,
  Button,
} from "antd";
import {
  ClockCircleOutlined,
  ThunderboltOutlined,
  GiftOutlined,
  CalendarOutlined,
  FlagOutlined,
  HistoryOutlined,
  CheckCircleOutlined,
  ReloadOutlined,
} from "@ant-design/icons";
import { useQuery } from "@tanstack/react-query";
import type { QuestDTO, QuestCompletedDTO } from "../../../types/quest";
import {
  getMyActiveQuests,
  getMyQuestCompleted,
} from "../../../services/quest.service";
import dayjs from "dayjs";

const { Title, Text } = Typography;

const formatCurrency = (value?: number | null) =>
  (value ?? 0).toLocaleString("vi-VN", { style: "currency", currency: "VND" });

const ProfileQuestsTab: React.FC = () => {
  // [REMOVED] Bỏ App.useApp() và message vì không dùng nữa

  // 1. Query Active
  const {
    data: activeQuests,
    isLoading: isLoadingActive,
    isError: isErrorActive,
    refetch: refetchActive, // Lấy hàm refetch để cho user thử lại nếu muốn
  } = useQuery<QuestDTO[]>({
    queryKey: ["myActiveQuests"],
    queryFn: getMyActiveQuests,
    retry: 1, 
  });

  // 2. Query History
  const {
    data: historyQuests,
    isLoading: isLoadingHistory,
    isError: isErrorHistory,
    refetch: refetchHistory,
  } = useQuery<QuestCompletedDTO[]>({
    queryKey: ["myQuestHistory"],
    queryFn: getMyQuestCompleted,
    retry: 1,
  });

  // [REMOVED] Bỏ hoàn toàn useEffect handle error message

  const renderScopeTag = (item: QuestDTO | QuestCompletedDTO) => {
    if (!("scope" in item) || !item.scope) return null;
    let color: string = "blue";
    let label = item.scope;
    const s = item.scope.toLowerCase();

    if (s === "daily") { color = "green"; label = "Hàng ngày"; }
    else if (s === "weekly") { color = "blue"; label = "Hàng tuần"; }
    else if (s === "monthly") { color = "purple"; label = "Hàng tháng"; }
    else if (s === "onetime") { color = "geekblue"; label = "Một lần"; }

    return <Tag color={color} className="font-semibold text-[11px] uppercase">{label}</Tag>;
  };

  const renderTargetText = (item: QuestDTO | QuestCompletedDTO) => {
    if ("finalDistanceKm" in item) {
       if (item.finalDistanceKm > 0) return `Tổng quãng đường: ${item.finalDistanceKm.toFixed(2)} km`;
       if (item.finalTrips > 0) return `Tổng chuyến đi: ${item.finalTrips} chuyến`;
       if (item.finalDurationMinutes > 0) return `Tổng thời gian: ${item.finalDurationMinutes} phút`;
       return "Hoàn thành nhiệm vụ";
    }
    const q = item as QuestDTO;
    switch (q.questType) {
      case "Distance": return `Quãng đường: ${(q.currentDistanceKm ?? 0).toFixed(2)} / ${q.targetDistanceKm ?? 0} km`;
      case "Trips": return `Chuyến đi: ${q.currentTrips ?? 0} / ${q.targetTrips ?? 0} chuyến`;
      case "Duration": return `Thời gian: ${q.currentDurationMinutes ?? 0} / ${q.targetDurationMinutes ?? 0} phút`;
      default: return q.description ?? "";
    }
  };

  // Hàm render
  const renderQuestGrid = (
    list: (QuestDTO | QuestCompletedDTO)[] | undefined,
    loading: boolean,
    emptyText: string,
    isHistoryMode: boolean,
    isError: boolean,
    onRefetch: () => void
  ) => {
    if (loading) return <Skeleton active paragraph={{ rows: 6 }} />;

    // [UI STATE] Nếu lỗi, hiện nút thử lại ngay trong giao diện thay vì popup
    if (isError) {
        return (
            <Card className="rounded-2xl shadow-sm border-0 bg-gray-50">
                <Empty 
                    image={Empty.PRESENTED_IMAGE_SIMPLE} 
                    description={
                        <div className="flex flex-col items-center gap-2">
                            <span className="text-gray-500">Không thể tải dữ liệu.</span>
                            <Button size="small" icon={<ReloadOutlined />} onClick={onRefetch}>Thử lại</Button>
                        </div>
                    } 
                />
            </Card>
        );
    }

    if (!list || list.length === 0) {
      return (
        <Card className="rounded-2xl shadow-sm border-0">
          <Empty description={<span>{emptyText}</span>} />
        </Card>
      );
    }

    return (
      <Row gutter={[16, 16]}>
        {list.map((item) => {
          let uniqueId: number;
          let reward: number;
          let progressPercent = 0;
          let isCompleted = false;
          let hasClaimed = false;
          let endDate: string | undefined;
          let description: string | undefined;
          let questType: string | undefined;

          if (isHistoryMode) {
            const h = item as QuestCompletedDTO;
            uniqueId = h.questId;
            reward = h.rewardValue;
            progressPercent = 100;
            isCompleted = true;
            hasClaimed = !!h.rewardClaimedAt;
          } else {
            const a = item as QuestDTO;
            uniqueId = a.id;
            reward = a.promoReward;
            progressPercent = a.progressPercent;
            isCompleted = a.isCompleted;
            hasClaimed = !!a.rewardClaimedAt;
            endDate = a.endAt;
            description = a.description;
            questType = a.questType;
          }

          let timePercent = 0;
          if (!isHistoryMode && "startAt" in item && "endAt" in item) {
             const start = dayjs(item.startAt);
             const end = dayjs(item.endAt);
             const total = end.diff(start, "second");
             const passed = Math.min(Math.max(dayjs().diff(start, "second"), 0), total);
             timePercent = total > 0 ? Math.round((passed / total) * 100) : 0;
          }

          return (
            <Col xs={24} md={12} key={`${isHistoryMode ? 'h' : 'a'}_${uniqueId}`}>
              <Card
                className={`rounded-2xl shadow-md border hover:shadow-lg transition-all duration-300 h-full flex flex-col ${
                    isHistoryMode ? "border-gray-200 bg-gray-50/50" : "border-gray-100"
                }`}
                bodyStyle={{ padding: 18, display: "flex", flexDirection: "column", height: "100%" }}
              >
                <div className="flex items-start justify-between gap-3 mb-2">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <FlagOutlined className={isHistoryMode ? "text-gray-400" : "text-emerald-500"} />
                      <Text className="text-xs text-gray-400 font-mono">{item.code}</Text>
                    </div>
                    <Title level={5} className={`mb-1! ${isHistoryMode ? "text-gray-600" : ""}`}>
                      {item.title}
                    </Title>
                    {description && <Text type="secondary" className="text-xs md:text-sm">{description}</Text>}
                  </div>
                  <div className="flex flex-col items-end gap-1 shrink-0">
                    {renderScopeTag(item)}
                    {questType && <Tag color="geekblue" className="text-[11px] uppercase mr-0">{questType}</Tag>}
                  </div>
                </div>

                <div className="mt-3 space-y-1 text-xs md:text-sm">
                  <div className={`flex items-center gap-2 ${isHistoryMode ? "text-gray-600" : "text-emerald-700"}`}>
                    <ThunderboltOutlined />
                    <span>{renderTargetText(item)}</span>
                  </div>
                  <div className={`flex items-center gap-2 ${isHistoryMode ? "text-gray-500" : "text-amber-600"}`}>
                    <GiftOutlined />
                    <span>Phần thưởng: <b>{formatCurrency(reward)} điểm</b></span>
                  </div>
                </div>

                <div className="mt-4 space-y-2 text-xs md:text-sm flex-1 flex flex-col justify-end">
                  <div className="flex items-center gap-2 text-gray-500">
                    <CalendarOutlined />
                    {isHistoryMode ? (
                        <span>Hoàn thành: <b>{dayjs(item.completedAt).format("DD/MM/YYYY HH:mm")}</b></span>
                    ) : (
                        <span>Hạn: <b>{endDate ? dayjs(endDate).format("DD/MM/YYYY") : "N/A"}</b></span>
                    )}
                  </div>

                  <Tooltip title={`Tiến độ: ${progressPercent.toFixed(0)}%`}>
                    <div className="flex items-center gap-2">
                      {isHistoryMode ? <CheckCircleOutlined className="text-green-500" /> : 
                        <ClockCircleOutlined className={isCompleted ? "text-green-500" : "text-sky-500"} />}
                      <Progress percent={Math.round(progressPercent)} size="small" strokeColor={isCompleted ? "#22c55e" : "#0ea5e9"} className="flex-1" format={() => isHistoryMode ? "Xong" : `${Math.round(progressPercent)}%`}/>
                    </div>
                  </Tooltip>

                  {!isHistoryMode && (
                    <Tooltip title="Thời gian trôi qua">
                      <div className="flex items-center gap-2 text-[11px] text-gray-500">
                        <ClockCircleOutlined className="text-gray-400" />
                        <span className="w-16 shrink-0">Thời gian:</span>
                        <Progress percent={timePercent} size="small" strokeColor="#9ca3af" className="flex-1" showInfo={false} />
                      </div>
                    </Tooltip>
                  )}

                  {isCompleted && (
                    <div className="flex items-center justify-between gap-2 mt-1 text-green-600 text-xs md:text-sm">
                      {!isHistoryMode && <span>🎉 Đã hoàn thành</span>}
                      {hasClaimed ? (
                        <Tag color="gold" className="text-[11px] font-semibold ml-auto">ĐÃ NHẬN THƯỞNG</Tag>
                      ) : (
                        <Tag color="orange" className="text-[11px] font-semibold ml-auto">CHƯA NHẬN</Tag>
                      )}
                    </div>
                  )}
                </div>
              </Card>
            </Col>
          );
        })}
      </Row>
    );
  };

  const tabsItems = [
    {
      key: "active",
      label: <span className="flex items-center gap-2"><ThunderboltOutlined /> Đang diễn ra</span>,
      children: renderQuestGrid(activeQuests, isLoadingActive, "Hiện tại bạn chưa có nhiệm vụ nào.", false, isErrorActive, refetchActive),
    },
    {
      key: "history",
      label: <span className="flex items-center gap-2"><HistoryOutlined /> Lịch sử</span>,
      children: renderQuestGrid(historyQuests, isLoadingHistory, "Bạn chưa có lịch sử.", true, isErrorHistory, refetchHistory),
    },
  ];

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between gap-3">
        <div>
          <Title level={4} className="mb-1!">Thử thách & Nhiệm vụ</Title>
          <Text type="secondary">Hoàn thành nhiệm vụ để nhận điểm thưởng EcoJourney.</Text>
        </div>
      </div>
      <Tabs defaultActiveKey="active" items={tabsItems} type="card" />
    </div>
  );
};

export default ProfileQuestsTab;