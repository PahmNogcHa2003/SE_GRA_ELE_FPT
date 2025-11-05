import React, { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useAuth } from "../../features/auth/context/authContext";
import { getMyActiveTickets } from "../../services/user.ticket.service";
import { Spin, Alert, Button, Tag, Tabs, Badge, Empty, Drawer } from "antd";
import { Link } from "react-router-dom";
import TicketCard from "../../features/tickets/components/TicketCard";
import { formatUtcToVN } from "../../utils/datetime";

type Ticket = any; // bạn có thể thay bằng UserTicket

// --- Helpers phân loại ---
const isSubTicket = (t: Ticket) => {
  const vf = (t as any).validFrom as string | undefined | null;
  const vt = (t as any).validTo as string | undefined | null;
  return !!(vf && vt);
};

const diffDays = (from?: string | null, to?: string | null) => {
  if (!from || !to) return 0;
  const a = new Date(from).getTime();
  const b = new Date(to).getTime();
  const ms = Math.max(0, b - a);
  return Math.ceil(ms / (24 * 60 * 60 * 1000));
};

type Bucket = "ride" | "day" | "month";
const getBucket = (t: Ticket): Bucket => {
  const vf = (t as any).validFrom as string | undefined | null;
  const vt = (t as any).validTo as string | undefined | null;
  if (!vf || !vt) return "ride";
  const days = diffDays(vf, vt);
  return days <= 1 ? "day" : "month";
};

// --- Compact item cho vé Lượt ---
const RideItem: React.FC<{ t: Ticket; onViewDetail: (t: Ticket) => void }> = ({ t, onViewDetail }) => {
  const status = t.status as string | undefined;
  const activationDeadline = (t as any).activationDeadline as string | undefined | null;
  const activatedAt = (t as any).activatedAt as string | undefined | null;

  const statusColor =
    status === "Ready" ? "green" :
    status === "Active" ? "blue" :
    status === "Expired" ? "default" : "default";

  return (
    <div className="flex items-center justify-between rounded-lg border border-gray-200 p-3 bg-white hover:shadow-sm transition">
      <div className="min-w-0">
        <div className="flex items-center gap-2">
          <span className="font-semibold text-gray-800 truncate">{t.planName || "Vé lượt"}</span>
          <Tag color="purple" className="m-0">Vé lượt</Tag>
          <Tag color={statusColor} className="m-0">{status || "Unknown"}</Tag>
        </div>
        <div className="text-xs text-gray-500 mt-1 flex flex-wrap gap-x-4 gap-y-1">
          {activatedAt && <span>Đã kích hoạt: <b>{formatUtcToVN(activatedAt)}</b></span>}
          {!activatedAt && activationDeadline && <span>Hạn kích hoạt: <b>{formatUtcToVN(activationDeadline)}</b></span>}
          {t.serialCode && <span>Mã vé: <span className="font-mono">{t.serialCode}</span></span>}
        </div>
      </div>

      <div className="shrink-0 flex items-center gap-2">
        <Button size="small" onClick={() => onViewDetail(t)}>Xem chi tiết</Button>
      </div>
    </div>
  );
};

