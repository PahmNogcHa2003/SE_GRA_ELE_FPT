import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { Spin } from 'antd';
import { useAuth } from '../features/auth/context/authContext';

const AdminGuard: React.FC = () => {
  const { isLoggedIn, isLoadingUser, hasRole } = useAuth();

  // 1. Đang load thông tin user -> Hiện loading
  if (isLoadingUser) {
    return (
      <div style={{ height: '100vh', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
        <Spin size="large" tip="Đang kiểm tra quyền truy cập..." />
      </div>
    );
  }

  // 2. Chưa đăng nhập -> Đá về trang Login Admin
  if (!isLoggedIn) {
    return <Navigate to="/admin/login" replace />;
  }

  // 3. Đã đăng nhập nhưng KHÔNG phải Admin/Staff -> Đá về Trang chủ User
  if (!hasRole('Admin', 'Staff')) {
    return <Navigate to="/" replace />;
  }

  // 4. Hợp lệ -> Cho vào (Render các route con)
  return <Outlet />;
};

export default AdminGuard;