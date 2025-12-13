// src/pages/staff/ManageTransactionsPage.tsx
import React, { useState } from 'react';
import { 
  Table, Tag, Card, DatePicker, Select, Input, Tabs 
} from 'antd';
import { 
  ShoppingCartOutlined, 
  WalletOutlined, 
  ArrowUpOutlined, 
  ArrowDownOutlined 
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import dayjs from 'dayjs';

// Imports
import { getPagedTransactions } from '../../services/manage.transaction.service';
import type { TransactionsDTO, TransactionQueryParams } from '../../types/manage.transaction';
import type { TableProps } from 'antd';

const { RangePicker } = DatePicker;
const { Option } = Select;

const ManageTransactionsPage: React.FC = () => {
  // State Filter: Mặc định transactionType là 'ORDER' (Tab đầu tiên)
  const [filters, setFilters] = useState<TransactionQueryParams>({
    page: 1,
    pageSize: 10,
    userId: undefined,
    transactionType: 'ORDER', 
    status: undefined,
  });

  // Query Data
  const { data: pagedData, isLoading } = useQuery({
    queryKey: ['staffTransactions', filters],
    queryFn: () => getPagedTransactions(filters),
    // keepPreviousData: true,
  });

  // Handlers
  const handleTabChange = (key: string) => {
    setFilters(prev => ({
      ...prev,
      page: 1,
      transactionType: key as 'ORDER' | 'WALLET',
      status: undefined, // Reset status khi chuyển qua Wallet
      direction: undefined
    }));
  };

  const handleTableChange: TableProps<TransactionsDTO>['onChange'] = (pagination) => {
    setFilters(prev => ({ 
      ...prev, 
      page: pagination.current || 1, 
      pageSize: pagination.pageSize || 10 
    }));
  };

  const handleDateRangeChange = (dates: any) => {
    if (dates) {
      setFilters(prev => ({
        ...prev,
        page: 1,
        from: dates[0].toISOString(),
        to: dates[1].toISOString(),
      }));
    } else {
      setFilters(prev => ({ ...prev, from: undefined, to: undefined }));
    }
  };

  // Helper render Status
  const renderOrderStatus = (status?: string) => {
    if (!status) return null;
    let color = 'default';
    if (status === 'Paid' || status === 'Success') color = 'success';
    if (status === 'Pending') color = 'warning';
    if (status === 'Cancelled' || status === 'Failed') color = 'error';
    return <Tag color={color}>{status}</Tag>;
  };

  // Columns definition (Tùy chỉnh cột hiển thị theo từng Tab cho gọn)
  const getColumns = (): TableProps<TransactionsDTO>['columns'] => {
    const isOrderTab = filters.transactionType === 'ORDER';

    return [
      {
        title: 'Thời gian',
        dataIndex: 'createdAt',
        key: 'time',
        width: 140,
        render: (val) => <span className="text-gray-600">{dayjs(val).format('HH:mm DD/MM/YYYY')}</span>,
      },
      {
        title: isOrderTab ? 'Mã Đơn & Loại' : 'Nguồn & Biến động',
        key: 'info',
        render: (_, record) => {
          if (isOrderTab) {
            return (
              <div>
                <div className="font-semibold text-blue-700">{record.orderNo}</div>
                <div className="text-xs text-gray-500">{record.orderType}</div>
              </div>
            );
          } else {
            return (
              <div>
                <div className="font-medium text-purple-700">{record.source}</div>
                <div className="text-xs text-gray-400">
                  Số dư sau: {record.balanceAfter?.toLocaleString()} đ
                </div>
              </div>
            );
          }
        },
      },
      {
        title: 'User ID',
        dataIndex: 'userId',
        key: 'userId',
        width: 100,
        render: (val) => <span className="text-gray-500">#{val}</span>
      },
      {
        title: 'Số tiền',
        key: 'amount',
        align: 'right',
        render: (_, record) => {
          let color = 'text-gray-800';
          let prefix = '';
          
          if (!isOrderTab) {
              if (record.direction === 'In') {
                  color = 'text-green-600';
                  prefix = '+';
              } else {
                  color = 'text-red-600';
                  prefix = '-';
              }
          }
  
          return (
            <div className={`font-bold ${color}`}>
              {prefix} {record.amount.toLocaleString()} <span className="text-xs font-normal text-gray-500">VND</span>
            </div>
          );
        },
      },
      {
        title: 'Trạng thái',
        key: 'status',
        width: 150,
        align: 'center',
        render: (_, record) => {
          if (isOrderTab) {
            return renderOrderStatus(record.status);
          } else {
            return record.direction === 'In' 
              ? <Tag color="green" icon={<ArrowUpOutlined />}>Nạp vào</Tag>
              : <Tag color="volcano" icon={<ArrowDownOutlined />}>Rút ra</Tag>;
          }
        },
      },
    ];
  };

  // Tab Items Configuration
  const tabItems = [
    {
      key: 'ORDER',
      label: (
        <span className="px-2">
          <ShoppingCartOutlined /> Đơn hàng
        </span>
      ),
    },
    {
      key: 'WALLET',
      label: (
        <span className="px-2">
          <WalletOutlined /> Ví điện tử
        </span>
      ),
    },
  ];

  return (
    <div className="p-3">
      <Card bordered={false} className="shadow-sm mb-4">
        <h2 className="text-xl font-bold mb-4">Quản lý Giao dịch</h2>

        {/* Tabs để chuyển đổi loại giao dịch */}
        <Tabs 
          activeKey={filters.transactionType} 
          onChange={handleTabChange}
          items={tabItems}
          type="card"
          className="mb-0" // Để dính liền với filter bar bên dưới
        />

        {/* Filter Bar */}
        <div className="flex flex-wrap gap-3 items-center bg-gray-50 p-3 border border-t-0 border-gray-200 rounded-b-lg mb-4">
          <Input 
            placeholder="Nhập User ID..." 
            type="number"
            allowClear
            style={{ width: 180 }}
            onPressEnter={(e) => setFilters(prev => ({ ...prev, page: 1, userId: e.currentTarget.value ? Number(e.currentTarget.value) : undefined }))}
          />
          
          <RangePicker 
            placeholder={['Từ ngày', 'Đến ngày']}
            onChange={handleDateRangeChange}
            style={{ width: 280 }}
          />

          {/* Chỉ hiện filter Trạng thái khi ở Tab ORDER */}
          {filters.transactionType === 'ORDER' && (
            <Select 
              placeholder="Trạng thái đơn" 
              allowClear 
              style={{ width: 180 }}
              onChange={(val) => setFilters(prev => ({ ...prev, page: 1, status: val }))}
            >
              <Option value="Pending">Chờ thanh toán</Option>
              <Option value="Paid">Đã thanh toán</Option>
              <Option value="Cancelled">Đã hủy</Option>
            </Select>
          )}
        </div>

        {/* Table */}
        <Table
          columns={getColumns()}
          dataSource={pagedData?.data?.items}
          rowKey={(record) => `${record.transactionType}_${record.id}`}
          loading={isLoading}
          pagination={{
            current: filters.page,
            pageSize: filters.pageSize,
            total: pagedData?.data?.totalCount,
            showSizeChanger: true,
            showTotal: (total) => `Tổng ${total} giao dịch`,
          }}
          onChange={handleTableChange}
          scroll={{ x: 800 }}
          size="middle"
        />
      </Card>
    </div>
  );
};

export default ManageTransactionsPage;