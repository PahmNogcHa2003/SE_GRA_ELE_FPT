// // src/routes/BlockRoles.tsx
// import React from 'react';
// import { Outlet, Navigate, useLocation } from 'react-router-dom';
// import { useAuth } from '../features/auth/context/authContext';

// /**
//  * Nếu đang đăng nhập và user có 1 trong các bannedRoles -> redirectTo
//  * Nếu khách hoặc user không thuộc bannedRoles -> cho vào (Outlet)
//  */
// const BlockRoles: React.FC<{ bannedRoles: string[]; redirectTo: string }> = ({ bannedRoles, redirectTo }) => {
//   const { isLoggedIn, user } = useAuth();
//   const location = useLocation();

//   if (isLoggedIn && user?.roles?.some(r => bannedRoles.includes(r))) {
//     return <Navigate to={redirectTo} replace state={{ from: location.pathname }} />;
//   }

//   return <Outlet />;
// };

// export default BlockRoles;
