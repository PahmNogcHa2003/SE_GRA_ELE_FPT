// src/features/home/components/HeroSection.tsx
import React from 'react';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';
import HeroBg from '../../../assets/images/about_us.png'; // Hoặc một ảnh background rộng hơn

const HeroSection: React.FC = () => {
  return (
    <div className="relative w-full h-[85vh] overflow-hidden flex items-center justify-center">
      {/* Background Image với Overlay Gradient */}
      <div className="absolute inset-0 z-0">
        <img 
          src={HeroBg} 
          alt="Eco Journey Hero" 
          className="w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-linear-to-r from-green-900/90 via-green-800/70 to-transparent" />
      </div>

      {/* Content chính */}
      <div className="container mx-auto px-4 z-10 grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
        <motion.div 
          initial={{ opacity: 0, x: -50 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.8 }}
          className="text-white space-y-6"
        >
          <div className="inline-block px-4 py-2 bg-white/20 backdrop-blur-md rounded-full border border-white/30 text-sm font-semibold text-green-100 mb-2">
            🌿 Giải pháp di chuyển xanh số 1
          </div>
          <h1 className="text-5xl md:text-7xl font-bold leading-tight">
            Khám phá thành phố <br />
            <span className="text-transparent bg-clip-text bg-linear-to-r from-lime-400 to-green-400">
              Theo cách riêng
            </span>
          </h1>
          <p className="text-lg text-gray-200 md:w-3/4 leading-relaxed">
            Eco Journey mang đến trải nghiệm thuê xe đạp điện thông minh, tiện lợi và thân thiện với môi trường. Chỉ 1 chạm để mở khóa hành trình.
          </p>
          
          <div className="flex flex-wrap gap-4 pt-4">
            <Link to="/stations">
                <button className="px-8 py-4 bg-lime-500 hover:bg-lime-400 text-green-900 font-bold rounded-full shadow-lg shadow-lime-500/30 transition-all transform hover:-translate-y-1">
                Tìm trạm ngay
                </button>
            </Link>
            <button className="px-8 py-4 bg-white/10 hover:bg-white/20 backdrop-blur-md border border-white/30 text-white font-bold rounded-full transition-all">
              Tải ứng dụng
            </button>
          </div>
        </motion.div>

        {/* Phần trang trí bên phải (Ví dụ: Thống kê nhanh hoặc Hình ảnh 3D) */}
        <motion.div 
             initial={{ opacity: 0, y: 50 }}
             animate={{ opacity: 1, y: 0 }}
             transition={{ delay: 0.3, duration: 0.8 }}
             className="hidden lg:block relative"
        >
            {/* Card bay lơ lửng - Glassmorphism */}
            <div className="absolute top-0 right-10 bg-white/10 backdrop-blur-xl border border-white/20 p-6 rounded-3xl shadow-2xl max-w-xs animate-float">
                <div className="flex items-center gap-4 mb-4">
                    <div className="w-12 h-12 rounded-full bg-green-500 flex items-center justify-center text-2xl">🚲</div>
                    <div>
                        <p className="text-gray-300 text-xs">Xe sẵn sàng</p>
                        <p className="text-white text-2xl font-bold">1,250+</p>
                    </div>
                </div>
                <div className="h-2 bg-gray-600 rounded-full overflow-hidden">
                    <div className="h-full bg-lime-400 w-3/4"></div>
                </div>
            </div>
        </motion.div>
      </div>
      
      {/* Wave shape ở dưới cùng để nối mượt với section sau */}
      <div className="absolute bottom-0 left-0 w-full overflow-hidden leading-0">
        <svg className="relative block w-[calc(100%+1.3px)] h-20" data-name="Layer 1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1200 120" preserveAspectRatio="none">
            <path d="M321.39,56.44c58-10.79,114.16-30.13,172-41.86,82.39-16.72,168.19-17.73,250.45-.39C823.78,31,906.67,72,985.66,92.83c70.05,18.48,146.53,26.09,214.34,3V0H0V27.35A600.21,600.21,0,0,0,321.39,56.44Z" className="fill-white"></path>
        </svg>
      </div>
    </div>
  );
};

export default HeroSection;