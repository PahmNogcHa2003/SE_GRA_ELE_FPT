import React, { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useAuth } from "../../features/auth/context/authContext";
import { getMyActiveTickets } from "../../services/user.ticket.service";
import { Spin, Alert, Button, Tag, Tabs, Badge, Empty, Drawer } from "antd";
import { Link } from "react-router-dom";
import TicketCard from "../../features/tickets/components/TicketCard";

type Ticket = any; 


// [FIX] Hàm này được giữ lại và sẽ được dùng trong useMemo bên dưới
const diffDays = (from?: string | null, to?: string | null) => {
  if (!from || !to) return 0;
  const a = new Date(from).getTime();
  const b = new Date(to).getTime();
  const ms = Math.max(0, b - a);
  return Math.ceil(ms / (24 * 60 * 60 * 1000));
};

type Bucket = "ride" | "day" | "month";

// [FIX] Hàm này được giữ lại và sẽ được dùng trong useMemo bên dưới
const getBucket = (t: Ticket): Bucket => {
  const vf = (t as any).validFrom as string | undefined | null;
  const vt = (t as any).validTo as string | undefined | null;
  if (!vf || !vt) return "ride";
  const days = diffDays(vf, vt);
  return days <= 1 ? "day" : "month";
};

const MyTicketsPage: React.FC = () => {
  const { user, isLoadingUser } = useAuth();
  const [rideExpanded, setRideExpanded] = useState(false);
  const VISIBLE_RIDE_COUNT = 4;
  
  // NEW: Drawer state
  const [detailOpen, setDetailOpen] = useState(false);
  const [selectedRide] = useState<Ticket | null>(null);

  const { data: ticketsResp, isLoading, isError, error } = useQuery({
    queryKey: ["myActiveTickets", user?.userId],
    queryFn: getMyActiveTickets,
    enabled: !!user?.userId,
  });

  const tickets: Ticket[] = ticketsResp?.data ?? [];

  const grouped = useMemo(() => {
    const buckets = { ride: [] as Ticket[], day: [] as Ticket[], month: [] as Ticket[] };
    
    // [FIX QUAN TRỌNG]: Đã xóa các hàm diffDays và getBucket khai báo lại ở đây.
    // Code sẽ tự động dùng hàm getBucket ở phạm vi global (dòng 32).
    // Điều này giúp hàm getBucket global không bị báo lỗi "never read".

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