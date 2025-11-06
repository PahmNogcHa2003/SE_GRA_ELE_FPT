// src/features/tags/TagForm.tsx

import React, { useEffect } from 'react';
import { Form, Input, Button } from 'antd';
import type { TagDTO } from '../../types/tag';

interface TagFormProps {
  initialValues?: TagDTO | null;
  onSubmit: (values: Omit<TagDTO, 'id'>) => void;
  onCancel: () => void;
  isLoading: boolean;
}

const TagForm: React.FC<TagFormProps> = ({ initialValues, onSubmit, onCancel, isLoading }) => {
  const [form] = Form.useForm();

  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue(initialValues);
    } else {
      form.resetFields();
    }
  }, [initialValues, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onSubmit}>
      <Form.Item
        name="name"
        label="Tên thẻ"
        rules={[{ required: true, message: 'Vui lòng nhập tên thẻ!' }]}
      >
        <Input placeholder="Ví dụ: Khuyến mãi, Sự kiện..." />
      </Form.Item>
      <div style={{ textAlign: 'right', marginTop: '16px' }}>
        <Button onClick={onCancel} style={{ marginRight: 8 }}>Hủy</Button>
        <Button type="primary" htmlType="submit" loading={isLoading}>
          {initialValues ? 'Cập nhật' : 'Tạo mới'}
        </Button>
      </div>
    </Form>
  );
};

export default TagForm;