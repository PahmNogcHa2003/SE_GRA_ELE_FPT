    import { useMutation } from '@tanstack/react-query';
    import { notification } from 'antd';
    import { useAuth } from '../context/authContext'; 
    import { loginApi } from '../../../services/auth.service';
    import type { LoginPayload, AuthResponseData } from '../../../types/auth';
    import type { ApiResponse } from '../../../types/api';

    const FORBIDDEN_ROLES = ['Admin', 'Staff'];

    export const useUserLogin = (
    form: any,
    closeLoginModal: () => void,
    navigate: (path: string, options?: any) => void
    ) => {
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
            const isForbidden = hasRole(...FORBIDDEN_ROLES);

            if (isForbidden) {
                logout(); 
                notification.warning({
                message: 'Sai cổng đăng nhập',
                description: 'Tài khoản Quản trị viên vui lòng đăng nhập tại trang Admin.',
                duration: 4,
                });
                
                form.resetFields();
                closeLoginModal();
                navigate('/admin/login'); 
            } else {
                notification.success({
                message: 'Đăng nhập thành công!',
                description: 'Chào mừng bạn trở lại Eco Journey!',
                });
                
                form.resetFields();
                closeLoginModal();
                navigate('/', { replace: true });
            }
            }, 100);
        } else {
            form.setFields([{ name: 'password', errors: [response.message] }]);
        }
        },
        onError: (error: any) => {
        const msg = error?.response?.data?.message || 'Email hoặc mật khẩu không đúng.';
        form.setFields([{ name: 'password', errors: [msg] }]);
        },
    });
    };