// src/pages/user/HomePage.tsx
import React, { useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { useAuth } from '../../features/auth/context/authContext';

// Import các component mới
import HeroSection from '../../features/home/components/HeroSection';
import AboutSection from '../../features/home/components/AboutSection';
import ModernStats from '../../features/home/components/ModernStats';
import HowToUseModern from '../../features/home/components/HowToUseModern';
import DynamicPricingSection from '../../features/home/components/DynamicPricingSection';
import DownloadAppSection from '../../features/home/components/DownloadAppSection';

const HomePage: React.FC = () => {
  const location = useLocation();
  const { openLoginModal } = useAuth();

  // Handle redirect login logic
  useEffect(() => {
    const state = location.state as any;
    if (state?.authRequired || state?.forbidden) {
      openLoginModal();
      window.history.replaceState({}, document.title);
    }
  }, [location.state, openLoginModal]);

  return (
    <div className="font-sans text-gray-800 bg-white">
      <HeroSection />
      <ModernStats />
      <AboutSection />
      <HowToUseModern />
      <DynamicPricingSection />
      <DownloadAppSection />
    </div>
  );
};

export default HomePage;