// src/pages/user/WalletPage.tsx
import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../features/auth/context/authContext';
import { getWallet, getWalletTransactions } from '../../services/wallet.service';
import { Spin, Table } from 'antd'; 
import type { TableProps } from 'antd';
import type { WalletTransaction, Wallet } from '../../types/wallet';
import { FaWallet } from 'react-icons/fa'; 

// Hàm format tiền tệ
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('vi-VN', { 
    style: 'currency', 
    currency: 'VND' 
  }).format(amount);
};

// Cấu hình cột (Giữ nguyên)
const getColumns = (): TableProps<WalletTransaction>['columns'] => [
  { title: 'ID', dataIndex: 'id', key: 'id', render: (id) => `#${id.toString().slice(-6)}`},
  { title: 'Ngày', dataIndex: 'createdAt', key: 'createdAt', render: (date) => new Date(date).toLocaleDateString('vi-VN')},
  {
    title: 'Số lượng', dataIndex: 'amount', key: 'amount',
    render: (amount, record) => (
      <span className={record.direction === 'In' ? 'text-green-600' : 'text-red-600'}>
        {record.direction === 'In' ? '+' : '-'}
        {formatCurrency(Math.abs(amount))}
      </span>
    ),
  },
  {
    title: 'Chi tiết giao dịch', dataIndex: 'direction', key: 'details',
    render: (direction: string, record: WalletTransaction) => {
      if (direction === 'In') return <span>Nạp điểm (Nguồn: {record.source})</span>;
      if (direction === 'Out') return <span>Tiêu thụ (Nguồn: {record.source})</span>;
      return record.source;
    },
  },
];


const WalletPage: React.FC = () => {
  const navigate = useNavigate();
  const { user, token, isLoadingUser } = useAuth(); 
  const [pagination, setPagination] = useState({ pageNumber: 1, pageSize: 10 });

  // Query lấy thông tin Ví
  const { data: walletData, isLoading: isLoadingWallet } = useQuery({
    queryKey: ['wallet'],
    queryFn: () => getWallet(), 
    enabled: !!token, 
    select: (data) => data.data || null, 
  });

  // Query lấy Lịch sử giao dịch
  const { data: transactionsData, isLoading: isLoadingTransactions } = useQuery({
    queryKey: ['walletTransactions', pagination.pageNumber],
    queryFn: () => getWalletTransactions(pagination.pageNumber, pagination.pageSize),
    enabled: !!token,
    select: (data) => data.data || null,
  });

  const handleTableChange: TableProps<WalletTransaction>['onChange'] = (pager) => {
    setPagination({
      pageNumber: pager.current || 1, 
      pageSize: pager.pageSize || 10,
    });
  };

  if (isLoadingUser || (isLoadingWallet && !walletData)) {
    return <div className="flex justify-center items-center h-[50vh]"><Spin size="large" /></div>;
  }
  
  const displayWallet: Wallet = walletData || {
    id: 0,
    userId: user!.userId,
    balance: 0,
    totalDebt: 0,
    status: 'Not Found',
    createdAt: '',
    updatedAt: '',
  };
  
  return (
    <div className="bg-gray-100 min-h-screen">
      <div className="container mx-auto p-4 md:p-8">
        
        <div className="w-full bg-eco-green-dark text-white text-center text-4xl font-bold p-6 rounded-t-xl shadow-lg">
          VÍ CỦA TÔI
        </div>

        <div className="bg-white p-6 rounded-b-xl shadow-lg">
          {/* Thông tin Ví */}
          <div className="bg-eco-green text-white p-6 rounded-lg flex flex-col md:flex-row shadow-inner-lg">
            
            {/* Tài khoản chính (chiếm 1/2) */}
            <div className="text-center p-4 flex-1">
              <h3 className="text-lg opacity-80">Tài khoản chính</h3>
              <p className="text-4xl font-bold my-2">
                {formatCurrency(displayWallet.balance)}
              </p>
              <p className="text-sm opacity-80">(Điểm = VND)</p>
            </div>
            <div className="border-t-2 md:border-t-0 md:border-l-2 border-white/50 my-4 md:my-0"></div>

            {/* Nợ cước (chiếm 1/2) */}
            <div className="text-center p-4 flex-1">
              <h3 className="text-lg opacity-80">Nợ cước</h3>
              <p className="text-4xl font-bold my-2 text-red-300">
                {formatCurrency(displayWallet.totalDebt)}
              </p>
              <p className="text-sm opacity-80">(Vui lòng thanh toán)</p>
            </div>
          </div>
          {/* Nút Nạp tiền */}
            <div className="grid grid-cols-2 gap-6 my-8">
                <div className="col-span-2 flex justify-center">
                    <button
                    onClick={() => navigate('/top-up')}
                    className="flex flex-col items-center justify-center p-6 bg-eco-green-dark text-white rounded-lg shadow-md hover:bg-eco-green-dark/90 transition-all"
                    >
                    <FaWallet size={40} />
                    <span className="text-xl font-semibold mt-3">Nạp Tiền</span>
                    </button>
                </div>
            </div>
          {/* Lịch sử giao dịch */}
          <div className="mt-12">
            <h2 className="text-3xl font-bold text-center text-gray-800 mb-6">
              LỊCH SỬ GIAO DỊCH
            </h2>
            <Table
              columns={getColumns()}
              dataSource={transactionsData?.items || []} 
              rowKey="id"
              loading={isLoadingTransactions}
              pagination={{
                current: transactionsData?.pageNumber, 
                pageSize: transactionsData?.pageSize,
                total: transactionsData?.totalCount,
              }}
              onChange={handleTableChange}
              scroll={{ x: true }}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default WalletPage;