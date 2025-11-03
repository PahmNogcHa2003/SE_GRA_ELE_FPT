// src/components/news/NewsCard.tsx
import React from 'react';
import { Tag } from 'antd';
import { Link } from 'react-router-dom';
import dayjs from 'dayjs';
import type { NewsDTO } from '../../types/news';

type NewsCardProps = {
  news: NewsDTO;
};

const NewsCard: React.FC<NewsCardProps> = ({ news }) => {
  const date = news.publishedAt || news.createdAt;
  const dateText = date ? dayjs(date).format('DD/MM/YYYY') : '';

  return (
    <Link
      to={`/news/${news.id}`}
      className="group bg-white rounded-2xl shadow-sm border border-gray-100 
                 overflow-hidden flex flex-col h-full w-full 
                 hover:-translate-y-1 hover:shadow-lg transition-all duration-200"
    >
      {/* ẢNH */}
      <div className="relative h-44 md:h-52 overflow-hidden shrink-0">
        {news.banner ? (
          <img
            src={news.banner}
            alt={news.title}
            className="h-full w-full object-cover group-hover:scale-105 transition-transform duration-300"
          />
        ) : (
          <div className="h-full w-full bg-gradient-to-br from-emerald-500 to-lime-400 flex items-center justify-center text-white text-lg font-semibold">
            Eco Journey
          </div>
        )}

        {dateText && (
          <span className="absolute bottom-3 left-3 bg-white/90 text-gray-800 text-xs font-medium px-3 py-1 rounded-full shadow-sm">
            {dateText}
          </span>
        )}
      </div>

      {/* NỘI DUNG */}
      <div className="flex flex-col flex-1 p-4">
        {/* Tags */}
        <div className="flex flex-wrap gap-1 mb-2">
          {news.tagNames?.slice(0, 3).map((tag) => (
            <Tag
              key={tag}
              color="gold"
              className="text-[11px] font-medium px-2 py-0.5 rounded-full"
            >
              {tag}
            </Tag>
          ))}
        </div>

        {/* Tiêu đề – cố định chiều cao để card bằng nhau */}
        <h3
          className="text-base md:text-lg font-semibold text-gray-900 
                     line-clamp-2 group-hover:text-emerald-600 transition-colors 
                     mb-2 min-h-[3rem]"
          title={news.title}
        >
          {news.title}
        </h3>

        {/* Mô tả ngắn – cũng đặt min-height */}
        <p
          className="text-sm text-gray-600 line-clamp-3 mb-3 flex-1 min-h-[4.5rem]"
          dangerouslySetInnerHTML={{
            __html:
              news.content && news.content.length > 160
                ? news.content.slice(0, 160) + '...'
                : news.content || '',
          }}
        />

        {/* Footer */}
        <div className="mt-auto flex justify-between items-center pt-2">
          <span className="text-xs text-gray-500">
            {dateText && `Ngày đăng: ${dateText}`}
          </span>
          <span className="text-sm font-medium text-emerald-600 group-hover:underline">
            Đọc thêm →
          </span>
        </div>
      </div>
    </Link>
  );
};
export default NewsCard;
