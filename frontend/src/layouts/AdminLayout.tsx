import React from 'react';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { Layout, Menu, Dropdown, Avatar, Tag, App as AntdApp } from 'antd';
import { useAuth } from '../features/auth/context/authContext';
import {
  UserOutlined,
  LogoutOutlined,
  TeamOutlined,
  SolutionOutlined,
  SafetyCertificateOutlined,
  DashboardOutlined
} from '@ant-design/icons';

const { Header, Content, Sider } = Layout;

// Menu chuyên biệt cho Admin (Quản trị hệ thống & Con người)
const adminMenuItems = [
  // 1. Dashboard tổng quan
  { 
    key: '/admin/dashboard', 
    icon: <DashboardOutlined />, 
    label: <Link to="/admin/dashboard">Tổng quan (Admin)</Link> 
  },

  // 2. Quản lý Nhân sự (Staff)
  {
    key: 'sub-staff-manage',
    icon: <SolutionOutlined />,
    label: 'Quản trị Nhân sự',
    children: [
      { 
        key: '/admin/manage-staff', 
        label: <Link to="/admin/manage-staff">Danh sách Nhân viên</Link> 
      },
      { 
        key: '/admin/create-staff', 
        label: <Link to="/admin/create-staff">Tạo nhân viên mới</Link> 
      },
    ],
  },

  // 3. Quản lý Khách hàng (End Users)
  {
    key: 'sub-user-manage',
    icon: <TeamOutlined />,
    label: 'Quản trị Khách hàng',
    children: [
      { 
        key: '/admin/manage-users', 
        label: <Link to="/admin/manage-users">Danh sách Khách hàng</Link> 
      },
      { 
        key: '/admin/user-kyc', 
        label: <Link to="/admin/user-kyc">Duyệt hồ sơ (KYC)</Link> 
      },
    ],
  },

  // 4. Phân quyền & Bảo mật (Tùy chọn)
  {
    key: 'sub-security',
    icon: <SafetyCertificateOutlined />,
    label: 'Bảo mật & Phân quyền',
    children: [
      { 
        key: '/admin/roles', 
        label: <Link to="/admin/roles">Vai trò (Roles)</Link> 
      },
      { 
        key: '/admin/audit-logs', 
        label: <Link to="/admin/audit-logs">Nhật ký hoạt động</Link> 
      },
    ],
  },
];

const AdminLayout: React.FC = () => {
  const location = useLocation();
  const { user, logout } = useAuth();
  const { modal, notification } = AntdApp.useApp();

  const showLogoutConfirm = () => {
    modal.confirm({
      title: 'Đăng xuất Admin?',
      content: 'Bạn sẽ cần đăng nhập lại để tiếp tục.',
      okText: 'Đăng xuất',
      okType: 'danger',
      cancelText: 'Hủy',
      centered: true,
      onOk: () => {
        logout();
        notification.success({ message: 'Đã đăng xuất' });
        window.location.replace('/admin/login');
      },
    });
  };

  const adminMenu = {
    items: [
      {
        key: 'profile',
        icon: <UserOutlined />,
        label: <Link to="/admin/profile">Hồ sơ cá nhân</Link>,
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

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider collapsible width={260} style={{ background: '#001529' }}>
        <div style={{ 
            height: 64, 
            margin: '0 16px', 
            color: '#fff', 
            display: 'flex', 
            alignItems: 'center', 
            justifyContent: 'center',
            borderBottom: '1px solid rgba(255,255,255,0.1)'
        }}>
          {/* Logo hoặc Tên Admin Panel */}
          <SafetyCertificateOutlined style={{ fontSize: 24, marginRight: 10, color: '#52c41a' }} />
          <strong style={{ fontSize: 18 }}>SUPER ADMIN</strong>
        </div>
        
        <Menu 
            theme="dark" 
            selectedKeys={[location.pathname]} 
            defaultOpenKeys={['sub-staff-manage', 'sub-user-manage']} // Mở sẵn các menu quan trọng
            mode="inline" 
            items={adminMenuItems} 
        />
      </Sider>

      <Layout>
        <Header style={{ padding: '0 24px', background: '#fff', display: 'flex', justifyContent: 'space-between', alignItems: 'center', boxShadow: '0 1px 4px rgba(0,21,41,0.08)' }}>
          {/* Breadcrumb hoặc Title trang hiện tại (Optional) */}
          <div style={{ fontSize: 16, fontWeight: 600, color: '#001529' }}>
            Hệ thống Quản trị Tài khoản Trung tâm
          </div>

          {/* User Info */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
            <Tag color="gold">ADMINISTRATOR</Tag>
            <Dropdown menu={adminMenu} placement="bottomRight" arrow trigger={['click']}>
              <div className="flex items-center gap-2 cursor-pointer hover:bg-gray-50 px-2 py-1 rounded transition-all">
                <Avatar style={{ backgroundColor: '#f56a00' }} icon={<UserOutlined />} />
                <span className="font-medium">{user?.fullName || 'Super Admin'}</span>
              </div>
            </Dropdown>
          </div>
        </Header>

        <Content style={{ margin: '24px 24px' }}>
          <div style={{ padding: 24, minHeight: 'calc(100vh - 112px)', background: '#fff', borderRadius: 8 }}>
            <Outlet />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default AdminLayout;