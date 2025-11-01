// src/features/home/components/DownloadAppSection.tsx
import React from 'react';
import AppMockup from '../../../assets/images/app-mockup.png';
import AppMockup2 from '../../../assets/images/app-mockup2.png';
// import QrCode from '../../../assets/qr-code.png';
import GooglePlay from '../../../assets/images/google-play.webp';

const DownloadAppSection: React.FC = () => {
  return (
    <section className="py-24 bg-gray-50">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <h2 className="text-4xl font-bold text-gray-800 text-center mb-12">
          ĐĂNG NHẬP TRÊN ỨNG DỤNG<br/>ĐỂ SỬ DỤNG CÁC DỊCH VỤ
        </h2>
        
        <div className="flex flex-col md:flex-row items-center justify-center gap-16">
          <div className="shrink-0">
            <div className="flex gap-4 justify-center">
              <div className="w-48 h-100 bg-gray-200 rounded-2xl border-4 border-gray-700 overflow-hidden flex items-center justify-center">
                <img
                  src={AppMockup2}
                  alt="Eco Journey App"
                  className="h-full w-full object-cover"
                />
              </div>
              <div className="w-48 h-100 bg-gray-200 rounded-2xl border-4 border-gray-700 overflow-hidden flex items-center justify-center">
                <img
                  src={AppMockup}
                  alt="Eco Journey App"
                  className="h-full w-full object-cover"
                />
              </div>
         </div>
        </div>       
          {/* Mã QR và nút tải */}
          <div className="text-center">
            {/* <img src={QrCode} alt="QR Code" className="w-64 h-64 mb-6 mx-auto border-4 border-gray-300 rounded-lg" /> */}
            <div className="w-64 h-64 bg-gray-200 mb-6 mx-auto border-4 border-gray-300 rounded-lg flex items-center justify-center">
              <span className="text-gray-500">(Placeholder: QR Code)</span>
            </div>
            
           <a href="#" className="inline-block">
              <div className="h-16 w-64 rounded-lg overflow-hidden">
                <img
                  src={GooglePlay}
                  alt="Get it on Google Play"
                  className="h-full w-full object-cover"
                />
             </div>
            </a>
          </div>
          
        </div>
      </div>
    </section>
  );
};

export default DownloadAppSection;