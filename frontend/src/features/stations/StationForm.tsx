import React, { useEffect } from 'react';
import { Form, Input, InputNumber, Switch, Button, Row, Col, Alert } from 'antd';
import type { StationDTO } from '../../types/station';
import type { MapPickerValue } from '../../components/maps/MapPicker';
import MapPicker from '../../components/maps/MapPicker';

interface StationFormProps {
  initialValues?: StationDTO | null;
  onSubmit: (values: Omit<StationDTO, 'id'>) => void;
  onCancel: () => void;
  isLoading: boolean;
}

const StationForm: React.FC<StationFormProps> = ({
  initialValues,
  onSubmit,
  onCancel,
  isLoading,
}) => {
  const [form] = Form.useForm();

  useEffect(() => {
    if (initialValues) {
      const { id, ...rest } = initialValues;

      form.setFieldsValue({
        ...rest,
        coordinates: {
          lat: initialValues.lat,
          lng: initialValues.lng,
        } as MapPickerValue,
      });
    } else {
      form.resetFields();
      form.setFieldsValue({ isActive: true });
    }
  }, [initialValues, form]);

  const handleFinish = (values: any) => {
    const { coordinates, ...restValues } = values;

    if (!coordinates || coordinates.lat == null || coordinates.lng == null) {
      return;
    }

    const base: Omit<StationDTO, 'id'> = initialValues
      ? (() => {
          const { id, ...others } = initialValues;
          return others;
        })()
      : ({} as Omit<StationDTO, 'id'>);

    const finalValues: Omit<StationDTO, 'id'> = {
      ...base,
      ...restValues,
      lat: coordinates.lat,
      lng: coordinates.lng,
    };

    onSubmit(finalValues);
  };

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={handleFinish}
      initialValues={{ isActive: true }}
    >
      <Row gutter={20}>
        <Col span={12}>
          <Form.Item
            name="name"
            label="Tên trạm"
            rules={[{ required: true, message: 'Vui lòng nhập tên trạm!' }]}
          >
            <Input placeholder="Ví dụ: Trạm Đại học FPT - Cổng chính" />
          </Form.Item>
        </Col>

        <Col span={12}>
          <Form.Item
            name="location"
            label="Địa chỉ"
            rules={[{ required: true, message: 'Vui lòng nhập địa chỉ!' }]}
          >
            <Input placeholder="Ví dụ: Khuôn viên Đại học FPT, Hòa Lạc..." />
          </Form.Item>
        </Col>
      </Row>

      <Row gutter={20}>
        <Col span={12}>
          <Form.Item
            name="capacity"
            label="Sức chứa"
            rules={[{ required: true, message: 'Vui lòng nhập sức chứa!' }]}
          >
            <InputNumber min={1} style={{ width: '100%' }} placeholder="40" />
          </Form.Item>
        </Col>

        <Col span={12}>
          <Form.Item
            name="isActive"
            label="Trạng thái hoạt động"
            valuePropName="checked"
          >
            <Switch />
          </Form.Item>
        </Col>
      </Row>
        <Form.Item
        name="coordinates"
        label="Vị trí trên bản đồ"
        rules={[
          { required: true, message: 'Vui lòng chọn vị trí trên bản đồ!' },
          () => ({
            validator(_, value: MapPickerValue) {
              if (value && value.lat != null && value.lng != null) {
                return Promise.resolve();
              }
              return Promise.reject(
                new Error('Vui lòng chọn đầy đủ tọa độ (lat, lng)!')
              );
            },
          }),
        ]}
      >
        <div style={{ width: '100%', height: '350px', marginBottom: 10 }}>
          <MapPicker height="350px" />
        </div>
      </Form.Item>

      <Alert
        message="Chọn vị trí trạm bằng cách click lên bản đồ hoặc nhập địa chỉ vào ô tìm kiếm."
        type="info"
        showIcon
        style={{ marginBottom: 24 }}
      />
      <div style={{ textAlign: 'right', marginTop: 16 }}>
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
