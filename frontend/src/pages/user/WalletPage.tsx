// src/pages/user/WalletPage.tsx
import React, { useState } from 'react';
import {
  useQuery,
  useMutation,
  useQueryClient,
} from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../features/auth/context/authContext';
import {
  getWallet,
  getWalletTransactions,
  convertPromoToBalance,
  payAllDebt,
} from '../../services/wallet.service';
import { App, Spin, Table, Modal, InputNumber } from 'antd';
import type { TableProps } from 'antd';
import type { WalletTransaction, Wallet } from '../../types/wallet';
import { FaWallet } from 'react-icons/fa';

// Hàm format tiền tệ
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
  }).format(amount);
};

// Cấu hình cột
const getColumns = (): TableProps<WalletTransaction>['columns'] => [
  {
    title: 'ID',
    dataIndex: 'id',
    key: 'id',
    render: (id: number) => `#${id.toString().slice(-6)}`,
  },
  {
    title: 'Ngày',
    dataIndex: 'createdAt',
    key: 'createdAt',
    render: (date: string) => new Date(date).toLocaleDateString('vi-VN'),
  },
  {
    title: 'Số lượng',
    dataIndex: 'amount',
    key: 'amount',
    render: (amount: number, record: WalletTransaction) => (
      <span
        className={
          record.direction === 'In' ? 'text-green-600' : 'text-red-600'
        }
      >
        {record.direction === 'In' ? '+' : '-'}
        {formatCurrency(Math.abs(amount))}
      </span>
    ),
  },
  {
    title: 'Chi tiết giao dịch',
    dataIndex: 'direction',
    key: 'details',
    render: (direction: string, record: WalletTransaction) => {
      if (direction === 'In')
        return <span>Nạp điểm (Nguồn: {record.source})</span>;
      if (direction === 'Out')
        return <span>Tiêu thụ (Nguồn: {record.source})</span>;
      return record.source;
    },
  },
  {
    title: 'Tài khoản chính sau giao dịch',
    dataIndex: 'balanceAfter',
    key: 'balanceAfter',
    render: (balanceAfter: number) => formatCurrency(balanceAfter),
  },
  {
    title: 'Khuyến mãi sau giao dịch',
    dataIndex: 'promoAfter',
    key: 'promoAfter',
    render: (promoAfter: number) => formatCurrency(promoAfter),
  },
];

