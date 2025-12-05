// src/layouts/StaffLayout.tsx
import React from 'react';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { Layout, Menu, Dropdown, Avatar, Tag, App as AntdApp } from 'antd';
import { HomeOutlined, CarOutlined, UserOutlined, LogoutOutlined, FileTextOutlined, DollarCircleOutlined } from '@ant-design/icons';
import { useAuth } from '../features/auth/context/authContext';
import { FaTicketAlt } from 'react-icons/fa';

const { Header, Content, Sider } = Layout;

const menuItems = [
  { key: '/staff/dashboard', icon: <HomeOutlined />, label: <Link to="/staff/dashboard">Dashboard</Link> },
  { key: '/staff/stations', icon: <CarOutlined />, label: <Link to="/staff/stations">Quản lý trạm</Link> },
  { key: '/staff/vehicles', icon: <CarOutlined />, label: <Link to="/staff/vehicles">Quản lý xe</Link> },
  { key: '/staff/categories-vehicle', icon: <CarOutlined />, label: <Link to="/staff/categories-vehicle">Quản lý loại xe</Link> },
  { key: '/staff/tags', icon: <CarOutlined />, label: <Link to="/staff/tags">Quản lý thẻ (tin tức)</Link> },
  { key: '/staff/news', icon: <CarOutlined />, label: <Link to="/staff/news">Quản lý tin tức</Link> },
  { key: '/staff/ticket-plans', icon: <FileTextOutlined />, label: <Link to="/staff/ticket-plans">Quản lý Gói Vé</Link> },
  { key: '/staff/ticket-plan-prices', icon: <DollarCircleOutlined />, label: <Link to="/staff/ticket-plan-prices">Quản lý Giá Vé</Link> },
  { key: '/staff/user-tickets', icon: <FaTicketAlt />, label: <Link to="/staff/user-tickets">Quản lý Vé của người dùng</Link> },
  // Bạn có thể thêm item riêng cho liên hệ & vé nếu đã có route cụ thể:
  // { key: '/staff/contacts', icon: <CarOutlined />, label: <Link to="/staff/contacts">Quản lý liên hệ/phản hồi</Link> },
  // { key: '/staff/tickets', icon: <CarOutlined />, label: <Link to="/staff/tickets">Quản lý vé</Link> },
];

const StaffLayout: React.FC = () => {
  const location = useLocation();
  const { user, logout } = useAuth();
  const { modal, notification } = AntdApp.useApp();

  const showLogoutConfirm = () => {
    modal.confirm({
      title: 'Đăng xuất quản trị?',
      content: 'Bạn sẽ cần đăng nhập lại để tiếp tục.',
      okText: 'Đăng xuất',
      okType: 'danger',
      cancelText: 'Hủy',
      centered: true,
      onOk: () => {
        logout();
        notification.success({ message: 'Đã đăng xuất' });
        // về Home và không cho Back quay lại
        window.location.replace('/');
      },
    });
  };

  const staffMenu = {
    items: [
      {
        key: 'profile',
        icon: <UserOutlined />,
        label: <Link to="/staff/profile">Hồ sơ quản trị</Link>,
      },
      { type: 'divider' as const },
      {
        key: 'logout',
        danger: true,
        icon: <LogoutOutlined />,
        label: 'Đăng xuất',
        onClick: showLogoutConfirm,
      },
    ],
  };

  const primaryRole =
    user?.roles?.find(r => r === 'Admin') ||
    user?.roles?.find(r => r === 'Staff') ||
    user?.roles?.[0];

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider collapsible>
        <div style={{ height: 48, margin: 16, color: '#fff', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <strong>Eco Admin</strong>
        </div>
        <Menu theme="dark" selectedKeys={[location.pathname]} mode="inline" items={menuItems} />
      </Sider>

      <Layout>
        <Header
  style={{
    padding: '0 16px',
    background: '#fff',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
  }}
>
  {/* Thông tin admin */}
  <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
    {primaryRole && (
      <Tag color={primaryRole === 'Admin' ? 'red' : 'blue'}>
        {primaryRole}
      </Tag>
    )}
    <Dropdown menu={staffMenu} placement="bottomRight" arrow trigger={['click']}>
      <button className="flex items-center gap-2 text-gray-700 hover:text-eco-green">
        <Avatar size="default" icon={<UserOutlined />} />
        <span className="font-medium">{user?.fullName || user?.email || 'Tài khoản'}</span>
      </button>
    </Dropdown>
  </div>
</Header>


        <Content style={{ margin: '24px 16px' }}>
          <div style={{ padding: 24, minHeight: 'calc(100vh - 112px)', background: '#fff' }}>
            <Outlet />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default StaffLayout;
