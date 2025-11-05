// src/layouts/UserLayout.tsx
import React from 'react';
import { Outlet } from 'react-router-dom'; // Giả sử bạn đang dùng react-router-dom
import Navbar from '../components/common/Navbar';
import Footer from '../components/common/Footer';

const UserLayout: React.FC = () => {
  return (
    <div className="flex flex-col min-h-screen">
      <Navbar />
      <main className="grow">
        {/* <Outlet /> sẽ render page component (vd: HomePage) */}
        <Outlet /> 
      </main>
      <Footer />
    </div>
  );
};

export default UserLayout;