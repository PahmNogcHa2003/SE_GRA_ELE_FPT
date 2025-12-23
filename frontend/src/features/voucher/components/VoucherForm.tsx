// features/voucher/components/VoucherForm.tsx

import React, { useEffect } from 'react';
import {
  Modal, Form, Input, InputNumber, Switch, DatePicker, Row, Col, Divider
} from 'antd';
import dayjs from 'dayjs';
import type { VoucherDTO, VoucherCreateDTO } from '../../../types/manage.voucher';

const { RangePicker } = DatePicker;
const { TextArea } = Input;

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
  
  // Watch để ẩn hiện field động
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
        form.setFieldsValue({ 
            isPercentage: true,
            minOrderAmount: 0, // Mặc định là 0
            usageLimit: 100, 
            usagePerUser: 1
        });
      }
    }
  }, [open, initialData, form]);

  const handleFinish = (v: any) => {
    const payload: VoucherCreateDTO = {
      code: v.code,
      description: v.description,
      isPercentage: v.isPercentage,
      value: v.value,
      maxDiscountAmount: v.maxDiscountAmount ?? null,
      minOrderAmount: v.minOrderAmount ?? 0,
      usageLimit: v.usageLimit ?? null,
      usagePerUser: v.usagePerUser ?? null,
      startDate: v.dateRange[0].toISOString(),
      endDate: v.dateRange[1].toISOString(),
    };
    
    onSubmit(payload);
  };

  return (
    <Modal
      open={open}
      title={initialData ? 'Cập nhật Voucher' : 'Tạo Voucher Mới'}
      onCancel={onClose}
      onOk={() => form.submit()}
      confirmLoading={isLoading}
      width={700}
      maskClosable={false}
    >
      <Form form={form} layout="vertical" onFinish={handleFinish}>
        
        {/* --- THÔNG TIN CƠ BẢN --- */}
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item 
                name="code" 
                label="Mã Voucher" 
                rules={[
                    { required: true, message: 'Vui lòng nhập mã voucher' },
                    { max: 50, message: 'Mã không được quá 50 ký tự' },
                    { pattern: /^[a-zA-Z0-9_]*$/, message: 'Mã chỉ chứa chữ, số và gạch dưới' }
                ]}
            >
              <Input placeholder="Vd: SUMMER2025" style={{ textTransform: 'uppercase' }} disabled={!!initialData} />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="isPercentage" label="Loại giảm giá" valuePropName="checked">
               <Switch checkedChildren="Theo %" unCheckedChildren="Số tiền cố định" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item 
            name="description" 
            label="Mô tả" 
            rules={[{ required: true, message: 'Vui lòng nhập mô tả voucher' }]}
        >
          <TextArea rows={2} placeholder="Vd: Giảm giá mùa hè cho khách hàng mới..." />
        </Form.Item>

        <Divider orientation="left">Cấu hình giá trị</Divider>

        <Row gutter={16}>
        <Col span={12}>
            <Form.Item 
                name="value" 
                label={isPercentage ? "Phần trăm giảm (%)" : "Số tiền giảm (VND)"}
                rules={[
                    { required: true, message: 'Vui lòng nhập giá trị' },
                    { type: 'number', min: 1, message: 'Giá trị phải lớn hơn 0' },
                    // Validate riêng cho phần trăm
                    ...(isPercentage ? [{
                        validator: (_: any, value: number) => {
                            if (value === undefined || value === null || value <= 100) {
                                return Promise.resolve();
                            }
                            return Promise.reject(new Error('Phần trăm không được quá 100%'));
                        }
                    }] : [])
                ]}
            >
              <InputNumber 
                style={{ width: '100%' }} 
                formatter={value => value ? `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : ''}
                parser={value => value?.replace(/\$\s?|(,*)/g, '') as unknown as number}
                addonAfter={isPercentage ? '%' : '₫'}
                placeholder="Nhập giá trị giảm"
              />
            </Form.Item>
          </Col>
          
          {isPercentage && (
            <Col span={12}>
              <Form.Item 
                name="maxDiscountAmount" 
                label="Giảm tối đa (VND)"
                help="Để trống nếu không giới hạn"
                rules={[
                    { type: 'number', min: 0, message: 'Số tiền không được âm' }
                ]}
              >
                <InputNumber 
                    style={{ width: '100%' }} 
                    formatter={value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                    parser={value => value?.replace(/\$\s?|(,*)/g, '') as unknown as number}
                    addonAfter="₫"
                />
              </Form.Item>
            </Col>
          )}
        </Row>

        <Row gutter={16}>
            <Col span={12}>
                <Form.Item 
                    name="minOrderAmount" 
                    label="Đơn hàng tối thiểu (VND)"
                    // SỬA Ở ĐÂY: Thêm message tiếng Việt vào rule min
                    rules={[
                        { required: true, message: 'Vui lòng nhập số tiền (nhập 0 nếu không cần)' }, 
                        { type: 'number', min: 0, message: 'Số tiền không được nhỏ hơn 0' }
                    ]}
                >
                    <InputNumber 
                        style={{ width: '100%' }} 
                        formatter={value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                        parser={value => value?.replace(/\$\s?|(,*)/g, '') as unknown as number}
                        addonAfter="₫"
                    />
                </Form.Item>
            </Col>
            <Col span={12}>
                 <Form.Item 
                    name="dateRange" 
                    label="Thời gian áp dụng" 
                    rules={[{ required: true, message: 'Vui lòng chọn thời gian bắt đầu và kết thúc' }]}
                >
                  <RangePicker showTime format="DD/MM/YYYY HH:mm" style={{ width: '100%' }} />
                </Form.Item>
            </Col>
        </Row>

        <Divider orientation="left">Giới hạn sử dụng</Divider>

        <Row gutter={16}>
            <Col span={12}>
                <Form.Item 
                    name="usageLimit" 
                    label="Tổng số lượng voucher"
                    // SỬA: Đổi help -> extra
                    extra="Để trống nếu không giới hạn"
                    rules={[
                        { type: 'number', min: 1, message: 'Số lượng phải lớn hơn 0' }
                    ]}
                >
                     <InputNumber style={{ width: '100%' }} placeholder="∞" />
                </Form.Item>
            </Col>
            
            <Col span={12}>
                <Form.Item 
                    name="usagePerUser" 
                    label="Giới hạn mỗi người dùng"
                    // SỬA: Đổi help -> extra
                    extra="Để trống nếu không giới hạn"
                    dependencies={['usageLimit']}
                    rules={[
                        { type: 'number', min: 1, message: 'Số lượng phải lớn hơn 0' },
                        ({ getFieldValue }) => ({
                            validator(_, value) {
                                const usageLimit = getFieldValue('usageLimit');
                                if (value && usageLimit && value > usageLimit) {
                                    return Promise.reject(new Error('Không được lớn hơn tổng số lượng voucher'));
                                }
                                return Promise.resolve();
                            },
                        }),
                    ]}
                >
                     <InputNumber style={{ width: '100%' }} placeholder="∞" />
                </Form.Item>
            </Col>
        </Row>

      </Form>
    </Modal>
  );
};

export default VoucherForm;