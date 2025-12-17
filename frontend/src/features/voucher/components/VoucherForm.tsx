import React, { useEffect } from 'react';
import {
  Modal, Form, Input, InputNumber, Switch, DatePicker, Row, Col
} from 'antd';
import dayjs from 'dayjs';
import type { VoucherDTO, VoucherCreateDTO } from '../../../types/manage.voucher';

const { RangePicker } = DatePicker;

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (v: VoucherCreateDTO) => void;
  initialData?: VoucherDTO | null;
  isLoading: boolean;
}

const VoucherForm: React.FC<Props> = ({
  open, onClose, onSubmit, initialData, isLoading
}) => {
  const [form] = Form.useForm();
  const isPercentage = Form.useWatch('isPercentage', form);

  useEffect(() => {
    if (open) {
      if (initialData) {
        form.setFieldsValue({
          ...initialData,
          dateRange: [dayjs(initialData.startDate), dayjs(initialData.endDate)]
        });
      } else {
        form.resetFields();
        form.setFieldsValue({ isPercentage: true });
      }
    }
  }, [open]);

  const handleFinish = (v: any) => {
    onSubmit({
      code: v.code,
      isPercentage: v.isPercentage,
      value: v.value,
      maxDiscountAmount: v.maxDiscountAmount,
      minOrderAmount: v.minOrderAmount,
      usageLimit: v.usageLimit,
      usagePerUser: v.usagePerUser,
      startDate: v.dateRange[0].toISOString(),
      endDate: v.dateRange[1].toISOString()
    });
  };

  return (
    <Modal
      open={open}
      title={initialData ? 'Cập nhật Voucher' : 'Tạo Voucher'}
      onCancel={onClose}
      onOk={() => form.submit()}
      confirmLoading={isLoading}
      width={700}
    >
      <Form form={form} layout="vertical" onFinish={handleFinish}>
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="code" label="Code" rules={[{ required: true }]}>
              <Input disabled={!!initialData} />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="isPercentage" label="Giảm theo %" valuePropName="checked">
              <Switch />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item name="value" label="Giá trị" rules={[{ required: true }]}>
          <InputNumber style={{ width: '100%' }} />
        </Form.Item>

        {!isPercentage && (
          <Form.Item name="maxDiscountAmount" label="Giảm tối đa">
            <InputNumber style={{ width: '100%' }} />
          </Form.Item>
        )}

        <Form.Item name="dateRange" label="Thời gian" rules={[{ required: true }]}>
          <RangePicker showTime style={{ width: '100%' }} />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default VoucherForm;
