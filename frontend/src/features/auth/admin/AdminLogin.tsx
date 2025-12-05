import React from 'react';
import { Form, Input, Button, Card, Typography } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { v4 as uuidv4 } from 'uuid';
import { useAdminLogin } from '../../../features/auth/hooks/useAdminLogin'; // Import hook Admin
import type { LoginPayload } from '../../../types/auth';

import EcoLogo from '../../../assets/images/logo_circle_green.png';

    const { Title, Text } = Typography;

    const AdminLogin: React.FC = () => {
    const { mutate, isPending } = useAdminLogin();

    const getDeviceId = (): string => {
        let deviceId = localStorage.getItem('deviceId');
        if (!deviceId) {
        deviceId = uuidv4();
        localStorage.setItem('deviceId', deviceId);
        }
        return deviceId;
    };

    const onFinish = (values: any) => {
        const payload: LoginPayload = {
        email: values.email, 
        password: values.password,
        deviceId: getDeviceId(),
        pushToken: 'admin-portal',
        platform: 'Web',
        };
        console.log("Dữ liệu gửi lên Server:", payload);
        mutate(payload);
    };

    return (
        <div 
        style={{ 
            minHeight: '100vh', 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center', 
            backgroundColor: '#f0f2f5',
            backgroundImage: 'url("https://www.transparenttextures.com/patterns/cube-coat.png")'
        }}
        >
        <Card 
            style={{ 
            width: 420, 
            padding: '24px',
            boxShadow: '0 8px 24px rgba(0, 0, 0, 0.12)',
            borderRadius: '12px'
            }}
        >
            <div style={{ textAlign: 'center', marginBottom: 32 }}>
            <img 
                src={EcoLogo} 
                alt="EcoJourney Logo" 
                style={{ width: 80, marginBottom: 16 }}
            />
            <Title level={3} style={{ margin: 0, color: '#389e0d' }}> {/* Màu xanh lá */}
                Eco Journey Admin
            </Title>
            <Text type="secondary">Đăng nhập hệ thống quản trị</Text>
            </div>

            <Form
            name="admin_login"
            initialValues={{ remember: true }}
            onFinish={onFinish}
            layout="vertical"
            size="large"
            >
            <Form.Item
                name="email"
                rules={[{ required: true, message: 'Vui lòng nhập email!' }]}
            >
                <Input 
                prefix={<UserOutlined style={{ color: 'rgba(0,0,0,.25)' }} />} 
                placeholder="Email" 
                />
            </Form.Item>

            <Form.Item
                name="password"
                rules={[{ required: true, message: 'Vui lòng nhập mật khẩu!' }]}
            >
                <Input.Password
                prefix={<LockOutlined style={{ color: 'rgba(0,0,0,.25)' }} />}
                placeholder="Mật khẩu"
                />
            </Form.Item>

            <Form.Item style={{ marginTop: 24 }}>
                <Button 
                type="primary" 
                htmlType="submit" 
                loading={isPending}
                block 
                style={{ 
                    backgroundColor: '#389e0d', // Màu xanh chủ đạo
                    borderColor: '#389e0d',
                    height: 48,
                    fontSize: 16,
                    fontWeight: 600
                }} 
                >
                ĐĂNG NHẬP
                </Button>
            </Form.Item>
            </Form>
            
            <div style={{ textAlign: 'center', marginTop: 16 }}>
            <Text type="secondary" style={{ fontSize: 12 }}>
                © 2024 Eco Journey System. All Rights Reserved.
            </Text>
            </div>
        </Card>
        </div>
    );
    };

    export default AdminLogin;