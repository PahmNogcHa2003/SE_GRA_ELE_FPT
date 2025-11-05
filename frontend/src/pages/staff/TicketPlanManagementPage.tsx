// src/pages/staff/TicketPlanManagementPage.tsx
import React, { useState } from 'react';
import { 
  Table, 
  Button, 
  Input, 
  Space, 
  Modal, 
  Form, 
  Input as AntInput, 
  Checkbox, 
  Popconfirm,
  Tag,
  App
} from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { 
  getPagedTicketPlans, 
  createTicketPlan, 
  updateTicketPlan, 
  deleteTicketPlan,
  type PagedRequestParams
} from '../../services/ticketplan.service';
import type { TableProps } from 'antd';
import type { TicketPlan, CreateTicketPlan, UpdateTicketPlan } from '../../types/manage.ticket';
import type { ApiResponse, PagedResult } from '../../types/api';

const { Search } = Input;

// Hook tùy chỉnh để quản lý Form và Modal
const useTicketPlanForm = (onFormSubmit: (values: any) => void) => {
  const [form] = Form.useForm();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<TicketPlan | null>(null);

  const showModal = (record: TicketPlan | null = null) => {
    setEditingRecord(record);
    if (record) {
      form.setFieldsValue(record); // Cập nhật form với dữ liệu cũ
    } else {
      form.resetFields(); // Xóa form cho bản ghi mới
    }
    setIsModalOpen(true);
  };

  const handleCancel = () => {
    setIsModalOpen(false);
    setEditingRecord(null);
  };

  const handleFinish = (values: CreateTicketPlan | UpdateTicketPlan) => {
    onFormSubmit(values);
    handleCancel();
  };

  return { form, isModalOpen, editingRecord, showModal, handleCancel, handleFinish };
};


const TicketPlanManagementPage: React.FC = () => {
  const { notification } = App.useApp();
  const queryClient = useQueryClient();
  const [queryParams, setQueryParams] = useState<PagedRequestParams>({
    page: 1,
    pageSize: 10,
  });

  // Query để lấy dữ liệu
  const { data: pagedData, isLoading } = useQuery({
    queryKey: ['staffTicketPlans', queryParams],
    queryFn: () => getPagedTicketPlans(queryParams),
    select: (data) => data.data, // Lấy PagedResult từ ApiResponse
  });

  // Mutation cho Create
  const createMutation = useMutation({
    mutationFn: createTicketPlan,
    onSuccess: (res) => {
      notification.success({ message: res.data.message });
      queryClient.invalidateQueries({ queryKey: ['staffTicketPlans'] });
    },
    onError: (err: ApiResponse<null>) => {
      notification.error({ message: err.message || 'Tạo mới thất bại' });
    }
  });

  // Mutation cho Update
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateTicketPlan }) => updateTicketPlan(id, data),
    onSuccess: (res) => {
      notification.success({ message: res.data.message });
      queryClient.invalidateQueries({ queryKey: ['staffTicketPlans'] });
    },
    onError: (err: ApiResponse<null>) => {
      notification.error({ message: err.message || 'Cập nhật thất bại' });
    }
  });

  // Mutation cho Delete
  const deleteMutation = useMutation({
    mutationFn: deleteTicketPlan,
    onSuccess: (res) => {
      notification.success({ message: res.data.message });
      queryClient.invalidateQueries({ queryKey: ['staffTicketPlans'] });
    },
    onError: (err: ApiResponse<null>) => {
      notification.error({ message: err.message || 'Xóa thất bại' });
    }
  });

  // Xử lý submit form
  const handleFormSubmit = (values: CreateTicketPlan | UpdateTicketPlan) => {
    if (formHook.editingRecord) {
      // Update
      updateMutation.mutate({ id: formHook.editingRecord.id, data: values });
    } else {
      // Create
      createMutation.mutate(values as CreateTicketPlan);
    }
  };

  const formHook = useTicketPlanForm(handleFormSubmit);

  // Xử lý thay đổi Table (Phân trang, Sắp xếp)
  const handleTableChange: TableProps<TicketPlan>['onChange'] = (
    pagination, filters, sorter
  ) => {
    setQueryParams(prev => ({
      ...prev,
      page: pagination.current,
      pageSize: pagination.pageSize,
      // Thêm logic Sorter nếu backend hỗ trợ
    }));
  };

  // Cấu hình cột
  const columns: TableProps<TicketPlan>['columns'] = [
    { title: 'ID', dataIndex: 'id', key: 'id', sorter: true },
    { title: 'Mã (Code)', dataIndex: 'code', key: 'code' },
    { title: 'Tên Gói', dataIndex: 'name', key: 'name' },
    { title: 'Loại (Type)', dataIndex: 'type', key: 'type' },
    { title: 'Mô tả', dataIndex: 'description', key: 'description' },
    { 
      title: 'Trạng thái', 
      dataIndex: 'isActive', 
      key: 'isActive',
      render: (isActive: boolean) => (
        <Tag color={isActive ? 'green' : 'red'}>
          {isActive ? 'ĐANG HOẠT ĐỘNG' : 'KHÔNG HOẠT ĐỘNG'}
        </Tag>
      )
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_, record) => (
        <Space size="middle">
          <Button icon={<EditOutlined />} onClick={() => formHook.showModal(record)}>
            Sửa
          </Button>
          <Popconfirm
            title="Xóa gói vé"
            description="Bạn có chắc muốn xóa gói vé này?"
            onConfirm={() => deleteMutation.mutate(record.id)}
            okText="Xóa"
            cancelText="Hủy"
          >
            <Button icon={<DeleteOutlined />} danger>
              Xóa
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <h2 className="text-2xl font-semibold mb-4">Quản lý Gói Vé (Ticket Plans)</h2>
      
      {/* Thanh công cụ: Thêm mới + Tìm kiếm */}
      <div className="flex justify-between mb-4">
        <Button
          type="primary"
          icon={<PlusOutlined />}
          className="bg-eco-green hover:bg-eco-green-dark"
          onClick={() => formHook.showModal(null)}
        >
          Thêm Gói Vé
        </Button>
        <Search
          placeholder="Tìm theo Tên, Mã..."
          onSearch={(value) => setQueryParams(prev => ({ ...prev, page: 1, searchQuery: value }))}
          style={{ width: 300 }}
        />
      </div>

      {/* Bảng dữ liệu */}
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

      {/* Modal Form (Thêm/Sửa) */}
      <Modal
        title={formHook.editingRecord ? 'Cập nhật Gói Vé' : 'Tạo Gói Vé Mới'}
        open={formHook.isModalOpen}
        onCancel={formHook.handleCancel}
        onOk={formHook.form.submit}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
      >
        <Form form={formHook.form} layout="vertical" onFinish={formHook.handleFinish} className="mt-6">
          <Form.Item name="name" label="Tên Gói Vé" rules={[{ required: true }]}>
            <AntInput />
          </Form.Item>
          <Form.Item name="code" label="Mã Gói (Code)" rules={[{ required: true }]}>
            <AntInput />
          </Form.Item>
          <Form.Item name="type" label="Loại (Type)" rules={[{ required: true }]}>
            <AntInput placeholder="Ví dụ: Lượt, Ngày, Tháng" />
          </Form.Item>
          <Form.Item name="description" label="Mô tả">
            <AntInput.TextArea rows={2} />
          </Form.Item>
          <Form.Item name="isActive" valuePropName="checked">
            <Checkbox>Đang hoạt động (Is Active)</Checkbox>
          </Form.Item>
        </Form>
      </Modal>

    </div>
  );
};

export default TicketPlanManagementPage;