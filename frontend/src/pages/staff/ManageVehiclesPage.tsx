import React, { useState, useMemo } from 'react';
import { Table, Button, Modal, Space, Typography, Input, Tag, Select, Row, Col } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { TableProps } from 'antd';
import Swal from 'sweetalert2';

// Import các service và type cần thiết cho trang Vehicle
import * as vehicleService from '../../services/vehicle.service';
import * as categoryVehicleService from '../../services/categoryVehicle.service';
import * as stationService from '../../services/station.service';
import type { VehicleDTO, GetVehiclesParams } from '../../types/vehicle';
import VehicleForm from '../../features/vehicles/VehicleForm';

const { Title } = Typography;
const { Search } = Input;
const { Option } = Select;

const ManageVehiclesPage: React.FC = () => {
  const queryClient = useQueryClient();

  // State quản lý các tham số truy vấn (pagination, search, filter)
  const [queryParams, setQueryParams] = useState<GetVehiclesParams>({
    page: 1,
    pageSize: 10,
  });

  // State quản lý việc mở/đóng Modal và xe đang được chỉnh sửa
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingVehicle, setEditingVehicle] = useState<VehicleDTO | null>(null);

  // --- React Query Hooks ---

  // 1. Query để lấy danh sách xe (dữ liệu chính của bảng)
  const { data: vehiclesData, isLoading: isLoadingVehicles } = useQuery({
    queryKey: ['vehicles', queryParams],
    queryFn: () => vehicleService.getVehicles(queryParams),
    select: (res) => res.data,
  });
  
  // 2. Query để lấy TẤT CẢ loại xe (dùng để map ID ra Tên)
  const { data: categories } = useQuery({
    queryKey: ['allCategoriesVehicle'],
    queryFn: () => categoryVehicleService.getCategories({ pageSize: 1000 }), // Lấy nhiều để đủ dùng
    select: (res) => res.data?.data?.items,
  });

  // 3. Query để lấy TẤT CẢ trạm xe (dùng để map ID ra Tên)
  const { data: stations } = useQuery({
    queryKey: ['allStations'],
    queryFn: () => stationService.getStations({ pageSize: 1000 }),
    select: (res) => res.data?.items,
  });

  // 4. Các Mutation cho việc Tạo, Cập nhật, Xóa
  const createMutation = useMutation({
    mutationFn: vehicleService.createVehicle,
    onSuccess: () => {
      Swal.fire('Thành công!', 'Tạo xe mới thành công!', 'success');
      queryClient.invalidateQueries({ queryKey: ['vehicles'] });
      setIsModalOpen(false);
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi tạo xe mới.', 'error'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: VehicleDTO }) =>
      vehicleService.updateVehicle(id, data),
    onSuccess: () => {
      Swal.fire('Thành công!', 'Cập nhật xe thành công!', 'success');
      queryClient.invalidateQueries({ queryKey: ['vehicles'] });
      setIsModalOpen(false);
      setEditingVehicle(null);
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi cập nhật xe.', 'error'),
  });

  const deleteMutation = useMutation({
    mutationFn: vehicleService.deleteVehicle,
    onSuccess: () => {
      Swal.fire('Đã xóa!', 'Xe đã được xóa thành công.', 'success');
      queryClient.invalidateQueries({ queryKey: ['vehicles'] });
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi xóa xe.', 'error'),
  });

  // --- Handlers ---

  const handleOpenCreateModal = () => {
    setEditingVehicle(null);
    setIsModalOpen(true);
  };

  const handleOpenEditModal = (vehicle: VehicleDTO) => {
    setEditingVehicle(vehicle);
    setIsModalOpen(true);
  };

  const handleCancelModal = () => setIsModalOpen(false);

  const handleFormSubmit = (values: Omit<VehicleDTO, 'id' | 'createdAt'>) => {
    if (editingVehicle) {
      updateMutation.mutate({ id: editingVehicle.id, data: { ...editingVehicle, ...values } });
    } else {
      createMutation.mutate(values);
    }
  };

  const handleDeleteConfirm = (id: number) => {
    Swal.fire({
      title: 'Bạn có chắc chắn muốn xóa?',
      text: "Hành động này không thể được hoàn tác!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      confirmButtonText: 'Chắc chắn, xóa nó!',
      cancelButtonText: 'Hủy bỏ'
    }).then((result) => {
      if (result.isConfirmed) {
        deleteMutation.mutate(id);
      }
    });
  };

  const handleTableChange: TableProps<VehicleDTO>['onChange'] = (pagination) => {
    setQueryParams(prev => ({ ...prev, page: pagination.current, pageSize: pagination.pageSize }));
  };
  
  const handleSearch = (value: string) => {
    setQueryParams(prev => ({ ...prev, page: 1, search: value.trim() }));
  };

  const handleFilterChange = (field: string, value: string | null) => {
    setQueryParams(prev => {
        const newParams = { ...prev, page: 1 };
        if (value === 'all' || value === null) {
            delete newParams.filterField;
            delete newParams.filterValue;
        } else {
            newParams.filterField = field;
            newParams.filterValue = value;
        }
        return newParams;
    });
  };

  // Tạo các map để tra cứu tên từ ID, tối ưu hiệu suất
  const categoryMap = useMemo(
    () =>
      new Map(
        (categories ?? []).map((c: { id: number; name: string }) => [c.id, c.name])
      ),
    [categories]
  );
  const stationMap = useMemo(
    () =>
      new Map(
        (stations ?? []).map((s: { id: number; name: string }) => [s.id, s.name])
      ),
    [stations]
  );

  // --- Định nghĩa cột cho bảng ---

  const columns: TableProps<VehicleDTO>['columns'] = [
    { title: 'ID', dataIndex: 'id', key: 'id', width: 60, sorter: true },
    { title: 'Mã xe', dataIndex: 'bikeCode', key: 'bikeCode', sorter: true },
    { 
      title: 'Loại xe', 
      dataIndex: 'categoryId', 
      key: 'categoryId', 
      render: (id) => categoryMap.get(id) || 'Không xác định'
    },
    { 
      title: 'Trạm hiện tại', 
      dataIndex: 'stationId', 
      key: 'stationId',
      render: (id) => id ? stationMap.get(id) || 'Không xác định' : 'Ngoài trạm'
    },
    { 
      title: 'Mức pin', 
      dataIndex: 'batteryLevel', 
      key: 'batteryLevel', 
      align: 'center',
      render: (level) => level != null ? `${level}%` : 'N/A'
    },
    {
  title: 'Trạng thái',
  dataIndex: 'status',
  key: 'status',
  align: 'center',
  render: (status: string) => {
    let color = 'default';
    if (status === 'Available') color = 'green';
    else if (status === 'InUse') color = 'volcano';
    else if (status === 'Maintenance') color = 'gold';
    else if (status === 'Unavailable') color = 'red';

    // Map status sang tiếng Việt
    const statusMap: Record<string, string> = {
      Available: 'Sẵn sàng',
      InUse: 'Đang sử dụng',
      Maintenance: 'Bảo trì',
      Unavailable: 'Không khả dụng',
    };

    // Lấy tên tiếng Việt, fallback là status gốc nếu không có
    const displayName = statusMap[status] || status;

    return <Tag color={color}>{displayName}</Tag>;
  },
},

    {
        title: 'Trạng thái sạc',
        dataIndex: 'chargingStatus',
        key: 'chargingStatus',
        align: 'center',
        render: (isCharging: boolean | undefined) => {
            if (isCharging === undefined) return 'N/A';
            return isCharging ? <Tag color="blue">ĐANG SẠC</Tag> : <Tag color="gray">KHÔNG SẠC</Tag>;
        }
    },
    {
      title: 'Hành động',
      key: 'action',
      align: 'center',
      width: 120,
      render: (_, record) => (
        <Space size="middle">
          <Button icon={<EditOutlined />} onClick={() => handleOpenEditModal(record)} />
          <Button
            icon={<DeleteOutlined />}
            danger
            onClick={() => handleDeleteConfirm(record.id)}
            loading={deleteMutation.isPending && deleteMutation.variables === record.id}
          />
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <Title level={2}>Quản lý Xe</Title>

      <Row justify="space-between" align="middle" style={{ marginBottom: 24 }} gutter={[16, 16]}>
        <Col flex="auto">
          <Space wrap>
            <Select defaultValue="all" style={{ width: 180 }} onChange={(val) => handleFilterChange('status', val)}>
                <Option value="all">Tất cả trạng thái</Option>
                <Option value="Available">Sẵn sàng</Option>
                <Option value="InUse">Đang sử dụng</Option>
                <Option value="Maintenance">Bảo trì</Option>
                <Option value="Unavailable">Không khả dụng</Option>
            </Select>
            <Select defaultValue="all" style={{ width: 180 }} onChange={(val) => handleFilterChange('chargingStatus', val)}>
                <Option value="all">Tất cả trạng thái sạc</Option>
                <Option value="true">Đang sạc</Option>
                <Option value="false">Không sạc</Option>
            </Select>
          </Space>
        </Col>
        <Col>
          <Space>
            <Search placeholder="Tìm theo mã xe..." onSearch={handleSearch} style={{ width: 300 }} allowClear enterButton />
            <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenCreateModal}>
              Thêm xe mới
            </Button>
          </Space>
        </Col>
      </Row>

      <Table
        columns={columns}
        dataSource={vehiclesData?.data?.items}
        loading={isLoadingVehicles}
        rowKey="id"
        pagination={{
          current: vehiclesData?.data?.pageNumber,
          pageSize: vehiclesData?.data?.pageSize,
          total: vehiclesData?.data?.totalCount,
          showSizeChanger: true,
        }}
        onChange={handleTableChange}
        scroll={{ x: 'max-content' }}
      />

      <Modal
        title={editingVehicle ? 'Chỉnh sửa thông tin xe' : 'Tạo xe mới'}
        open={isModalOpen}
        onCancel={handleCancelModal}
        footer={null}
        destroyOnClose
        width={600}
      >
        <VehicleForm
          initialValues={editingVehicle}
          onSubmit={handleFormSubmit}
          onCancel={handleCancelModal}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      </Modal>
    </div>
  );
};

export default ManageVehiclesPage;