// src/features/quest/components/QuestForm.tsx
import React, { useEffect } from 'react';
import { 
  Modal, Form, Input, InputNumber, Select, DatePicker, Row, Col 
} from 'antd';
import dayjs from 'dayjs';
import type { QuestDTO, QuestCreateDTO } from '../../../types/manage.quest';

const { Option } = Select;
const { RangePicker } = DatePicker;
const { TextArea } = Input;

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (values: QuestCreateDTO) => void;
  initialData?: QuestDTO | null;
  isLoading: boolean;
}

const QuestFormModal: React.FC<Props> = ({ open, onClose, onSubmit, initialData, isLoading }) => {
  const [form] = Form.useForm();
  
  // Watch QuestType để hiển thị field nhập liệu tương ứng
  const questType = Form.useWatch('questType', form);

  useEffect(() => {
    if (open) {
      if (initialData) {
        // Mode Edit
        form.setFieldsValue({
          ...initialData,
          dateRange: [dayjs(initialData.startAt), dayjs(initialData.endAt)],
        });
      } else {
        // Mode Create
        form.resetFields();
        form.setFieldsValue({
          questType: 'Distance',
          scope: 'Weekly',
          promoReward: 0,
        });
      }
    }
  }, [open, initialData, form]);

  const handleFinish = (values: any) => {
    const submitData: QuestCreateDTO = {
      code: values.code,
      title: values.title,
      description: values.description,
      questType: values.questType,
      scope: values.scope,
      promoReward: values.promoReward,
      startAt: values.dateRange[0].toISOString(),
      endAt: values.dateRange[1].toISOString(),
      // Chỉ lấy target tương ứng
      targetDistanceKm: values.questType === 'Distance' ? values.targetDistanceKm : undefined,
      targetTrips: values.questType === 'Trips' ? values.targetTrips : undefined,
      targetDurationMinutes: values.questType === 'Duration' ? values.targetDurationMinutes : undefined,
    };
    onSubmit(submitData);
  };

  return (
    <Modal
      title={initialData ? `Cập nhật Quest: ${initialData.code}` : "Tạo Quest mới"}
      open={open}
      onCancel={onClose}
      onOk={() => form.submit()} // Kích hoạt submit form khi ấn nút OK của Modal
      confirmLoading={isLoading} // Hiệu ứng loading trên nút OK
      width={700} // Độ rộng hợp lý cho form 2 cột
      okText={initialData ? 'Cập nhật' : 'Tạo mới'}
      cancelText="Hủy bỏ"
      centered // Thuộc tính quan trọng để Modal nằm chính giữa màn hình dọc/ngang
    >
      <Form form={form} layout="vertical" onFinish={handleFinish} className="pt-4">
        
        {/* Block 1: Thông tin cơ bản */}
        <div className="bg-gray-50 p-4 rounded-md mb-4 border border-gray-100">
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item 
                name="code" 
                label="Mã Quest (Code)" 
                rules={[{ required: true, message: 'Nhập mã unique' }]}
              >
                <Input placeholder="Vd: Q_WEEKLY_01" disabled={!!initialData} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item 
                name="scope" 
                label="Phạm vi (Scope)"
                rules={[{ required: true }]}
              >
                <Select>
                  <Option value="Daily">Hàng ngày (Daily)</Option>
                  <Option value="Weekly">Hàng tuần (Weekly)</Option>
                  <Option value="Monthly">Hàng tháng (Monthly)</Option>
                </Select>
              </Form.Item>
            </Col>
          </Row>

          <Form.Item 
            name="title" 
            label="Tên hiển thị" 
            rules={[{ required: true, message: 'Nhập tên quest' }]}
          >
            <Input placeholder="Vd: Đua xe cuối tuần" />
          </Form.Item>

          <Form.Item name="description" label="Mô tả">
            <TextArea rows={2} placeholder="Mô tả chi tiết nhiệm vụ..." />
          </Form.Item>
        </div>

        {/* Block 2: Cấu hình mục tiêu */}
        <div className="bg-blue-50 p-4 rounded-md mb-4 border border-blue-100">
          <h4 className="text-blue-700 font-semibold mb-3">🎯 Mục tiêu & Loại hình</h4>
          
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="questType" label="Loại nhiệm vụ" rules={[{ required: true }]}>
                <Select onChange={() => {
                  form.setFieldsValue({ targetDistanceKm: null, targetTrips: null, targetDurationMinutes: null });
                }}>
                  <Option value="Distance">Quãng đường (Distance)</Option>
                  <Option value="Trips">Số chuyến đi (Trips)</Option>
                  <Option value="Duration">Thời gian lái (Duration)</Option>
                </Select>
              </Form.Item>
            </Col>

            <Col span={12}>
              {questType === 'Distance' && (
                <Form.Item 
                  name="targetDistanceKm" 
                  label="Quãng đường (Km)" 
                  rules={[{ required: true, message: 'Nhập số Km' }]}
                >
                  <InputNumber style={{ width: '100%' }} min={0.1} step={0.1} suffix="km" />
                </Form.Item>
              )}

              {questType === 'Trips' && (
                <Form.Item 
                  name="targetTrips" 
                  label="Số chuyến đi" 
                  rules={[{ required: true, message: 'Nhập số chuyến' }]}
                >
                  <InputNumber style={{ width: '100%' }} min={1} step={1} suffix="chuyến" />
                </Form.Item>
              )}

              {questType === 'Duration' && (
                <Form.Item 
                  name="targetDurationMinutes" 
                  label="Thời gian lái" 
                  rules={[{ required: true, message: 'Nhập số phút' }]}
                >
                  <InputNumber style={{ width: '100%' }} min={1} suffix="phút" />
                </Form.Item>
              )}
            </Col>
          </Row>
        </div>

        {/* Block 3: Thời gian & Thưởng */}
        <Row gutter={16}>
          <Col span={12}>
             <Form.Item 
                name="dateRange" 
                label="Thời gian áp dụng" 
                rules={[{ required: true, message: 'Chọn thời gian' }]}
              >
                <RangePicker showTime format="DD/MM/YYYY HH:mm" style={{ width: '100%' }} />
              </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item 
              name="promoReward" 
              label="Điểm thưởng" 
              rules={[{ required: true, message: 'Nhập thưởng' }]}
            >
              <InputNumber 
                style={{ width: '100%' }} 
                formatter={value => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                min={0}
                addonAfter="Pts"
              />
            </Form.Item>
          </Col>
        </Row>
      </Form>
    </Modal>
  );
};

export default QuestFormModal;