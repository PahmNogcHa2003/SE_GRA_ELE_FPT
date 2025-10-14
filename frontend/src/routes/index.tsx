import { BrowserRouter, Routes, Route } from 'react-router-dom';
import StaffLayout from '../layouts/StaffLayout';
import ManageStationsPage from '../pages/staff/ManageStationsPage';

// ... import các trang khác

const AppRoutes = () => {
  return (
    <BrowserRouter>
      <Routes>
        {/* Các route khác cho public, user, admin... */}

        {/* Route cho staff */}
        <Route path="/staff/*" element={<StaffLayout />}>       
          {/* Các trang khác của staff */}
          <Route path="stations" element={<ManageStationsPage />} />
          {/* Ví dụ: <Route path="/staff/dashboard" element={<StaffDashboard />} /> */}
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default AppRoutes;