const WalletPage: React.FC = () => {
  const navigate = useNavigate();
  const { user, token, isLoadingUser } = useAuth();
  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 10,
  });

  const queryClient = useQueryClient();
  const { message, modal } = App.useApp();


  // ====== MODAL STATE (convert promo) ======
  const [isConvertModalOpen, setIsConvertModalOpen] =
    useState<boolean>(false);
  const [convertAmount, setConvertAmount] = useState<number>(0);

  // ====== MUTATION: Convert promo → balance ======
  const { mutateAsync: mutateConvertPromo, isPending: isConverting } =
    useMutation({
      mutationFn: (amount: number) => convertPromoToBalance(amount),
      onSuccess: (_data, amount) => {
        message.success(
          `Đã chuyển thành công ${formatCurrency(
            amount,
          )} sang tài khoản chính!`,
        );
        queryClient.invalidateQueries({ queryKey: ['wallet'] });
        queryClient.invalidateQueries({
          queryKey: ['walletTransactions'],
        });
        setIsConvertModalOpen(false);
        setConvertAmount(0);
      },
      onError: (error: any) => {
        const msg =
          error?.response?.data?.message ||
          error?.response?.data?.Message ||
          'Chuyển khuyến mãi không thành công.';
        message.error(msg);
      },
    });

  // ====== MUTATION: Pay all debt ======
  const { mutateAsync: mutatePayAllDebt, isPending: isPayingDebt } =
    useMutation({
      mutationFn: () => payAllDebt(),
      onSuccess: (res) => {
        const msg =
          res?.message ||
          'Đã thanh toán toàn bộ nợ thành công từ ví chính.';
        message.success(msg);
        queryClient.invalidateQueries({ queryKey: ['wallet'] });
        queryClient.invalidateQueries({
          queryKey: ['walletTransactions'],
        });
      },
      onError: (error: any) => {
        const msg =
          error?.response?.data?.message ||
          error?.response?.data?.Message ||
          'Thanh toán nợ không thành công.';
        message.error(msg);
      },
    });

  // Query lấy thông tin Ví
  const { data: walletData, isLoading: isLoadingWallet } = useQuery({
    queryKey: ['wallet'],
    queryFn: () => getWallet(),
    enabled: !!token,
    select: (data) => data.data || null,
  });

  // Query lấy Lịch sử giao dịch
  const { data: transactionsData, isLoading: isLoadingTransactions } =
    useQuery({
      queryKey: ['walletTransactions', pagination.pageNumber],
      queryFn: () =>
        getWalletTransactions(
          pagination.pageNumber,
          pagination.pageSize,
        ),
      enabled: !!token,
      select: (data) => data.data || null,
    });

  const handleTableChange: TableProps<WalletTransaction>['onChange'] = (
    pager,
  ) => {
    setPagination({
      pageNumber: pager.current || 1,
      pageSize: pager.pageSize || 10,
    });
  };

  if (isLoadingUser || (isLoadingWallet && !walletData)) {
    return (
      <div className="flex justify-center items-center h-[50vh]">
        <Spin size="large" />
      </div>
    );
  }

  const displayWallet: Wallet = walletData || {
    id: 0,
    userId: user!.userId,
    balance: 0,
    promoBalance: 0,
    totalDebt: 0,
    status: 'Not Found',
    createdAt: '',
    updatedAt: '',
  };

  // ====== HANDLE: mở modal convert promo ======
  const handleConvertPromo = () => {
    const maxAmount = displayWallet.promoBalance;

    if (maxAmount <= 0) {
      message.info('Bạn không có số dư khuyến mãi để chuyển.');
      return;
    }

    setConvertAmount(maxAmount); // mặc định chuyển hết
    setIsConvertModalOpen(true);
  };

  // ====== HANDLE: xác nhận chuyển promo ======
  const handleConfirmConvert = async () => {
    const maxAmount = displayWallet.promoBalance;

    if (!convertAmount || convertAmount <= 0) {
      message.warning('Vui lòng nhập số tiền hợp lệ.');
      return;
    }

    if (convertAmount > maxAmount) {
      message.warning('Số tiền chuyển không được vượt quá số dư hiện có.');
      return;
    }

    await mutateConvertPromo(convertAmount);
  };

  // ====== HANDLE: thanh toán toàn bộ nợ ======
  const handlePayAllDebt = () => {
    if (displayWallet.totalDebt <= 0) {
      message.info('Bạn không có khoản nợ nào cần thanh toán.');
      return;
    }

    modal.confirm({
      title: 'Thanh toán toàn bộ nợ',
      content: `Hệ thống sẽ dùng số dư ví chính để thanh toán toàn bộ nợ (${formatCurrency(
        displayWallet.totalDebt,
      )}). Tiếp tục?`,
      okText: 'Thanh toán ngay',
      cancelText: 'Hủy',
      okButtonProps: { loading: isPayingDebt },
      onOk: async () => {
        await mutatePayAllDebt();
      },
    });
  };

  return (
    <div className="bg-gray-100 min-h-screen">
      <div className="container mx-auto p-4 md:p-8">
        <div className="w-full bg-eco-green-dark text-white text-center text-4xl font-bold p-6 rounded-t-xl shadow-lg">
          VÍ CỦA TÔI
        </div>

        <div className="bg-white p-6 rounded-b-xl shadow-lg">
          {/* Thông tin Ví */}
          <div className="bg-eco-green text-white p-6 rounded-lg shadow-inner-lg">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {/* Tài khoản chính */}
              <div className="text-center p-4 border-b md:border-b-0 md:border-r border-white/40">
                <h3 className="text-lg opacity-80">Tài khoản chính</h3>
                <p className="text-4xl font-bold my-2">
                  {formatCurrency(displayWallet.balance)}
                </p>
                <p className="text-sm opacity-80">(Điểm = VND)</p>
              </div>

              {/* Tài khoản khuyến mãi */}
              <div className="text-center p-4 border-b md:border-b-0 md:border-r border-white/40">
                <h3 className="text-lg opacity-80">Khuyến mãi</h3>
                <p className="text-4xl font-bold my-2 text-yellow-200">
                  {formatCurrency(displayWallet.promoBalance)}
                </p>
                <p className="text-sm opacity-80">
                  (Điểm khuyến mãi khả dụng)
                </p>
              </div>

              {/* Nợ cước */}
              <div className="text-center p-4">
                <h3 className="text-lg opacity-80">Nợ cước</h3>
                <p className="text-4xl font-bold my-2 text-red-300">
                  {formatCurrency(displayWallet.totalDebt)}
                </p>
                <p className="text-sm opacity-80">(Vui lòng thanh toán)</p>
              </div>
            </div>
          </div>

          {/* Nút hành động */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 my-8">
            {/* Nút Nạp tiền */}
            <div className="flex justify-center">
              <button
                onClick={() => navigate('/top-up')}
                className="flex flex-col items-center justify-center w-full md:w-64 p-6 bg-eco-green-dark text-white rounded-lg shadow-md hover:bg-eco-green-dark/90 transition-all"
              >
                <FaWallet size={40} />
                <span className="text-xl font-semibold mt-3">Nạp Tiền</span>
              </button>
            </div>

            {/* Nút Chuyển khuyến mãi */}
            <div className="flex justify-center">
              <button
                onClick={handleConvertPromo}
                disabled={
                  displayWallet.promoBalance <= 0 || isConverting
                }
                className={`flex flex-col items-center justify-center w-full md:w-64 p-6 rounded-lg shadow-md transition-all ${
                  displayWallet.promoBalance <= 0 || isConverting
                    ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    : 'bg-yellow-500 text-white hover:bg-yellow-600'
                }`}
              >
                <FaWallet size={40} />
                <span className="text-xl font-semibold mt-3">
                  Chuyển khuyến mãi
                </span>
                <span className="text-sm mt-1">
                  (Số dư: {formatCurrency(displayWallet.promoBalance)})
                </span>
              </button>
            </div>

            {/* Nút Thanh toán nợ */}
            <div className="flex justify-center">
              <button
                onClick={handlePayAllDebt}
                disabled={displayWallet.totalDebt <= 0 || isPayingDebt}
                className={`flex flex-col items-center justify-center w-full md:w-64 p-6 rounded-lg shadow-md transition-all ${
                  displayWallet.totalDebt <= 0 || isPayingDebt
                    ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    : 'bg-red-500 text-white hover:bg-red-600'
                }`}
              >
                <FaWallet size={40} />
                <span className="text-xl font-semibold mt-3">
                  Thanh toán nợ
                </span>
                <span className="text-sm mt-1">
                  (Đang nợ: {formatCurrency(displayWallet.totalDebt)})
                </span>
              </button>
            </div>
          </div>

          {/* Modal Chuyển khuyến mãi */}
          <Modal
            title="Chuyển khuyến mãi sang tài khoản chính"
            open={isConvertModalOpen}
            onCancel={() => setIsConvertModalOpen(false)}
            onOk={handleConfirmConvert}
            okText="Xác nhận"
            cancelText="Hủy"
            confirmLoading={isConverting}
          >
            <div className="flex flex-col gap-2 mt-3">
              <p>
                Số dư khuyến mãi hiện tại:{' '}
                <b>{formatCurrency(displayWallet.promoBalance)}</b>
              </p>
              <p>Nhập số tiền muốn chuyển:</p>
              <InputNumber
                style={{ width: '100%' }}
                min={1}
                max={displayWallet.promoBalance}
                value={convertAmount}
                onChange={(val) =>
                  setConvertAmount(Number(val || 0))
                }
                formatter={(value) =>
                  `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
                }
                parser={(value) =>
                  value ? Number(value.replace(/,/g, '')) : 0
                }
              />
            </div>
          </Modal>

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
