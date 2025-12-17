import React from 'react';
import { Modal, Form, Input, Button } from 'antd';
import { MailOutlined, LockOutlined } from '@ant-design/icons';
import { v4 as uuidv4 } from 'uuid';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/authContext';
import { useUserLogin } from '../hooks/useUserLogin'; // Import Hook

import EcoLogo from '../../../assets/images/logo_circle_green.png';
import GooglePlayBadge from '../../../assets/images/google-play.webp';
import AppQR from '../../../assets/images/app-qr.png';

const getDeviceId = (): string => {
  let deviceId = localStorage.getItem('deviceId');
  if (!deviceId) {
    deviceId = uuidv4();
    localStorage.setItem('deviceId', deviceId);
  }
  return deviceId;
};

const LoginModal: React.FC = () => {
  const { isLoginModalOpen, closeLoginModal } = useAuth();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  
  // 👉 SỬ DỤNG HOOK
  const { mutate, isPending } = useUserLogin(form, closeLoginModal, navigate);

  const onFinish = (values: any) => {
    const payload = {
      email: values.email,
      password: values.password,
      deviceId: getDeviceId(),
      pushToken: 'web-push-token-placeholder',
      platform: 'Web',
    };
    mutate(payload);
  };

  return (
    <Modal
      title={
        <div className="text-center font-bold text-eco-green flex flex-col items-center">
          <img src={EcoLogo} alt="Eco Journey" className="h-16 sm:h-20 md:h-24 lg:h-28 mb-4" />
          <span className="text-xl md:text-2xl">Đăng nhập Eco Journey</span>
        </div>
      }
      open={isLoginModalOpen}
      onCancel={() => {
        if (!isPending) {
          closeLoginModal();
          form.resetFields();
        }
      }}
      footer={null}
      centered
    >
      <Form
        form={form}
        name="login"
        onFinish={onFinish}
        layout="vertical"
        size="large"
        className="mt-6"
      >
        <Form.Item
          name="email"
          rules={[
            { required: true, message: 'Vui lòng nhập Email!' },
            { type: 'email', message: 'Email không hợp lệ!' },
          ]}
        >
          <Input prefix={<MailOutlined />} placeholder="Email" />
        </Form.Item>

        <Form.Item
          name="password"
          rules={[{ required: true, message: 'Vui lòng nhập Mật khẩu!' }]}
        >
          <Input.Password prefix={<LockOutlined />} placeholder="Mật khẩu" />
        </Form.Item>

        <Form.Item>
          <a className="float-right text-eco-green hover:text-eco-green-dark" href="/auth/forgot-password">
            Quên mật khẩu?
          </a>
        </Form.Item>

        <Form.Item>
          <Button
            type="primary"
            htmlType="submit"
            className="w-full bg-eco-green hover:bg-eco-green-dark"
            loading={isPending}
          >
            Đăng nhập
          </Button>
        </Form.Item>
      </Form>

      {/* --- Footer giữ nguyên --- */}
      <div className="mt-8 text-center border-t pt-6">
        <p className="text-gray-600 mb-3 text-base">
          Chưa có tài khoản? <br />
          Hãy tải ứng dụng <span className="font-semibold text-eco-green">Eco Journey</span> để đăng ký!
        </p>
        <div className="flex flex-row justify-center items-center gap-x-6 mt-4">
          <img src={AppQR} alt="QR" className="h-32 w-32 rounded-lg border-2 border-gray-300" />
          <a href="#" target="_blank" rel="noopener noreferrer">
             <img src={GooglePlayBadge} alt="Google Play" className="h-16 w-auto object-contain" />
          </a>
        </div>
      </div>
    </Modal>
  );
};

export default LoginModal;