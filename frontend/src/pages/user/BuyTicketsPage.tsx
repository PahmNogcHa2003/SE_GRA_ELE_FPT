import React, { useMemo, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useAuth } from "../../features/auth/context/authContext";
import { getTicketMarket, purchaseTicket } from "../../services/user.ticket.service";
import { getWallet } from "../../services/wallet.service";

import {
  App,
  Alert,
  Badge,
  Button,
  Card,
  Divider,
  Empty,
  Segmented,
  Skeleton,
  Space,
  Statistic,
  Tabs,
  Tag,
  Tooltip,
} from "antd";
import { currencyVN } from "../../utils/datetime";

// ======= local icon components (tránh thêm lib) =======
const IconBase: React.FC<{ className?: string; label?: string; children?: React.ReactNode }> = ({
  className,
  label,
  children,
}) => (
  <span role="img" aria-label={label} className={className} style={{ display: "inline-flex", alignItems: "center" }}>
    {children}
  </span>
);
const Bike: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="bike" className={className}>
    🚲
  </IconBase>
);
const BikeElectric: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="ebike" className={className}>
    🚲⚡
  </IconBase>
);
const Clock3: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="clock" className={className}>
    🕒
  </IconBase>
);
const Info: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="info" className={className}>
    ℹ️
  </IconBase>
);
const ShieldCheck: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="shield" className={className}>
    🛡️
  </IconBase>
);
const Ticket: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="ticket" className={className}>
    🎫
  </IconBase>
);
const Timer: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="timer" className={className}>
    ⏱️
  </IconBase>
);
const WalletIcon: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="wallet" className={className}>
    👛
  </IconBase>
);
const RefreshCcw: React.FC<{ className?: string }> = ({ className }) => (
  <IconBase label="refresh" className={className}>
    🔄
  </IconBase>
);

const toVehicleLabel = (v: string | undefined) => (v?.toLowerCase() === "ebike" ? "Xe điện" : "Xe đạp");

const prettyErr = (e: any) => {
  if (!e) return "Đã có lỗi xảy ra";
  if (typeof e === "string") return e;
  if (e?.message) {
    try {
      const j = JSON.parse(e.message);
      return j?.message || j?.error || e.message;
    } catch {
      return e.message;
    }
  }
  return "Đã có lỗi xảy ra";
};

const mapMode = (m: number | string | undefined): "IMMEDIATE" | "ON_FIRST_USE" =>
  m === 1 || m === "ON_FIRST_USE" ? "ON_FIRST_USE" : "IMMEDIATE";

const isSubscription = (price: any) =>
  typeof price?.validityDays === "number" && price.validityDays > 0;

const ecoGreen = {
  main: "#2E7D32",
  light: "#A5D6A7",
  dark: "#1B5E20",
  gradient: "from-emerald-100 via-emerald-50 to-white", // tailwind
};

const ecoBtnStyle: React.CSSProperties = {
  backgroundColor: ecoGreen.main,
  borderColor: ecoGreen.main,
  color: "#fff",
};

const PlanRibbon: React.FC<{ code?: string | null; type?: string | null }> = ({ code, type }) => {
  if (code === "RIDE" || type === "Ride") return <Tag color="purple">Vé lượt</Tag>;
  if (code === "DAY" || type === "Day") return <Tag color="green">Vé ngày</Tag>;
  if (type === "Month") return <Tag color="blue">Vé tháng</Tag>;
  return <Tag>Vé</Tag>;
};

const ModeBadge: React.FC<{ mode: "IMMEDIATE" | "ON_FIRST_USE" }> = ({ mode }) => (
  <Badge
    color={mode === "ON_FIRST_USE" ? "purple" : ecoGreen.main}
    text={mode === "ON_FIRST_USE" ? "Kích hoạt khi dùng" : "Kích hoạt ngay"}
  />
);

const VIcon: React.FC<{ type?: string | null; className?: string }> = ({ type, className }) =>
  type?.toLowerCase() === "ebike" ? <BikeElectric className={className} /> : <Bike className={className} />;

