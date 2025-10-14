import React from 'react';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { Layout, Menu } from 'antd';
import { HomeOutlined, CarOutlined } from '@ant-design/icons';

const { Header, Content, Sider } = Layout;

const menuItems = [
    {
        key: '/staff/dashboard',
        icon: <HomeOutlined />,
        label: <Link to="/staff/dashboard">Dashboard</Link>,
    },
    {
        key: '/staff/stations',
        icon: <CarOutlined />,
        label: <Link to="/staff/stations">Quản lý trạm</Link>,
    },
];

const StaffLayout: React.FC = () => {
  const location = useLocation();

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider collapsible>
        <div style={{ height: '32px', margin: '16px', background: 'rgba(255, 255, 255, 0.2)' }} />
        <Menu theme="dark" selectedKeys={[location.pathname]} mode="inline" items={menuItems} />
      </Sider>
      <Layout>
        <Header style={{ padding: '0 16px', background: '#fff' }}>
          {/* Bạn có thể thêm các component Header ở đây, ví dụ: thông tin user, nút đăng xuất... */}
        </Header>
        <Content style={{ margin: '24px 16px' }}>
          <div style={{ padding: 24, minHeight: 'calc(100vh - 112px)', background: '#fff' }}>
            {/* Dòng quan trọng nhất: Nội dung của các trang con sẽ được render ở đây */}
            <Outlet />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default StaffLayout;