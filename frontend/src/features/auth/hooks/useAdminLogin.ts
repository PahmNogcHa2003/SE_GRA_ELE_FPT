    import { useMutation } from '@tanstack/react-query';
    import { notification } from 'antd';
    import { useAuth } from '../context/authContext';
    import { loginApi } from '../../../services/auth.service';
    import type { LoginPayload, AuthResponseData } from '../../../types/auth';
    import type { ApiResponse } from '../../../types/api';

    const ALLOWED_ADMIN_ROLES = ['Admin', 'Staff'];

    export const useAdminLogin = () => {
    const { login, hasRole, logout } = useAuth();

    return useMutation<AuthResponseData, ApiResponse<null>, LoginPayload>({
        mutationFn: loginApi,
        onSuccess: async (response) => {
        if (response.isSuccess && response.token) {
            await login({
            token: response.token,
            roles: response.roles ?? [],
            isSuccess: response.isSuccess,
            message: response.message,
            });

            setTimeout(() => {
            const isAdmin = hasRole(...ALLOWED_ADMIN_ROLES);

            if (isAdmin) {
                notification.success({
                message: 'Xin chào Quản trị viên',
                description: 'Đăng nhập hệ thống quản trị thành công.',
                });
                window.location.replace('/staff'); 
            } else {
                logout();
                notification.error({
                message: 'Truy cập bị từ chối',
                description: 'Tài khoản này không có quyền truy cập trang quản trị.',
                });
                window.location.replace('/'); 
            }
            }, 100);
        }
        },
        onError: (error: any) => {
        const msg = error?.response?.data?.message || 'Đăng nhập thất bại.';
        notification.error({
            message: 'Lỗi đăng nhập',
            description: msg,
        });
        },
    });
    };