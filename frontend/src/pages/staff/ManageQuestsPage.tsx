// src/pages/staff/ManageQuestsPage.tsx
import React, { useState, useRef, useEffect } from 'react';
import { 
  Table, Button, Input, Tag, Space, Select, App, Switch, Tooltip 
} from 'antd';
import { 
  PlusOutlined, TrophyOutlined, EditOutlined, SearchOutlined, StopOutlined 
} from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import dayjs from 'dayjs';

import { 
  getPagedQuests, createQuest, updateQuest, toggleQuestStatus 
} from '../../services/manage.quest.service';
import QuestForm from '../../features/quest/components/QuestForm';
import type { QuestDTO, QuestFilterDTO, QuestCreateDTO } from '../../types/manage.quest';
import type { TableProps } from 'antd';

const ManageQuestsPage: React.FC = () => {
  const { notification, message } = App.useApp();
  const queryClient = useQueryClient();

  const [filters, setFilters] = useState<QuestFilterDTO>({
    page: 1, pageSize: 10, sort: 'Id desc'
  });
  const [searchText, setSearchText] = useState('');
  const searchTimer = useRef<number | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [editingQuest, setEditingQuest] = useState<QuestDTO | null>(null);

  const { data: pagedData, isLoading } = useQuery({
    queryKey: ['staffQuests', filters],
    queryFn: () => getPagedQuests(filters),
  });

  useEffect(() => {
    if (searchTimer.current) window.clearTimeout(searchTimer.current);
    searchTimer.current = window.setTimeout(() => {
      setFilters(prev => ({ ...prev, page: 1, search: searchText || undefined }));
    }, 300);
    return () => { if (searchTimer.current) window.clearTimeout(searchTimer.current); };
  }, [searchText]);

  const handleError = (error: any, action: string) => {
      const errorMsg = error?.response?.data?.message || error.message || 'Có lỗi xảy ra';
      notification.error({
          message: `${action} thất bại`,
          description: errorMsg,
      });
  };

  const createMutation = useMutation({
    mutationFn: createQuest,
    onSuccess: () => {
      notification.success({ message: 'Tạo Quest thành công!' });
      setDrawerOpen(false);
      queryClient.invalidateQueries({ queryKey: ['staffQuests'] });
    },
    onError: (err) => handleError(err, 'Tạo Quest')
  });

  const updateMutation = useMutation({
    mutationFn: (data: {id: number, dto: QuestCreateDTO}) => updateQuest(data.id, data.dto),
    onSuccess: () => {
      notification.success({ message: 'Cập nhật Quest thành công!' });
      setDrawerOpen(false);
      queryClient.invalidateQueries({ queryKey: ['staffQuests'] });
    },
    onError: (err) => handleError(err, 'Cập nhật Quest')
  });

  const toggleMutation = useMutation({
    mutationFn: toggleQuestStatus,
    onSuccess: () => {
      message.success('Đã thay đổi trạng thái Quest');
      queryClient.invalidateQueries({ queryKey: ['staffQuests'] });
    },
    onError: (err: any) => {
        const errorMsg = err?.response?.data?.message || err.message;
        message.error(errorMsg || 'Không thể đổi trạng thái');
    }
  });

  const handleEdit = (record: QuestDTO) => {
    // Chặn thêm 1 tầng ở hàm click cho chắc chắn
    if (dayjs(record.endAt).isBefore(dayjs())) {
        message.warning("Quest đã kết thúc, không thể chỉnh sửa.");
        return;
    }
    setEditingQuest(record);
    setDrawerOpen(true);
  };

  const handleCreate = () => {
    setEditingQuest(null);
    setDrawerOpen(true);
  };

  const handleFormSubmit = (values: QuestCreateDTO) => {
    if (editingQuest) {
      updateMutation.mutate({ id: editingQuest.id, dto: values });
    } else {
      createMutation.mutate(values);
    }
  };

  const handleToggle = (id: number) => {
    toggleMutation.mutate(id);
  };

  const columns: TableProps<QuestDTO>['columns'] = [
    {
      title: 'Info',
      key: 'info',
      width: 250,
      render: (_, record) => (
        <div>
          <div className="font-bold text-blue-700">{record.code}</div>
          <div className="font-medium">{record.title}</div>
          <div className="text-gray-400 text-xs truncate max-w-[200px]">{record.description}</div>
        </div>
      )
    },
    {
      title: 'Type & Scope',
      key: 'type',
      render: (_, record) => (
        <Space direction="vertical" size={2}>
          <Tag color="cyan">{record.scope}</Tag>
          <Tag color={record.questType === 'Distance' ? 'purple'  : (record.questType === 'Trips' ? 'magenta' : 'orange')}>
            {record.questType}
          </Tag>
        </Space>
      )
    },
    {
      title: 'Mục tiêu',
      key: 'target',
      render: (_, record) => {
        if (record.questType === 'Distance') return <strong>{record.targetDistanceKm} km</strong>;
        if (record.questType === 'Trips') return <strong>{record.targetTrips} chuyến</strong>;
        return <strong>{record.targetDurationMinutes} phút</strong>;
      }
    },
    {
      title: 'Thưởng',
      dataIndex: 'promoReward',
      key: 'reward',
      render: (val) => <span className="text-yellow-600 font-bold">+{val.toLocaleString()} pts</span>
    },
    {
      title: 'Thời gian',
      key: 'time',
      render: (_, record) => {
        // Kiểm tra xem đã hết hạn chưa
        const isExpired = dayjs(record.endAt).isBefore(dayjs());
        return (
            <div className={`text-xs ${isExpired ? 'text-red-500 font-semibold' : ''}`}>
              <div>{dayjs(record.startAt).format('DD/MM/YY HH:mm')}</div>
              <div className="text-gray-400">đến</div>
              <div>{dayjs(record.endAt).format('DD/MM/YY HH:mm')}</div>
            </div>
        );
      }
    },
    {
      title: 'Trạng thái',
      key: 'status',
      render: (_, record) => {
        // LOGIC MỚI: Kiểm tra hết hạn dựa trên thời gian thực hoặc status từ BE
        const isExpired = dayjs(record.endAt).isBefore(dayjs()) || record.status === 'Expired';
        
        if (isExpired) {
            return <Tag icon={<StopOutlined />} color="error">Expired</Tag>;
        }

        return (
            <Switch 
              checked={record.status === 'Active'}
              checkedChildren="Active"
              unCheckedChildren="Inactive"
              loading={toggleMutation.isPending && toggleMutation.variables === record.id}
              onChange={() => handleToggle(record.id)}
            />
        );
      }
    },
    {
      title: '',
      key: 'action',
      render: (_, record) => {
        const isExpired = dayjs(record.endAt).isBefore(dayjs()) || record.status === 'Expired';
        
        return (
            <Tooltip title={isExpired ? "Quest đã kết thúc, không thể sửa" : "Chỉnh sửa"}>
                <Button 
                  type="text" 
                  icon={<EditOutlined />} 
                  disabled={isExpired} // Disable nút nếu hết hạn
                  onClick={() => handleEdit(record)} 
                />
            </Tooltip>
        );
      }
    }
  ];

  return (
    <div className="p-2">
      <div className="bg-white p-4 rounded-lg shadow-sm mb-4">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold m-0 flex items-center gap-2">
            <TrophyOutlined className="text-yellow-500"/> Quản lý Nhiệm vụ (Quest)
          </h2>
          <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
            Tạo Quest
          </Button>
        </div>

        <div className="flex gap-3">
          <Input 
            placeholder="Tìm theo Mã, Tên..." 
            allowClear
            prefix={<SearchOutlined />}
            style={{ width: 300 }}
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
          />
          <Select 
            placeholder="Lọc theo Scope"
            allowClear
            style={{ width: 150 }}
            onChange={(val) => setFilters(prev => ({ ...prev, page: 1, filterField: val ? 'Scope' : undefined, filterValue: val }))}
          >
              <Select.Option value="Daily">Hàng ngày</Select.Option>
              <Select.Option value="Weekly">Hàng tuần</Select.Option>
              <Select.Option value="Monthly">Hàng tháng</Select.Option>
          </Select>
        </div>
      </div>

      <Table
        columns={columns}
        dataSource={pagedData?.data?.items}
        rowKey="id"
        loading={isLoading}
        pagination={{
          current: filters.page,
          pageSize: filters.pageSize,
          total: pagedData?.data?.totalCount,
          onChange: (p, ps) => setFilters(prev => ({ ...prev, page: p, pageSize: ps }))
        }}
        scroll={{ x: 800 }}
        className="bg-white rounded-lg shadow-sm"
      />

      <QuestForm
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        onSubmit={handleFormSubmit}
        initialData={editingQuest}
        isLoading={createMutation.isPending || updateMutation.isPending}
      />
    </div>
  );
};

export default ManageQuestsPage;