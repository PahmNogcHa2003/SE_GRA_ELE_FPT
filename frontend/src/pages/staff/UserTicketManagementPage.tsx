// src/pages/staff/UserTicketManagementPage.tsx
import React, { useMemo, useState } from 'react';
import {
  Table,
  Button,
  Input,
  Space,
  Form,
  Input as AntInput,
  Tag,
  App,
  Select,
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
import dayjs from 'dayjs';

const { Search } = Input;
const { Option } = Select;

type LocalPagedParams = PagedRequestParams & {
  filterPlanName?: string | undefined;
  filterUserEmail?: string | undefined;
};

const UserTicketManagementPage: React.FC = () => {
  const { notification, modal } = App.useApp();
  const queryClient = useQueryClient();
  const [queryParams, setQueryParams] = useState<LocalPagedParams>({
    page: 1,
    pageSize: 10,
    searchQuery: undefined,
    filterPlanName: undefined,
    filterUserEmail: undefined,
  });

  // Query để lấy dữ liệu (lưu ý: trả về ApiResponse<PagedResult<...>>)
  const { data: apiResp, isLoading } = useQuery({
    queryKey: ['staffUserTickets', queryParams],
    queryFn: () => getPagedUserTickets(queryParams as any),
    // không select ở đây để còn lấy metadata bên ngoài nếu cần
  });

  // pagedData là object PagedResult
  const pagedData = apiResp?.data;

  // Lấy danh sách planName unique từ dữ liệu hiện tại (client-side fallback)
  const planOptions = useMemo(() => {
    const items = pagedData?.data?.items || [];
    const setNames = new Set<string>();
    items.forEach((it: any) => {
      if (it.planName) setNames.add(it.planName);
    });
    return Array.from(setNames).sort();
  }, [pagedData]);

  // Mutation cho Hủy Vé (Void)
  const voidMutation = useMutation({
    mutationFn: ({ id, reason }: { id: number; reason: string }) => voidUserTicket(id, reason),
    onSuccess: (res) => {
      notification.success({ message: res.data?.message || 'Hủy vé thành công' });
      queryClient.invalidateQueries({ queryKey: ['staffUserTickets'] });
    },
    onError: (err: any) => {
      const message = (err && err.message) || 'Hủy vé thất bại';
      notification.error({ message });
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
              onChange={(e) => (reason = e.target.value)}
            />
          </Form.Item>
        </Form>
      ),
      okText: 'Xác nhận hủy',
      okType: 'danger',
      cancelText: 'Hủy bỏ',
      onOk: () => {
        if (!reason || !reason.trim()) {
          notification.error({ message: 'Lý do không được để trống!' });
          return Promise.reject();
        }
        voidMutation.mutate({ id: record.id, reason });
      }
    });
  };

  // Xử lý thay đổi Table (pagination / sorter)
  const handleTableChange: TableProps<ManageUserTicket>['onChange'] = (
    pagination, /* filters, sorter */
  ) => {
    setQueryParams(prev => ({
      ...prev,
      page: pagination.current || 1,
      pageSize: pagination.pageSize || 10,
    }));
  };

  // Handlers: search chung (searchQuery)
  const handleSearch = (value?: string) => {
    setQueryParams(prev => ({
      ...prev,
      page: 1,
      searchQuery: value && value.trim() ? value.trim() : undefined
    }));
  };

  // Handler: filter theo planName
  const handlePlanFilterChange = (val?: string) => {
    setQueryParams(prev => ({
      ...prev,
      page: 1,
      filterPlanName: val && val.length ? val : undefined,
    }));
  };


  // Cấu hình cột
  const columns: TableProps<ManageUserTicket>['columns'] = [
    { title: 'ID Vé', dataIndex: 'id', key: 'id', width: 80 },
    { title: 'Mã Vé', dataIndex: 'serialCode', key: 'serialCode', width: 140 },
    { title: 'Email User', dataIndex: 'userEmail', key: 'userEmail', sorter: true },
    { title: 'Tên Gói', dataIndex: 'planName', key: 'planName' },
    {
      title: 'Giá Mua',
      dataIndex: 'purchasedPrice',
      key: 'purchasedPrice',
      render: (val) => val ? Number(val).toLocaleString('vi-VN') + ' VND' : '-',
      align: 'right'
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
        return <Tag color={color}>{String(status).toUpperCase()}</Tag>;
      }
    },
    {
      title: 'Ngày Kích hoạt',
      dataIndex: 'activatedAt',
      key: 'activatedAt',
      render: (val) => val ? dayjs(val).format('YYYY-MM-DD HH:mm') : 'N/A'
    },
    {
      title: 'Hành động',
      key: 'action',
      width: 180,
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

      <div className="flex flex-wrap items-center justify-between gap-3 mb-4">
        <div style={{ display: 'flex', gap: 12, alignItems: 'flex-end' }}>
          <div>
            <div style={{ fontSize: 12, color: '#6b7280', marginBottom: 6 }}>Tìm nhanh (Email / Mã vé)</div>
            <Search
              placeholder="Tìm theo Email, Mã vé..."
              onSearch={handleSearch}
              allowClear
              style={{ width: 340 }}
            />
          </div>

          <div>
            <div style={{ fontSize: 12, color: '#6b7280', marginBottom: 6 }}>Lọc theo tên gói</div>
            <Select
              allowClear
              showSearch
              placeholder="Chọn tên gói"
              style={{ width: 260 }}
              optionFilterProp="children"
              value={queryParams.filterPlanName}
              onChange={(v) => handlePlanFilterChange(v as string | undefined)}
              filterOption={(input, option) =>
                (option?.children as unknown as string || '').toLowerCase().includes(input.toLowerCase())
              }
            >
              {planOptions.map(p => <Option key={p} value={p}>{p}</Option>)}
            </Select>
          </div>

          <div>
          </div>
        </div>
      </div>

      <Table
        columns={columns}
        rowKey="id"
        dataSource={pagedData?.data?.items}
        pagination={{
          current: pagedData?.data?.pageNumber ?? queryParams.page,
          pageSize: pagedData?.data?.pageSize ?? queryParams.pageSize,
          total: pagedData?.data?.totalCount ?? 0,
          showSizeChanger: true,
        }}
        loading={isLoading}
        onChange={handleTableChange}
        scroll={{ x: true }}
      />
    </div>
  );
};

export default UserTicketManagementPage;
