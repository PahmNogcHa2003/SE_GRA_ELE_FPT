import { useMutation } from '@tanstack/react-query';
import { notification } from 'antd';
import { useAuth } from '../context/authContext';
import { loginApiAdmin } from '../../../services/auth.service';
import type { LoginPayload, AuthResponseData } from '../../../types/auth';
import type { ApiResponse } from '../../../types/api';

const ALLOWED_ADMIN_ROLES = ['Admin', 'Staff'];

export const useAdminLogin = () => {
  const { login, logout } = useAuth(); // Không cần hasRole từ context ở đây

  return useMutation<AuthResponseData, ApiResponse<null>, LoginPayload>({
    mutationFn: loginApiAdmin,
    onSuccess: async (response) => {
      if (response.isSuccess && response.token) {
        // 1. Đánh dấu là Admin login
        localStorage.setItem('loginType', 'admin');
        
        // 2. Cập nhật context
        await login({
          token: response.token,
          roles: response.roles ?? [],
          isSuccess: response.isSuccess,
          message: response.message,
        });

        // 3. Check quyền NGAY LẬP TỨC từ response
        const userRoles = response.roles ?? [];
        const isAdmin = userRoles.some(r => ALLOWED_ADMIN_ROLES.includes(r));

        if (isAdmin) {
          notification.success({
            message: 'Xin chào Quản trị viên',
            description: 'Đăng nhập hệ thống quản trị thành công.',
          });
          window.location.replace('/staff');
        } else {
          // Login thành công nhưng không có quyền Admin
          logout(); 
          notification.error({
            message: 'Truy cập bị từ chối',
            description: 'Tài khoản này không có quyền truy cập trang quản trị.',
          });
          // Không chuyển trang linh tinh, ở lại trang login
        }
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