// src/pages/staff/ManageCategoriesVehiclePage.tsx

import React, { useState } from 'react';
import { Table, Button, Modal, Space, Typography, Input, Tag, Select, Row, Col } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { TableProps } from 'antd';
import Swal from 'sweetalert2';

import * as categoryVehicleService from '../../services/categoryVehicle.service';
import type { CategoryVehicleDTO, GetCategoriesVehicleParams } from '../../types/categoryVehicle';
import CategoryVehicleForm from '../../features/categories/CategoryVehicleForm';

const { Title } = Typography;
const { Search } = Input;

const ManageCategoriesVehiclePage: React.FC = () => {
  const queryClient = useQueryClient();

  const [queryParams, setQueryParams] = useState<GetCategoriesVehicleParams>({
    page: 1,
    pageSize: 10,
  });

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<CategoryVehicleDTO | null>(null);

  const { data: categoriesData, isLoading } = useQuery({
    queryKey: ['categoriesVehicle', queryParams],
    queryFn: () => categoryVehicleService.getCategories(queryParams),
    select: (res) => res.data,
  });

  const createMutation = useMutation({
    mutationFn: categoryVehicleService.createCategory,
    onSuccess: () => {
      Swal.fire('Thành công!', 'Tạo loại xe mới thành công!', 'success');
      queryClient.invalidateQueries({ queryKey: ['categoriesVehicle'] });
      setIsModalOpen(false);
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi tạo loại xe mới.', 'error'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: CategoryVehicleDTO }) =>
      categoryVehicleService.updateCategory(id, data),
    onSuccess: () => {
      Swal.fire('Thành công!', 'Cập nhật loại xe thành công!', 'success');
      queryClient.invalidateQueries({ queryKey: ['categoriesVehicle'] });
      setIsModalOpen(false);
      setEditingCategory(null);
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi cập nhật.', 'error'),
  });

  const deleteMutation = useMutation({
    mutationFn: categoryVehicleService.deleteCategory,
    onSuccess: () => {
      Swal.fire('Đã xóa!', 'Loại xe đã được xóa.', 'success');
      queryClient.invalidateQueries({ queryKey: ['categoriesVehicle'] });
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi xóa.', 'error'),
  });

  const handleOpenCreateModal = () => {
    setEditingCategory(null);
    setIsModalOpen(true);
  };

  const handleOpenEditModal = (category: CategoryVehicleDTO) => {
    setEditingCategory(category);
    setIsModalOpen(true);
  };

  const handleCancelModal = () => setIsModalOpen(false);

  const handleFormSubmit = (values: Omit<CategoryVehicleDTO, 'id'>) => {
    if (editingCategory) {
      updateMutation.mutate({ id: editingCategory.id, data: { ...editingCategory, ...values } });
    } else {
      createMutation.mutate(values);
    }
  };

  const handleDeleteConfirm = (id: number) => {
    Swal.fire({
      title: 'Bạn có chắc không?',
      text: "Bạn sẽ không thể hoàn tác hành động này!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      confirmButtonText: 'Vâng, xóa nó!',
      cancelButtonText: 'Hủy'
    }).then((result) => {
      if (result.isConfirmed) {
        deleteMutation.mutate(id);
      }
    });
  };

  const handleTableChange: TableProps<CategoryVehicleDTO>['onChange'] = (pagination) => {
    setQueryParams(prev => ({
      ...prev,
      page: pagination.current,
      pageSize: pagination.pageSize,
    }));
  };
  
  const handleSearch = (value: string) => {
    setQueryParams(prev => ({ ...prev, page: 1, search: value.trim() }));
  };

  const handleFilterChange = (value: string) => {
    const newParams: GetCategoriesVehicleParams = { ...queryParams, page: 1 };
    if (value === 'all') {
      delete newParams.filterField;
      delete newParams.filterValue;
    } else {
      newParams.filterField = 'isActive';
      newParams.filterValue = value;
    }
    setQueryParams(newParams);
  };

  const columns: TableProps<CategoryVehicleDTO>['columns'] = [
    { title: 'ID', dataIndex: 'id', key: 'id' },
    { title: 'Tên loại xe', dataIndex: 'name', key: 'name' },
    { title: 'Mô tả', dataIndex: 'description', key: 'description' },
    {
      title: 'Trạng thái',
      dataIndex: 'isActive',
      key: 'isActive',
      align: 'center',
      render: (isActive: boolean) => (
        <Tag color={isActive ? 'green' : 'red'}>{isActive ? 'HOẠT ĐỘNG' : 'KHÓA'}</Tag>
      ),
    },
    {
      title: 'Hành động',
      key: 'action',
      align: 'center',
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
      <Title level={2}>Quản lý Loại xe</Title>

      <Row justify="space-between" align="middle" style={{ marginBottom: 24 }} gutter={[16, 16]}>
        <Col>
          <Space>
            <span>Lọc theo trạng thái:</span>
            <Select defaultValue="all" style={{ width: 150 }} onChange={handleFilterChange}>
              <Select.Option value="all">Tất cả</Select.Option>
              <Select.Option value="true">Hoạt động</Select.Option>
              <Select.Option value="false">Khóa</Select.Option>
            </Select>
          </Space>
        </Col>
        <Col>
          <Space>
            <Search placeholder="Tìm theo tên, mô tả..." onSearch={handleSearch} style={{ width: 300 }} allowClear enterButton />
            <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenCreateModal}>
              Thêm loại xe
            </Button>
          </Space>
        </Col>
      </Row>

      <Table
        columns={columns}
        dataSource={categoriesData?.data?.items}
        loading={isLoading}
        rowKey="id"
        pagination={{
          current: categoriesData?.data?.pageNumber,
          pageSize: categoriesData?.data?.pageSize,
          total: categoriesData?.data?.totalCount,
          showSizeChanger: true,
        }}
        onChange={handleTableChange}
        scroll={{ x: 'max-content' }}
      />

      <Modal
        title={editingCategory ? 'Chỉnh sửa loại xe' : 'Tạo loại xe mới'}
        open={isModalOpen}
        onCancel={handleCancelModal}
        footer={null}
        destroyOnClose
      >
        <CategoryVehicleForm
          initialValues={editingCategory}
          onSubmit={handleFormSubmit}
          onCancel={handleCancelModal}
          isLoading={createMutation.isPending || updateMutation.isPending}
        />
      </Modal>
    </div>
  );
};

export default ManageCategoriesVehiclePage;