import React, { useState } from 'react';
import {
  Table, Button, Switch, Tag, App
} from 'antd';
import {
  PlusOutlined, EditOutlined, GiftOutlined
} from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import dayjs from 'dayjs';

import {
  getPagedVouchers,
  createVoucher,
  updateVoucher,
  toggleVoucherStatus
} from '../../services/manage.voucher.service';

import VoucherForm from '../../features/voucher/components/VoucherForm';
import type { VoucherDTO, VoucherFilterDTO, VoucherCreateDTO } from '../../types/manage.voucher';
import type { TableProps } from 'antd';

const ManageVouchersPage: React.FC = () => {
  const { message, notification } = App.useApp();
  const queryClient = useQueryClient();

  const [filters, setFilters] = useState<VoucherFilterDTO>({
    page: 1,
    pageSize: 10,
    sortOrder: 'Id desc'
  });

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editingVoucher, setEditingVoucher] = useState<VoucherDTO | null>(null);

  const { data, isLoading } = useQuery({
    queryKey: ['staffVouchers', filters],
    queryFn: () => getPagedVouchers(filters)
  });

  const createMutation = useMutation({
    mutationFn: createVoucher,
    onSuccess: () => {
      notification.success({ message: 'Tạo Voucher thành công' });
      setDrawerOpen(false);
      queryClient.invalidateQueries({ queryKey: ['staffVouchers'] });
    }
  });

  const updateMutation = useMutation({
    mutationFn: (p: { id: number; dto: VoucherCreateDTO }) =>
      updateVoucher(p.id, p.dto),
    onSuccess: () => {
      notification.success({ message: 'Cập nhật Voucher thành công' });
      setDrawerOpen(false);
      queryClient.invalidateQueries({ queryKey: ['staffVouchers'] });
    }
  });

  const toggleMutation = useMutation({
    mutationFn: toggleVoucherStatus,
    onSuccess: () => {
      message.success('Đã đổi trạng thái');
      queryClient.invalidateQueries({ queryKey: ['staffVouchers'] });
    }
  });

  const columns: TableProps<VoucherDTO>['columns'] = [
    {
      title: 'Mã',
      dataIndex: 'code',
      render: (v) => <strong className="text-blue-600">{v}</strong>
    },
    {
      title: 'Giá trị',
      render: (_, r) =>
        r.isPercentage ? `${r.value}%` : `${r.value.toLocaleString()}₫`
    },
    {
      title: 'Thời gian',
      render: (_, r) => (
        <div className="text-xs">
          <div>{dayjs(r.startDate).format('DD/MM/YYYY')}</div>
          <div className="text-gray-400">→</div>
          <div>{dayjs(r.endDate).format('DD/MM/YYYY')}</div>
        </div>
      )
    },
    {
      title: 'Sử dụng',
      render: (_, r) => (
        <Tag color="blue">{r.usageCount}/{r.usageLimit ?? '∞'}</Tag>
      )
    },
    {
      title: 'Trạng thái',
      render: (_, r) => (
        <Switch
          checked={r.status === 'Active'}
          loading={toggleMutation.isPending}
          onChange={() => toggleMutation.mutate(r.id)}
        />
      )
    },
    {
      title: '',
      render: (_, r) => (
        <Button
          type="text"
          icon={<EditOutlined />}
          onClick={() => {
            setEditingVoucher(r);
            setDrawerOpen(true);
          }}
        />
      )
    }
  ];

  return (
    <div className="p-4">
      <div className="bg-white p-4 rounded shadow mb-4 flex justify-between">
        <h2 className="text-xl font-bold flex items-center gap-2">
          <GiftOutlined className="text-pink-500" /> Quản lý Voucher
        </h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => {
            setEditingVoucher(null);
            setDrawerOpen(true);
          }}
        >
          Tạo Voucher
        </Button>
      </div>

      <Table
        rowKey="id"
        loading={isLoading}
        columns={columns}
        dataSource={data?.data?.items}
        pagination={{
          current: filters.page,
          pageSize: filters.pageSize,
          total: data?.data?.totalCount,
          onChange: (p, ps) =>
            setFilters(prev => ({ ...prev, page: p, pageSize: ps }))
        }}
      />

      <VoucherForm
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        onSubmit={(v) =>
          editingVoucher
            ? updateMutation.mutate({ id: editingVoucher.id, dto: v })
            : createMutation.mutate(v)
        }
        initialData={editingVoucher}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />
    </div>
  );
};

export default ManageVouchersPage;
