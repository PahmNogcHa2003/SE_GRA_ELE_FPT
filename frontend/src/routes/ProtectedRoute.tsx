// src/routes/ProtectedRoute.tsx
import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { Spin } from 'antd';
import { useAuth } from '../features/auth/context/authContext';

type Props = { allowRoles?: string[] };

const ProtectedRoute: React.FC<Props> = ({ allowRoles }) => {
  const { isLoggedIn, isLoadingUser, openLoginModal, hasRole } = useAuth();
  const location = useLocation();

  if (isLoadingUser) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Spin size="large" />
      </div>
    );
  }

  // Chưa đăng nhập
  if (!isLoggedIn) {
    setTimeout(() => openLoginModal(), 0);
    return <Navigate to="/" replace state={{ authRequired: true, from: location.pathname }} />;
  }

  // Sai quyền
  if (allowRoles && !hasRole(...allowRoles)) {
    return <Navigate to="/" replace state={{ forbidden: true, from: location.pathname }} />;
  }

  return <Outlet />;
};

export default ProtectedRoute;
