import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import UserLayout from '../layouts/UserLayout';
import StaffLayout from '../layouts/StaffLayout';
import AdminGuard from './AdminGuard';
import ProtectedRoute from './ProtectedRoute'; 
import AdminLayout from '../layouts/AdminLayout';

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
import ChangePasswordPage from '../pages/user/Auth/ChangePasswordPage';
import ForgotPassword from '../pages/user/Auth/ForgotPassword';
import ResetPasswordPage from '../pages/user/Auth/ResetPassword';
import LeaderboardPage from '../pages/user/Leaderboard/LeaderboardPage';

// Staff pages
import ManageStationsPage from '../pages/staff/ManageStationsPage';
import ManageVehiclesPage from '../pages/staff/ManageVehiclesPage';
import ManageCategoriesVehiclePage from '../pages/staff/ManageCategoriesVehiclePage';
import ManageNewsPage from '../pages/staff/ManageNewsPage';
import ManageTagsPage from '../pages/staff/ManageTagsPage';
import TicketPlanManagementPage from '../pages/staff/TicketPlanManagementPage';
import TicketPlanPriceManagementPage from '../pages/staff/TicketPlanPriceManagementPage';
import UserTicketManagementPage from '../pages/staff/UserTicketManagementPage';
import ManageRentalsPage from '../pages/staff/ManageRentalsPage';
import ManageQuestsPage from '../pages/staff/ManageQuestsPage';
import ManageTransactionsPage from '../pages/staff/ManageTransactionsPage';
import ManageVouchersPage from '../pages/staff/ManageVouchersPage';
import DashboardPage  from '../pages/staff/DashboardPage';
// Auth
import AdminLogin from '../features/auth/admin/AdminLogin';
import LoginModal from '../features/auth/user/LoginModal'; 

const AppRoutes = () => (
  <BrowserRouter>
    <LoginModal />
    <Routes>
      <Route path="/admin/login" element={<AdminLogin />} />
      <Route element={<AdminGuard />}>
        <Route path="/admin" element={<AdminLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard" element={<div>Dashboard</div>} />
            <Route path="manage-staff" element={<div>Manage Staff</div>} />
            <Route path="manage-users" element={<div>Manage Users</div>} />
            <Route path="user-kyc" element={<div>User KYC</div>} />
            <Route path="roles" element={<div>Roles</div>} />
            <Route path="audit-logs" element={<div>Audit Logs</div>} />
        </Route>
        <Route path="/staff" element={<StaffLayout />}>
            <Route index element={<Navigate to="dashboard" replace />} />
            <Route path="dashboard" element={<DashboardPage/>} />
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
            <Route path="vouchers" element={<ManageVouchersPage />} />
        </Route>
      </Route>
      
      <Route path="/" element={<UserLayout />}>
        <Route index element={<HomePage />} />
        <Route path="stations" element={<StationPage />} />
        <Route path="contact" element={<ContactPage/>} />
        <Route path="news" element={<NewsListPage />} />
        <Route path="news/:id" element={<NewsDetailPage />} />
        <Route path="pricing" element={<BuyTicketsPage />} />
        <Route path="leaderboard" element={<LeaderboardPage />} />
        <Route path="auth/forgot-password" element={<ForgotPassword />} />
        <Route path="auth/reset-password" element={<ResetPasswordPage />} />
        <Route element={<ProtectedRoute allowRoles={['User']} />}> 
             <Route path="wallet" element={<WalletPage />} />
             <Route path="top-up" element={<TopUpPage />} />
             <Route path="payment/payment-return" element={<PaymentReturnPage />} />
             <Route path="my-tickets" element={<MyTicketsPage />} />
             <Route path="profile" element={<ProfilePage />} />
             <Route path="change-password" element={<ChangePasswordPage />} />

        </Route>
      </Route>
      <Route path="/unauthorized" element={
        <div className="flex h-screen items-center justify-center text-xl font-bold text-red-500">
            Bạn không có quyền truy cập (403)
        </div>
      } />
      <Route path="*" element={
        <div className="flex h-screen items-center justify-center text-xl">
            404 Not Found
        </div>
      } />

    </Routes>
  </BrowserRouter>
);

export default AppRoutes;