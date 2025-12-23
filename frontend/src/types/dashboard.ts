// types/dashboard.ts

// ==========================================
// ROOT DTO
// ==========================================

/**
 * DTO chứa toàn bộ thống kê cho Dashboard
 */
export interface DashboardStatisticsDTO {
  revenue: RevenueStatisticsDTO;
  rentals: RentalStatisticsDTO;
  users: UserStatisticsDTO;
  vehicles: VehicleStatisticsDTO;
  stations: StationStatisticsDTO;
  orders: OrderStatisticsDTO;
  payments: PaymentStatisticsDTO;
}

// ==========================================
// REVENUE (DOANH THU)
// ==========================================

export interface RevenueStatisticsDTO {
  /** Tổng doanh thu (từ Orders có Status = Success) */
  totalRevenue: number;
  /** Doanh thu hôm nay */
  todayRevenue: number;
  /** Doanh thu tuần này */
  thisWeekRevenue: number;
  /** Doanh thu tháng này */
  thisMonthRevenue: number;
  /** Doanh thu tháng trước */
  lastMonthRevenue: number;
  /** Doanh thu theo ngày (7 ngày gần nhất) */
  revenueByDate: RevenueByDateDTO[];
  /** Doanh thu theo tuần (12 tuần gần nhất) */
  revenueByWeek: RevenueByWeekDTO[];
  /** Doanh thu theo tháng (12 tháng gần nhất) */
  revenueByMonth: RevenueByMonthDTO[];
  /** Doanh thu theo loại Order */
  revenueByOrderType: RevenueByOrderTypeDTO[];
}

export interface RevenueByDateDTO {
  date: string; // DateTime ISO string
  revenue: number;
  orderCount: number;
}

export interface RevenueByWeekDTO {
  weekNumber: number;
  year: number;
  weekStartDate: string; // DateTime ISO string
  revenue: number;
  orderCount: number;
}

export interface RevenueByMonthDTO {
  month: number;
  year: number;
  monthName: string;
  revenue: number;
  orderCount: number;
}

export interface RevenueByOrderTypeDTO {
  orderType: string;
  revenue: number;
  orderCount: number;
  percentage: number;
}

// ==========================================
// RENTALS (THUÊ XE)
// ==========================================

export interface RentalStatisticsDTO {
  totalRentals: number;
  ongoingRentals: number;
  completedRentals: number;
  cancelledRentals: number;
  todayRentals: number;
  thisWeekRentals: number;
  thisMonthRentals: number;
  /** Số lượng rental theo ngày (7 ngày gần nhất) */
  rentalCountByDate: RentalCountByDateDTO[];
}

export interface RentalCountByDateDTO {
  date: string; // DateTime ISO string
  count: number;
}

// ==========================================
// USERS (NGƯỜI DÙNG)
// ==========================================

export interface UserStatisticsDTO {
  totalUsers: number;
  newUsersToday: number;
  newUsersThisWeek: number;
  newUsersThisMonth: number;
  /** Users có ít nhất 1 rental trong tháng */
  activeUsersThisMonth: number;
  /** Số lượng user mới theo ngày (30 ngày gần nhất) */
  newUsersByDate: UserCountByDateDTO[];
}

export interface UserCountByDateDTO {
  date: string; // DateTime ISO string
  count: number;
}

// ==========================================
// VEHICLES (XE)
// ==========================================

export interface VehicleStatisticsDTO {
  totalVehicles: number;
  availableVehicles: number;
  inUseVehicles: number;
  maintenanceVehicles: number;
  unavailableVehicles: number;
  /** Phân bổ vehicle theo status */
  vehicleCountByStatus: VehicleCountByStatusDTO[];
}

export interface VehicleCountByStatusDTO {
  status: string;
  count: number;
  percentage: number;
}

// ==========================================
// STATIONS (TRẠM XE)
// ==========================================

export interface StationStatisticsDTO {
  totalStations: number;
  activeStations: number;
  inactiveStations: number;
}

// ==========================================
// ORDERS (ĐƠN HÀNG)
// ==========================================

export interface OrderStatisticsDTO {
  totalOrders: number;
  pendingOrders: number;
  successOrders: number;
  failedOrders: number;
  todayOrders: number;
  thisWeekOrders: number;
  thisMonthOrders: number;
  /** Số lượng order theo status */
  orderCountByStatus: OrderCountByStatusDTO[];
  /** Số lượng order theo loại */
  orderCountByType: OrderCountByTypeDTO[];
}

export interface OrderCountByStatusDTO {
  status: string;
  count: number;
  percentage: number;
}

export interface OrderCountByTypeDTO {
  orderType: string;
  count: number;
  percentage: number;
}

// ==========================================
// PAYMENTS (THANH TOÁN)
// ==========================================

export interface PaymentStatisticsDTO {
  totalPayments: number;
  successPayments: number;
  failedPayments: number;
  pendingPayments: number;
  totalPaymentAmount: number;
  successPaymentAmount: number;
  /** Số lượng payment theo status */
  paymentCountByStatus: PaymentCountByStatusDTO[];
  /** Số lượng payment theo provider (VNPay, Wallet, ...) */
  paymentCountByProvider: PaymentCountByProviderDTO[];
}

export interface PaymentCountByStatusDTO {
  status: string;
  count: number;
  amount: number;
  percentage: number;
}

export interface PaymentCountByProviderDTO {
  provider: string;
  count: number;
  amount: number;
  percentage: number;
}