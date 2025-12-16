// src/features/auth/context/AuthContext.tsx
import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import type { ReactNode } from 'react'; 
import type { AuthResponseData, User } from '../../../types/auth';
import { getMeApi, getMeApiAdmin } from '../../../services/auth.service';
import type { ApiResponse } from '../../../types/api';

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
  updateUser: (updater: (prev: User | null) => User | null) => void;
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
    localStorage.removeItem('loginType'); 
    window.location.replace('/');
  }, []);

  /**
   * Hàm lấy thông tin user từ token (bọc trong useCallback)
   */
const fetchUserByToken = useCallback(async () => {
    try {
      // Lấy cờ đã lưu lúc login
      const loginType = localStorage.getItem('loginType'); 
      let response: ApiResponse<User>;
      if (loginType === 'admin') {
         response = await getMeApiAdmin(); 
      } else {
         response = await getMeApi();
      }

      if (response.success && response.data) {
        setUser(response.data);
      } else {
        throw new Error(response.message);
      }
    } catch (error) {
      console.log("Lỗi lấy thông tin user:", error);
      setUser(null);
      setToken(null); 
      localStorage.removeItem('authToken'); 
    } finally {
      setIsLoadingUser(false); 
    }
  }, []);

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
    setToken(data.token || null);
    localStorage.setItem('authToken', data.token || ''); 
    
    setIsLoadingUser(true); 
    await fetchUserByToken(); 
    
    closeLoginModal();
  };

  const openLoginModal = () => setIsLoginModalOpen(true);
  const closeLoginModal = () => setIsLoginModalOpen(false);
const updateUser = (updater: (prev: User | null) => User | null) => {
  setUser(prev => updater(prev));
};
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
    updateUser,
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