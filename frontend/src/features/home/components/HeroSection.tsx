// src/features/home/components/HeroSection.tsx
import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import Slider from 'react-slick';

// Import CSS cho thư viện react-slick
import "slick-carousel/slick/slick.css"; 
import "slick-carousel/slick/slick-theme.css";

// Import kiểu dữ liệu News
import type { NewsDTO } from '../../../types/news';
// Import hàm gọi API tin tức (bạn cần tạo hàm này)
// import { getNews } from '../../../services/newsService';

// ----- BẮT ĐẦU GIẢ LẬP API (XÓA KHI CÓ API THẬT) -----
// Giả lập dữ liệu trả về từ API
const mockNewsData: NewsDTO[] = [
   {
    id: 1,
    title: 'Eco Journey Ra Mắt Dịch Vụ Mới Tại Hà Nội',
    slug: 'eco-journey-ra-mat-dich-vu-moi',
    bannerImage: 'https://images.unsplash.com/photo-1517649763962-0c6f41b5a3b2?q=80&w=2070&auto=format&fit=crop',
    content: 'Nội dung demo...',
    authorId: 101,
    createdAt: '2025-10-21T12:00:00Z',
    isActive: true,
    tagIds: [1, 3],
    tags: [{ id: 1, name: 'Tips' }, { id: 3, name: 'Eco' }],
  },
  {
    id: 2,
    title: 'Hướng Dẫn Sử Dụng Xe Đạp Điện An Toàn',
    slug: 'huong-dan-su-dung-xe-dap-dien',
    bannerImage: 'https://images.unsplash.com/photo-1507149833265-60c370da57a4?q=80&w=2070&auto=format&fit=crop',
    content: 'Nội dung demo...',
    authorId: 102,
    createdAt: '2025-10-20T09:30:00Z',
    isActive: true,
    tagIds: [2],
    tags: [{ id: 2, name: 'Guide' }],
  },
  {
    id: 3,
    title: 'Top 5 Lợi Ích Của Việc Đi Xe Đạp Hàng Ngày',
    slug: 'top-5-loi-ich-cua-viec-di-xe-dap',
    bannerImage: 'https://images.unsplash.com/photo-1440557653082-e8e18673eeb?auto=format&fit=crop',
    content: 'Nội dung demo...',
    authorId: 101,
    createdAt: '2025-10-18T08:15:00Z',
    isActive: false,
    tagIds: [1, 4],
    tags: [{ id: 4, name: 'Health' }],
  },
];

// Giả lập hàm gọi API, mất 1 giây để trả về dữ liệu
const fetchMockNews = (): Promise<NewsDTO[]> => {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve(mockNewsData);
    }, 1000);
  });
};
// ----- KẾT THÚC GIẢ LẬP API -----


const HeroSection: React.FC = () => {
  // Cấu hình cho slider
  const sliderSettings = {
    dots: true, // Hiển thị dấu chấm điều hướng
    infinite: true, // Vòng lặp vô hạn
    speed: 500, // Tốc độ chuyển slide (ms)
    slidesToShow: 1, // Hiển thị 1 slide mỗi lần
    slidesToScroll: 1, // Cuộn 1 slide mỗi lần
    autoplay: true, // Tự động chạy
    autoplaySpeed: 4000, // Thời gian chờ 4 giây
    pauseOnHover: true, // Tạm dừng khi rê chuột vào
    arrows: false, // Ẩn mũi tên (để đơn giản và đẹp)
    dotsClass: "slick-dots custom-dots", // Thêm lớp CSS tùy chỉnh cho dấu chấm
  };

  // Sử dụng useQuery để lấy dữ liệu tin tức
  // THAY THẾ: `fetchMockNews` bằng hàm API thật của bạn
  // Ví dụ: queryFn: () => getNews({ page: 1, limit: 3 })
  const { data: newsList, isLoading, isError } = useQuery({
    queryKey: ['heroNews'], // Key để cache dữ liệu
    queryFn: fetchMockNews, 
    // Nếu API thật trả về cấu trúc { data: [...] }, dùng `select` để trích xuất:
    // select: (apiResponse) => apiResponse.data, 
  });

  // Trạng thái Đang tải...
  if (isLoading) {
    return (
      <div className="w-full h-[60vh] bg-gray-200 flex items-center justify-center">
        {/* Bạn có thể dùng spinner của Antd hoặc thư viện khác */}
        <span className="text-gray-500 text-lg">Đang tải banner...</span>
      </div>
    );
  }

  // Trạng thái Lỗi
  if (isError) {
    return (
      <div className="w-full h-[60vh] bg-red-100 flex items-center justify-center">
        <span className="text-red-600 text-lg">Không thể tải tin tức</span>
      </div>
    );
  }

  // Trạng thái có dữ liệu
  return (
    <div className="relative w-full h-[60vh] overflow-hidden group">
      <Slider {...sliderSettings}>
        {newsList?.map((news) => (
          // Mỗi slide là một thẻ Link
          <Link to={`/news/${news.slug}`} key={news.id} className="block w-full h-[60vh] relative outline-none">
            {/* Ảnh banner */}
            <img 
              src={news.bannerImage} 
              alt={news.title} 
              className="w-full h-full object-cover transition-transform duration-500 ease-in-out group-hover:scale-110" 
            />
            {/* Lớp phủ mờ để chữ dễ đọc hơn */}
            <div className="absolute inset-0 bg-linear-to-t from-black/60 via-black/30 to-transparent"></div>
            
            {/* Tiêu đề tin tức */}
            <div className="absolute bottom-0 left-0 p-8 md:p-12 text-white max-w-4xl">
              <h1 className="text-3xl md:text-5xl font-bold mb-4 line-clamp-2 transition-transform duration-300 group-hover:-translate-y-2">
                {news.title}
              </h1>
              {/* Bạn có thể thêm tóm tắt ở đây nếu muốn */}
              {/* <p className="hidden md:block text-lg line-clamp-2">{news.summary}</p> */}
            </div>
          </Link>
        ))}
      </Slider>
    </div>
  );
};

export default HeroSection;