import React, { useEffect } from 'react';
import { Form, Input, InputNumber, Switch, Button, Row, Col } from 'antd';
import type { StationDTO } from '../../types/station';

interface StationFormProps {
  initialValues?: StationDTO | null;
  onSubmit: (values: Omit<StationDTO, 'id'>) => void;
  onCancel: () => void;
  isLoading: boolean;
}

const StationForm: React.FC<StationFormProps> = ({ initialValues, onSubmit, onCancel, isLoading }) => {
  const [form] = Form.useForm();

  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue(initialValues);
    } else {
      form.resetFields();
    }
  }, [initialValues, form]);

  const handleFinish = (values: any) => {
    onSubmit({ ...initialValues, ...values });
  };

  return (
    <Form form={form} layout="vertical" onFinish={handleFinish} initialValues={{ isActive: true }}>
      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="name"
            label="Tên trạm"
            rules={[{ required: true, message: 'Vui lòng nhập tên trạm!' }]}
          >
            <Input placeholder="Ví dụ: Trạm Đại học FPT - Cổng chính" />
          </Form.Item>
        </Col>
      </Row>
      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="location"
            label="Địa chỉ"
            rules={[{ required: true, message: 'Vui lòng nhập địa chỉ!' }]}
          >
            <Input placeholder="Ví dụ: Khuôn viên Đại học FPT, Hòa Lạc..." />
          </Form.Item>
        </Col>
      </Row>
      <Row gutter={16}>
        <Col span={12}>
          <Form.Item name="lat" label="Vĩ độ (Latitude)" rules={[{ required: true, message: 'Vui lòng nhập vĩ độ!' }]}>
            <InputNumber style={{ width: '100%' }} placeholder="21.013276" />
          </Form.Item>
        </Col>
        <Col span={12}>
          <Form.Item name="lng" label="Kinh độ (Longitude)" rules={[{ required: true, message: 'Vui lòng nhập kinh độ!' }]}>
            <InputNumber style={{ width: '100%' }} placeholder="105.526527" />
          </Form.Item>
        </Col>
      </Row>
      <Row gutter={16}>
        <Col span={12}>
          <Form.Item name="capacity" label="Sức chứa" rules={[{ required: true, message: 'Vui lòng nhập sức chứa!' }]}>
            <InputNumber min={1} style={{ width: '100%' }} placeholder="40" />
          </Form.Item>
        </Col>
        <Col span={12}>
          <Form.Item name="isActive" label="Trạng thái hoạt động" valuePropName="checked">
            <Switch />
          </Form.Item>
        </Col>
      </Row>
       <Form.Item name="image" label="URL Hình ảnh (tùy chọn)">
          <Input placeholder="/images/stations/fpt_main.jpg" />
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

export default StationForm;