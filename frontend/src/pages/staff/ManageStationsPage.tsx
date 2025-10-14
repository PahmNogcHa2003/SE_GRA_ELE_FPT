import React, { useState } from 'react';
import {
  Table,
  Button,
  Modal,
  Space,
  Typography,
  Input,
  Tag,
  message,
  Popconfirm,
  Select,
  Row,
  Col,
} from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { TableProps } from 'antd';

import * as stationService from '../../services/station.service';
import type { StationDTO, GetStationsParams } from '../../types/station';
import StationForm from '../../features/stations/StationForm'; // Đảm bảo bạn đã có component này

const { Title } = Typography;
const { Search } = Input;

const ManageStationsPage: React.FC = () => {
  const queryClient = useQueryClient();

  // State để quản lý tất cả các tham số query
  const [queryParams, setQueryParams] = useState<GetStationsParams>({
    page: 1,
    pageSize: 10,
    search: '',
  });

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingStation, setEditingStation] = useState<StationDTO | null>(null);

  // --- React Query Hooks ---

  // 1. Query để lấy dữ liệu
  const { data: stationsData, isLoading } = useQuery({
    queryKey: ['stations', queryParams], // Tự động refetch khi queryParams thay đổi
    queryFn: () => stationService.getStations(queryParams),
    select: (res) => res.data, // Chỉ lấy phần data từ ApiResponse
  });

  // 2. Mutation để tạo mới
  const createMutation = useMutation({
    mutationFn: stationService.createStation,
    onSuccess: () => {
      message.success('Tạo trạm mới thành công!');
      queryClient.invalidateQueries({ queryKey: ['stations'] });
      setIsModalOpen(false);
    },
    onError: () => {
      message.error('Có lỗi xảy ra khi tạo trạm mới.');
    },
  });

  // 3. Mutation để cập nhật
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: StationDTO }) =>
      stationService.updateStation(id, data),
    onSuccess: () => {
      message.success('Cập nhật trạm thành công!');
      queryClient.invalidateQueries({ queryKey: ['stations'] });
      setIsModalOpen(false);
      setEditingStation(null);
    },
    onError: () => {
      message.error('Có lỗi xảy ra khi cập nhật trạm.');
    },
  });

  // 4. Mutation để xóa
  const deleteMutation = useMutation({
    mutationFn: stationService.deleteStation,
    onSuccess: () => {
      message.success('Xóa trạm thành công!');
      queryClient.invalidateQueries({ queryKey: ['stations'] });
    },
    onError: () => {
      message.error('Có lỗi xảy ra khi xóa trạm.');
    },
  });


  // --- Handlers ---

  const handleOpenCreateModal = () => {
    setEditingStation(null);
    setIsModalOpen(true);
  };

  const handleOpenEditModal = (station: StationDTO) => {
    setEditingStation(station);
    setIsModalOpen(true);
  };

  const handleCancelModal = () => {
    setIsModalOpen(false);
    setEditingStation(null);
  };

  const handleFormSubmit = (values: Omit<StationDTO, 'id'>) => {
    if (editingStation) {
      updateMutation.mutate({ id: editingStation.id, data: { ...editingStation, ...values } });
    } else {
      createMutation.mutate(values);
    }
  };

  const handleDelete = (id: number) => {
    deleteMutation.mutate(id);
  };

  const handleSearch = (value: string) => {
    setQueryParams(prev => ({ ...prev, page: 1, search: value.trim() }));
  };
  
  const handleFilterChange = (value: string) => {
    // Tạo một bản sao của params để xóa key an toàn
    const newParams: GetStationsParams = { ...queryParams, page: 1 };
    
    if (value === 'all') {
      delete newParams.filterField;
      delete newParams.filterValue;
    } else {
      newParams.filterField = 'isActive';
      newParams.filterValue = value;
    }
    setQueryParams(newParams);
  };

  const handleTableChange: TableProps<StationDTO>['onChange'] = (pagination, filters, sorter) => {
    const singleSorter = Array.isArray(sorter) ? sorter[0] : sorter;
    
    const newParams: GetStationsParams = {
        ...queryParams,
        page: pagination.current,
        pageSize: pagination.pageSize,
    };
    
    if (singleSorter && singleSorter.order) {
      newParams.sortOrder = `${singleSorter.field}_${singleSorter.order === 'ascend' ? 'asc' : 'desc'}`;
    } else {
      delete newParams.sortOrder;
    }

    setQueryParams(newParams);
  };

  // --- Table Columns Definition ---

  const columns: TableProps<StationDTO>['columns'] = [
    { title: 'ID', dataIndex: 'id', key: 'id', sorter: true },
    { title: 'Tên trạm', dataIndex: 'name', key: 'name', sorter: true },
    { title: 'Địa chỉ', dataIndex: 'location', key: 'location', sorter: false },
    { title: 'Sức chứa', dataIndex: 'capacity', key: 'capacity', align: 'center', sorter: true },
    {
      title: 'Trạng thái',
      dataIndex: 'isActive',
      key: 'isActive',
      align: 'center',
      render: (isActive: boolean) => (
        <Tag color={isActive ? 'green' : 'red'}>
          {isActive ? 'HOẠT ĐỘNG' : 'TẠM DỪNG'}
        </Tag>
      ),
    },
    {
      title: 'Hành động',
      key: 'action',
      align: 'center',
      render: (_, record) => (
        <Space size="middle">
          <Button icon={<EditOutlined />} onClick={() => handleOpenEditModal(record)} />
          <Popconfirm
            title="Xóa trạm"
            description="Bạn có chắc chắn muốn xóa trạm này không?"
            onConfirm={() => handleDelete(record.id)}
            okText="Xóa"
            cancelText="Hủy"
          >
            <Button icon={<DeleteOutlined />} danger loading={deleteMutation.isPending && deleteMutation.variables === record.id} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <Title level={2}>Quản lý trạm xe</Title>

      <Row justify="space-between" align="middle" style={{ marginBottom: 24 }} gutter={[16, 16]}>
        <Col>
            <Space>
                <span>Lọc theo trạng thái:</span>
                <Select defaultValue="all" style={{ width: 150 }} onChange={handleFilterChange}>
                    <Select.Option value="all">Tất cả</Select.Option>
                    <Select.Option value="true">Hoạt động</Select.Option>
                    <Select.Option value="false">Tạm dừng</Select.Option>
                </Select>
            </Space>
        </Col>
        <Col>
            <Space>
                <Search 
                    placeholder="Tìm kiếm theo tên, địa chỉ..." 
                    onSearch={handleSearch} 
                    style={{ width: 300 }}
                    allowClear
                    enterButton
                />
                <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenCreateModal}>
                    Thêm trạm mới
                </Button>
            </Space>
        </Col>
      </Row>

      <Table
        columns={columns}
        dataSource={stationsData?.items}
        loading={isLoading}
        rowKey="id"
        pagination={{
          current: stationsData?.pageNumber,
          pageSize: stationsData?.pageSize,
          total: stationsData?.totalCount,
          showSizeChanger: true,
        }}
        onChange={handleTableChange}
        scroll={{ x: 'max-content' }}
      />

      <Modal
        title={editingStation ? 'Chỉnh sửa thông tin trạm' : 'Tạo trạm xe mới'}
        open={isModalOpen}
        onCancel={handleCancelModal}
        footer={null}
        destroyOnClose
      >
        <StationForm
          initialValues={editingStation}
          onSubmit={handleFormSubmit}
          onCancel={handleCancelModal}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      </Modal>
    </div>
  );
};

export default ManageStationsPage;