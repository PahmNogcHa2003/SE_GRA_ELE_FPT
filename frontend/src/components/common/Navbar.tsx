// src/components/common/Navbar.tsx
import React, { useState } from 'react';
import { FaBell, FaUser, FaGlobe  } from 'react-icons/fa';
import Button from './Button.tsx'; // Chúng ta sẽ tạo Button.tsx ngay sau đây
import Logo from '../../assets/images/logo_green.png'; // Giả sử bạn có file logo
import { useAuth } from '../../features/auth/context/authContext';
import { App, Dropdown, Avatar } from 'antd';
import type { MenuProps } from 'antd';
import { 
  UserOutlined, 
  WalletOutlined, 
  CreditCardOutlined, 
  LogoutOutlined 
} from '@ant-design/icons';

const navLinks = [
  { name: 'Hướng dẫn sử dụng', href: '/how-to-use' },
  { name: 'Danh sách trạm', href: '/stations' },
  { name: 'Bảng giá', href: '/pricing' },
  { name: 'Dịch vụ', href: '/services' },
  { name: 'Tin tức', href: '/news' },
  { name: 'Liên hệ', href: '/contact' },
  { name: 'Chính sách bảo mật', href: '/privacy-policy' },
];

const Navbar: React.FC = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const { isLoggedIn, openLoginModal, logout ,user} = useAuth();
  const { modal } = App.useApp();
  const { notification } = App.useApp();
  const showLogoutConfirm = () => {
    modal.confirm({
      title: 'Bạn có chắc chắn muốn đăng xuất?',
      content: 'Bạn sẽ cần đăng nhập lại để sử dụng dịch vụ.',
      okText: 'Đăng xuất',
      okType: 'danger',
      cancelText: 'Hủy',
      centered: true,
      onOk() {
         notification.success({
          message: 'Đăng xuất thành công!',
          description: 'Hẹn gặp lại bạn ở EcoJourney sau nhé !',
        });
        logout(); 
      },
      onCancel() {
        // Không làm gì khi hủy
      },
    });
  };
  const menuItems: MenuProps['items'] = [
    {
      key: 'profile',
      label: (
        <a href="/profile">
          Trang cá nhân
        </a>
      ),
      icon: <UserOutlined />,
    },
    {
      key: 'wallet',
      label: (
        <a href="/wallet">
          Ví của tôi
        </a>
      ),
      icon: <WalletOutlined />,
    },
    {
      key: 'tickets',
      label: (
        <a href="/my-tickets">
          Vé của tôi
        </a>
      ),
      icon: <CreditCardOutlined />,
    },
    {
      type: 'divider', // Đường kẻ phân cách
    },
    {
      key: 'logout',
      label: 'Đăng xuất',
      icon: <LogoutOutlined />,
      danger: true, // Hiển thị màu đỏ
      onClick: showLogoutConfirm, // Gọi hàm xác nhận khi nhấn
    },
  ];
  return (
    <nav className="bg-white shadow-md sticky top-0 z-50">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-20">
          {/* Logo */}
 <div className="shrink-0 flex items-center">
  <a href="/">
  <img
    className="h-16 w-auto md:h-42" // 👈 logo cao hơn
    src={Logo}
    alt="Eco Journey"
  />
  </a>
</div>
          {/* Nav Links - Desktop */}
          <div className="hidden md:flex md:items-center md:space-x-6">
            {navLinks.map((link) => (
              <a
                key={link.name}
                href={link.href}
                className="text-gray-900! hover:text-eco-green! font-semibold text-lg transition-colors duration-200"
              >
                {link.name}
              </a>
            ))}
          </div>

          {/* Right Icons & Button - Desktop */}
          <div className="hidden md:flex items-center space-x-4">
            <Button>Tải app</Button>
            <button className="text-gray-900 hover:text-eco-green font-semibold text-lg transition-colors duration-200">
              <FaGlobe size={20} />
            </button>
            <button className="text-gray-900 hover:text-eco-green font-semibold text-lg transition-colors duration-200">
              <FaBell size={20} />
            </button>
            {isLoggedIn ? (
              // NẾU ĐÃ ĐĂNG NHẬP: HIỂN THỊ DROPDOWN
              <Dropdown menu={{ items: menuItems }} placement="bottomRight" arrow trigger={['click']}>
                {/* Đây là nút kích hoạt dropdown */}
                <button className="flex items-center text-gray-700 hover:text-eco-green transition-colors outline-none">
                  <Avatar
                    size="default"
                    src={user?.avatarUrl || undefined}
                    icon={!user?.avatarUrl && <UserOutlined />}
                  />
                  <span className="ml-2 font-medium">
                    {user?.fullName || "Tài khoản"}
                  </span>
                </button>
              </Dropdown>
            ) : (
              // NẾU CHƯA ĐĂNG NHẬP: HIỂN THỊ NÚT LOGIN
              <button 
                className="text-gray-600 hover:text-eco-green p-2 rounded-full"
                onClick={openLoginModal} 
              >
                <UserOutlined style={{ fontSize: '20px' }} />
              </button>
            )}
          </div>
          
          {/* Mobile Menu Button */}
          <div className="md:hidden flex items-center">
            <button 
              onClick={() => setIsMenuOpen(!isMenuOpen)}
              className="text-gray-700 hover:text-eco-green"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16m-7 6h7" /></svg>
            </button>
          </div>
        </div>
      </div>

      {/* Mobile Menu - Dropdown */}
      {isMenuOpen && (
        <div className="md:hidden bg-white shadow-lg">
          <div className="px-2 pt-2 pb-3 space-y-1 sm:px-3">
            {navLinks.map((link) => (
              <a
                key={link.name}
                href={link.href}
                className="block px-3 py-2 rounded-md text-base font-medium text-gray-700 hover:text-eco-green hover:bg-gray-50"
              >
                {link.name}
              </a>
            ))}
          </div>
          <div className="border-t border-gray-200 pt-4 pb-3 px-4">
            <Button fullWidth>Tải app</Button>
            <div className="flex items-center justify-around mt-4">
              <button className="text-gray-600 hover:text-eco-green p-2 rounded-full">
                <FaGlobe size={20} />
              </button>
              <button className="text-gray-600 hover:text-eco-green p-2 rounded-full">
                <FaBell size={20} />
              </button>
              <button className="text-gray-600 hover:text-eco-green p-2 rounded-full">
                <FaUser size={20} />
              </button>
            </div>
          </div>
        </div>
      )}
    </nav>
  );
};

export default Navbar;