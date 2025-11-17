import React, { useState } from 'react';
import {
  Table,
  Button,
  Modal,
  Space,
  Typography,
  Input,
  Tag,
  // message, // THAY ĐỔI: Không cần dùng message của antd nữa
  // Popconfirm, // THAY ĐỔI: Không cần dùng Popconfirm nữa
  Select,
  Row,
  Col,
} from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { TableProps } from 'antd';
import Swal from 'sweetalert2'; // THÊM MỚI: Import SweetAlert2

import * as stationService from '../../services/station.service';
import type { StationDTO, GetStationsParams } from '../../types/station';
import StationForm from '../../features/stations/StationForm';

const { Title } = Typography;
const { Search } = Input;

const ManageStationsPage: React.FC = () => {
  const queryClient = useQueryClient();

  const [queryParams, setQueryParams] = useState<GetStationsParams>({
    page: 1,
    pageSize: 10,
    search: '',
  });

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingStation, setEditingStation] = useState<StationDTO | null>(null);

  // --- React Query Hooks ---

  const { data: stationsData, isLoading } = useQuery({
    queryKey: ['stations', queryParams],
    queryFn: () => stationService.getStations(queryParams),
    select: (res) => res.data,
  });

  const createMutation = useMutation({
    mutationFn: stationService.createStation,
    onSuccess: () => {
      // THAY ĐỔI: Sử dụng SweetAlert2
      Swal.fire({
        title: 'Thành công!',
        text: 'Tạo trạm mới thành công!',
        icon: 'success',
        confirmButtonText: 'Tuyệt vời',
      });
      queryClient.invalidateQueries({ queryKey: ['stations'] });
      setIsModalOpen(false);
    },
    onError: () => {
      // THAY ĐỔI: Sử dụng SweetAlert2
      Swal.fire({
        title: 'Lỗi!',
        text: 'Có lỗi xảy ra khi tạo trạm mới.',
        icon: 'error',
        confirmButtonText: 'Đã hiểu',
      });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: StationDTO }) =>
      stationService.updateStation(id, data),
    onSuccess: () => {
      // THAY ĐỔI: Sử dụng SweetAlert2
      Swal.fire({
        title: 'Thành công!',
        text: 'Cập nhật trạm thành công!',
        icon: 'success',
        confirmButtonText: 'OK',
      });
      queryClient.invalidateQueries({ queryKey: ['stations'] });
      setIsModalOpen(false);
      setEditingStation(null);
    },
    onError: () => {
      // THAY ĐỔI: Sử dụng SweetAlert2
      Swal.fire({
        title: 'Lỗi!',
        text: 'Có lỗi xảy ra khi cập nhật trạm.',
        icon: 'error',
        confirmButtonText: 'Đóng',
      });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: stationService.deleteStation,
    onSuccess: () => {
      // THAY ĐỔI: Sử dụng SweetAlert2
      Swal.fire('Đã xóa!', 'Trạm đã được xóa thành công.', 'success');
      queryClient.invalidateQueries({ queryKey: ['stations'] });
    },
    onError: () => {
      // THAY ĐỔI: Sử dụng SweetAlert2
      Swal.fire('Lỗi!', 'Có lỗi xảy ra khi xóa trạm.', 'error');
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

  // THAY ĐỔI: Tạo hàm xử lý xác nhận xóa bằng SweetAlert2
  const handleDeleteConfirm = (id: number) => {
    Swal.fire({
      title: 'Bạn có chắc chắn muốn xóa?',
      text: "Hành động này không thể được hoàn tác!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Chắc chắn, xóa nó!',
      cancelButtonText: 'Hủy bỏ'
    }).then((result) => {
      if (result.isConfirmed) {
        deleteMutation.mutate(id);
      }
    });
  };


  const handleSearch = (value: string) => {
    setQueryParams(prev => ({ ...prev, page: 1, search: value.trim() }));
  };

  const handleFilterChange = (value: string) => {
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

  const handleTableChange: TableProps<StationDTO>['onChange'] = (pagination, /*filters*/ sorter) => {
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
    { title: 'Ảnh' , dataIndex: 'image', key: 'image', render: (url: string) => (<img src={url} alt="station" style={{ width: 80, height: 50, objectFit: 'cover', borderRadius: 4 }} />) },
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
          {/* THAY ĐỔI: Bỏ Popconfirm và gọi hàm mới */}
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