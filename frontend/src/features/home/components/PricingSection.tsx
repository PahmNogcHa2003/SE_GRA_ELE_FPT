// src/features/home/components/PricingSection.tsx
import React from 'react';
import PricingCard from './PricingCard'; // Component card giá

const pricingData = [
  {
    title: 'Vé lượt',
    price: '20.000đ',
    period: '/lượt',
    features: [
      'Thời lượng 40 phút',
      'Mỗi 6 phút tiếp theo: 2.000đ',
      'Chi phí quá thời lượng 1.000đ/1 phút',
      'Chi phí hỗ trợ đỗ xe không đúng trạm: 20.000đ',
    ],
  },
  {
    title: 'Vé ngày',
    price: '50.000đ',
    period: '/ngày',
    features: [
      'Thời lượng 450 phút',
      'Trong 24 giờ kể từ lúc đăng ký',
      'Chi phí quá thời lượng 1.000đ/1 phút',
    ],
    isPopular: true, // Đánh dấu gói phổ biến
  },
  {
    title: 'Vé tháng',
    price: '100.000đ',
    period: '/tháng',
    features: [
      'Thời lượng 450 phút/ngày',
      'Sử dụng 30 ngày kể từ lúc đăng ký',
      'Chi phí quá thời lượng 1.000đ/1 phút',
      'Lưu ý: Vé chỉ có giá trị tối thiểu trong 15 ngày.',
    ],
  },
];

const PricingSection: React.FC = () => {
  return (
    <section className="py-24 bg-gray-50">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <h2 className="text-4xl font-bold text-gray-800 text-center mb-16">
          BẢNG GIÁ
        </h2>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8 max-w-5xl mx-auto">
          {pricingData.map((plan, index) => (
            <PricingCard 
              key={index} 
              {...plan}
            />
          ))}
        </div>
      </div>
    </section>
  );
};

export default PricingSection;