// src/pages/admin/DashboardPage.tsx
import React, { useState, useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import {
  Card,
  Row,
  Col,
  Statistic,
  DatePicker,
  Spin,
  Typography,
  Space,
  Tag,
  Progress,
  Select,
  Alert,
  Grid,
} from 'antd';
import {
  AreaChart,
  Area,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  LineChart,
  Line,
} from 'recharts';
import {
  DollarOutlined,
  UserOutlined,
  CarOutlined,
  ShoppingCartOutlined,
  CreditCardOutlined,
  EnvironmentOutlined,
  RiseOutlined,
  FallOutlined,
  ClockCircleOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  BarChartOutlined,
  DashboardOutlined,
  CalendarOutlined,
} from '@ant-design/icons';
import dayjs from 'dayjs';
import { dashboardService } from '../../services/dashboard.service';


const { Title, Text } = Typography;
const { RangePicker } = DatePicker;
const { Option } = Select;
const { useBreakpoint } = Grid;

// Format currency
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
  }).format(amount);
};

// Format number with commas
const formatNumber = (num: number) => {
  return new Intl.NumberFormat('vi-VN').format(num);
};

// Custom tooltip for charts
const CustomTooltip = ({ active, payload, label }: any) => {
  if (active && payload && payload.length) {
    return (
      <div className="bg-white p-3 border border-gray-200 shadow-lg rounded-lg">
        <p className="font-semibold text-gray-800 mb-2">{label}</p>
        {payload.map((entry: any, index: number) => (
          <div key={index} className="flex items-center gap-2 mb-1">
            <div
              className="w-3 h-3 rounded-full"
              style={{ backgroundColor: entry.color }}
            />
            <span className="text-gray-600">{entry.name}: </span>
            <span className="font-semibold">
              {entry.name.includes('Doanh thu') || entry.name.includes('Tiền')
                ? formatCurrency(entry.value)
                : formatNumber(entry.value)}
            </span>
          </div>
        ))}
      </div>
    );
  }
  return null;
};

