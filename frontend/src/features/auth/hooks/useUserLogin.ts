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
  const { login, logout } = useAuth();

  return useMutation<AuthResponseData, ApiResponse<null>, LoginPayload>({
    mutationFn: loginApi,
    onSuccess: async (response) => {
      if (response.isSuccess && response.token) {
        // 1. Đánh dấu là User login
        localStorage.setItem('loginType', 'user');

        // 2. Cập nhật context
        await login({
          token: response.token,
          roles: response.roles ?? [],
          isSuccess: response.isSuccess,
          message: response.message,
        });

        // 3. Check quyền NGAY LẬP TỨC
        const userRoles = response.roles ?? [];
        const isForbidden = userRoles.some(r => FORBIDDEN_ROLES.includes(r));

        if (isForbidden) {
          logout(); // Logout ngay
          notification.warning({
            message: 'Sai cổng đăng nhập',
            description: 'Tài khoản Quản trị viên vui lòng đăng nhập tại trang Admin.',
            duration: 4,
          });
          
          form.resetFields();
          closeLoginModal();
          navigate('/admin/login'); // Đá sang trang Admin
        } else {
          notification.success({
            message: 'Đăng nhập thành công!',
            description: 'Chào mừng bạn trở lại Eco Journey!',
          });
          
          form.resetFields();
          closeLoginModal();
          // navigate('/', { replace: true }); // Có thể reload hoặc không
        }
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