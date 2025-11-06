// src/components/common/Footer.tsx
import React from 'react';
import { FaFacebook, FaTwitter, FaInstagram, FaLinkedin } from 'react-icons/fa';
import Logo from '../../assets/images/logo_white.png'; // Giả sử bạn có logo phiên bản màu trắng

const Footer: React.FC = () => {
  return (
    <footer className="bg-eco-green-dark text-white p-4 rounded-lg">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          
          {/* Cột 1: Logo và Giới thiệu */}
          <div>
            <img src={Logo} alt="Eco Journey" className="h-30 mb-4 mx-auto md:mx-0" />
            <span className="font-bold text-2xl text-white mb-4 block">Eco Journey</span>
            <p className="text-sm">
              Dịch vụ xe đạp công cộng Hola-Bike đem đến một hình thức giao thông đô thị mới văn minh, tiện lợi, tốt cho sức khỏe và thân thiện với môi trường.
            </p>
          </div>

           <div>
      <h5 className="text-lg font-semibold text-white mb-4">KHÁM PHÁ</h5>
      <ul className="space-y-2">
        <li><a href="/about" className="text-gray-300! hover:text-eco-green! transition-colors">Về chúng tôi</a></li>
        <li><a href="/news" className="text-gray-300! hover:text-eco-green! transition-colors">Tin tức</a></li>
        <li><a href="/contact" className="text-gray-300! hover:text-eco-green! transition-colors">Liên hệ</a></li>
      </ul>
    </div>

    {/* Cột 3: THEO DÕI */}
    <div>
      <h5 className="text-lg font-semibold text-white mb-4">THEO DÕI</h5>
      <div className="flex space-x-4">
        <a href="#" className="text-gray-300! hover:text-eco-green! transition-colors"><FaFacebook size={24} /></a>
        <a href="#" className="text-gray-300! hover:text-eco-green! transition-colors"><FaTwitter size={24} /></a>
        <a href="#" className="text-gray-300! hover:text-eco-green! transition-colors"><FaInstagram size={24} /></a>
        <a href="#" className="text-gray-300! hover:text-eco-green! transition-colors"><FaLinkedin size={24} /></a>
      </div>
    </div>

          {/* Cột 4: Liên hệ */}
          <div>
            <h5 className="text-lg font-semibold text-white mb-4">LIÊN HỆ</h5>
            <ul className="space-y-2 text-sm">
              <li>Hotline: (+84) 204 6296 288</li>
              <li>Email: Work.nguyenductan@gmail.com</li>
            </ul>
          </div>
          
        </div>
        
      </div>
      <div className="bg-eco-green text-white p-4 rounded-lg">
        <div className="container mx-auto px-4 sm:px-6 lg:px-8 text-center text-sm text-white-500">
          © {new Date().getFullYear()} - Copyright belongs to Electric Motorbike Bike Bike VietNam Co.,LTD
        </div>
      </div>
    </footer>
  );
};

export default Footer;