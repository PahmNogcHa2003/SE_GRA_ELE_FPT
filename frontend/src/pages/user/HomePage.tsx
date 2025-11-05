// src/pages/user/HomePage.tsx
import React, { useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { useAuth } from '../../features/auth/context/authContext';

import HeroSection from '../../features/home/components/HeroSection';
import AboutSection from '../../features/home/components/AboutSection';
import DownloadAppSection from '../../features/home/components/DownloadAppSection';
import HowToUseSection from '../../features/home/components/HowToUseSection';
import PricingSection from '../../features/home/components/PricingSection';

const HomePage: React.FC = () => {
  const location = useLocation();
  const { openLoginModal } = useAuth();

  /**
   * Nếu được redirect từ ProtectedRoute (authRequired hoặc forbidden)
   * → tự động mở modal đăng nhập
   */
  useEffect(() => {
    const state = location.state as any;
    if (state?.authRequired || state?.forbidden) {
      openLoginModal();
      // Xóa state để F5 không trigger lại
      window.history.replaceState({}, document.title);
    }
  }, [location.state, openLoginModal]);

  return (
    <div>
      <HeroSection />
      <AboutSection />
      <DownloadAppSection />
      <HowToUseSection />
      <PricingSection />
    </div>
  );
};

export default HomePage;
