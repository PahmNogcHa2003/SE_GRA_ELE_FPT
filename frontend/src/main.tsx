// src/main.tsx
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.tsx';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './features/auth/context/authContext.tsx';
import { ConfigProvider, App as AntdApp } from 'antd';

// ⚠️ Thứ tự import rất quan trọng
// 1️⃣ AntD reset (nếu có)
// import 'antd/dist/reset.css';

// 2️⃣ Tailwind CSS (phải sau AntD)
import './index.css';

const queryClient = new QueryClient();

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <ConfigProvider
          theme={{
            token: {
              colorPrimary: '#4CAF50',   
              colorInfo: '#4CAF50',      
              colorSuccess: '#388E3C',  
              colorWarning: '#FF9800',
              colorError: '#F44336',
              colorText: '#333333',
            },
          }}
        >
          <AntdApp
            notification={{
              placement: 'topRight',
              top: 80,
              duration: 3,
            }}
          >
            <App />
          </AntdApp>
        </ConfigProvider>
      </AuthProvider>
    </QueryClientProvider>
  </React.StrictMode>
);
