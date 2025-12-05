// src/features/home/components/AboutSection.tsx
import React from 'react';
import AboutUsImage from '../../../assets/images/about_us.png';

const AboutSection: React.FC = () => {
  return (
    <section className="py-24 bg-white">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex flex-col md:flex-row items-center gap-12">
          
          {/* Cột bên trái: Text */}
          <div className="md:w-1/2">
            <h2 className="text-4xl font-bold text-gray-800 mb-4">
              GIỚI THIỆU VỀ CHÚNG TÔI
            </h2>
            <h3 className="text-3xl font-semibold text-eco-green mb-6">
              ECO JOURNEY
            </h3>
            {/* Thanh gạch ngang */}
            <hr className="border-t-4 border-eco-green w-24 mb-6" /> 
            
            <p className="text-gray-600 leading-relaxed">
              Dịch vụ xe đạp công cộng Hola-Bike đem đến một hình thức giao thông đô thị mới văn minh, tiện lợi, tốt cho sức khỏe và thân thiện với môi trường. Chỉ với vài thao tác đơn giản trên ứng dụng di động, người tham gia giao thông có thể dễ dàng thuê xe, di chuyển và trả xe tại các trạm xe đạp Hola-Bike trong Ký túc xá hoặc Học Lạc... 
              <br/><br/>
              Thạch Thất sẽ giúp kết nối các hệ thống giao thông công cộng khác như xe buýt, tàu điện... giúp việc di chuyển trở nên linh hoạt thuận tiện và thân thiện với môi trường hơn.
            </p>
          </div>
          
          {/* Cột bên phải: Hình ảnh */}
          <div className="md:w-1/2">
            <img 
              src={AboutUsImage} 
              alt="Eco Journey E-Bike" 
              className="w-full h-auto rounded-lg" 
            /> 
          </div>

        </div>
      </div>
    </section>
  );
};

export default AboutSection;