// ======= main page =======
const BuyTicketsPage: React.FC = () => {
  const [vehicleTab, setVehicleTab] = useState<"bike" | "ebike">("bike");

  const { isLoggedIn, isLoadingUser } = useAuth();
  const { notification, modal } = App.useApp();
  const qc = useQueryClient();

  const vehicleParam = vehicleTab === "bike" ? "bike" : "ebike";

  // ⚠️ SỬA 1: đảm bảo marketQ.data là MẢNG
  const marketQ = useQuery({
    queryKey: ["ticketMarket", vehicleParam],
    queryFn: () => getTicketMarket(vehicleParam),
    enabled: isLoggedIn && !isLoadingUser,
    select: (res: any) => {
      // nếu là AxiosResponse<ApiResponse<Plan[]>> thì:
      // res.data = ApiResponse, res.data.data = Plan[]
      const api = res?.data ?? res;
      return api?.data ?? api ?? [];
    },
  });

  const walletQ = useQuery({
    queryKey: ["wallet", isLoggedIn],
    queryFn: getWallet,
    enabled: isLoggedIn && !isLoadingUser,
    select: (res) => res.data,
    staleTime: 5 * 60 * 1000,
  });

  const purchaseMut = useMutation({
    mutationFn: purchaseTicket,
    onSuccess: (res) => {
      const data = (res as any)?.data ?? res;
      notification.success({
        message: "Mua vé thành công",
        description: `Đã thêm vé: ${data?.planName ?? "Gói vé"}`,
      });
      qc.invalidateQueries({ queryKey: ["wallet"] });
      qc.invalidateQueries({ queryKey: ["walletTransactions"] });
      qc.invalidateQueries({ queryKey: ["myActiveTickets"] });
    },
    onError: (e: any) =>
      notification.error({ message: "Mua vé thất bại", description: prettyErr(e) }),
  });

  // ⚠️ SỬA 2: phòng thủ nếu data không phải mảng
  const plansFiltered = useMemo(() => {
    const list = Array.isArray(marketQ.data) ? marketQ.data : [];

    return list
      .map((p: any) => ({
        ...p,
        prices: (Array.isArray(p.prices) ? p.prices : []).filter((pr: any) =>
          vehicleParam ? pr?.vehicleType?.toLowerCase() === vehicleParam.toLowerCase() : true
        ),
      }))
      .filter((p: any) => p.prices.length > 0);
  }, [marketQ.data, vehicleParam]);

  const onBuy = (plan: any, price: any) => {
    if (!walletQ.data) {
      notification.warning({ message: "Không thể lấy thông tin ví." });
      return;
    }
    if (walletQ.data.balance < (price.price ?? 0)) {
      notification.error({
        message: "Số dư không đủ",
        description: `Cần ${currencyVN(price.price)} nhưng ví chỉ có ${currencyVN(walletQ.data.balance)}.`,
      });
      return;
    }

    modal.confirm({
      title: `Xác nhận mua ${plan.name} – ${toVehicleLabel(price.vehicleType)}`,
      content: `Sẽ trừ ${currencyVN(price.price)} từ ví của bạn.`,
      okText: "Mua ngay",
      cancelText: "Huỷ",
      okButtonProps: { style: ecoBtnStyle },
      onOk: () => purchaseMut.mutate({ planPriceId: price.id }),
    });
  };

  // ===== VIEW 2: trang mua vé bình thường =====
  return (
    <div className={`min-h-screen bg-linear-to-b ${ecoGreen.gradient}`}>
      {/* hero */}
      <div className="container mx-auto px-4 pt-8 pb-4">
        <div className="rounded-3xl bg-white shadow-sm border border-emerald-100 p-6 flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <div className="text-2xl md:text-3xl font-bold tracking-tight text-emerald-800">
              Mua gói vé EcoJourney
            </div>
            <div className="text-gray-600 mt-1">
              Chọn gói phù hợp – thanh toán bằng ví – dùng ngay.
            </div>
            <div className="mt-3 flex items-center gap-3 text-gray-700 text-sm">
              <ShieldCheck className="w-4 h-4" /> An toàn • Nhanh chóng • Tiện lợi
            </div>
          </div>
          <div className="flex items-center gap-3">
            <span className="text-sm text-gray-600">Loại xe</span>
            <Segmented
              options={[
                {
                  label: (
                    <span className="flex items-center gap-2 text-emerald-700">
                      <Bike className="w-4 h-4" />
                      Xe đạp
                    </span>
                  ),
                  value: "bike",
                },
                {
                  label: (
                    <span className="flex items-center gap-2 text-emerald-700">
                      <BikeElectric className="w-7 h-4" />
                      Xe điện
                    </span>
                  ),
                  value: "ebike",
                },
              ]}
              value={vehicleTab}
              onChange={(v) => setVehicleTab(v as any)}
            />
          </div>
        </div>
      </div>

      {/* wallet bar */}
      <div className="container mx-auto px-4 mb-6">
        <Card className="rounded-2xl border border-emerald-100 shadow-md">
          <div className="flex items-center justify-between flex-wrap gap-4">
            <Space size={8} className="text-gray-700">
              <WalletIcon className="w-5 h-5" />
              <span className="font-medium">Số dư ví</span>
            </Space>
            {walletQ.isLoading ? (
              <Skeleton active paragraph={false} />
            ) : walletQ.data ? (
              <Space size={32}>
                <Statistic
                  title="Số dư hiện tại"
                  value={walletQ.data.balance}
                  groupSeparator=","
                  suffix=" đ"
                  valueStyle={{ fontSize: 18, color: ecoGreen.main }}
                />
                <Tag color={walletQ.data.status === "Active" ? "green" : "red"}>
                  {walletQ.data.status}
                </Tag>
              </Space>
            ) : (
              <Alert
                type="warning"
                showIcon
                message="Bạn chưa có ví hoặc không lấy được thông tin ví."
              />
            )}
          </div>
        </Card>
      </div>

      {/* content */}
      <div className="container mx-auto px-4 pb-12">
        <Tabs
          defaultActiveKey="market"
          items={[
            {
              key: "market",
              label: <span className="text-emerald-700 font-medium">Gói khả dụng</span>,
              children: (
                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-5">
                  {marketQ.isLoading &&
                    Array.from({ length: 6 }).map((_, i) => (
                      <Card key={i} className="rounded-2xl border-emerald-50">
                        <Skeleton active paragraph={{ rows: 4 }} />
                      </Card>
                    ))}

                  {!marketQ.isLoading && plansFiltered.length === 0 && (
                    <div className="col-span-full">
                      <Empty description="Không có gói cho loại xe này" />
                    </div>
                  )}

                  {plansFiltered.map((plan: any) =>
                    plan.prices.map((price: any) => (
                      <Card
                        key={`${plan.id}-${price.id}`}
                        className="rounded-2xl shadow-md hover:shadow-lg transition-all border border-emerald-100 hover:border-emerald-400"
                      >
                        {/* Header 2 dòng */}
                        <div className="mb-3 flex flex-col gap-1">
                          {/* Dòng 1 */}
                          <div className="flex items-center justify-between gap-2 flex-wrap">
                            <Space
                              align="center"
                              size={8}
                              className="min-w-0 flex-1 flex-wrap"
                            >
                              <Ticket className="w-4 h-4" />
                              <span className="font-semibold text-emerald-800 truncate">
                                {plan.name}
                              </span>
                              <PlanRibbon code={plan.code} type={plan.type} />
                              {isSubscription(price) ? (
                                <Tag color="blue">Gói theo thời gian</Tag>
                              ) : (
                                <Tag color="purple">Vé lượt</Tag>
                              )}
                            </Space>
                            <VIcon type={price.vehicleType} className="w-10 h-5" />
                          </div>

                          {/* Dòng 2 */}
                          <div className="flex items-center justify-start mt-1">
                            <ModeBadge mode={mapMode(price.activationMode)} />
                          </div>
                        </div>

                        <div className="space-y-3">
                          <div className="flex items-end gap-2">
                            <div className="text-3xl font-bold leading-none text-emerald-700">
                              {currencyVN(price.price)}
                            </div>
                            <span className="text-gray-500 mb-1">
                              /{toVehicleLabel(price.vehicleType)}
                            </span>
                          </div>

                          <div className="grid grid-cols-2 gap-2 text-sm text-gray-700">
                            {typeof price.durationLimitMinutes === "number" && (
                              <div className="flex items-center gap-2">
                                <Timer className="w-4 h-4" /> Giới hạn:{" "}
                                {price.durationLimitMinutes} phút / ngày
                              </div>
                            )}
                            {typeof price.overageFeePer15Min === "number" && (
                              <div className="flex items-center gap-2">
                                <Clock3 className="w-4 h-4" /> Phí vượt/15p:{" "}
                                {currencyVN(price.overageFeePer15Min)}
                              </div>
                            )}
                            {plan.type === "Day" && (
                              <div className="col-span-2 text-gray-600">
                                Hiệu lực trong ngày theo giờ địa phương
                              </div>
                            )}
                            {plan.type === "Month" && (
                              <div className="col-span-2 text-gray-600">
                                Hiệu lực {price.validityDays ?? 30} ngày từ thời điểm mua
                              </div>
                            )}
                            {mapMode(price.activationMode) === "ON_FIRST_USE" && (
                              <div className="col-span-2 flex items-center gap-2 text-gray-700">
                                <Info className="w-4 h-4" /> Kích hoạt khi mở khoá lần
                                đầu (hạn kích hoạt:{" "}
                                {price.activationWindowDays ?? 30} ngày)
                              </div>
                            )}
                          </div>

                          <Divider className="my-2 border-emerald-100" />

                          <div className="flex items-center justify-between flex-wrap gap-3">
                            <Space size={8}>
                              <WalletIcon className="w-4 h-4" />
                              <span className="text-gray-600">Thanh toán bằng ví</span>
                            </Space>
                            <Button
                              type="primary"
                              shape="round"
                              loading={purchaseMut.isPending}
                              onClick={() => onBuy(plan, price)}
                              style={ecoBtnStyle}
                            >
                              Mua ngay
                            </Button>
                          </div>
                        </div>
                      </Card>
                    ))
                  )}
                </div>
              ),
            },
            {
              key: "notes",
              label: <span className="text-emerald-700 font-medium">Ghi chú</span>,
              children: (
                <Card className="rounded-2xl border-emerald-100">
                  <div className="space-y-2 text-gray-700">
                    <div>
                      • Vé lượt (RIDE) <b>không kích hoạt ngay</b>; kích hoạt khi bạn
                      bắt đầu chuyến.
                    </div>
                    <div>
                      • Vé ngày/tháng (IMMEDIATE) <b>kích hoạt ngay khi mua</b>. Vé ngày
                      có hiệu lực theo giờ địa phương.
                    </div>
                    <div>• Cần hỗ trợ hoá đơn, vui lòng liên hệ CSKH.</div>
                  </div>
                </Card>
              ),
            },
          ]}
        />
      </div>
    </div>
  );
};

export default BuyTicketsPage;
