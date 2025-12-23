// src/features/home/components/DynamicPricingSection.tsx
import React, { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { 
  Card, 
  Segmented, 
  Tag, 
  Badge, 
  Button, 
  Skeleton, 
  Empty, 
  Space, 
  Divider 
} from 'antd';
// Sửa import: Dùng service giống BuyTicketsPage
import { getTicketMarket } from '../../../services/user.ticket.service'; 

// --- ICONS & ASSETS (Giữ nguyên) ---
const IconBase: React.FC<{ className?: string; label?: string; children?: React.ReactNode }> = ({ className, label, children }) => (
  <span role="img" aria-label={label} className={className} style={{ display: "inline-flex", alignItems: "center" }}>
    {children}
  </span>
);

const Bike: React.FC<{ className?: string }> = ({ className }) => <IconBase label="bike" className={className}>🚲</IconBase>;
const BikeElectric: React.FC<{ className?: string }> = ({ className }) => <IconBase label="ebike" className={className}>🚲⚡</IconBase>;
const Clock3: React.FC<{ className?: string }> = ({ className }) => <IconBase label="clock" className={className}>🕒</IconBase>;
const Ticket: React.FC<{ className?: string }> = ({ className }) => <IconBase label="ticket" className={className}>🎫</IconBase>;
const Info: React.FC<{ className?: string }> = ({ className }) => <IconBase label="info" className={className}>ℹ️</IconBase>;

// --- HELPER CONSTANTS & FUNCTIONS (Giống BuyTicketsPage) ---
const ecoGreen = {
  main: "#2E7D32",
  light: "#A5D6A7",
  dark: "#1B5E20",
};

const ecoBtnStyle: React.CSSProperties = {
  backgroundColor: ecoGreen.main,
  borderColor: ecoGreen.main,
  color: "#fff",
  width: '100%',
  marginTop: 'auto'
};

const currencyVN = (amount: number | undefined) => 
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount ?? 0);

const mapMode = (m: number | string | undefined): "IMMEDIATE" | "ON_FIRST_USE" =>
  m === 1 || m === "ON_FIRST_USE" ? "ON_FIRST_USE" : "IMMEDIATE";

const ModeBadge: React.FC<{ mode: "IMMEDIATE" | "ON_FIRST_USE" }> = ({ mode }) => (
  <Badge
    color={mode === "ON_FIRST_USE" ? "purple" : ecoGreen.main}
    text={mode === "ON_FIRST_USE" ? "Kích hoạt khi dùng" : "Kích hoạt ngay"}
  />
);

