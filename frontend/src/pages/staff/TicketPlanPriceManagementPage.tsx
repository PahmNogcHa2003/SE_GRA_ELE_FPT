// src/pages/staff/TicketPlanPriceManagementPage.tsx
import React, { useState } from 'react';
import { 
  Table, 
  Button, 
  Input, 
  Space, 
  Modal, 
  Form, 
  InputNumber,
  Checkbox, 
  Popconfirm,
  Tag,
  Select,
  DatePicker,
  App
} from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { 
  getPagedTicketPlanPrices, 
  createTicketPlanPrice, 
  updateTicketPlanPrice, 
  deleteTicketPlanPrice, // Dùng để lấy danh sách Plan
  type PagedRequestParams
} from '../../services/ticketplanprice.service';
import { 
  getPagedTicketPlans, // Dùng để lấy danh sách Plan
} from '../../services/ticketplan.service';
import type { TableProps } from 'antd';
import type { TicketPlanPrice, CreateTicketPlanPrice, UpdateTicketPlanPrice } from '../../types/manage.ticket';
import type { ApiResponse} from '../../types/api';
import dayjs from 'dayjs'; // Cần cài: npm install dayjs

const { Search } = Input;
const { RangePicker } = DatePicker;

// Hook tùy chỉnh để quản lý Form và Modal (Phức tạp hơn)
const useTicketPriceForm = (onFormSubmit: (values: any) => void) => {
  const [form] = Form.useForm();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<TicketPlanPrice | null>(null);

  // Lấy danh sách TicketPlan để làm ô Select
  const { data: plansData } = useQuery({
    queryKey: ['staffTicketPlans', { pageSize: 1000 }], // Lấy tất cả
    queryFn: () => getPagedTicketPlans({ pageSize: 1000 }),
    select: (data) => data.data.data?.items,
  });

  const showModal = (record: TicketPlanPrice | null = null) => {
    setEditingRecord(record);
    if (record) {
      // Chuyển đổi ngày tháng cho DatePicker
      form.setFieldsValue({
        ...record,
        validDates: (record.validFrom && record.validTo) 
          ? [dayjs(record.validFrom), dayjs(record.validTo)] 
          : null,
      });
    } else {
      form.resetFields();
      form.setFieldsValue({ isActive: true }); // Mặc định
    }
    setIsModalOpen(true);
  };

  const handleCancel = () => setIsModalOpen(false);

  const handleFinish = (values: any) => {
    // Xử lý ngày tháng trước khi gửi
    const { validDates, ...rest } = values;
    const payload = {
      ...rest,
      validFrom: validDates?.[0] ? dayjs(validDates[0]).toISOString() : null,
      validTo: validDates?.[1] ? dayjs(validDates[1]).toISOString() : null,
    };
    onFormSubmit(payload);
    handleCancel();
  };

  return { form, isModalOpen, editingRecord, plansData, showModal, handleCancel, handleFinish };
};


