// src/features/home/components/HeroSection.tsx
import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import Slider from 'react-slick';

import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

import type { NewsDTO } from '../../../types/news';
import { getNews } from '../../../services/news.service';

// Slider config
const sliderSettings = {
  dots: true,
  infinite: true,
  speed: 600,
  slidesToShow: 1,
  slidesToScroll: 1,
  autoplay: true,
  autoplaySpeed: 4500,
  pauseOnHover: true,
  arrows: false,
  dotsClass: "slick-dots custom-dots",
};

// Aspect = banner mỏng (9:4 ≈ 44%)
const HERO_ASPECT: React.CSSProperties = {
  position: "relative",
  width: "100%",
  paddingTop: "44%",
};

const HeroSection: React.FC = () => {
  const { data: newsList } = useQuery({
    queryKey: ['heroNews'],
    queryFn: () => getNews({ pageNumber: 1, pageSize: 5 }),
    select: (res): NewsDTO[] => {
      const paged = res.data.data;
      const items = paged?.items ?? [];

      const published = items.filter(
        (n) =>
          n.status?.toLowerCase() === "published" ||
          n.status?.toLowerCase() === "active"
      );

      return [...published].sort((a, b) => {
        const da = new Date(a.publishedAt || a.createdAt).getTime();
        const db = new Date(b.publishedAt || b.createdAt).getTime();
        return db - da;
      });
    },
  });

  if (!newsList || newsList.length === 0) {
    return (
      <div className="w-full bg-gray-900">
        <div style={HERO_ASPECT} className="flex items-center justify-center">
          <div className="absolute text-white text-center">
            <p className="text-lg">Không có tin tức</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="relative w-full overflow-hidden">
      <Slider {...sliderSettings}>
        {newsList.map((news) => (
          <Link
            key={news.id}
            to={`/news/${news.id}`}
            className="block relative group"
          >
            <div style={HERO_ASPECT}>
              {/* Banner */}
              <img
                src={news.banner || "/images/news-hero.jpg"}
                alt={news.title}
                className="
                  absolute inset-0 w-full h-full object-cover object-center
                  transition-transform duration-1400 ease-out
                  group-hover:scale-[1.06]
                "
              />

              {/* Overlay */}
              <div className="absolute inset-0 bg-linear-to-t from-black/60 via-black/30 to-transparent" />

              {/* Text */}
              <div className="absolute bottom-0 left-0 px-6 md:px-12 pb-8 text-white">
                <p className="tracking-[0.35em] text-[10px] md:text-xs uppercase opacity-80">
                  Tin tức mới nhất
                </p>
                <h1 className="text-xl md:text-3xl font-bold line-clamp-2 transition-transform duration-500 group-hover:-translate-y-1">
                  {news.title}
                </h1>
              </div>
            </div>
          </Link>
        ))}
      </Slider>
    </div>
  );
};

export default HeroSection;
