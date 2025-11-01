// src/features/home/components/HowToUseSection.tsx
import React from 'react';
import { FaMapMarkedAlt, FaQrcode, FaBicycle, FaCreditCard } from 'react-icons/fa';

const steps = [
  {
    icon: <FaMapMarkedAlt size={40} className="text-eco-green" />,
    title: 'Chọn điểm xe và thanh toán',
    description: 'Chọn địa điểm trạm trên map và chọn gói thuê xe.',
  },
  {
    icon: <FaQrcode size={40} className="text-eco-green" />,
    title: 'Mở khóa',
    description: 'Chạm vào nút "Mở khóa" và quét mã QR trên xe hoặc nhập mã số xe để mở khóa.',
  },
  {
    icon: <FaBicycle size={40} className="text-eco-green" />,
    title: 'Đi xe',
    description: 'Tận hưởng chuyến đi, nên đội mũ bảo hiểm và tuân thủ luật giao thông. Dừng xe và khóa tạm thời.',
  },
  {
    icon: <FaCreditCard size={40} className="text-eco-green" />, // Icon cho Trả xe
    title: 'Trả xe',
    description: 'Trả xe đúng vào trạm Hola Bike bất kỳ. Thực hiện các thao tác trả xe để kết thúc chuyến đi.',
  },
];

const HowToUseSection: React.FC = () => {
  return (
    <section className="py-24 bg-white">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <h2 className="text-4xl font-bold text-gray-800 text-center mb-16">
          CÁCH SỬ DỤNG
        </h2>
        
        {/* Chúng ta sẽ dùng grid layout cho đẹp hơn là timeline */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-10">
          {steps.map((step, index) => (
            <div key={index} className="text-center p-6 bg-gray-50 rounded-lg shadow-lg hover:shadow-xl transition-shadow">
              <div className="flex justify-center mb-6">
                {step.icon}
              </div>
              <h3 className="text-xl font-semibold text-gray-800 mb-3">{step.title}</h3>
              <p className="text-gray-600 text-sm">{step.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default HowToUseSection;