const TicketPlanPriceManagementPage: React.FC = () => {
  const { notification } = App.useApp();
  const queryClient = useQueryClient();
  const [queryParams, setQueryParams] = useState<PagedRequestParams>({
    page: 1,
    pageSize: 10,
  });

  // Query để lấy dữ liệu
  const { data: pagedData, isLoading } = useQuery({
    queryKey: ['staffTicketPrices', queryParams],
    queryFn: () => getPagedTicketPlanPrices(queryParams),
    select: (data) => data.data,
  });

  // Mutations
  const createMutation = useMutation({
    mutationFn: createTicketPlanPrice,
    onSuccess: (res) => {
      notification.success({ message: res.data.message });
      queryClient.invalidateQueries({ queryKey: ['staffTicketPrices'] });
    },
    onError: (err: ApiResponse<null>) => notification.error({ message: err.message || 'Tạo mới thất bại' })
  });
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateTicketPlanPrice }) => updateTicketPlanPrice(id, data),
    onSuccess: (res) => {
      notification.success({ message: res.data.message });
      queryClient.invalidateQueries({ queryKey: ['staffTicketPrices'] });
    },
    onError: (err: ApiResponse<null>) => notification.error({ message: err.message || 'Cập nhật thất bại' })
  });
  const deleteMutation = useMutation({
    mutationFn: deleteTicketPlanPrice,
    onSuccess: (res) => {
      notification.success({ message: res.data.message });
      queryClient.invalidateQueries({ queryKey: ['staffTicketPrices'] });
    },
    onError: (err: ApiResponse<null>) => notification.error({ message: err.message || 'Xóa thất bại' })
  });

  // Xử lý submit
  const handleFormSubmit = (values: CreateTicketPlanPrice | UpdateTicketPlanPrice) => {
    if (formHook.editingRecord) {
      updateMutation.mutate({ id: formHook.editingRecord.id, data: values as UpdateTicketPlanPrice });
    } else {
      createMutation.mutate(values as CreateTicketPlanPrice);
    }
  };
  
  const formHook = useTicketPriceForm(handleFormSubmit);
  const [vehicleTypeFilter, setVehicleTypeFilter] = useState<string | null>(null);

  const handleTableChange: TableProps<TicketPlanPrice>['onChange'] = (pagination) => {
  setQueryParams(prev => ({
    ...prev,
    page: pagination.current,
    pageSize: pagination.pageSize,
  }));
};


  const columns: TableProps<TicketPlanPrice>['columns'] = [
    { title: 'ID', dataIndex: 'id', key: 'id' },
    { title: 'Tên Gói', dataIndex: 'planName', key: 'planName' },
    { 
      title: 'Loại Xe', 
      dataIndex: 'vehicleType', 
      key: 'vehicleType',
      render: (type) => type === 'EBike' ? 'Xe điện' : 'Xe đạp'
    },
    { title: 'Giá (VND)', dataIndex: 'price', key: 'price', render: (val) => val.toLocaleString('vi-VN') },
    { title: 'Thời hạn (Ngày)', dataIndex: 'validityDays', key: 'validityDays' },
    { title: 'Giới hạn (Phút)', dataIndex: 'durationLimitMinutes', key: 'durationLimitMinutes' },
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
          <Button icon={<EditOutlined />} onClick={() => formHook.showModal(record)}>Sửa</Button>
          <Popconfirm
            title="Xóa giá vé"
            description="Bạn có chắc muốn xóa giá vé này?"
            onConfirm={() => deleteMutation.mutate(record.id)}
            okText="Xóa"
            cancelText="Hủy"
          >
            <Button icon={<DeleteOutlined />} danger>Xóa</Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <h2 className="text-2xl font-semibold mb-4">Quản lý Giá Vé</h2>
      
      <div className="flex justify-between mb-4 gap-4">
        <Button
          type="primary"
          icon={<PlusOutlined />}
          className="bg-eco-green hover:bg-eco-green-dark"
          onClick={() => formHook.showModal(null)}
        >
          Thêm Giá Vé
        </Button>

        <div className="flex gap-2">
          {/* FILTER LOẠI XE */}
          <Select
            placeholder="Loại xe"
            style={{ width: 160 }}
            allowClear
            onChange={(val) => {
              setVehicleTypeFilter(val);
              setQueryParams((prev) => ({
                ...prev,
                page: 1,
                filterField: val ? "vehicleType" : undefined,
                filterValue: val || undefined,
              }));
            }}
            options={[
              { label: "Xe đạp", value: "bike" },
              { label: "Xe điện", value: "ebike" },
            ]}
          />

          {/* SEARCH */}
          <Search
            placeholder="Tìm theo Tên gói, Loại xe..."
            allowClear
            onSearch={(value) =>
              setQueryParams((prev) => ({
                ...prev,
                page: 1,
                searchQuery: value,
                filterField: vehicleTypeFilter ? "vehicleType" : undefined,
                filterValue: vehicleTypeFilter || undefined,
              }))
            }
            style={{ width: 300 }}
          />
        </div>
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

      {/* Modal Form */}
      <Modal
        title={formHook.editingRecord ? 'Cập nhật Giá Vé' : 'Tạo Giá Vé Mới'}
        open={formHook.isModalOpen}
        onCancel={formHook.handleCancel}
        onOk={formHook.form.submit}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
        width={800} // Modal rộng hơn
      >
        <Form form={formHook.form} layout="vertical" onFinish={formHook.handleFinish} className="mt-6">
          <Form.Item name="planId" label="Gói Vé (Plan)" rules={[{ required: true }]}>
            <Select 
              placeholder="Chọn một gói vé"
              options={formHook.plansData?.map(plan => ({
                label: `${plan.name} (Code: ${plan.code})`,
                value: plan.id
              }))}
              disabled={!!formHook.editingRecord} // Không cho sửa PlanId
            />
          </Form.Item>
          
          <div className="grid grid-cols-2 gap-x-4">
            <Form.Item name="vehicleType" label="Loại Xe" rules={[{ required: true }]}>
              <Select placeholder="Chọn loại xe">
                <Select.Option value="bike">Xe đạp</Select.Option>
                <Select.Option value="ebike">Xe điện</Select.Option>
              </Select>
            </Form.Item>
            <Form.Item name="price" label="Giá (VND)" rules={[{ required: true }]}>
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>
          </div>
          
          <div className="grid grid-cols-2 gap-x-4">
            <Form.Item name="validityDays" label="Thời hạn (Số ngày)">
              <InputNumber min={0} style={{ width: '100%' }} placeholder="VD: 30 (cho vé tháng)" />
            </Form.Item>
            <Form.Item name="durationLimitMinutes" label="Giới hạn (Tổng số phút)">
              <InputNumber min={0} style={{ width: '100%' }} placeholder="VD: 450 (cho vé ngày)" />
            </Form.Item>
          </div>
          
          <Form.Item name="validDates" label="Hiệu lực từ - đến (Tùy chọn)">
             <RangePicker showTime />
          </Form.Item>

          <Form.Item name="isActive" valuePropName="checked">
            <Checkbox>Đang hoạt động</Checkbox>
          </Form.Item>
        </Form>
      </Modal>

    </div>
  );
};

export default TicketPlanPriceManagementPage;