const DashboardPage: React.FC = () => {
  const [dateRange, setDateRange] = useState<[dayjs.Dayjs, dayjs.Dayjs] | null>(null);
  const [viewMode, setViewMode] = useState<'overview' | 'revenue' | 'rentals' | 'users'>('overview');
  const screens = useBreakpoint();

  // Query for dashboard data
    const { data: dashboardData = {} as any, isLoading, isError } = useQuery({
      queryKey: ['dashboard', dateRange, viewMode],
      queryFn: async () => {
        const fromDate = dateRange?.[0]?.format('YYYY-MM-DD');
        const toDate = dateRange?.[1]?.format('YYYY-MM-DD');
        
        if (viewMode === 'overview') {
          return await dashboardService.getAllStatistics(fromDate, toDate);
        } else if (viewMode === 'revenue') {
          return await dashboardService.getRevenueStatistics(fromDate, toDate);
        } else if (viewMode === 'rentals') {
          return await dashboardService.getRentalStatistics(fromDate, toDate);
        } else {
          return await dashboardService.getUserStatistics(fromDate, toDate);
        }
      },
      select: (data) => data.data,
    });

  // Prepare data for charts
  const revenueChartData = useMemo(() => {
    if (!dashboardData) return [];
    if ('revenue' in dashboardData) {
      return dashboardData.revenue.revenueByDate.map((item: { date: string | number | dayjs.Dayjs | Date | null | undefined; revenue: any; orderCount: any; }) => ({
        name: dayjs(item.date).format('DD/MM'),
        'Doanh thu': item.revenue,
        'Số đơn': item.orderCount,
      }));
    }
    if ('revenueByDate' in dashboardData) {
      return dashboardData.revenueByDate.map((item: { date: string | number | dayjs.Dayjs | Date | null | undefined; revenue: any; orderCount: any; }) => ({
        name: dayjs(item.date).format('DD/MM'),
        'Doanh thu': item.revenue,
        'Số đơn': item.orderCount,
      }));
    }
    return [];
  }, [dashboardData]);

  const userGrowthData = useMemo(() => {
    if (!dashboardData) return [];
    if ('users' in dashboardData) {
      return dashboardData.users.newUsersByDate.slice(-15).map((item: { date: string | number | dayjs.Dayjs | Date | null | undefined; count: any; }) => ({
        name: dayjs(item.date).format('DD/MM'),
        'Người dùng mới': item.count,
      }));
    }
    if ('newUsersByDate' in dashboardData) {
      return dashboardData.newUsersByDate.slice(-15).map((item: { date: string | number | Date | dayjs.Dayjs | null | undefined; count: any; }) => ({
        name: dayjs(item.date).format('DD/MM'),
        'Người dùng mới': item.count,
      }));
    }
    return [];
  }, [dashboardData]);

  const rentalChartData = useMemo(() => {
    if (!dashboardData) return [];
    if ('rentals' in dashboardData) {
      return dashboardData.rentals.rentalCountByDate.map((item: { date: string | number | dayjs.Dayjs | Date | null | undefined; count: any; }) => ({
        name: dayjs(item.date).format('DD/MM'),
        'Lượt thuê': item.count,
      }));
    }
    if ('rentalCountByDate' in dashboardData) {
      return dashboardData.rentalCountByDate.map((item: { date: string | number | Date | dayjs.Dayjs | null | undefined; count: any; }) => ({
        name: dayjs(item.date).format('DD/MM'),
        'Lượt thuê': item.count,
      }));
    }
    return [];
  }, [dashboardData]);

  const vehicleStatusData = useMemo(() => {
    if (!dashboardData || !('vehicles' in dashboardData)) return [];
    return dashboardData.vehicles.vehicleCountByStatus.map((item: { status: any; count: any; percentage: any; }) => ({
      name: item.status,
      value: item.count,
      percentage: item.percentage,
    }));
  }, [dashboardData]);

  const orderStatusData = useMemo(() => {
    if (!dashboardData || !('orders' in dashboardData)) return [];
    return dashboardData.orders.orderCountByStatus.map((item: { status: any; count: any; percentage: any; }) => ({
      name: item.status,
      value: item.count,
      percentage: item.percentage,
    }));
  }, [dashboardData]);

  const paymentProviderData = useMemo(() => {
    if (!dashboardData || !('payments' in dashboardData)) return [];
    return dashboardData.payments.paymentCountByProvider.map((item: { provider: any; count: any; amount: any; percentage: any; }) => ({
      name: item.provider,
      value: item.count,
      amount: item.amount,
      percentage: item.percentage,
    }));
  }, [dashboardData]);

  const COLORS = ['#10B981', '#3B82F6', '#F59E0B', '#EF4444', '#8B5CF6'];


  const getStatusText = (status: string) => {
    switch (status.toLowerCase()) {
      case 'success': return 'Thành công';
      case 'pending': return 'Đang chờ';
      case 'failed': return 'Thất bại';
      case 'available': return 'Khả dụng';
      case 'inuse': return 'Đang sử dụng';
      case 'maintenance': return 'Bảo trì';
      case 'unavailable': return 'Không khả dụng';
      case 'ongoing': return 'Đang diễn ra';
      case 'completed': return 'Hoàn thành';
      case 'cancelled': return 'Đã hủy';
      default: return status;
    }
  };

  if (isError) {
    return (
      <div className="p-8">
        <Alert
          message="Lỗi tải dữ liệu"
          description="Không thể tải dữ liệu dashboard. Vui lòng thử lại sau."
          type="error"
          showIcon
        />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 p-4 md:p-6">
      {/* Header */}
      <div className="mb-6">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-6">
          <div className="flex items-center gap-3">
            <div className="p-3 bg-linear-to-r from-blue-500 to-blue-600 rounded-xl shadow-lg">
              <DashboardOutlined className="text-2xl text-white" />
            </div>
            <div>
              <Title level={2} className="mb-0! text-gray-800">
                Dashboard Analytics
              </Title>
              <Text type="secondary">
                Tổng quan hệ thống và thống kê chi tiết
              </Text>
            </div>
          </div>

          <Space direction={screens.xs ? "vertical" : "horizontal"} size="middle">
            <Select
              value={viewMode}
              onChange={setViewMode}
              style={{ width: screens.xs ? '100%' : 180 }}
              size="large"
              className="rounded-lg"
            >
              <Option value="overview">
                <div className="flex items-center gap-2">
                  <BarChartOutlined />
                  Tổng quan
                </div>
              </Option>
              <Option value="revenue">
                <div className="flex items-center gap-2">
                  <DollarOutlined />
                  Doanh thu
                </div>
              </Option>
              <Option value="rentals">
                <div className="flex items-center gap-2">
                  <CarOutlined />
                  Thuê xe
                </div>
              </Option>
              <Option value="users">
                <div className="flex items-center gap-2">
                  <UserOutlined />
                  Người dùng
                </div>
              </Option>
            </Select>

            <RangePicker
              size="large"
              format="DD/MM/YYYY"
              className="rounded-lg"
              onChange={(dates) => setDateRange(dates as any)}
            />
          </Space>
        </div>

        {dateRange && (
          <Alert
            message={`Đang hiển thị dữ liệu từ ${dateRange[0].format('DD/MM/YYYY')} đến ${dateRange[1].format('DD/MM/YYYY')}`}
            type="info"
            showIcon
            icon={<CalendarOutlined />}
            className="mb-6 rounded-lg"
          />
        )}
      </div>

      {isLoading ? (
        <div className="flex justify-center items-center h-96">
          <Spin size="large" />
        </div>
      ) : (
        <>
          {/* Stats Cards */}
          <Row gutter={[16, 16]} className="mb-6">
            {/* Revenue Card */}
            <Col xs={24} md={6}>
              <Card className="h-full border-0 shadow-lg hover:shadow-xl transition-shadow rounded-2xl">
                <Statistic
                  title={
                    <div className="flex items-center gap-2 text-gray-600">
                      <DollarOutlined />
                      <span>Doanh thu tổng</span>
                    </div>
                  }
                  value={
                    'revenue' in dashboardData
                      ? dashboardData.revenue.totalRevenue
                      : 'totalRevenue' in dashboardData
                      ? dashboardData.totalRevenue
                      : 0
                  }
                  prefix="₫"
                  valueStyle={{
                    fontSize: screens.xs ? '24px' : '28px',
                    fontWeight: 'bold',
                    color: '#10B981',
                  }}
                  suffix={
                    'revenue' in dashboardData && dashboardData.revenue.lastMonthRevenue > 0 ? (
                      <div className="flex items-center gap-1 text-sm ml-2">
                        {dashboardData.revenue.thisMonthRevenue > dashboardData.revenue.lastMonthRevenue ? (
                          <RiseOutlined className="text-green-500" />
                        ) : (
                          <FallOutlined className="text-red-500" />
                        )}
                        <span className={
                          dashboardData.revenue.thisMonthRevenue > dashboardData.revenue.lastMonthRevenue
                            ? 'text-green-500'
                            : 'text-red-500'
                        }>
                          {Math.abs(
                            ((dashboardData.revenue.thisMonthRevenue - dashboardData.revenue.lastMonthRevenue) /
                              dashboardData.revenue.lastMonthRevenue) *
                              100
                          ).toFixed(1)}
                          %
                        </span>
                      </div>
                    ) : null
                  }
                />
                <div className="mt-4 pt-4 border-t border-gray-100">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Hôm nay:</span>
                    <span className="font-semibold">
                      {formatCurrency(
                        'revenue' in dashboardData
                          ? dashboardData.revenue.todayRevenue
                          : 'todayRevenue' in dashboardData
                          ? dashboardData.todayRevenue
                          : 0
                      )}
                    </span>
                  </div>
                  <div className="flex justify-between text-sm mt-1">
                    <span className="text-gray-500">Tháng này:</span>
                    <span className="font-semibold">
                      {formatCurrency(
                        'revenue' in dashboardData
                          ? dashboardData.revenue.thisMonthRevenue
                          : 'thisMonthRevenue' in dashboardData
                          ? dashboardData.thisMonthRevenue
                          : 0
                      )}
                    </span>
                  </div>
                </div>
              </Card>
            </Col>

            {/* Users Card */}
            <Col xs={24} md={6}>
              <Card className="h-full border-0 shadow-lg hover:shadow-xl transition-shadow rounded-2xl">
                <Statistic
                  title={
                    <div className="flex items-center gap-2 text-gray-600">
                      <UserOutlined />
                      <span>Tổng người dùng</span>
                    </div>
                  }
                  value={
                    'users' in dashboardData
                      ? dashboardData.users.totalUsers
                      : 'totalUsers' in dashboardData
                      ? dashboardData.totalUsers
                      : 0
                  }
                  valueStyle={{
                    fontSize: screens.xs ? '24px' : '28px',
                    fontWeight: 'bold',
                    color: '#3B82F6',
                  }}
                  suffix={
                    <Tag color="blue" className="ml-2">
                      {formatNumber(
                        'users' in dashboardData
                          ? dashboardData.users.activeUsersThisMonth
                          : 'activeUsersThisMonth' in dashboardData
                          ? dashboardData.activeUsersThisMonth
                          : 0
                      )}{' '}
                      active
                    </Tag>
                  }
                />
                <div className="mt-4 pt-4 border-t border-gray-100">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Hôm nay:</span>
                    <span className="font-semibold text-blue-600">
                      +
                      {'users' in dashboardData
                        ? dashboardData.users.newUsersToday
                        : 'newUsersToday' in dashboardData
                        ? dashboardData.newUsersToday
                        : 0}
                    </span>
                  </div>
                  <div className="flex justify-between text-sm mt-1">
                    <span className="text-gray-500">Tháng này:</span>
                    <span className="font-semibold text-blue-600">
                      +
                      {'users' in dashboardData
                        ? dashboardData.users.newUsersThisMonth
                        : 'newUsersThisMonth' in dashboardData
                        ? dashboardData.newUsersThisMonth
                        : 0}
                    </span>
                  </div>
                </div>
              </Card>
            </Col>

            {/* Rentals Card */}
            <Col xs={24} md={6}>
              <Card className="h-full border-0 shadow-lg hover:shadow-xl transition-shadow rounded-2xl">
                <Statistic
                  title={
                    <div className="flex items-center gap-2 text-gray-600">
                      <CarOutlined />
                      <span>Tổng lượt thuê</span>
                    </div>
                  }
                  value={
                    'rentals' in dashboardData
                      ? dashboardData.rentals.totalRentals
                      : 'totalRentals' in dashboardData
                      ? dashboardData.totalRentals
                      : 0
                  }
                  valueStyle={{
                    fontSize: screens.xs ? '24px' : '28px',
                    fontWeight: 'bold',
                    color: '#8B5CF6',
                  }}
                  suffix={
                    <div className="flex items-center gap-1 ml-2">
                      <Tag color="green">
                        {
                          'rentals' in dashboardData
                            ? dashboardData.rentals.completedRentals
                            : 'completedRentals' in dashboardData
                            ? dashboardData.completedRentals
                            : 0
                        }
                      </Tag>
                      <Tag color="red">
                        {
                          'rentals' in dashboardData
                            ? dashboardData.rentals.cancelledRentals
                            : 'cancelledRentals' in dashboardData
                            ? dashboardData.cancelledRentals
                            : 0
                        }
                      </Tag>
                    </div>
                  }
                />
                <div className="mt-4 pt-4 border-t border-gray-100">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Hôm nay:</span>
                    <span className="font-semibold text-purple-600">
                      {
                        'rentals' in dashboardData
                          ? dashboardData.rentals.todayRentals
                          : 'todayRentals' in dashboardData
                          ? dashboardData.todayRentals
                          : 0
                      }
                    </span>
                  </div>
                  <div className="flex justify-between text-sm mt-1">
                    <span className="text-gray-500">Tháng này:</span>
                    <span className="font-semibold text-purple-600">
                      {
                        'rentals' in dashboardData
                          ? dashboardData.rentals.thisMonthRentals
                          : 'thisMonthRentals' in dashboardData
                          ? dashboardData.thisMonthRentals
                          : 0
                      }
                    </span>
                  </div>
                </div>
              </Card>
            </Col>

            {/* Orders Card */}
            <Col xs={24} md={6}>
              <Card className="h-full border-0 shadow-lg hover:shadow-xl transition-shadow rounded-2xl">
                <Statistic
                  title={
                    <div className="flex items-center gap-2 text-gray-600">
                      <ShoppingCartOutlined />
                      <span>Tổng đơn hàng</span>
                    </div>
                  }
                  value={
                    'orders' in dashboardData
                      ? dashboardData.orders.totalOrders
                      : 'totalOrders' in dashboardData
                      ? dashboardData.totalOrders
                      : 0
                  }
                  valueStyle={{
                    fontSize: screens.xs ? '24px' : '28px',
                    fontWeight: 'bold',
                    color: '#F59E0B',
                  }}
                  suffix={
                    <div className="flex items-center gap-1 ml-2">
                      <Tag color="green">
                        {
                          'orders' in dashboardData
                            ? dashboardData.orders.successOrders
                            : 'successOrders' in dashboardData
                            ? dashboardData.successOrders
                            : 0
                        }
                      </Tag>
                      <Tag color="orange">
                        {
                          'orders' in dashboardData
                            ? dashboardData.orders.pendingOrders
                            : 'pendingOrders' in dashboardData
                            ? dashboardData.pendingOrders
                            : 0
                        }
                      </Tag>
                    </div>
                  }
                />
                <div className="mt-4 pt-4 border-t border-gray-100">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Tỉ lệ thành công:</span>
                    <span className="font-semibold text-green-600">
                      {(
                        (('orders' in dashboardData
                          ? dashboardData.orders.successOrders
                          : 'successOrders' in dashboardData
                          ? dashboardData.successOrders
                          : 0) /
                          ('orders' in dashboardData
                            ? dashboardData.orders.totalOrders
                            : 'totalOrders' in dashboardData
                            ? dashboardData.totalOrders
                            : 1)) *
                        100
                      ).toFixed(1)}
                      %
                    </span>
                  </div>
                  <div className="flex justify-between text-sm mt-1">
                    <span className="text-gray-500">Hôm nay:</span>
                    <span className="font-semibold text-orange-600">
                      {
                        'orders' in dashboardData
                          ? dashboardData.orders.todayOrders
                          : 'todayOrders' in dashboardData
                          ? dashboardData.todayOrders
                          : 0
                      }
                    </span>
                  </div>
                </div>
              </Card>
            </Col>
          </Row>

          {/* Charts Section */}
          <Row gutter={[16, 16]} className="mb-6">
            {/* Revenue Chart */}
            <Col xs={24} lg={16}>
              <Card
                title={
                  <div className="flex items-center gap-2">
                    <DollarOutlined className="text-green-500" />
                    <span>Doanh thu 7 ngày gần nhất</span>
                  </div>
                }
                className="border-0 shadow-lg rounded-2xl h-full"
              >
                <div className="h-72">
                  <ResponsiveContainer width="100%" height="100%">
                    <AreaChart data={revenueChartData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                      <XAxis
                        dataKey="name"
                        stroke="#666"
                        fontSize={12}
                      />
                      <YAxis
                        stroke="#666"
                        fontSize={12}
                        tickFormatter={(value) => formatCurrency(value)}
                      />
                      <Tooltip content={<CustomTooltip />} />
                      <Legend />
                      <Area
                        type="monotone"
                        dataKey="Doanh thu"
                        stroke="#10B981"
                        fill="#10B981"
                        fillOpacity={0.3}
                        strokeWidth={2}
                      />
                      <Area
                        type="monotone"
                        dataKey="Số đơn"
                        stroke="#3B82F6"
                        fill="#3B82F6"
                        fillOpacity={0.3}
                        strokeWidth={2}
                      />
                    </AreaChart>
                  </ResponsiveContainer>
                </div>
              </Card>
            </Col>

            {/* Vehicle Status */}
            <Col xs={24} lg={8}>
              <Card
                title={
                  <div className="flex items-center gap-2">
                    <CarOutlined className="text-purple-500" />
                    <span>Trạng thái xe</span>
                  </div>
                }
                className="border-0 shadow-lg rounded-2xl h-full"
              >
                {vehicleStatusData.length > 0 ? (
                  <>
                    <div className="h-48 mb-4">
                      <ResponsiveContainer width="100%" height="100%">
                        <PieChart>
                          <Pie
                            data={vehicleStatusData}
                            cx="50%"
                            cy="50%"
                            innerRadius={60}
                            outerRadius={80}
                            paddingAngle={5}
                            dataKey="value"
                          >
                            {vehicleStatusData.map((_entry: any, index: number) => (
                              <Cell
                                key={`cell-${index}`}
                                fill={COLORS[index % COLORS.length]}
                              />
                            ))}
                          </Pie>
                          <Tooltip formatter={(value) => formatNumber(value as number)} />
                          <Legend />
                        </PieChart>
                      </ResponsiveContainer>
                    </div>
                    <div className="space-y-2">
                      {vehicleStatusData.map((item: { name: string; value: number; percentage: number; }, index: number) => (
                        <div key={index} className="flex items-center justify-between">
                          <div className="flex items-center gap-2">
                            <div
                              className="w-3 h-3 rounded-full"
                              style={{ backgroundColor: COLORS[index % COLORS.length] }}
                            />
                            <span>{getStatusText(item.name)}</span>
                          </div>
                          <div className="text-right">
                            <span className="font-semibold">{formatNumber(item.value)}</span>
                            <span className="text-gray-500 text-sm ml-2">
                              ({item.percentage.toFixed(1)}%)
                            </span>
                          </div>
                        </div>
                      ))}
                    </div>
                  </>
                ) : (
                  <div className="h-48 flex items-center justify-center">
                    <Text type="secondary">Không có dữ liệu</Text>
                  </div>
                )}
              </Card>
            </Col>
          </Row>

          {/* Second Row Charts */}
          <Row gutter={[16, 16]} className="mb-6">
            {/* User Growth */}
            <Col xs={24} lg={12}>
              <Card
                title={
                  <div className="flex items-center gap-2">
                    <UserOutlined className="text-blue-500" />
                    <span>Tăng trưởng người dùng (15 ngày)</span>
                  </div>
                }
                className="border-0 shadow-lg rounded-2xl h-full"
              >
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={userGrowthData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                      <XAxis
                        dataKey="name"
                        stroke="#666"
                        fontSize={12}
                      />
                      <YAxis
                        stroke="#666"
                        fontSize={12}
                      />
                      <Tooltip content={<CustomTooltip />} />
                      <Legend />
                      <Bar
                        dataKey="Người dùng mới"
                        fill="#3B82F6"
                        radius={[4, 4, 0, 0]}
                      />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </Card>
            </Col>

            {/* Rental Trends */}
            <Col xs={24} lg={12}>
              <Card
                title={
                  <div className="flex items-center gap-2">
                    <CarOutlined className="text-green-500" />
                    <span>Xu hướng thuê xe</span>
                  </div>
                }
                className="border-0 shadow-lg rounded-2xl h-full"
              >
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <LineChart data={rentalChartData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                      <XAxis
                        dataKey="name"
                        stroke="#666"
                        fontSize={12}
                      />
                      <YAxis
                        stroke="#666"
                        fontSize={12}
                      />
                      <Tooltip content={<CustomTooltip />} />
                      <Legend />
                      <Line
                        type="monotone"
                        dataKey="Lượt thuê"
                        stroke="#10B981"
                        strokeWidth={2}
                        dot={{ r: 4 }}
                        activeDot={{ r: 6 }}
                      />
                    </LineChart>
                  </ResponsiveContainer>
                </div>
              </Card>
            </Col>
          </Row>

          {/* Details Section */}
          <Row gutter={[16, 16]}>
            {/* Order Status */}
            <Col xs={24} lg={8}>
              <Card
                title={
                  <div className="flex items-center gap-2">
                    <ShoppingCartOutlined className="text-orange-500" />
                    <span>Trạng thái đơn hàng</span>
                  </div>
                }
                className="border-0 shadow-lg rounded-2xl h-full"
              >
                <div className="space-y-4">
                  {orderStatusData.map((item: { name: string; value: number; percentage: number }, index: number) => (
                    <div key={index} className="space-y-2">
                      <div className="flex justify-between items-center">
                        <div className="flex items-center gap-2">
                          {item.name.toLowerCase() === 'success' && (
                            <CheckCircleOutlined className="text-green-500" />
                          )}
                          {item.name.toLowerCase() === 'pending' && (
                            <ClockCircleOutlined className="text-orange-500" />
                          )}
                          {item.name.toLowerCase() === 'failed' && (
                            <CloseCircleOutlined className="text-red-500" />
                          )}
                          <span>{getStatusText(item.name)}</span>
                        </div>
                        <div className="text-right">
                          <span className="font-semibold">{formatNumber(item.value)}</span>
                          <span className="text-gray-500 text-sm ml-2">
                            ({item.percentage.toFixed(1)}%)
                          </span>
                        </div>
                      </div>
                      <Progress
                        percent={parseFloat(item.percentage.toFixed(1))}
                        strokeColor={
                          item.name.toLowerCase() === 'success'
                            ? '#10B981'
                            : item.name.toLowerCase() === 'pending'
                            ? '#F59E0B'
                            : '#EF4444'
                        }
                        showInfo={false}
                      />
                    </div>
                  ))}
                </div>
              </Card>
            </Col>

            {/* Payment Providers */}
            <Col xs={24} lg={8}>
              <Card
                title={
                  <div className="flex items-center gap-2">
                    <CreditCardOutlined className="text-purple-500" />
                    <span>Nhà cung cấp thanh toán</span>
                  </div>
                }
                className="border-0 shadow-lg rounded-2xl h-full"
              >
                {paymentProviderData.length > 0 ? (
                  <div className="space-y-4">
                    {paymentProviderData.map((item: { name: string; value: number; amount: number; percentage: number }, index: number) => (
                      <div key={index} className="space-y-2">
                        <div className="flex justify-between items-center">
                          <span className="font-medium">{item.name}</span>
                          <div className="text-right">
                            <div className="font-semibold">{formatNumber(item.value)} giao dịch</div>
                            <div className="text-gray-500 text-sm">
                              {formatCurrency(item.amount)}
                            </div>
                          </div>
                        </div>
                        <div className="flex items-center gap-2">
                          <Progress
                            percent={parseFloat(item.percentage.toFixed(1))}
                            strokeColor={COLORS[index % COLORS.length]}
                            className="flex-1"
                          />
                          <span className="text-gray-500 text-sm w-12 text-right">
                            {item.percentage.toFixed(1)}%
                          </span>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="h-48 flex items-center justify-center">
                    <Text type="secondary">Không có dữ liệu</Text>
                  </div>
                )}
              </Card>
            </Col>

            {/* System Info */}
            <Col xs={24} lg={8}>
              <Card
                title={
                  <div className="flex items-center gap-2">
                    <EnvironmentOutlined className="text-blue-500" />
                    <span>Thông tin hệ thống</span>
                  </div>
                }
                className="border-0 shadow-lg rounded-2xl h-full"
              >
                {'vehicles' in dashboardData && (
                  <div className="space-y-4">
                    <div className="flex justify-between items-center p-3 bg-blue-50 rounded-lg">
                      <div className="flex items-center gap-2">
                        <CarOutlined className="text-blue-500" />
                        <span>Tổng số xe</span>
                      </div>
                      <span className="text-2xl font-bold text-blue-600">
                        {formatNumber(dashboardData.vehicles.totalVehicles)}
                      </span>
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                      <div className="p-3 bg-green-50 rounded-lg text-center">
                        <div className="text-sm text-gray-600">Xe khả dụng</div>
                        <div className="text-xl font-bold text-green-600">
                          {formatNumber(dashboardData.vehicles.availableVehicles)}
                        </div>
                      </div>
                      <div className="p-3 bg-blue-50 rounded-lg text-center">
                        <div className="text-sm text-gray-600">Xe đang thuê</div>
                        <div className="text-xl font-bold text-blue-600">
                          {formatNumber(dashboardData.vehicles.inUseVehicles)}
                        </div>
                      </div>
                    </div>

                    {'stations' in dashboardData && (
                      <div className="p-3 bg-purple-50 rounded-lg">
                        <div className="flex justify-between items-center">
                          <div className="flex items-center gap-2">
                            <EnvironmentOutlined className="text-purple-500" />
                            <span>Trạm xe</span>
                          </div>
                          <div className="text-right">
                            <div className="text-lg font-bold text-purple-600">
                              {formatNumber(dashboardData.stations.totalStations)}
                            </div>
                            <div className="text-sm text-gray-500">
                              {dashboardData.stations.activeStations} đang hoạt động
                            </div>
                          </div>
                        </div>
                      </div>
                    )}

                    {'payments' in dashboardData && (
                      <div className="p-3 bg-emerald-50 rounded-lg">
                        <div className="flex justify-between items-center">
                          <div className="flex items-center gap-2">
                            <CreditCardOutlined className="text-emerald-500" />
                            <span>Tổng thanh toán</span>
                          </div>
                          <div className="text-right">
                            <div className="text-lg font-bold text-emerald-600">
                              {formatCurrency(dashboardData.payments.totalPaymentAmount)}
                            </div>
                            <div className="text-sm text-gray-500">
                              {dashboardData.payments.successPayments} thành công
                            </div>
                          </div>
                        </div>
                      </div>
                    )}
                  </div>
                )}
              </Card>
            </Col>
          </Row>
        </>
      )}
    </div>
  );
};

export default DashboardPage;