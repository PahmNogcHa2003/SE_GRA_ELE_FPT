import { Form, Input, Button, message, Card } from 'antd';
import { MailOutlined } from '@ant-design/icons';
import { sendPasswordResetEmail } from '../../../services/auth.service';

const ForgotPassword = () => {
  const [form] = Form.useForm();

  const onFinish = async (values: { email: string }) => {
    try {
      await sendPasswordResetEmail(values.email);
      message.success('Đã gửi link đặt lại mật khẩu. Vui lòng kiểm tra email!');
      form.resetFields();
    } catch (err: any) {
      message.error(err?.response?.data?.message || 'Gửi email thất bại');
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <Card title="Quên mật khẩu" className="w-[400px]">
        <Form form={form} layout="vertical" onFinish={onFinish}>
          <Form.Item
            name="email"
            label="Email"
            rules={[
              { required: true, message: 'Vui lòng nhập email' },
              { type: 'email', message: 'Email không hợp lệ' },
            ]}
          >
            <Input prefix={<MailOutlined />} placeholder="Email đăng ký" />
          </Form.Item>

          <Button type="primary" htmlType="submit" block>
            Gửi link đặt lại mật khẩu
          </Button>
        </Form>
      </Card>
    </div>
  );
};

export default ForgotPassword;
