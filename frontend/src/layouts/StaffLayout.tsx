// src/layouts/StaffLayout.tsx
import React from 'react';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { Layout, Menu, Dropdown, Avatar, Tag, App as AntdApp } from 'antd';
import { useAuth } from '../features/auth/context/authContext';
import { FaTicketAlt , FaMoneyBillAlt, FaChargingStation  } from 'react-icons/fa';
import { FaTags , FaFileInvoiceDollar } from "react-icons/fa6";
import { BiSolidDiscount } from "react-icons/bi";
import { MdElectricBike, MdDirectionsBike  } from 'react-icons/md';
import { LuGoal, LuTicketsPlane ,LuBike  } from "react-icons/lu";
import { GiTicket } from "react-icons/gi";
import {
  HomeOutlined,
  ShoppingOutlined,  
  GiftOutlined,        
  CustomerServiceOutlined, 
  HistoryOutlined,     
  WalletOutlined,      
  TrophyOutlined,       
  ReadOutlined,
  UserOutlined, 
  LogoutOutlined
} from '@ant-design/icons';

const { Header, Content, Sider } = Layout;

const menuItems = [
  // 1. Dashboard
  { 
    key: '/staff/dashboard', 
    icon: <HomeOutlined />, 
    label: <Link to="/staff/dashboard">Dashboard</Link> 
  },

  // 2. Nhóm VẬN HÀNH (Quản lý tài sản vật lý)
  {
    key: 'sub-operation',
    icon: <MdElectricBike style={{ fontSize: '18px' }} />, 
    label: 'Quản lý Vận hành',
    children: [
      { key: '/staff/stations', 
        icon: <FaChargingStation />,
        label: <Link to="/staff/stations">Trạm xe</Link> },

      { key: '/staff/vehicles', 
        icon: <LuBike style={{ fontSize: '18px' }} />,
        label: <Link to="/staff/vehicles">Danh sách xe</Link> },

      { key: '/staff/categories-vehicle', 
        icon: <MdDirectionsBike style={{ fontSize: '18px' }} />,
        label: <Link to="/staff/categories-vehicle">Loại xe</Link> },
    ],
  },

  // 3. Nhóm KINH DOANH & GIAO DỊCH (Dữ liệu phát sinh hàng ngày)
  {
    key: 'sub-business',
    icon: <ShoppingOutlined />,
    label: 'Quản lý Giao dịch',
    children: [
      { 
        key: '/staff/rentals', 
        icon: <HistoryOutlined />, 
        label: <Link to="/staff/rentals">Lịch sử Thuê xe</Link> 
      },
      { 
        key: '/staff/orders', 
        icon: <FaFileInvoiceDollar />,
        label: <Link to="/staff/orders">Đơn hàng </Link> 
      },
      { 
        key: '/staff/user-tickets', 
        icon: <LuTicketsPlane />,
        label: <Link to="/staff/user-tickets">Vé người dùng</Link> 
      },
      { 
        key: '/staff/wallet-transactions', 
        icon: <WalletOutlined />,
        label: <Link to="/staff/transactions">Giao dịch Ví</Link> 
      },
    ],
  },

  // 4. Nhóm CẤU HÌNH VÉ (Setup sản phẩm)
  {
    key: 'sub-ticket-config',
    icon: <FaTicketAlt />,
    label: 'Cấu hình Gói Vé',
    children: [
      { key: '/staff/ticket-plans',
        icon: <GiTicket />, 
        label: <Link to="/staff/ticket-plans">Danh sách Gói vé</Link> },
      { key: '/staff/ticket-plan-prices', 
        icon: <FaMoneyBillAlt />,
        label: <Link to="/staff/ticket-plan-prices">Bảng giá vé</Link> },
    ],
  },

  // 5. Nhóm MARKETING & GAMIFICATION (Thu hút người dùng)
  {
    key: 'sub-marketing',
    icon: <GiftOutlined />,
    label: 'Marketing & Sự kiện',
    children: [
      { 
        key: '/staff/campaigns', 
        icon: <LuGoal />,
        label: <Link to="/staff/campaigns">Chiến dịch khuyến mãi</Link> 
      },
      { 
        key: '/staff/vouchers', 
        icon: <BiSolidDiscount />,  
        label: <Link to="/staff/vouchers">Mã giảm giá</Link> 
      },
      { 
        key: '/staff/quests', 
        icon: <TrophyOutlined />,
        label: <Link to="/staff/quests">Nhiệm vụ</Link> 
      },
      { 
        key: '/staff/news', 
        icon: <ReadOutlined />,
        label: <Link to="/staff/news">Tin tức</Link> 
      },
      { 
        key: '/staff/tags', 
        icon: <FaTags />,
        label: <Link to="/staff/tags">Thẻ (Tags)</Link> 
      },
    ],
  },

  // 6. Nhóm CSKH (Hỗ trợ)
  { 
    key: '/staff/contacts', 
    icon: <CustomerServiceOutlined />, 
    label: <Link to="/staff/contacts">Liên hệ & Phản hồi</Link> 
  },
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
