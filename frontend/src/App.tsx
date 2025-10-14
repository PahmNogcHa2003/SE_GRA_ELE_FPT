import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import AppRoutes from './routes'; // Giả sử file routes/index.tsx export default một component

// Tạo một client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false, // Tùy chọn: tắt refetch khi focus lại cửa sổ
      retry: 1, // Tùy chọn: thử lại 1 lần nếu query lỗi
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      {/* Các providers khác của bạn (Theme, Auth, ...) */}
      <AppRoutes />
    </QueryClientProvider>
  );
}

export default App;