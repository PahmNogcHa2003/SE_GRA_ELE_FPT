// src/features/categories/CategoryVehicleForm.tsx

import React, { useEffect } from 'react';
import { Form, Input, Switch, Button } from 'antd';
import type { CategoryVehicleDTO } from '../../types/categoryVehicle';

interface CategoryVehicleFormProps {
  initialValues?: CategoryVehicleDTO | null;
  onSubmit: (values: Omit<CategoryVehicleDTO, 'id'>) => void;
  onCancel: () => void;
  isLoading: boolean;
}

const CategoryVehicleForm: React.FC<CategoryVehicleFormProps> = ({ initialValues, onSubmit, onCancel, isLoading }) => {
  const [form] = Form.useForm();

  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue(initialValues);
    } else {
      form.resetFields();
    }
  }, [initialValues, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onSubmit} initialValues={{ isActive: true }}>
      <Form.Item
        name="name"
        label="Tên loại xe"
        rules={[{ required: true, message: 'Vui lòng nhập tên loại xe!' }]}
      >
        <Input placeholder="Ví dụ: Xe đạp điện thông thường" />
      </Form.Item>
      <Form.Item
        name="description"
        label="Mô tả"
        rules={[{ required: true, message: 'Vui lòng nhập mô tả!' }]}
      >
        <Input.TextArea rows={3} placeholder="Mô tả chi tiết về loại xe..." />
      </Form.Item>
      <Form.Item name="isActive" label="Trạng thái hoạt động" valuePropName="checked">
        <Switch />
      </Form.Item>
      <div style={{ textAlign: 'right', marginTop: '16px' }}>
        <Button onClick={onCancel} style={{ marginRight: 8 }}>
          Hủy
        </Button>
        <Button type="primary" htmlType="submit" loading={isLoading}>
          {initialValues ? 'Cập nhật' : 'Tạo mới'}
        </Button>
      </div>
    </Form>
  );
};

export default CategoryVehicleForm;