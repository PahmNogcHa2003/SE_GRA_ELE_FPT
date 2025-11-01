// src/features/vehicles/VehicleForm.tsx

import React, { useEffect, useState } from 'react'; // THÊM MỚI: useState
import { Form, Input, Select, Button, InputNumber, Switch } from 'antd';
import { useQuery } from '@tanstack/react-query';
import * as categoryVehicleService from '../../services/categoryVehicle.service';
import * as stationService from '../../services/station.service';
import type { VehicleDTO } from '../../types/vehicle';
import type { CategoryVehicleDTO } from '../../types/categoryVehicle'; // THÊM MỚI: Import type

const { Option } = Select;

interface VehicleFormProps {
  initialValues?: VehicleDTO | null;
  onSubmit: (values: Omit<VehicleDTO, 'id' | 'createdAt'>) => void;
  onCancel: () => void;
  isLoading: boolean;
}

const VehicleForm: React.FC<VehicleFormProps> = ({ initialValues, onSubmit, onCancel, isLoading }) => {
  const [form] = Form.useForm();

  // THÊM MỚI: State để lưu tên của loại xe đang được chọn
  const [selectedCategoryName, setSelectedCategoryName] = useState<string | null>(null);

  const { data: categoriesData, isLoading: isLoadingCategories } = useQuery({
    queryKey: ['allCategoriesVehicle'],
    queryFn: () => categoryVehicleService.getCategories({ pageSize: 100 }),
    select: (res) => res.data?.data?.items,
  });

  const { data: stationsData, isLoading: isLoadingStations } = useQuery({
    queryKey: ['allStations'],
    queryFn: () => stationService.getStations({ pageSize: 100 }),
    select: (res) => res.data?.items, // Sửa lại đường dẫn cho nhất quán
  });

  // THAY ĐỔI: Cập nhật useEffect để xử lý trạng thái ban đầu khi chỉnh sửa
  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue(initialValues);
      // Tìm và set tên category ban đầu khi form được mở ở chế độ edit
      if (categoriesData && initialValues.categoryId) {
        const initialCategory = categoriesData.find(cat => cat.id === initialValues.categoryId);
        setSelectedCategoryName(initialCategory ? initialCategory.name : null);
      }
    } else {
      // Reset khi tạo mới
      form.resetFields();
      setSelectedCategoryName(null);
    }
  }, [initialValues, form, categoriesData]);

  // THÊM MỚI: Hàm xử lý khi người dùng thay đổi loại xe
  const handleCategoryChange = (value: number) => {
    const selectedCategory = categoriesData?.find((cat: CategoryVehicleDTO) => cat.id === value);
    const categoryName = selectedCategory ? selectedCategory.name : null;
    setSelectedCategoryName(categoryName);

    // Nếu loại xe không phải là xe điện, reset các trường liên quan
    if (!categoryName?.toLowerCase().includes('điện')) {
      form.setFieldsValue({
        batteryLevel: null,
        chargingStatus: false,
      });
    }
  };
  
  // THÊM MỚI: Điều kiện để quyết định có hiển thị các trường của xe điện không
  const shouldShowElectricFields = selectedCategoryName?.toLowerCase().includes('điện');

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={onSubmit}
      initialValues={{ status: 'Available', chargingStatus: false }}
    >
      <Form.Item name="bikeCode" label="Mã xe" rules={[{ required: true, message: 'Vui lòng nhập mã xe!' }]}>
        <Input placeholder="Ví dụ: XE0001" />
      </Form.Item>
      {/* THAY ĐỔI: Thêm sự kiện onChange cho Select loại xe */}
      <Form.Item name="categoryId" label="Loại xe" rules={[{ required: true, message: 'Vui lòng chọn loại xe!' }]}>
        <Select placeholder="Chọn một loại xe" loading={isLoadingCategories} onChange={handleCategoryChange}>
          {categoriesData?.map((cat: CategoryVehicleDTO) => (
            <Option key={cat.id} value={cat.id}>{cat.name}</Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item name="stationId" label="Trạm hiện tại (tùy chọn)">
        <Select placeholder="Chọn trạm xe" loading={isLoadingStations} allowClear>
          {stationsData?.map((st: { id: number | string; name: string }) => (
            <Option key={st.id} value={st.id}>{st.name}</Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item name="status" label="Trạng thái" rules={[{ required: true, message: 'Vui lòng chọn trạng thái!' }]}>
        <Select>
          <Option value="Available">Sẵn sàng</Option>
          <Option value="InUse">Đang sử dụng</Option>
          <Option value="Maintenance">Bảo trì</Option>
          <Option value="Unavailable">Không khả dụng</Option>
        </Select>
      </Form.Item>

      {/* THAY ĐỔI: Dùng điều kiện để ẩn/hiện các trường */}
      {shouldShowElectricFields && (
        <>
          <Form.Item name="batteryLevel" label="Mức pin (%)">
            <InputNumber min={0} max={100} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="chargingStatus" label="Trạng thái sạc" valuePropName="checked">
            <Switch />
          </Form.Item>
        </>
      )}

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

export default VehicleForm;