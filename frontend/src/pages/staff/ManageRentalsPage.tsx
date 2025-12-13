import React, { useEffect, useMemo, useRef, useState } from 'react';
import {
  Table,
  Input,
  Tag,
  Space,
  Button,
  DatePicker,
  Select,
  Card,
  Tooltip,
  Row,
  Col,
  Divider,
} from 'antd';
import { EyeOutlined } from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';

import { getPagedRentals, getRentalDetail } from '../../services/manage.rental.service';
import RentalDetailDrawer from '../../features/rental/components/RentalDetailDrawer';
import type { RentalListDTO, RentalFilterDTO } from '../../types/rental';
import type { TableProps, ColumnsType } from 'antd/lib/table';

const { RangePicker } = DatePicker;
const { Option } = Select;

const STATUS_MAP: Record<string, { text: string; tag: string }> = {
  Ongoing: { text: 'Đang chạy', tag: 'processing' },
  End: { text: 'Đã kết thúc', tag: 'success' },
  Cancelled: { text: 'Đã hủy', tag: 'error' },
};

const DEFAULT_FILTERS: RentalFilterDTO = {
  page: 1,
  pageSize: 10,
  status: undefined,
  keyword: undefined,
  fromStartTimeUtc: undefined,
  toStartTimeUtc: undefined,
  fromEndTimeUtc: undefined,
  toEndTimeUtc: undefined,
};

