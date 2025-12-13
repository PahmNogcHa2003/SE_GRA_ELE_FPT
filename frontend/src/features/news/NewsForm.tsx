// src/features/news/NewsForm.tsx

import React, { useEffect } from 'react';
import { Form, Input, Select, Button, Switch } from 'antd';
import { useQuery } from '@tanstack/react-query';
import * as tagService from '../../services/tag.service';
import type { NewsDTO } from '../../types/news';
import type { TagDTO } from '../../types/tag';

const { Option } = Select;
const { TextArea } = Input;

interface NewsFormProps {
  initialValues?: NewsDTO | null;
  onSubmit: (values: Omit<NewsDTO, 'id' | 'createdAt' | 'tags'>) => void;
  onCancel: () => void;
  isLoading: boolean;
}

const NewsForm: React.FC<NewsFormProps> = ({ initialValues, onSubmit, onCancel, isLoading }) => {
  const [form] = Form.useForm();

  // Lấy danh sách tất cả các tag để chọn
  const { data: tagsData, isLoading: isLoadingTags } = useQuery({
    queryKey: ['allTags'],
    queryFn: () => tagService.getTagsAdmin({ pageSize: 1000 }),
    select: (res) => res.data.data?.items,
  });

  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue(initialValues);
    } else {
      form.resetFields();
    }
  }, [initialValues, form]);

  return (
    <Form form={form} layout="vertical" onFinish={onSubmit} initialValues={{ isActive: true }}>
      <Form.Item name="title" label="Tiêu đề" rules={[{ required: true, message: 'Vui lòng nhập tiêu đề!' }]}>
        <Input placeholder="Tiêu đề bài viết..." />
      </Form.Item>
      <Form.Item name="content" label="Nội dung" rules={[{ required: true, message: 'Vui lòng nhập nội dung!' }]}>
        <TextArea rows={6} placeholder="Nội dung chi tiết..." />
      </Form.Item>
      <Form.Item name="tagIds" label="Gắn thẻ (Tags)">
        <Select mode="multiple" allowClear style={{ width: '100%' }} placeholder="Chọn các thẻ liên quan" loading={isLoadingTags}>
          {tagsData?.map((tag: TagDTO) => (
            <Option key={tag.id} value={tag.id}>{tag.name}</Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item name="isActive" label="Trạng thái" valuePropName="checked">
        <Switch checkedChildren="Công khai" unCheckedChildren="Bản nháp" />
      </Form.Item>
      <div style={{ textAlign: 'right', marginTop: '16px' }}>
        <Button onClick={onCancel} style={{ marginRight: 8 }}>Hủy</Button>
        <Button type="primary" htmlType="submit" loading={isLoading}>
          {initialValues ? 'Cập nhật' : 'Lưu'}
        </Button>
      </div>
    </Form>
  );
};

export default NewsForm;