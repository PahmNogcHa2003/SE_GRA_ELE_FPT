// src/components/common/Navbar.tsx
import React, { useState } from 'react';
import { FaBell, FaGlobe } from 'react-icons/fa';
import Button from './Button'; 
import Logo from '../../assets/images/logo_green.png';
import { useAuth } from '../../features/auth/context/authContext';
import { App, Dropdown, Avatar, type MenuProps, Space } from 'antd';
import { Link, useNavigate } from 'react-router-dom'; // Dùng Link/Navigate cho SPA
import {
  UserOutlined,
  WalletOutlined,
  CreditCardOutlined,
  LogoutOutlined,
  HistoryOutlined,
  DownOutlined,
} from '@ant-design/icons';
import { CgPassword } from 'react-icons/cg';

// Cấu trúc NavLinks có hỗ trợ submenu
const navLinks = [
  { name: 'Hướng dẫn sử dụng', href: '/how-to-use' },
  { name: 'Danh sách trạm', href: '/stations' },
  { name: 'Bảng giá', href: '/pricing' },
  { name: 'Dịch vụ', href: '/services',},
  { name : 'Xếp hạng', href: '/leaderboard'},
  { name: 'Tin tức', href: '/news' },
  { name: 'Liên hệ', href: '/contact' },
];

const Navbar: React.FC = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const { isLoggedIn, openLoginModal, logout, user } = useAuth();
  const { modal, notification } = App.useApp();
  const navigate = useNavigate();

  const showLogoutConfirm = () => {
    modal.confirm({
      title: 'Bạn có chắc chắn muốn đăng xuất?',
      content: 'Bạn sẽ cần đăng nhập lại để sử dụng dịch vụ.',
      okText: 'Đăng xuất',
      okType: 'danger',
      cancelText: 'Hủy',
      centered: true,
      onOk() {
        logout();
        notification.success({ message: 'Đăng xuất thành công!', description: 'Hẹn gặp lại bạn!' });
        navigate('/');
      },
    });
  };

  // 1. MENU USER (Thêm Lịch sử chuyến đi)
  const userMenuItems: MenuProps['items'] = [
    {
      key: 'profile',
      label: <Link to="/profile">Trang cá nhân</Link>,
      icon: <UserOutlined />,
    },
    {
      key: 'wallet',
      label: <Link to="/wallet">Ví của tôi</Link>,
      icon: <WalletOutlined />,
    },
    {
      key: 'tickets',
      label: <Link to="/my-tickets">Vé của tôi</Link>,
      icon: <CreditCardOutlined />,
    },
    {
      key: 'changePassword',
      label: <Link to="/change-password">Đổi mật khẩu</Link>,
      icon: <CgPassword />,
    },
    { type: 'divider' },
    {
      key: 'logout',
      label: 'Đăng xuất',
      icon: <LogoutOutlined />,
      danger: true,
      onClick: showLogoutConfirm,
    },
  ];

  // Helper function để render NavItem
  const renderNavItem = (link: any) => {
    // Nếu có children thì render Dropdown
    if (link.children) {
        const menuProps: MenuProps = {
            items: link.children.map((child: any) => ({
                key: child.key,
                label: <Link to={child.href}>{child.label}</Link>,
                icon: child.icon
            }))
        };
        return (
            <Dropdown key={link.name} menu={menuProps} placement="bottom" arrow>
                <a className="text-gray-900 hover:text-eco-green font-semibold text-lg cursor-pointer flex items-center gap-1 transition-colors">
                    {link.name} <DownOutlined style={{ fontSize: '12px'}}/>
                </a>
            </Dropdown>
        );
    }
    // Nếu không thì render link thường
    return (
        <Link
            key={link.name}
            to={link.href}
            className="text-gray-900 hover:text-eco-green font-semibold text-lg transition-colors duration-200"
        >
            {link.name}
        </Link>
    );
  };

  return (
    <nav className="bg-white shadow-md sticky top-0 z-50">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-20">
          {/* Logo */}
          <div className="shrink-0 flex items-center">
            <Link to="/">
              <img className="h-16 w-auto md:h-42" src={Logo} alt="Eco Journey" />
            </Link>
          </div>

          {/* Nav Links - Desktop */}
          <div className="hidden md:flex md:items-center md:space-x-6">
            {navLinks.map((link) => renderNavItem(link))}
          </div>

          {/* Right Icons & Button - Desktop */}
          <div className="hidden md:flex items-center space-x-4">
            <Button>Tải app</Button>
            {/* Các nút icon Globe, Bell giữ nguyên ... */}
            
            {isLoggedIn ? (
              <Dropdown menu={{ items: userMenuItems }} placement="bottomRight" arrow trigger={['click']}>
                <button className="flex items-center text-gray-700 hover:text-eco-green transition-colors outline-none">
                  <Avatar size="default" src={user?.avatarUrl} icon={!user?.avatarUrl && <UserOutlined />} />
                  <span className="ml-2 font-medium hidden lg:inline-block">{user?.fullName || "Tài khoản"}</span>
                </button>
              </Dropdown>
            ) : (
              <button className="text-gray-600 hover:text-eco-green p-2 rounded-full" onClick={openLoginModal}>
                <UserOutlined style={{ fontSize: '20px' }} />
              </button>
            )}
          </div>
          
          {/* Mobile Menu Button giữ nguyên... */}
           <div className="md:hidden flex items-center">
             <button onClick={() => setIsMenuOpen(!isMenuOpen)} className="text-gray-700">
               {/* Icon Hamburger */}
               <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16m-7 6h7" /></svg>
             </button>
           </div>
        </div>
      </div>
      
      {/* Mobile Menu logic giữ nguyên, chỉ cần map navLinks tương tự... */}
    </nav>
  );
};

export default Navbar;