const ManageRentalsPage: React.FC = () => {
  // State
  const [filters, setFilters] = useState<RentalFilterDTO>(DEFAULT_FILTERS);
  const [detailId, setDetailId] = useState<number | null>(null);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);

  // local UI state
  const [searchText, setSearchText] = useState('');
  const searchTimer = useRef<number | null>(null);

  // Queries
  const { data: pagedData, isLoading  } = useQuery({
    queryKey: ['staffRentals', filters],
    queryFn: () => getPagedRentals(filters),
  });

  const { data: detailData, isLoading: isLoadingDetail } = useQuery({
    queryKey: ['staffRentalDetail', detailId],
    queryFn: () => getRentalDetail(detailId!),
    enabled: !!detailId,
  });

  // Debounced search
  useEffect(() => {
    if (searchTimer.current) window.clearTimeout(searchTimer.current);
    // @ts-ignore
    searchTimer.current = window.setTimeout(() => {
      setFilters(prev => ({ ...prev, page: 1, keyword: searchText || undefined }));
    }, 300);

    return () => {
      if (searchTimer.current) window.clearTimeout(searchTimer.current);
    };
  }, [searchText]);

  // Handlers
  const handleTableChange: TableProps<RentalListDTO>['onChange'] = (pagination) => {
    setFilters(prev => ({ ...prev, page: pagination.current || 1, pageSize: pagination.pageSize || 10 }));
  };

  // Start time range handler
  const handleStartRangeChange = (dates: any) => {
    if (dates && dates.length === 2) {
      const fromIso = dayjs(dates[0]).startOf('day').toISOString();
      const toIso = dayjs(dates[1]).endOf('day').toISOString();
      setFilters(prev => ({
        ...prev,
        page: 1,
        fromStartTimeUtc: fromIso,
        toStartTimeUtc: toIso,
      }));
    } else {
      setFilters(prev => ({
        ...prev,
        page: 1,
        fromStartTimeUtc: undefined,
        toStartTimeUtc: undefined,
      }));
    }
  };

  // End time range handler
  const handleEndRangeChange = (dates: any) => {
    if (dates && dates.length === 2) {
      const fromIso = dayjs(dates[0]).startOf('day').toISOString();
      const toIso = dayjs(dates[1]).endOf('day').toISOString();
      setFilters(prev => ({
        ...prev,
        page: 1,
        fromEndTimeUtc: fromIso,
        toEndTimeUtc: toIso,
      }));
    } else {
      setFilters(prev => ({
        ...prev,
        page: 1,
        fromEndTimeUtc: undefined,
        toEndTimeUtc: undefined,
      }));
    }
  };

  const handleStatusChange = (val: string | undefined) => {
    setFilters(prev => ({ ...prev, page: 1, status: val }));
  };

  const openDetail = (id: number) => {
    setDetailId(id);
    setIsDrawerOpen(true);
  };

  const closeDetail = () => {
    setIsDrawerOpen(false);
    setDetailId(null);
  };

  // Columns
  const columns: ColumnsType<RentalListDTO> = useMemo(() => [
    {
      title: 'Mã & Khách hàng',
      key: 'info',
      render: (_, record) => (
        <div>
          <div className="font-semibold text-blue-600">#{record.id} · {record.bikeCode}</div>
          <div className="text-gray-500 text-xs">{record.userFullName}</div>
        </div>
      ),
      width: 260,
    },
    {
      title: 'Hành trình',
      key: 'route',
      render: (_, record) => (
        <div className="text-sm">
          <div className="flex items-center gap-2"><span className="text-green-600">●</span> {record.startStationName}</div>
          {record.endStationName && (
            <div className="flex items-center gap-2"><span className="text-red-500">●</span> {record.endStationName}</div>
          )}
        </div>
      ),
      width: 260,
    },
    {
      title: 'Thời gian',
      dataIndex: 'startTimeUtc',
      key: 'time',
      render: (val, record) => (
        <div className="text-xs">
          <div>Bắt đầu: {val ? dayjs(val).format('HH:mm DD/MM/YYYY') : '-'}</div>
          {record.durationMinutes ? (
            <div className="font-semibold text-gray-600">{record.durationMinutes} phút</div>
          ) : <span className="text-blue-400 italic">Đang chạy...</span>}
        </div>
      ),
      width: 160,
    },
    {
      title: 'Phí',
      dataIndex: 'overusedFee',
      key: 'fee',
      render: (val) => val ? <span className="text-red-600 font-medium">{Number(val).toLocaleString('vi-VN')} đ</span> : '-',
      align: 'right',
      width: 120,
    },
    {
      title: 'Trạng thái',
      dataIndex: 'status',
      key: 'status',
      render: (status) => {
        const mapped = STATUS_MAP[status] ?? { text: status ?? '-', tag: 'default' };
        return <Tag color={mapped.tag}>{mapped.text}</Tag>;
      },
      width: 120,
    },
    {
      title: '',
      key: 'action',
      width: 80,
      render: (_, record) => (
        <Space size="small">
          <Tooltip title="Xem chi tiết">
            <Button type="text" icon={<EyeOutlined />} onClick={() => openDetail(record.id)} />
          </Tooltip>
        </Space>
      ),
    },
  ], []);

  return (
    <div className="p-3">
      <Card bordered={false} className="shadow-sm mb-4">
        <Row gutter={[16, 12]} align="middle" justify="space-between">
          <Col xs={24} md={14} lg={16}>
            <h2 className="text-xl font-bold m-0">Quản lý Thuê xe (Rentals)</h2>
            <p className="text-gray-500 text-sm m-0">Theo dõi hành trình, phí quá giờ và trạng thái</p>
          </Col>
        </Row>

        <Divider />

        <div className="flex flex-wrap gap-3 items-center bg-gray-50 p-3 rounded-lg border border-gray-100">
          <Input
            placeholder="Tìm theo Tên, Email, Mã xe..."
            allowClear
            style={{ width: 300 }}
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
          />

          <Select
            placeholder="Trạng thái"
            allowClear
            style={{ width: 160 }}
            value={filters.status}
            onChange={handleStatusChange}
          >
            <Option value="Ongoing">Đang chạy</Option>
            <Option value="End">Hoàn thành</Option>
            <Option value="Cancelled">Đã hủy</Option>
          </Select>

          {/* Start time range */}
          <div style={{ minWidth: 260 }}>
            <div style={{ fontSize: 12, color: '#6b7280', marginBottom: 6 }}>Thời gian bắt đầu</div>
            <RangePicker
              placeholder={["Từ ngày", "Đến ngày"]}
              onChange={handleStartRangeChange}
              style={{ width: 260 }}
              allowClear
            />
          </div>

          {/* End time range */}
          <div style={{ minWidth: 260 }}>
            <div style={{ fontSize: 12, color: '#6b7280', marginBottom: 6 }}>Thời gian kết thúc</div>
            <RangePicker
              placeholder={["Từ ngày (kết thúc)", "Đến ngày (kết thúc)"]}
              onChange={handleEndRangeChange}
              style={{ width: 260 }}
              allowClear
            />
          </div>

          <div className="ml-auto" />
        </div>
      </Card>

      <Table
        columns={columns}
        dataSource={pagedData?.data?.items}
        rowKey="id"
        loading={isLoading}
        pagination={{
          current: filters.page,
          pageSize: filters.pageSize,
          total: pagedData?.data?.totalCount,
          showSizeChanger: true,
          showTotal: (total) => `Tổng ${total} chuyến đi`,
        }}
        onChange={handleTableChange}
        scroll={{ x: 1000 }}
        size="middle"
        className="bg-white rounded-lg shadow-sm"
      />

      <RentalDetailDrawer
        open={isDrawerOpen}
        onClose={closeDetail}
        data={detailData?.data}
        isLoading={isLoadingDetail}
      />
    </div>
  );
};

export default ManageRentalsPage;