const MyTicketsPage: React.FC = () => {
  const { user, isLoadingUser } = useAuth();
  const [rideExpanded, setRideExpanded] = useState(false);
  const VISIBLE_RIDE_COUNT = 4; 
  // NEW: Drawer state
  const [detailOpen, setDetailOpen] = useState(false);
  const [selectedRide, setSelectedRide] = useState<Ticket | null>(null);

  const { data: ticketsResp, isLoading, isError, error } = useQuery({
    queryKey: ["myActiveTickets", user?.userId],
    queryFn: getMyActiveTickets,
    enabled: !!user?.userId,
  });

  const tickets: Ticket[] = ticketsResp?.data ?? [];

  const grouped = useMemo(() => {
    const buckets = { ride: [] as Ticket[], day: [] as Ticket[], month: [] as Ticket[] };
    const diffDays = (from?: string | null, to?: string | null) => {
      if (!from || !to) return 0;
      const ms = Math.max(0, new Date(to).getTime() - new Date(from).getTime());
      return Math.ceil(ms / 86400000);
    };
    const getBucket = (t: Ticket) => {
      const vf = (t as any).validFrom as string | undefined | null;
      const vt = (t as any).validTo as string | undefined | null;
      if (!vf || !vt) return "ride";
      return diffDays(vf, vt) <= 1 ? "day" : "month";
    };
    for (const t of tickets) buckets[getBucket(t)].push(t);
    return buckets;
  }, [tickets]);

  const counts = {
    ride: grouped.ride.length,
    day: grouped.day.length,
    month: grouped.month.length
  };

  const hasActiveSubscription = useMemo(
    () => tickets.some((t: any) => t.expiresAt && new Date(t.expiresAt) > new Date()),
    [tickets]
  );

  if (isLoadingUser || isLoading) {
    return <div className="flex justify-center items-center h-[50vh]"><Spin size="large" /></div>;
  }

  if (isError) {
    return (
      <div className="container mx-auto p-8">
        <Alert
          message="Lỗi"
          description={(error as any)?.message || "Không thể tải danh sách vé. Vui lòng thử lại."}
          type="error"
          showIcon
        />
      </div>
    );
  }

  const noTickets = tickets.length === 0;

  // Handler mở chi tiết
  const onViewDetail = (t: Ticket) => {
    setSelectedRide(t);
    setDetailOpen(true);
  };

  return (
    <div className="bg-gray-100 min-h-screen">
      <div className="container mx-auto p-4 md:p-8">
        <div className="w-full bg-eco-green-dark text-white text-center text-4xl font-bold p-6 rounded-t-xl shadow-lg">
          VÉ CỦA TÔI
        </div>

        <div className="bg-white p-6 rounded-b-xl shadow-lg">
          {hasActiveSubscription && (
            <Alert
              className="mb-6"
              type="success"
              showIcon
              message={<>Bạn đang có <Tag color="green">gói theo thời gian</Tag> còn hiệu lực.</>}
              description="Khi gói này hết hạn, bạn có thể mua gói Day/Month khác."
            />
          )}

          {noTickets ? (
            <div className="text-center p-12">
              <img src="/empty-box.png" alt="Không có vé" className="w-40 h-40 mx-auto mb-6" />
              <h3 className="text-2xl font-semibold text-gray-700 mb-3">Bạn chưa có vé nào</h3>
              <p className="text-gray-500 mb-6">Hãy mua một gói vé để bắt đầu hành trình của bạn!</p>
              <Button type="primary" size="large" className="bg-eco-green hover:bg-eco-green-dark">
                <Link to="/pricing">Xem bảng giá</Link>
              </Button>
            </div>
          ) : (
            <Tabs
  defaultActiveKey={counts.ride ? "ride" : counts.day ? "day" : "month"}
  items={[
    {
  key: "ride",
  label: (
    <span>
      Vé lượt <Badge count={counts.ride} overflowCount={99} offset={[6, -2]} />
    </span>
  ),
  children: counts.ride === 0 ? (
    <Empty description="Không có vé lượt" />
  ) : (
    <>
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {(rideExpanded ? grouped.ride : grouped.ride.slice(0, VISIBLE_RIDE_COUNT)).map((t) => (
          <TicketCard key={t.id} ticket={t} />
        ))}
      </div>
        {!rideExpanded && grouped.ride.length > VISIBLE_RIDE_COUNT && (
  <div className="pointer-events-none -mt-8 h-8 w-full bg-linear-to-t from-white to-transparent" />
)}
      {/* Nút xem thêm / thu gọn */}
      {grouped.ride.length > VISIBLE_RIDE_COUNT && (
        <div className="mt-4 text-center">
          <Button type="link" onClick={() => setRideExpanded(v => !v)}>
            {rideExpanded
              ? "Thu gọn"
              : `Xem thêm ${grouped.ride.length - VISIBLE_RIDE_COUNT} vé`}
          </Button>
        </div>
      )}
    </>
  ),
},
    {
      key: "day",
      label: (
        <span>
          Vé ngày <Badge count={counts.day} overflowCount={99} offset={[6, -2]} />
        </span>
      ),
      children: counts.day === 0 ? (
        <Empty description="Không có vé ngày" />
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {grouped.day.map((t) => (
            <TicketCard key={t.id} ticket={t} />
          ))}
        </div>
      ),
    },
    {
      key: "month",
      label: (
        <span>
          Vé tháng <Badge count={counts.month} overflowCount={99} offset={[6, -2]} />
        </span>
      ),
      children: counts.month === 0 ? (
        <Empty description="Không có vé tháng" />
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {grouped.month.map((t) => (
            <TicketCard key={t.id} ticket={t} />
          ))}
        </div>
      ),
    },
  ]}
/>
          )}
        </div>
      </div>

      {/* Drawer chi tiết vé lượt */}
      <Drawer
        open={detailOpen}
        onClose={() => setDetailOpen(false)}
        width={560}
        destroyOnClose
        title="Chi tiết vé lượt"
      >
        {selectedRide ? (
          <TicketCard ticket={selectedRide} />
        ) : (
          <Empty />
        )}
      </Drawer>
    </div>
  );
};

export default MyTicketsPage;