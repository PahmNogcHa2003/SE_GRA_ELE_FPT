import React from 'react';
import { App, Card, Form, Input, Button, Typography, Space } from 'antd';
import type { FormProps } from 'antd';
import { useMutation } from '@tanstack/react-query';
import { changePassword } from '../../../services/auth.service';
import type {ChangePasswordPayload } from '../../../types/auth.ts';
import { useAuth } from "../../../features/auth/context/authContext";
import { strongPasswordRules } from '../../../utils/passwordRules';

const { Title, Text } = Typography;

    const prettyErr = (e: any) => {
    if (!e) return 'Đã có lỗi xảy ra';

    const resp = e?.response;
    if (resp && resp.data) {
        const d = resp.data;
        if (typeof d === 'string') return d;
        if (typeof d === 'object') {
        if (d.message) return d.message;
        if (d.error) return d.error;
        if (Array.isArray(d.errors) && d.errors.length > 0) {
            return d.errors.join('; ');
        }
        if (typeof d.title === 'string') return d.title;
        }
    }

    if (typeof e?.message === 'string') {
        try {
        const j = JSON.parse(e.message);
        if (j?.message) return j.message;
        if (j?.error) return j.error;
        } catch {
        return e.message;
        }
    }

    return 'Đã có lỗi xảy ra khi đổi mật khẩu.';
    };

    const ChangePasswordPage: React.FC = () => {
    const { isLoggedIn } = useAuth();
    const { notification } = App.useApp();
    const [form] = Form.useForm<ChangePasswordPayload>();

    const changeMut = useMutation({
        mutationFn: (payload: ChangePasswordPayload) => changePassword(payload),
        onSuccess: (data) => {
        // data = AuthResponseDTO
        if (data.isSuccess) {
            notification.success({
            message: 'Đổi mật khẩu thành công',
            description: data.message ?? 'Mật khẩu của bạn đã được cập nhật.',
            });
            form.resetFields();
        } else {
            notification.error({
            message: 'Đổi mật khẩu thất bại',
            description: data.message ?? 'Không thể đổi mật khẩu.',
            });
        }
        },
        onError: (e: any) => {
        notification.error({
            message: 'Đổi mật khẩu thất bại',
            description: prettyErr(e),
        });
        },
    });

    const onFinish: FormProps<ChangePasswordPayload>['onFinish'] = (values) => {
        changeMut.mutate(values);
    };

    return (
        <div className="min-h-screen bg-emerald-50 flex items-center justify-center px-4 py-8">
        <div className="w-full max-w-md">
            <Card className="rounded-3xl shadow-md border border-emerald-100">
            <Space direction="vertical" size="large" style={{ width: '100%' }}>
                <div>
                <Title level={3} style={{ marginBottom: 4, color: '#065f46' }}>
                    Đổi mật khẩu
                </Title>
                <Text type="secondary">
                    Vui lòng nhập mật khẩu hiện tại và đặt mật khẩu mới theo đúng yêu cầu bảo mật.
                </Text>
                </div>

                {!isLoggedIn && (
                <Text type="danger">
                    Bạn chưa đăng nhập. Vui lòng đăng nhập trước khi đổi mật khẩu.
                </Text>
                )}

                <Form<ChangePasswordPayload>
                form={form}
                layout="vertical"
                onFinish={onFinish}
                disabled={changeMut.isPending || !isLoggedIn}
                >
                <Form.Item
                    label="Mật khẩu hiện tại"
                    name="currentPassword"
                    rules={[{ required: true, message: 'Vui lòng nhập mật khẩu hiện tại.' }]}
                >
                    <Input.Password placeholder="Nhập mật khẩu hiện tại" />
                </Form.Item>

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
                    name="confirmNewPassword"
                    dependencies={['newPassword']}
                    hasFeedback
                    rules={[
                    { required: true, message: 'Vui lòng xác nhận mật khẩu mới.' },
                    ({ getFieldValue }) => ({
                        validator(_, value) {
                        if (!value || getFieldValue('newPassword') === value) {
                            return Promise.resolve();
                        }
                        return Promise.reject('Mật khẩu xác nhận không trùng khớp.');
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
                    style={{
                        backgroundColor: '#047857',
                        borderColor: '#047857',
                    }}
                    disabled={
                        !form.isFieldsTouched(['newPassword', 'confirmNewPassword'], true) ||
                        !!form.getFieldsError().filter(({ errors }) => errors.length).length
                    }
                    loading={changeMut.isPending}
                    >
                    Đổi mật khẩu
                    </Button>
                </Form.Item>

                <div className="text-xs text-gray-500">
                    Mật khẩu mới phải có:
                    <ul className="list-disc list-inside mt-1">
                    <li>Ít nhất 8 ký tự</li>
                    <li>Ít nhất 1 chữ hoa (A–Z)</li>
                    <li>Ít nhất 1 chữ thường (a–z)</li>
                    <li>Ít nhất 1 chữ số (0–9)</li>
                    <li>Ít nhất 1 ký tự đặc biệt (!@#$%^&amp;...)</li>
                    </ul>
                </div>
                </Form>
            </Space>
            </Card>
        </div>
        </div>
    );
    };

    export default ChangePasswordPage;
