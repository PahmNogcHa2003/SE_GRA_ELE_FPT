import { BrowserRouter, Routes, Route } from 'react-router-dom';
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
import ProfilePage from '../pages/user/ProfilePage';
// Staff pages
import ManageStationsPage from '../pages/staff/ManageStationsPage';
import ManageVehiclesPage from '../pages/staff/ManageVehiclesPage';
import ManageNewsPage from '../pages/staff/ManageNewsPage';
import ManageTagsPage from '../pages/staff/ManageTagsPage';
import ManageCategoriesVehiclePage from '../pages/staff/ManageCategoriesVehiclePage';
import TicketPlanManagementPage from '../pages/staff/TicketPlanManagementPage';
import TicketPlanPriceManagementPage from '../pages/staff/TicketPlanPriceManagementPage';
import UserTicketManagementPage from '../pages/staff/UserTicketManagementPage';
// Auth
import LoginModal from '../features/auth/components/LoginModal';
import ProtectedRoute from './ProtectedRoute';
import BlockRoles from './BlockRoles';

const AppRoutes = () => (
  <BrowserRouter>
    <LoginModal />

    <Routes>
      {/* Guest */}
      <Route element={<BlockRoles bannedRoles={['Admin', 'Staff']} redirectTo="/staff" />}>
        <Route path="/" element={<UserLayout />}>
          <Route index element={<HomePage />} />
          <Route path="stations" element={<StationPage />} />
          <Route path="contact" element={<ContactPage/>}/>
          <Route path="news" element={<NewsListPage />} />
          <Route path="news/:id" element={<NewsDetailPage />} />
        </Route>
      </Route>

      {/* USER */}
      <Route element={<ProtectedRoute allowRoles={['User']} />}>
        <Route path="/" element={<UserLayout />}>
          <Route path="wallet" element={<WalletPage />} />
          <Route path="top-up" element={<TopUpPage />} />
          <Route path="payment/payment-return" element={<PaymentReturnPage />} />
          <Route path="my-tickets" element={<MyTicketsPage />} />
          <Route path="pricing" element={<BuyTicketsPage />} />
          <Route path="profile" element={<ProfilePage />} />
        </Route>
      </Route>

      {/* STAFF */}
      <Route element={<ProtectedRoute allowRoles={['Staff', 'Admin']} />}>
        <Route path="/staff" element={<StaffLayout />}>
          <Route path="stations" element={<ManageStationsPage />} />
          <Route path="vehicles" element={<ManageVehiclesPage />} />
          <Route path="categories-vehicle" element={<ManageCategoriesVehiclePage />} />
          <Route path="news" element={<ManageNewsPage />} />
          <Route path="tags" element={<ManageTagsPage />} />
          <Route path="ticket-plans" element={<TicketPlanManagementPage />} />
          <Route path="ticket-plan-prices" element={<TicketPlanPriceManagementPage />} />
          <Route path="user-tickets" element={<UserTicketManagementPage />} />
        </Route>
      </Route>
      {/* OTHER */}
      <Route path="/login" element={<div>Trang Đăng Nhập</div>} />
      <Route path="/unauthorized" element={<div>Bạn không có quyền truy cập</div>} />
      <Route path="*" element={<div>404 Not Found</div>} />
    </Routes>
  </BrowserRouter>
);

export default AppRoutes;
