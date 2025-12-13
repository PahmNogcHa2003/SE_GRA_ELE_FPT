import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import UserLayout from '../layouts/UserLayout';
import StaffLayout from '../layouts/StaffLayout';

// User pages
import HomePage from '../pages/user/HomePage';
import WalletPage from '../pages/user/WalletPage';
import TopUpPage from '../pages/user/TopUpPage';
import PaymentReturnPage from '../pages/user/PaymentReturnPage';
import MyTicketsPage from '../pages/user/MyTicketsPage';
import StationPage from '../pages/user/StationsPage';
import BuyTicketsPage from '../pages/user/BuyTicketsPage';
import ContactPage  from '../pages/user/ContactPage';
import NewsListPage from '../pages/user/NewsListPage';
import NewsDetailPage from '../pages/user/NewsDetailPage';
import ProfilePage from '../pages/user/Profile/ProfilePage';
import ChangePasswordPage from '../pages/user/ChangePasswordPage';
import LeaderboardPage from '../pages/user/Leaderboard/LeaderboardPage';
// Staff pages
import ManageStationsPage from '../pages/staff/ManageStationsPage';
import ManageVehiclesPage from '../pages/staff/ManageVehiclesPage';
import ManageNewsPage from '../pages/staff/ManageNewsPage';
import ManageTagsPage from '../pages/staff/ManageTagsPage';
import ManageCategoriesVehiclePage from '../pages/staff/ManageCategoriesVehiclePage';
import TicketPlanManagementPage from '../pages/staff/TicketPlanManagementPage';
import TicketPlanPriceManagementPage from '../pages/staff/TicketPlanPriceManagementPage';
import UserTicketManagementPage from '../pages/staff/UserTicketManagementPage';
import ManageRentalsPage from '../pages/staff/ManageRentalsPage';
import ManageQuestsPage from '../pages/staff/ManageQuestsPage';
import ManageTransactionsPage from '../pages/staff/ManageTransactionsPage';
// Auth
import AdminLogin from '../features/auth/admin/AdminLogin';
import LoginModal from '../features/auth/components/LoginModal';
// import ProtectedRoute from './ProtectedRoute';
// import BlockRoles from './BlockRoles';

const AppRoutes = () => (
  <BrowserRouter>
    <LoginModal />

    <Routes>
      <Route path="/admin/login" element={<AdminLogin />} />

      {/* Guest (User chưa đăng nhập hoặc User thường) */}
      {/* <Route element={<BlockRoles bannedRoles={['Admin', 'Staff']} redirectTo="/staff" />}> */}
        <Route path="/" element={<UserLayout />}>
          <Route index element={<HomePage />} />
          <Route path="stations" element={<StationPage />} />
          <Route path="contact" element={<ContactPage/>} />
          <Route path="news" element={<NewsListPage />} />
          <Route path="news/:id" element={<NewsDetailPage />} />
        </Route>
      {/* </Route> */}

      {/* USER Protected Routes */}
      {/* <Route element={<ProtectedRoute allowRoles={['User']} />}> */}
        <Route path="/" element={<UserLayout />}>
          <Route path="wallet" element={<WalletPage />} />
          <Route path="top-up" element={<TopUpPage />} />
          <Route path="payment/payment-return" element={<PaymentReturnPage />} />
          <Route path="my-tickets" element={<MyTicketsPage />} />
          <Route path="pricing" element={<BuyTicketsPage />} />
          <Route path="profile" element={<ProfilePage />} />
          <Route path="change-password" element={<ChangePasswordPage />} />
          <Route path="leaderboard" element={<LeaderboardPage />} />
        </Route>
      {/* </Route> */}

      {/* STAFF / ADMIN Protected Routes */}
      {/* Khi login thành công, hook useAdminLogin sẽ redirect về /staff hoặc /staff/stations */}
      {/* <Route element={<ProtectedRoute allowRoles={['Staff', 'Admin']} />}> */}
        <Route path="/staff" element={<StaffLayout />}>
            <Route index element={<Navigate to="stations" replace />} /> {/* Thêm dòng này nếu muốn default */}
            <Route path="stations" element={<ManageStationsPage />} />
            <Route path="vehicles" element={<ManageVehiclesPage />} />
            <Route path="categories-vehicle" element={<ManageCategoriesVehiclePage />} />
            <Route path="news" element={<ManageNewsPage />} />
            <Route path="tags" element={<ManageTagsPage />} />
            <Route path="ticket-plans" element={<TicketPlanManagementPage />} />
            <Route path="ticket-plan-prices" element={<TicketPlanPriceManagementPage />} />
            <Route path="user-tickets" element={<UserTicketManagementPage />} />
            <Route path="rentals" element={<ManageRentalsPage />} />
            <Route path="quests" element={<ManageQuestsPage />} />
            <Route path="transactions" element={<ManageTransactionsPage />} />
        </Route>
      {/* </Route> */}
      <Route path="/unauthorized" element={<div>Bạn không có quyền truy cập (403)</div>} />
      <Route path="*" element={<div>404 Not Found</div>} />
    </Routes>
  </BrowserRouter>
);
export default AppRoutes;
