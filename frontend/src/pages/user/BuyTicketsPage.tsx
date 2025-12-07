// src/pages/user/BuyTicketsPage.tsx
import React, { useMemo, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useAuth } from "../../features/auth/context/authContext";
import {
  getTicketMarket,
  purchaseTicket,
  previewTicketPrice,
} from "../../services/user.ticket.service";
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
  Modal,
} from "antd";
import { currencyVN } from "../../utils/datetime";
import type { PreviewTicketPriceDTO } from "../../types/user.ticket";

const IconBase: React.FC<{
  className?: string;
  label?: string;
  children?: React.ReactNode;
}> = ({ className, label, children }) => (
  <span
    role="img"
    aria-label={label}
    className={className}
    style={{ display: "inline-flex", alignItems: "center" }}
  >
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

const toVehicleLabel = (v: string | undefined) =>
  v?.toLowerCase() === "ebike" ? "Xe điện" : "Xe đạp";

const prettyErr = (e: any) => {
  if (!e) return "Đã có lỗi xảy ra";
  const resp = e.response;
  if (resp && resp.data) {
    const d = resp.data;
    if (typeof d === "string") return d;
    if (typeof d === "object") {
      if (d.message) return d.message;
      if (d.error) return d.error;
      if (Array.isArray(d.errors) && d.errors.length > 0) {
        return d.errors.join("; ");
      }
      if (typeof d.title === "string") return d.title;
    }
  }
  if (typeof e === "string") return e;
  return "Đã có lỗi xảy ra";
};

const mapMode = (
  m: number | string | undefined
): "IMMEDIATE" | "ON_FIRST_USE" =>
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

const PlanRibbon: React.FC<{ code?: string | null; type?: string | null }> = ({
  code,
  type,
}) => {
  if (code === "RIDE" || type === "Ride") return <Tag color="purple">Vé lượt</Tag>;
  if (code === "DAY" || type === "Day") return <Tag color="green">Vé ngày</Tag>;
  if (type === "Month") return <Tag color="blue">Vé tháng</Tag>;
  return <Tag>Vé</Tag>;
};

const ModeBadge: React.FC<{ mode: "IMMEDIATE" | "ON_FIRST_USE" }> = ({
  mode,
}) => (
  <Badge
    color={mode === "ON_FIRST_USE" ? "purple" : ecoGreen.main}
    text={mode === "ON_FIRST_USE" ? "Kích hoạt khi dùng" : "Kích hoạt ngay"}
  />
);

const VIcon: React.FC<{ type?: string | null; className?: string }> = ({
  type,
  className,
}) =>
  type?.toLowerCase() === "ebike" ? (
    <BikeElectric className={className} />
  ) : (
    <Bike className={className} />
  );

// ========================= MAIN PAGE =========================
const BuyTicketsPage: React.FC = () => {
  const [vehicleTab, setVehicleTab] = useState<"bike" | "ebike">("bike");

  const { isLoggedIn, isLoadingUser } = useAuth();
  const { notification } = App.useApp();
  const qc = useQueryClient();

  const vehicleParam = vehicleTab === "bike" ? "bike" : "ebike";

  // ===== Query: Market tickets (LUÔN load, kể cả chưa login) =====
  const marketQ = useQuery({
    queryKey: ["ticketMarket", vehicleParam],
    queryFn: () => getTicketMarket(vehicleParam),
    enabled: !isLoadingUser,
    select: (res: any) => {
      const api = res?.data ?? res;
      return api?.data ?? api ?? [];
    },
  });

  // ===== Query: Wallet (chỉ load khi đã login) =====
  const walletQ = useQuery({
    queryKey: ["wallet", isLoggedIn],
    queryFn: getWallet,
    enabled: isLoggedIn && !isLoadingUser,
    select: (res) => res.data,
    staleTime: 5 * 60 * 1000,
  });

  // ===== State: Modal mua vé + voucher =====
  const [buyModal, setBuyModal] = useState<{
    open: boolean;
    plan: any | null;
    price: any | null;
    voucherCode: string;
    preview: PreviewTicketPriceDTO | null;
  }>({
    open: false,
    plan: null,
    price: null,
    voucherCode: "",
    preview: null,
  });

  // ===== Mutation: Mua vé =====
  const purchaseMut = useMutation({
    mutationFn: purchaseTicket, // (payload) => Promise<ApiResponse<UserTicket>>
    onSuccess: (res) => {
      const data = (res as any)?.data ?? res;
      notification.success({
        message: "Mua vé thành công",
        description: `Đã thêm vé: ${data?.planName ?? "Gói vé"}`,
      });
      qc.invalidateQueries({ queryKey: ["wallet"] });
      qc.invalidateQueries({ queryKey: ["walletTransactions"] });
      qc.invalidateQueries({ queryKey: ["myActiveTickets"] });
      setBuyModal((prev) => ({ ...prev, open: false, preview: null }));
    },
    onError: (e: any) =>
      notification.error({
        message: "Mua vé thất bại",
        description: prettyErr(e),
      }),
  });

  // ===== Mutation: Preview voucher =====
  const previewMut = useMutation({
    mutationFn: (payload: { planPriceId: number; voucherCode?: string }) =>
      previewTicketPrice(payload), // Promise<ApiResponse<PreviewTicketPriceDTO>>
    onSuccess: (res) => {
      const api = (res as any) ?? res;
      const data: PreviewTicketPriceDTO = api.data ?? api;
      setBuyModal((prev) => ({ ...prev, preview: data }));
    },
    onError: (e: any) => {
      setBuyModal((prev) => ({ ...prev, preview: null }));
      notification.error({
        message: "Áp dụng voucher thất bại",
        description: prettyErr(e),
      });
    },
  });

  // ===== Filter plan theo vehicle =====
  const plansFiltered = useMemo(() => {
    const list = Array.isArray(marketQ.data) ? marketQ.data : [];
    return list
      .map((p: any) => ({
        ...p,
        prices: (Array.isArray(p.prices) ? p.prices : []).filter((pr: any) =>
          vehicleParam
            ? pr?.vehicleType?.toLowerCase() === vehicleParam.toLowerCase()
            : true
        ),
      }))
      .filter((p: any) => p.prices.length > 0);
  }, [marketQ.data, vehicleParam]);

  // ===== Khi bấm mua =====
  const handleRequireLogin = () => {
    notification.info({
      message: "Bạn cần đăng nhập",
      description: "Vui lòng đăng nhập để có thể mua vé và sử dụng ví.",
    });
  };

  const openBuyModal = (plan: any, price: any) => {
    // nếu chưa login thì redirect login, không mở modal
    if (!isLoggedIn) {
      handleRequireLogin();
      return;
    }
    setBuyModal({
      open: true,
      plan,
      price,
      voucherCode: "",
      preview: null,
    });
  };

  // ===== Apply voucher -> gọi preview API =====
  const handleApplyVoucher = () => {
    if (!buyModal.price) return;

    previewMut.mutate({
      planPriceId: buyModal.price.id,
      voucherCode: buyModal.voucherCode || undefined,
    });
  };

  // ===== Xác nhận mua =====
  const handleConfirmBuy = () => {
    if (!buyModal.price) return;

    const wallet = walletQ.data;
    if (!wallet) {
      notification.warning({
        message: "Không thể lấy thông tin ví.",
      });
      return;
    }

    const expectedTotal =
      buyModal.preview?.total ?? (buyModal.price.price ?? 0);

    if (wallet.balance < expectedTotal) {
      notification.error({
        message: "Số dư không đủ",
        description: `Cần ${currencyVN(
          expectedTotal
        )} nhưng ví chỉ có ${currencyVN(wallet.balance)}.`,
      });
      return;
    }

    purchaseMut.mutate({
      planPriceId: buyModal.price.id,
      voucherCode: buyModal.voucherCode || undefined,
    });
  };

  // ======================= RENDER =======================
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
              <ShieldCheck className="w-4 h-4" /> An toàn • Nhanh chóng •
              Tiện lợi
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
          {!isLoggedIn ? (
            <Alert
              type="info"
              showIcon
              message="Bạn cần đăng nhập để xem số dư ví và mua vé."
            />
          ) : (
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
                  <Tag
                    color={walletQ.data.status === "Active" ? "green" : "red"}
                  >
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
          )}
        </Card>
      </div>

      {/* content */}
      <div className="container mx-auto px-4 pb-12">
        <Tabs
          defaultActiveKey="market"
          items={[
            {
              key: "market",
              label: (
                <span className="text-emerald-700 font-medium">
                  Gói khả dụng
                </span>
              ),
              children: (
                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-5">
                  {marketQ.isLoading &&
                    Array.from({ length: 6 }).map((_, i) => (
                      <Card
                        key={i}
                        className="rounded-2xl border-emerald-50"
                      >
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
                              <PlanRibbon
                                code={plan.code}
                                type={plan.type}
                              />
                              {isSubscription(price) ? (
                                <Tag color="blue">Gói theo thời gian</Tag>
                              ) : (
                                <Tag color="purple">Vé lượt</Tag>
                              )}
                            </Space>
                            <VIcon
                              type={price.vehicleType}
                              className="w-10 h-5"
                            />
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
                                Hiệu lực {price.validityDays ?? 30} ngày từ
                                thời điểm mua
                              </div>
                            )}
                            {mapMode(price.activationMode) ===
                              "ON_FIRST_USE" && (
                              <div className="col-span-2 flex items-center gap-2 text-gray-700">
                                <Info className="w-4 h-4" /> Kích hoạt khi mở
                                khoá lần đầu (hạn kích hoạt:{" "}
                                {price.activationWindowDays ?? 30} ngày)
                              </div>
                            )}
                          </div>

                          <Divider className="my-2 border-emerald-100" />

                          <div className="flex items-center justify-between flex-wrap gap-3">
                            <Space size={8}>
                              <WalletIcon className="w-4 h-4" />
                              <span className="text-gray-600">
                                Thanh toán bằng ví
                              </span>
                            </Space>
                            <Button
                              type="primary"
                              shape="round"
                              loading={purchaseMut.isPending}
                              onClick={() => openBuyModal(plan, price)}
                              style={
                                isLoggedIn
                                  ? ecoBtnStyle
                                  : {
                                      ...ecoBtnStyle,
                                      backgroundColor: "#9CA3AF",
                                      borderColor: "#9CA3AF",
                                    }
                              }
                            >
                              {isLoggedIn ? "Mua ngay" : "Đăng nhập để mua vé"}
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
              label: (
                <span className="text-emerald-700 font-medium">Ghi chú</span>
              ),
              children: (
                <Card className="rounded-2xl border-emerald-100">
                  <div className="space-y-2 text-gray-700">
                    <div>
                      • Vé lượt (RIDE) <b>không kích hoạt ngay</b>; kích hoạt
                      khi bạn bắt đầu chuyến.
                    </div>
                    <div>
                      • Vé ngày/tháng (IMMEDIATE) <b>kích hoạt ngay khi mua</b>.
                      Vé ngày có hiệu lực theo giờ địa phương.
                    </div>
                    <div>
                      • Cần hỗ trợ hoá đơn, vui lòng liên hệ CSKH.
                    </div>
                  </div>
                </Card>
              ),
            },
          ]}
        />
      </div>

      {/* Modal Mua vé + Voucher */}
      <Modal
        open={buyModal.open}
        onCancel={() =>
          setBuyModal((prev) => ({ ...prev, open: false, preview: null }))
        }
        title={
          buyModal.plan
            ? `Mua ${buyModal.plan.name} – ${toVehicleLabel(
                buyModal.price?.vehicleType
              )}`
            : "Mua vé"
        }
        footer={[
          <Button
            key="cancel"
            onClick={() =>
              setBuyModal((prev) => ({ ...prev, open: false, preview: null }))
            }
          >
            Hủy
          </Button>,
          <Button
            key="buy"
            type="primary"
            style={ecoBtnStyle}
            loading={purchaseMut.isPending}
            onClick={handleConfirmBuy}
          >
            Mua ngay
          </Button>,
        ]}
      >
        {buyModal.price && (
          <div className="space-y-4">
            <div>
              <div className="text-sm text-gray-600">Giá gốc</div>
              <div className="text-xl font-semibold text-emerald-700">
                {currencyVN(buyModal.price.price)}
              </div>
            </div>

            <div>
              <div className="text-sm text-gray-600 mb-1">Mã voucher</div>
              <div className="flex gap-2">
                <input
                  type="text"
                  className="flex-1 border rounded-md px-2 py-1 text-sm"
                  placeholder="Nhập mã giảm giá"
                  value={buyModal.voucherCode}
                  onChange={(e) =>
                    setBuyModal((prev) => ({
                      ...prev,
                      voucherCode: e.target.value.trim(),
                    }))
                  }
                />
                <Button
                  onClick={handleApplyVoucher}
                  loading={previewMut.isPending}
                >
                  Apply
                </Button>
              </div>
              <div className="text-[11px] text-gray-500 mt-1">
                Hệ thống sẽ kiểm tra mã và hiển thị giá sau giảm trước khi bạn
                xác nhận mua.
              </div>
            </div>

            {buyModal.preview && (
              <div className="mt-3 p-3 rounded-lg bg-emerald-50 border border-emerald-100 text-sm">
                <div>
                  Giá gốc:{" "}
                  <b>{currencyVN(buyModal.preview.subtotal)}</b>
                </div>
                <div>
                  Giảm giá:{" "}
                  <b>-{currencyVN(buyModal.preview.discount)}</b>
                </div>
                <div>
                  <span>Giá sau giảm: </span>
                  <b className="text-emerald-700">
                    {currencyVN(buyModal.preview.total)}
                  </b>
                </div>
                {buyModal.preview.voucherMessage && (
                  <div className="text-xs text-emerald-700 mt-1">
                    {buyModal.preview.voucherMessage}
                  </div>
                )}
              </div>
            )}
          </div>
        )}
      </Modal>
    </div>
  );
};

export default BuyTicketsPage;