const DynamicPricingSection: React.FC = () => {
  // 1. State quản lý Tab
  const [vehicleTab, setVehicleTab] = useState<"bike" | "ebike">("bike");
  const vehicleParam = vehicleTab === "bike" ? "bike" : "ebike";

  // 2. Fetch dữ liệu dùng API getTicketMarket (GIỐNG BuyTicketsPage)
  const { data: marketData, isLoading, isError } = useQuery({
    queryKey: ["ticketMarket", vehicleParam], // Key thay đổi theo tab
    queryFn: () => getTicketMarket(vehicleParam),
    select: (res: any) => {
      // Xử lý response structure giống BuyTicketsPage
      const api = res?.data ?? res;
      return api?.data ?? api ?? []; 
    },
  });

  // 3. Logic lọc plans (Copy từ BuyTicketsPage)
  // Cấu trúc API này trả về: [ { id, name, prices: [...] }, ... ]
  const plansFiltered = useMemo(() => {
    const list = Array.isArray(marketData) ? marketData : [];
    return list
      .map((p: any) => ({
        ...p,
        // Lọc price bên trong từng plan khớp với loại xe đang chọn
        prices: (Array.isArray(p.prices) ? p.prices : []).filter((pr: any) =>
          vehicleParam
            ? pr?.vehicleType?.toLowerCase() === vehicleParam.toLowerCase()
            : true
        ),
      }))
      // Chỉ lấy những plan có ít nhất 1 price phù hợp
      .filter((p: any) => p.prices.length > 0);
  }, [marketData, vehicleParam]);


  if (isError) return null;

  return (
    <section className="py-24 bg-gray-50 relative overflow-hidden">
      {/* Background decoration */}
      <div className="absolute top-0 left-0 w-64 h-64 bg-green-200 rounded-full mix-blend-multiply filter blur-3xl opacity-30 animate-blob"></div>
      <div className="absolute top-0 right-0 w-64 h-64 bg-lime-200 rounded-full mix-blend-multiply filter blur-3xl opacity-30 animate-blob animation-delay-2000"></div>

      <div className="container mx-auto px-4 relative z-10">
        
        {/* Header Section */}
        <div className="text-center max-w-3xl mx-auto mb-10">
          <h2 className="text-sm font-bold tracking-widest text-green-600 uppercase mb-3">Bảng giá linh hoạt</h2>
          <h3 className="text-4xl font-bold text-gray-900 mb-6">Chọn gói phù hợp với hành trình</h3>
          <p className="text-gray-500 mb-8">
            Dù bạn đi dạo 1 vòng hay cần xe cả tháng, chúng tôi đều có gói cước tối ưu chi phí nhất.
          </p>

          {/* Segmented Control */}
          <div className="flex justify-center">
             <Segmented
              size="large"
              className="p-1 bg-white border border-green-100 shadow-sm rounded-xl"
              options={[
                {
                  label: <span className="flex items-center gap-2 px-4 py-1"><Bike className="text-xl" /> Xe đạp</span>,
                  value: 'bike',
                },
                {
                  label: <span className="flex items-center gap-2 px-4 py-1"><BikeElectric className="text-xl" /> Xe điện</span>,
                  value: 'ebike',
                },
              ]}
              value={vehicleTab}
              onChange={(v) => setVehicleTab(v as any)}
            />
          </div>
        </div>

        {/* Content Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 justify-center max-w-6xl mx-auto">
          
          {isLoading && Array.from({ length: 3 }).map((_, i) => (
             <Card key={i} className="rounded-2xl border-emerald-50">
                <Skeleton active paragraph={{ rows: 4 }} />
             </Card>
          ))}

          {!isLoading && plansFiltered.length === 0 && (
             <div className="col-span-full py-10 flex justify-center">
                <Empty description="Hiện chưa có gói cước cho loại xe này" />
             </div>
          )}

          {/* Render: Map 2 vòng lặp (Plan -> Prices) giống BuyTicketsPage */}
          {!isLoading && plansFiltered.map((plan: any) => (
             plan.prices.map((price: any) => {
               // Logic xác định gói phổ biến (Ví dụ: Vé ngày)
               const isPopular = plan.type === 'Day' || plan.name?.toLowerCase().includes('ngày');

               return (
                <Card
                  key={`${plan.id}-${price.id}`}
                  className={`flex flex-col h-full rounded-2xl transition-all duration-300
                    ${isPopular 
                      ? 'border-green-500 shadow-xl shadow-green-100 scale-105 z-10 border-2' 
                      : 'border-emerald-100 shadow-md hover:shadow-lg hover:border-emerald-400'
                    }
                  `}
                  bodyStyle={{ display: 'flex', flexDirection: 'column', height: '100%', padding: '1.5rem' }}
                >
                  {/* Badge Popular */}
                  {isPopular && (
                    <div className="absolute top-0 right-0 bg-orange-500 text-white text-xs font-bold px-3 py-1 rounded-bl-xl rounded-tr-lg">
                      PHỔ BIẾN
                    </div>
                  )}

                  {/* HEADER */}
                  <div className="mb-4">
                    <div className="flex items-center gap-2 mb-2">
                      <Ticket className="text-emerald-600" />
                      <h4 className="font-bold text-lg text-gray-800 line-clamp-1">
                        {plan.name} {/* Dùng plan.name thay vì ticketPlanName */}
                      </h4>
                    </div>
                    
                    <Space size={[0, 8]} wrap>
                       {/* Ribbon Logic */}
                       {plan.code === "RIDE" || plan.type === "Ride" ? <Tag color="purple">Vé lượt</Tag> : null}
                       {plan.code === "DAY" || plan.type === "Day" ? <Tag color="green">Vé ngày</Tag> : null}
                       {plan.type === "Month" ? <Tag color="blue">Vé tháng</Tag> : null}

                       {/* Vehicle Tag */}
                       {price.vehicleType?.toLowerCase() === 'ebike' 
                         ? <Tag color="orange" className="flex items-center gap-1"><BikeElectric /> Xe điện</Tag>
                         : <Tag color="cyan" className="flex items-center gap-1"><Bike /> Xe đạp</Tag>
                       }
                    </Space>
                  </div>

                  {/* PRICE BOX */}
                  <div className="mb-4 p-4 bg-emerald-50 rounded-xl border border-emerald-100 text-center">
                    <div className="text-3xl font-extrabold text-emerald-700">
                      {currencyVN(price.price)}
                    </div>
                    <div className="text-xs text-emerald-600 mt-1">
                      /{price.vehicleType === 'ebike' ? 'Xe điện' : 'Xe đạp'}
                    </div>
                  </div>

                  {/* DETAILS LIST */}
                  <div className="space-y-3 text-sm text-gray-600 mb-6 grow">
                    {/* Activation Mode */}
                    <div className="flex items-center justify-start">
                       <ModeBadge mode={mapMode(price.activationMode)} />
                    </div>

                    <Divider className="my-2 border-gray-200" />

                    {/* Limit Time */}
                    {typeof price.durationLimitMinutes === "number" && (
                      <div className="flex items-center gap-2">
                        <Clock3 className="text-gray-400" />
                        <span>Giới hạn: <b>{price.durationLimitMinutes} phút</b>/lượt</span>
                      </div>
                    )}

                    {/* Validity Days */}
                    {plan.type === "Month" && (
                       <div className="flex items-center gap-2">
                         <Info className="text-gray-400" />
                         <span>Hiệu lực: {price.validityDays} ngày</span>
                       </div>
                    )}
                    
                    {/* Note */}
                    <div className="flex items-start gap-2">
                      <span className="mt-0.5">✅</span>
                      <span>Hỗ trợ 24/7</span>
                    </div>
                  </div>

                  {/* FOOTER BUTTON */}
                  <Button 
                    type="primary" 
                    shape="round" 
                    size="large"
                    // Chuyển hướng sang trang mua vé khi bấm
                    href="/pricing" 
                    style={isPopular 
                      ? { ...ecoBtnStyle, boxShadow: '0 4px 14px 0 rgba(46, 125, 50, 0.39)' } 
                      : { backgroundColor: '#fff', color: ecoGreen.main, borderColor: ecoGreen.main, width: '100%', marginTop: 'auto' }
                    }
                    className="font-semibold"
                  >
                    Xem chi tiết
                  </Button>

                </Card>
               );
             })
          ))}
        </div>
      </div>
    </section>
  );
};

export default DynamicPricingSection;