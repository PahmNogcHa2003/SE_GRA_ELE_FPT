// src/features/home/components/HowToUseModern.tsx
import React from 'react';
import { motion } from 'framer-motion';
import { FaMapMarkerAlt, FaQrcode, FaBiking, FaCheckDouble } from 'react-icons/fa';

const steps = [
  {
    icon: FaMapMarkerAlt,
    title: 'Tìm trạm gần nhất',
    desc: 'Mở ứng dụng EcoJourney để xác định trạm xe gần vị trí của bạn nhất.',
    color: 'bg-blue-100 text-blue-600'
  },
  {
    icon: FaQrcode,
    title: 'Quét mã mở khóa',
    desc: 'Quét mã QR trên thân xe hoặc nhập ID để mở khóa trong 1 giây.',
    color: 'bg-purple-100 text-purple-600'
  },
  {
    icon: FaBiking,
    title: 'Tận hưởng chuyến đi',
    desc: 'Di chuyển tự do, an toàn. Bạn có thể khóa xe tạm thời bất cứ lúc nào.',
    color: 'bg-orange-100 text-orange-600'
  },
  {
    icon: FaCheckDouble,
    title: 'Trả xe tại trạm',
    desc: 'Đưa xe vào bến bất kỳ và xác nhận trả xe trên ứng dụng để hoàn tất.',
    color: 'bg-green-100 text-green-600'
  }
];

const HowToUseModern: React.FC = () => {
  return (
    <section className="py-24 bg-white">
      <div className="container mx-auto px-4">
        <div className="text-center mb-16">
          <h2 className="text-4xl font-bold text-gray-900">Bắt đầu dễ dàng</h2>
          <p className="text-gray-500 mt-4">Chỉ với 4 bước đơn giản để tham gia giao thông xanh</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8 relative">
          {/* Connecting Line (Desktop only) */}
          <div className="hidden lg:block absolute top-12 left-[10%] right-[10%] h-0.5 bg-gray-200 z-0 border-t-2 border-dashed border-gray-300" />

          {steps.map((step, index) => (
            <motion.div 
              key={index}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.2 }}
              className="relative bg-white p-6 rounded-2xl shadow-sm hover:shadow-lg transition-all text-center z-10 group"
            >
              <div className={`w-20 h-20 mx-auto rounded-2xl ${step.color} flex items-center justify-center mb-6 text-3xl shadow-inner group-hover:scale-110 transition-transform duration-300`}>
                <step.icon />
              </div>
              <h3 className="text-xl font-bold text-gray-800 mb-3">{step.title}</h3>
              <p className="text-gray-500 text-sm leading-relaxed">{step.desc}</p>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default HowToUseModern;