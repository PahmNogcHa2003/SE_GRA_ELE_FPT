// src/pages/staff/UserTicketManagementPage.tsx
import React, { useState } from 'react';
import { 
  Table, 
  Button, 
  Input, 
  Space, 
  Modal, 
  Form, 
  Input as AntInput, 
  Popconfirm,
  Tag,
  App
} from 'antd';
import { StopOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { 
  getPagedUserTickets, 
  voidUserTicket,
  type PagedRequestParams
} from '../../services/manage.user.ticket.service';
import type { TableProps } from 'antd';
import type { ManageUserTicket } from '../../types/manage.ticket';
import type { ApiResponse,PagedResult } from '../../types/api';

const { Search } = Input;

const UserTicketManagementPage: React.FC = () => {
  const { notification, modal } = App.useApp();
  const queryClient = useQueryClient();
  const [queryParams, setQueryParams] = useState<PagedRequestParams>({
    page: 1,
    pageSize: 10,
  });

  // Query để lấy dữ liệu
  const { data: pagedData, isLoading } = useQuery({
    queryKey: ['staffUserTickets', queryParams],
    queryFn: () => getPagedUserTickets(queryParams),
    select: (data) => data.data,
  });

  // Mutation cho Hủy Vé (Void)
  const voidMutation = useMutation({
    mutationFn: ({ id, reason }: { id: number; reason: string }) => voidUserTicket(id, reason),
    onSuccess: (res) => {
      notification.success({ message: res.data.message });
      queryClient.invalidateQueries({ queryKey: ['staffUserTickets'] });
    },
    onError: (err: ApiResponse<null>) => {
      notification.error({ message: err.message || 'Hủy vé thất bại' });
    }
  });

  // Xử lý mở Modal Hủy Vé
  const showVoidModal = (record: ManageUserTicket) => {
    let reason = '';
    modal.confirm({
      title: `Hủy vé #${record.serialCode}`,
      content: (
        <Form layout="vertical" className="mt-4">
          <Form.Item label="Lý do hủy vé" required>
            <AntInput.TextArea 
              rows={3} 
              placeholder="Nhập lý do (bắt buộc)"
              onChange={(e) => reason = e.target.value}
            />
          </Form.Item>
        </Form>
      ),
      okText: 'Xác nhận hủy',
      okType: 'danger',
      cancelText: 'Hủy bỏ',
      onOk: () => {
        if (!reason.trim()) {
          notification.error({ message: 'Lý do không được để trống!' });
          return Promise.reject(); // Ngăn modal đóng
        }
        voidMutation.mutate({ id: record.id, reason });
      }
    });
  };

  // Xử lý thay đổi Table
  const handleTableChange: TableProps<ManageUserTicket>['onChange'] = (
    pagination, filters, sorter
  ) => {
    setQueryParams(prev => ({
      ...prev,
      page: pagination.current,
      pageSize: pagination.pageSize,
    }));
  };

  // Cấu hình cột
  const columns: TableProps<ManageUserTicket>['columns'] = [
    { title: 'ID Vé', dataIndex: 'id', key: 'id' },
    { title: 'Mã Vé', dataIndex: 'serialCode', key: 'serialCode' },
    { title: 'Email User', dataIndex: 'userEmail', key: 'userEmail', sorter: true },
    { title: 'Tên Gói', dataIndex: 'planName', key: 'planName' },
    { 
      title: 'Giá Mua', 
      dataIndex: 'purchasedPrice', 
      key: 'purchasedPrice', 
      render: (val) => val?.toLocaleString('vi-VN') + ' VND' 
    },
    { 
      title: 'Trạng thái', 
      dataIndex: 'status', 
      key: 'status',
      render: (status: string) => {
        let color = 'blue';
        if (status === 'Used' || status === 'Expired') color = 'gray';
        if (status === 'Voided') color = 'red';
        if (status === 'Active') color = 'green';
        return <Tag color={color}>{status.toUpperCase()}</Tag>;
      }
    },
    { 
      title: 'Ngày Kích hoạt', 
      dataIndex: 'activatedAt', 
      key: 'activatedAt', 
      render: (val) => val ? new Date(val).toLocaleString('vi-VN') : 'N/A' 
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_, record) => (
        <Space size="middle">
          <Button 
            icon={<StopOutlined />} 
            danger
            onClick={() => showVoidModal(record)}
            disabled={record.status === 'Voided' || record.status === 'Expired'}
          >
            Hủy Vé (Void)
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <h2 className="text-2xl font-semibold mb-4">Quản lý Vé của User (User Tickets)</h2>
      
      <div className="flex justify-end mb-4">
        <Search
          placeholder="Tìm theo Email, Mã vé..."
          onSearch={(value) => setQueryParams(prev => ({ ...prev, page: 1, searchQuery: value }))}
          style={{ width: 300 }}
        />
      </div>

      <Table
        columns={columns}
        rowKey="id"
        dataSource={pagedData?.data?.items}
        pagination={{
          current: pagedData?.data?.pageNumber,
          pageSize: pagedData?.data?.pageSize,
          total: pagedData?.data?.totalCount,
        }}
        loading={isLoading}
        onChange={handleTableChange}
        scroll={{ x: true }}
      />
    </div>
  );
};

export default UserTicketManagementPage;