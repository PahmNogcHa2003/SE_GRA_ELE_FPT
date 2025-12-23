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
  Input,
  Row,
  Col,
  Typography,
  Grid,
} from "antd";
import { currencyVN } from "../../utils/datetime";
import type { PreviewTicketPriceDTO } from "../../types/user.ticket";
import type { AvailableVoucherDTO } from "../../types/voucher";
import { getAvailableVouchers } from "../../services/voucher.service";
import {
  ShoppingCartOutlined,
  ThunderboltOutlined,
  EnvironmentOutlined,
  WalletOutlined,
  TagOutlined,
  ClockCircleOutlined,
  CheckCircleOutlined,
  InfoCircleOutlined,
  GiftOutlined,
  CopyOutlined,
  SafetyOutlined,
  StarOutlined,
  CreditCardOutlined,
  PercentageOutlined,
  CalendarOutlined,
  LockOutlined,
} from "@ant-design/icons";

const { Title, Text } = Typography;
const { useBreakpoint } = Grid;

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
  main: "#10B981",
  light: "#D1FAE5",
  lighter: "#ECFDF5",
  dark: "#059669",
  gradient: "from-emerald-50 via-white to-emerald-25",
  gradientCard: "from-white to-emerald-50",
};

const PlanRibbon: React.FC<{ code?: string | null; type?: string | null }> = ({
  code,
  type,
}) => {
  if (code === "RIDE" || type === "Ride")
    return (
      <Tag
        color="purple"
        icon={<TagOutlined />}
        className="font-semibold"
      >
        Vé lượt
      </Tag>
    );
  if (code === "DAY" || type === "Day")
    return (
      <Tag
        color="green"
        icon={<CalendarOutlined />}
        className="font-semibold"
      >
        Vé ngày
      </Tag>
    );
  if (type === "Month")
    return (
      <Tag
        color="blue"
        icon={<CalendarOutlined />}
        className="font-semibold"
      >
        Vé tháng
      </Tag>
    );
  return <Tag icon={<TagOutlined />}>Vé</Tag>;
};

const ModeBadge: React.FC<{ mode: "IMMEDIATE" | "ON_FIRST_USE" }> = ({
  mode,
}) => (
  <Badge
    color={mode === "ON_FIRST_USE" ? "purple" : ecoGreen.main}
    text={
      <span className="font-medium text-xs">
        {mode === "ON_FIRST_USE" ? "Kích hoạt khi dùng" : "Kích hoạt ngay"}
      </span>
    }
  />
);

const VehicleIcon: React.FC<{ type?: string | null; className?: string }> = ({
  type,
  className = "",
}) =>
  type?.toLowerCase() === "ebike" ? (
    <ThunderboltOutlined className={`text-purple-600 ${className}`} />
  ) : (
    <EnvironmentOutlined className={`text-emerald-600 ${className}`} />
  );

