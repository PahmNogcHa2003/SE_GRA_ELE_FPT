import React from 'react';
import { App, Card, Form, Input, Button, Typography, Space } from 'antd';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import type { FormProps } from 'antd';

import { resetPassword } from '../../../services/auth.service';
import { strongPasswordRules } from '../../../utils/passwordRules';

const { Title, Text } = Typography;

interface ResetPasswordPayload {
  newPassword: string;
  confirmPassword: string;
}

const ResetPasswordPage: React.FC = () => {
  const { notification } = App.useApp();
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const [form] = Form.useForm<ResetPasswordPayload>();

  const email = params.get('email');
  const token = params.get('token');

  const resetMut = useMutation({
    mutationFn: (payload: ResetPasswordPayload) =>
      resetPassword({
        email: email!,
        token: token!,
        newPassword: payload.newPassword,
        confirmPassword: payload.confirmPassword,
      }),
    onSuccess: (data) => {
      if (data.isSuccess) {
        notification.success({
          message: 'Đặt lại mật khẩu thành công',
          description: data.message ?? 'Bạn có thể đăng nhập bằng mật khẩu mới.',
        });
        form.resetFields();
        navigate('/');
      } else {
        notification.error({
          message: 'Đặt lại mật khẩu thất bại',
          description: data.message ?? 'Không thể đặt lại mật khẩu.',
        });
      }
    },
    onError: (e: any) => {
      notification.error({
        message: 'Đặt lại mật khẩu thất bại',
        description: e?.response?.data?.message ?? 'Có lỗi xảy ra.',
      });
    },
  });

  const onFinish: FormProps<ResetPasswordPayload>['onFinish'] = (values) => {
    if (!email || !token) {
      notification.error({
        message: 'Link không hợp lệ',
        description: 'Thiếu email hoặc token.',
      });
      return;
    }
    resetMut.mutate(values);
  };

  return (
    <div className="min-h-screen bg-emerald-50 flex items-center justify-center px-4 py-8">
      <div className="w-full max-w-md">
        <Card className="rounded-3xl shadow-md border border-emerald-100">
          <Space direction="vertical" size="large" style={{ width: '100%' }}>
            <div>
              <Title level={3} style={{ marginBottom: 4, color: '#065f46' }}>
                Đặt lại mật khẩu
              </Title>
              <Text type="secondary">
                Vui lòng nhập mật khẩu mới theo đúng yêu cầu bảo mật.
              </Text>
            </div>

            <Form<ResetPasswordPayload>
              form={form}
              layout="vertical"
              onFinish={onFinish}
              disabled={resetMut.isPending}
            >
              <Form.Item
                label="Mật khẩu mới"
                name="newPassword"
                rules={strongPasswordRules}
                hasFeedback
              >
                <Input.Password placeholder="Nhập mật khẩu mới" />
              </Form.Item>

              <Form.Item
                label="Xác nhận mật khẩu mới"
                name="confirmPassword"
                dependencies={['newPassword']}
                hasFeedback
                rules={[
                  { required: true, message: 'Vui lòng xác nhận mật khẩu.' },
                  ({ getFieldValue }) => ({
                    validator(_, value) {
                      if (!value || getFieldValue('newPassword') === value) {
                        return Promise.resolve();
                      }
                      return Promise.reject(
                        'Mật khẩu xác nhận không trùng khớp.'
                      );
                    },
                  }),
                ]}
              >
                <Input.Password placeholder="Nhập lại mật khẩu mới" />
              </Form.Item>

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  block
                  loading={resetMut.isPending}
                  style={{
                    backgroundColor: '#047857',
                    borderColor: '#047857',
                  }}
                   disabled={
                        !form.isFieldsTouched(['newPassword', 'confirmPassword'], true) ||
                        !!form.getFieldsError().filter(({ errors }) => errors.length).length
                    }
                >
                  Đặt lại mật khẩu
                </Button>
              </Form.Item>

              <div className="text-xs text-gray-500">
                Mật khẩu mới phải có:
                <ul className="list-disc list-inside mt-1">
                  <li>Ít nhất 8 ký tự</li>
                  <li>Ít nhất 1 chữ hoa (A–Z)</li>
                  <li>Ít nhất 1 chữ thường (a–z)</li>
                  <li>Ít nhất 1 chữ số (0–9)</li>
                  <li>Ít nhất 1 ký tự đặc biệt</li>
                </ul>
              </div>
            </Form>
          </Space>
        </Card>
      </div>
    </div>
  );
};

export default ResetPasswordPage;
