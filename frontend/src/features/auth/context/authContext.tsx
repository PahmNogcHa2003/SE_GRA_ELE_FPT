// src/features/auth/context/AuthContext.tsx
import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import type { ReactNode } from 'react'; 
import type { AuthResponseData, User } from '../../../types/auth';
import { getMeApi } from '../../../services/auth.service'; 
import type { ApiResponse } from '../../../types/api';

// ... (Interface AuthContextType giữ nguyên) ...
interface AuthContextType {
  isLoggedIn: boolean;
  token: string | null;
  user: User | null;
  isLoadingUser: boolean; 
  isLoginModalOpen: boolean;
  login: (data: AuthResponseData) => Promise<void>; 
  logout: () => void;
  openLoginModal: () => void;
  closeLoginModal: () => void;
  hasRole: (...roles: string[]) => boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<User | null>(null);
  const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);
  const [isLoadingUser, setIsLoadingUser] = useState(true); 

  /**
   * Hàm logout (bọc trong useCallback)
   */
  const logout = useCallback(() => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('authToken');
    window.location.replace('/');
  }, []); // Không có dependency

  /**
   * Hàm lấy thông tin user
   * (KHÔNG CẦN 'currentToken' param nữa)
   */
  const fetchUserByToken = useCallback(async () => {
    try {
      // Interceptor sẽ tự động thêm token
      const response: ApiResponse<User> = await getMeApi(); 
      
      if (response.success && response.data) {
        setUser(response.data); 
      } else {
        throw new Error(response.message);
      }
    } catch (error) {
      // Interceptor đã xử lý lỗi 401 và gọi logout rồi
      // Chúng ta chỉ cần log lỗi nếu muốn
      console.error("AuthContext: Không thể fetch user", error);
    } finally {
      setIsLoadingUser(false);
    }
  }, []); // Không có dependency

  /**
   * Lắng nghe sự kiện 401 từ Interceptor
   */
  useEffect(() => {
    const handleAuthError = () => {
      console.warn('Interceptor phát hiện lỗi 401, đang đăng xuất...');
      logout();
    };
    
    window.addEventListener('auth-error-401', handleAuthError);
    
    // Dọn dẹp listener khi component unmount
    return () => {
      window.removeEventListener('auth-error-401', handleAuthError);
    };
  }, [logout]); // Phụ thuộc vào hàm logout

  /**
   * Kiểm tra localStorage khi mount
   */
  useEffect(() => {
    const storedToken = localStorage.getItem('authToken');
    if (storedToken) {
      setToken(storedToken);
      fetchUserByToken(); // <-- Không cần truyền token
    } else {
      setIsLoadingUser(false); 
    }
  }, [fetchUserByToken]);

  /**
   * Hàm login
   */
  const login = async (data: AuthResponseData) => {
    setToken(data.token);
    localStorage.setItem('authToken', data.token); // Interceptor sẽ đọc từ đây
    
    setIsLoadingUser(true); 
    await fetchUserByToken(); // <-- Không cần truyền token
    
    closeLoginModal();
  };

  const openLoginModal = () => setIsLoginModalOpen(true);
  const closeLoginModal = () => setIsLoginModalOpen(false);

  const value = {
    isLoggedIn: !!token && !!user, 
    token,
    user,
    isLoadingUser,
    isLoginModalOpen,
    login,
    logout,
    openLoginModal,
    closeLoginModal,
    hasRole: (...allowed: string[]) => {
    if (!user?.roles?.length) return false;
    return user.roles.some((r) => allowed.includes(r));
  },
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

// hook useAuth (giữ nguyên)
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth phải được dùng trong AuthProvider');
  }
  return context;
};