const BuyTicketsPage: React.FC = () => {
  const [vehicleTab, setVehicleTab] = useState<"bike" | "ebike">("bike");
  const [voucherApplied, setVoucherApplied] = useState(false);

  const { isLoggedIn, isLoadingUser } = useAuth();
  const { notification } = App.useApp();
  const qc = useQueryClient();
  const screens = useBreakpoint();

  const vehicleParam = vehicleTab === "bike" ? "bike" : "ebike";

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

  const marketQ = useQuery({
    queryKey: ["ticketMarket", vehicleParam],
    queryFn: () => getTicketMarket(vehicleParam),
    enabled: !isLoadingUser,
    select: (res: any) => {
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

  const vouchersQ = useQuery({
    queryKey: ["availableVouchers", buyModal.price?.price],
    queryFn: () => getAvailableVouchers(buyModal.price.price),
    enabled: !!buyModal.price && isLoggedIn,
  });

  // ===== Mutation: Mua vé =====
  const purchaseMut = useMutation({
    mutationFn: purchaseTicket,
    onSuccess: (res) => {
      const data = (res as any)?.data ?? res;
      notification.success({
        message: "🎉 Mua vé thành công!",
        description: `Đã thêm vé "${data?.planName || "Gói vé"}" vào tài khoản của bạn.`,
        placement: "topRight",
      });
      qc.invalidateQueries({ queryKey: ["wallet"] });
      qc.invalidateQueries({ queryKey: ["walletTransactions"] });
      qc.invalidateQueries({ queryKey: ["myActiveTickets"] });
      setBuyModal((prev) => ({ ...prev, open: false, preview: null }));
      setVoucherApplied(false);
    },
    onError: (e: any) =>
      notification.error({
        message: "Mua vé thất bại",
        description: prettyErr(e),
        placement: "topRight",
      }),
  });

  // ===== Mutation: Preview voucher =====
  const previewMut = useMutation({
    mutationFn: (payload: { planPriceId: number; voucherCode?: string }) =>
      previewTicketPrice(payload),
    onSuccess: (res) => {
      const api = (res as any) ?? res;
      const data: PreviewTicketPriceDTO = api.data ?? api;
      setBuyModal((prev) => ({ ...prev, preview: data }));
    },
    onError: (e: any) => {
      setBuyModal((prev) => ({ ...prev, preview: null }));
      notification.error({
        message: "Voucher không hợp lệ",
        description: prettyErr(e),
        placement: "topRight",
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
      message: "🔐 Bạn cần đăng nhập",
      description: "Vui lòng đăng nhập để mua vé và sử dụng ví.",
      placement: "topRight",
    });
  };

  const openBuyModal = (plan: any, price: any) => {
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
    setVoucherApplied(false);
  };

  // ===== Apply voucher =====
  const handleApplyVoucher = () => {
    if (!buyModal.price || !buyModal.voucherCode) return;

    previewMut.mutate({
      planPriceId: buyModal.price.id,
      voucherCode: buyModal.voucherCode,
    });

    setVoucherApplied(true);
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
        message: "💰 Số dư không đủ",
        description: `Cần ${currencyVN(
          expectedTotal
        )} nhưng ví chỉ có ${currencyVN(wallet.balance)}.`,
        placement: "topRight",
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
    <div className={`min-h-screen bg-linear-to-r ${ecoGreen.gradient}`}>
      {/* Header Section */}
      <div className="container mx-auto px-4 pt-8 pb-4">
        <Card 
          className="rounded-2xl border-0 shadow-lg overflow-hidden"
          bodyStyle={{ padding: 0 }}
        >
          <div className="p-6 md:p-8 bg-linear-to-r from-emerald-600 to-emerald-700 text-white">
            <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-6">
              <div className="space-y-3">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-white/20 rounded-lg backdrop-blur-sm">
                    <TagOutlined className="text-xl" />
                  </div>
                  <div>
                    <Title level={2} className="mb-0! text-white!">
                      Mua Vé EcoJourney
                    </Title>
                    <Text className="text-emerald-100">
                      Chọn gói phù hợp – thanh toán bằng ví – dùng ngay
                    </Text>
                  </div>
                </div>
                <div className="flex flex-wrap gap-3">
                  <div className="flex items-center gap-2 text-sm">
                    <SafetyOutlined />
                    <span>An toàn</span>
                  </div>
                  <div className="flex items-center gap-2 text-sm">
                    <CheckCircleOutlined />
                    <span>Nhanh chóng</span>
                  </div>
                  <div className="flex items-center gap-2 text-sm">
                    <StarOutlined />
                    <span>Tiện lợi</span>
                  </div>
                </div>
              </div>
              <div className="flex flex-col gap-2">
                <Text className="text-emerald-100">Loại xe</Text>
                <Segmented
                  sizes={screens.xs ? "small" : "default"}
                  options={[
                    {
                      label: (
                        <div className="flex items-center gap-2 px-2">
                          <EnvironmentOutlined />
                          <span>Xe đạp</span>
                        </div>
                      ),
                      value: "bike",
                    },
                    {
                      label: (
                        <div className="flex items-center gap-2 px-2">
                          <ThunderboltOutlined />
                          <span>Xe điện</span>
                        </div>
                      ),
                      value: "ebike",
                    },
                  ]}
                  value={vehicleTab}
                  onChange={(v) => setVehicleTab(v as any)}
                  className="bg-white/10"
                />
              </div>
            </div>
          </div>
        </Card>
      </div>

      {/* Wallet Info */}
      <div className="container mx-auto px-4 mb-6">
        <Card className="rounded-2xl border-0 shadow-md">
          {!isLoggedIn ? (
            <Alert
              type="info"
              showIcon
              message="Bạn cần đăng nhập để xem số dư ví và mua vé."
              className="rounded-lg"
            />
          ) : (
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-emerald-100 rounded-lg">
                  <WalletOutlined className="text-emerald-600 text-lg" />
                </div>
                <div>
                  <Text className="text-gray-600 block text-sm">Số dư ví</Text>
                  {walletQ.isLoading ? (
                    <Skeleton active paragraph={false} style={{ width: 200 }} />
                  ) : walletQ.data ? (
                    <Space size="small">
                      <Statistic
                        value={walletQ.data.balance}
                        prefix="₫"
                        valueStyle={{
                          fontSize: "20px",
                          fontWeight: "bold",
                          color: ecoGreen.dark,
                        }}
                      />
                      <Tag
                        color={walletQ.data.status === "Active" ? "green" : "red"}
                        className="font-medium"
                      >
                        {walletQ.data.status}
                      </Tag>
                    </Space>
                  ) : (
                    <Alert
                      type="warning"
                      showIcon
                      message="Bạn chưa có ví hoặc không lấy được thông tin ví."
                      className="mt-2"
                    />
                  )}
                </div>
              </div>
              {isLoggedIn && walletQ.data && (
                <Button
                  type="default"
                  href="/top-up"
                  icon={<CreditCardOutlined />}
                  className="border-emerald-200 text-emerald-600 hover:border-emerald-300 hover:text-emerald-700"
                >
                  Nạp thêm tiền
                </Button>
              )}
            </div>
          )}
        </Card>
      </div>

      {/* Main Content */}
      <div className="container mx-auto px-4 pb-12">
        <Tabs
          defaultActiveKey="market"
          items={[
            {
              key: "market",
              label: (
                <span className="font-medium text-emerald-700 flex items-center gap-2">
                  <ShoppingCartOutlined />
                  Gói khả dụng
                </span>
              ),
              children: (
                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                  {marketQ.isLoading &&
                    Array.from({ length: 6 }).map((_, i) => (
                      <Card
                        key={i}
                        className="rounded-2xl border-0 shadow-sm"
                      >
                        <Skeleton active paragraph={{ rows: 4 }} />
                      </Card>
                    ))}

                  {!marketQ.isLoading && plansFiltered.length === 0 && (
                    <div className="col-span-full">
                      <Empty 
                        description="Không có gói cho loại xe này" 
                        image={Empty.PRESENTED_IMAGE_SIMPLE}
                        className="py-12"
                      />
                    </div>
                  )}

                  {plansFiltered.map((plan: any) =>
                    plan.prices.map((price: any) => (
                      <Card
                        key={`${plan.id}-${price.id}`}
                        className="rounded-2xl shadow-sm hover:shadow-lg transition-all duration-300 border border-emerald-100 hover:border-emerald-300"
                      >
                        {/* Card Header */}
                        <div className="mb-4">
                          <div className="flex justify-between items-start mb-2">
                            <div className="flex items-center gap-2">
                              <div className="p-1 bg-emerald-100 rounded">
                                <TagOutlined className="text-emerald-600" />
                              </div>
                              <Text strong className="text-emerald-800 text-lg">
                                {plan.name}
                              </Text>
                            </div>
                            <VehicleIcon type={price.vehicleType} />
                          </div>
                          <div className="flex flex-wrap gap-2">
                            <PlanRibbon code={plan.code} type={plan.type} />
                            {isSubscription(price) ? (
                              <Tag color="blue" className="text-xs">
                                Gói thời gian
                              </Tag>
                            ) : (
                              <Tag color="purple" className="text-xs">
                                Gói lượt
                              </Tag>
                            )}
                          </div>
                          <div className="mt-2">
                            <ModeBadge mode={mapMode(price.activationMode)} />
                          </div>
                        </div>

                        {/* Price Section */}
                        <div className="mb-4">
                          <div className="flex items-baseline gap-1">
                            <div className="text-2xl font-bold text-emerald-700">
                              {currencyVN(price.price)}
                            </div>
                            <div className="text-gray-500 text-sm">
                              /{toVehicleLabel(price.vehicleType)}
                            </div>
                          </div>
                        </div>

                        {/* Features */}
                        <div className="space-y-2 mb-4">
                          {typeof price.durationLimitMinutes === "number" && (
                            <div className="flex items-center gap-2 text-sm text-gray-600">
                              <ClockCircleOutlined className="text-blue-500" />
                              <span>Giới hạn: {price.durationLimitMinutes} phút/ngày</span>
                            </div>
                          )}
                          {typeof price.overageFeePer15Min === "number" && (
                            <div className="flex items-center gap-2 text-sm text-gray-600">
                              <InfoCircleOutlined className="text-orange-500" />
                              <span>Phí vượt: {currencyVN(price.overageFeePer15Min)}/15p</span>
                            </div>
                          )}
                          {plan.type === "Day" && (
                            <div className="text-xs text-gray-500">
                              Hiệu lực trong ngày theo giờ địa phương
                            </div>
                          )}
                          {plan.type === "Month" && (
                            <div className="text-xs text-gray-500">
                              Hiệu lực {price.validityDays ?? 30} ngày từ thời điểm mua
                            </div>
                          )}
                          {mapMode(price.activationMode) === "ON_FIRST_USE" && (
                            <div className="text-xs text-gray-500 flex items-start gap-1">
                              <LockOutlined className="mt-0.5" />
                              <span>
                                Kích hoạt khi mở khoá lần đầu (hạn kích hoạt: {price.activationWindowDays ?? 30} ngày)
                              </span>
                            </div>
                          )}
                        </div>

                        <Divider className="my-4" />

                        {/* Action Button */}
                        <div className="flex items-center justify-between">
                          <div className="flex items-center gap-2 text-sm text-gray-600">
                            <WalletOutlined />
                            <span>Thanh toán bằng ví</span>
                          </div>
                          <Button
                            type="primary"
                            size={screens.xs ? "small" : "middle"}
                            icon={<ShoppingCartOutlined />}
                            loading={purchaseMut.isPending}
                            onClick={() => openBuyModal(plan, price)}
                            className="bg-emerald-600 hover:bg-emerald-700 border-emerald-600"
                          >
                            {isLoggedIn ? "Mua ngay" : "Đăng nhập"}
                          </Button>
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
                <span className="font-medium text-emerald-700 flex items-center gap-2">
                  <InfoCircleOutlined />
                  Ghi chú
                </span>
              ),
              children: (
                <Card className="rounded-2xl border-0 shadow-sm">
                  <div className="space-y-4">
                    <Alert
                      type="info"
                      message="Thông tin quan trọng"
                      description="Vui lòng đọc kỹ trước khi mua vé"
                      showIcon
                      className="rounded-lg"
                    />
                    <div className="space-y-3 text-gray-700">
                      <div className="flex items-start gap-2">
                        <div className="p-1 bg-purple-100 rounded mt-0.5">
                          <TagOutlined className="text-purple-600 text-xs" />
                        </div>
                        <div>
                          <Text strong>Vé lượt (RIDE)</Text>
                          <Text className="block text-sm">
                            <b>Không kích hoạt ngay</b>; kích hoạt khi bạn bắt đầu chuyến.
                          </Text>
                        </div>
                      </div>
                      <div className="flex items-start gap-2">
                        <div className="p-1 bg-green-100 rounded mt-0.5">
                          <CalendarOutlined className="text-green-600 text-xs" />
                        </div>
                        <div>
                          <Text strong>Vé ngày/tháng (IMMEDIATE)</Text>
                          <Text className="block text-sm">
                            <b>Kích hoạt ngay khi mua</b>. Vé ngày có hiệu lực theo giờ địa phương.
                          </Text>
                        </div>
                      </div>
                      <div className="flex items-start gap-2">
                        <div className="p-1 bg-blue-100 rounded mt-0.5">
                          <InfoCircleOutlined className="text-blue-600 text-xs" />
                        </div>
                        <div>
                          <Text strong>Hỗ trợ & Hoá đơn</Text>
                          <Text className="block text-sm">
                            Cần hỗ trợ hoá đơn, vui lòng liên hệ CSKH qua email: support@ecojourney.com
                          </Text>
                        </div>
                      </div>
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
        onCancel={() => {
          setBuyModal((prev) => ({ ...prev, open: false, preview: null }));
          setVoucherApplied(false);
        }}
        title={
          <div className="flex items-center gap-3">
            <div className="p-2 bg-emerald-100 rounded-lg">
              <ShoppingCartOutlined className="text-emerald-600" />
            </div>
            <div>
              <Title level={4} className="mb-0!">
                Mua {buyModal.plan?.name}
              </Title>
              <Text type="secondary" className="text-sm">
                {toVehicleLabel(buyModal.price?.vehicleType)} •{" "}
                {mapMode(buyModal.price?.activationMode) === "ON_FIRST_USE"
                  ? "Kích hoạt khi dùng"
                  : "Kích hoạt ngay"}
              </Text>
            </div>
          </div>
        }
        footer={[
          <Button
            key="cancel"
            onClick={() => {
              setBuyModal((prev) => ({ ...prev, open: false, preview: null }));
              setVoucherApplied(false);
            }}
            className="rounded-lg"
          >
            Hủy
          </Button>,
          <Button
            key="buy"
            type="primary"
            icon={<ShoppingCartOutlined />}
            loading={purchaseMut.isPending}
            onClick={handleConfirmBuy}
            className="bg-emerald-600 hover:bg-emerald-700 border-emerald-600 rounded-lg"
            size="large"
          >
            Xác nhận mua
          </Button>,
        ]}
        width={screens.xs ? "95%" : 600}
        className="rounded-2xl"
      >
        {buyModal.price && (
          <div className="space-y-6">
            {/* Price Summary */}
            <Row gutter={[16, 16]}>
              <Col span={12}>
                <Card className="border-0 shadow-sm">
                  <Statistic
                    title="Giá gốc"
                    value={buyModal.price.price}
                    prefix="₫"
                    valueStyle={{ fontSize: "20px", color: ecoGreen.dark }}
                  />
                </Card>
              </Col>
              <Col span={12}>
                <Card className="border-0 shadow-sm">
                  <Statistic
                    title="Số dư ví"
                    value={walletQ.data?.balance ?? 0}
                    prefix="₫"
                    valueStyle={{
                      fontSize: "20px",
                      color: (walletQ.data?.balance ?? 0) >= (buyModal.price?.price ?? 0) ? ecoGreen.dark : "#EF4444",
                    }}
                  />
                </Card>
              </Col>
            </Row>

            {/* Voucher Section */}
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <GiftOutlined className="text-purple-500" />
                  <Text strong>Voucher ưu đãi</Text>
                </div>
                <Tag color="default" className="text-xs">
                  {vouchersQ.data?.length || 0} mã có sẵn
                </Tag>
              </div>

              {/* Voucher Input */}
              <div className="space-y-3">
                <div className="flex gap-2">
                  <Input
                    size="middle"
                    placeholder="Nhập mã voucher..."
                    value={buyModal.voucherCode}
                    disabled={voucherApplied}
                    onChange={(e) =>
                      setBuyModal((prev) => ({
                        ...prev,
                        voucherCode: e.target.value.trim(),
                      }))
                    }
                    className="flex-1"
                    prefix={<GiftOutlined className="text-gray-400" />}
                  />
                  <Button
                    type="default"
                    loading={previewMut.isPending}
                    disabled={!buyModal.voucherCode}
                    onClick={handleApplyVoucher}
                    className="whitespace-nowrap"
                  >
                    {voucherApplied ? "Đã áp dụng" : "Áp dụng"}
                  </Button>
                </div>

                {/* Voucher List */}
                {vouchersQ.isLoading ? (
                  <Skeleton active paragraph={{ rows: 2 }} />
                ) : (
                  <div className="space-y-2 max-h-60 overflow-y-auto pr-2">
                    {vouchersQ.data?.map((v: AvailableVoucherDTO) => {
                      const isSelected = buyModal.voucherCode === v.code;
                      const isApplicable = v.isApplicable;

                      return (
                        <div
                          key={v.id}
                          className={`
                            p-3 rounded-lg border transition-all duration-200 cursor-pointer
                            ${
                              isApplicable
                                ? isSelected
                                  ? "border-emerald-500 bg-emerald-50 ring-1 ring-emerald-500"
                                  : "border-gray-200 hover:border-emerald-300 hover:shadow-sm"
                                : "border-gray-200 bg-gray-50 opacity-70 cursor-not-allowed"
                            }
                          `}
                          onClick={() => {
                            if (!isApplicable) return;
                            setBuyModal((prev) => ({
                              ...prev,
                              voucherCode: v.code,
                            }));
                          }}
                        >
                          <div className="flex justify-between items-start">
                            <div className="flex-1">
                              <div className="flex items-center gap-2">
                                <Text 
                                  strong 
                                  className={isApplicable ? "text-emerald-700" : "text-gray-500"}
                                >
                                  {v.code}
                                </Text>
                                {isSelected && (
                                  <Tag color="green">Đang chọn</Tag>
                                )}
                              </div>
                              <Text type="secondary" className="text-xs mt-1 block">
                                {v.description}
                              </Text>
                              {v.endDate && (
                                <Text type="secondary" className="text-xs block mt-1">
                                  HSD: {new Date(v.endDate).toLocaleDateString("vi-VN")}
                                </Text>
                              )}
                              {!isApplicable && v.notApplicableReason && (
                                <div className="mt-2 text-xs text-red-500 bg-red-50 p-2 rounded-md">
                                  🚫 {v.notApplicableReason}
                                </div>
                              )}
                            </div>
                            <div className="text-right ml-4">
                              <div
                                className={`text-lg font-bold ${
                                  isApplicable ? "text-orange-500" : "text-gray-400"
                                }`}
                              >
                                -{v.displayValue}
                              </div>
                              {isApplicable && (
                                <Button
                                  size="small"
                                  type={isSelected ? "primary" : "default"}
                                  icon={<CopyOutlined />}
                                  className="mt-2"
                                  onClick={(e) => {
                                    e.stopPropagation();
                                    navigator.clipboard.writeText(v.code);
                                    setBuyModal((prev) => ({
                                      ...prev,
                                      voucherCode: v.code,
                                    }));
                                    notification.success({
                                      message: "📋 Đã sao chép mã",
                                      description: v.code,
                                      placement: "topRight",
                                      duration: 2,
                                    });
                                  }}
                                >
                                  Sao chép
                                </Button>
                              )}
                            </div>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            </div>

            {/* Preview Summary */}
            {buyModal.preview && (
              <Card className="border-0 shadow-sm bg-emerald-50">
                <div className="space-y-3">
                  <div className="flex justify-between items-center">
                    <Text>Giá gốc:</Text>
                    <Text strong>{currencyVN(buyModal.preview.subtotal)}</Text>
                  </div>
                  <div className="flex justify-between items-center text-orange-600">
                    <div className="flex items-center gap-1">
                      <PercentageOutlined />
                      <Text>Giảm giá:</Text>
                    </div>
                    <Text strong>-{currencyVN(buyModal.preview.discount)}</Text>
                  </div>
                  <Divider className="my-2" />
                  <div className="flex justify-between items-center">
                    <Text strong className="text-base">Tổng thanh toán:</Text>
                    <Text strong className="text-xl text-emerald-700">
                      {currencyVN(buyModal.preview.total)}
                    </Text>
                  </div>
                  {buyModal.preview.voucherMessage && (
                    <Alert
                      type="success"
                      message={buyModal.preview.voucherMessage}
                      showIcon
                      className="mt-3"
                    />
                  )}
                </div>
              </Card>
            )}
          </div>
        )}
      </Modal>
    </div>
  );
};

export default BuyTicketsPage;