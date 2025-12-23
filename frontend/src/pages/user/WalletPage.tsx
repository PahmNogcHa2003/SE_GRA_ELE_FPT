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
import { App, Spin, Table, Modal, InputNumber, Card, Statistic, Row, Col, Button, Typography, Divider } from 'antd';
import type { TableProps } from 'antd';
import type { WalletTransaction, Wallet } from '../../types/wallet';
import { 
  WalletOutlined, 
  GiftOutlined, 
  CreditCardOutlined, 
  HistoryOutlined,
  ArrowRightOutlined,
  CheckCircleOutlined,
  PlusCircleOutlined,
  DollarCircleOutlined
} from '@ant-design/icons';

const { Title, Text } = Typography;

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
    title: 'Mã GD',
    dataIndex: 'id',
    key: 'id',
    width: 100,
    render: (id: number) => (
      <Text strong className="text-gray-700">
        #{id.toString().slice(-6)}
      </Text>
    ),
  },
  {
    title: 'Thời gian',
    dataIndex: 'createdAt',
    key: 'createdAt',
    render: (date: string) => (
      <div>
        <div className="font-medium">
          {new Date(date).toLocaleDateString('vi-VN')}
        </div>
        <div className="text-xs text-gray-500">
          {new Date(date).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}
        </div>
      </div>
    ),
  },
  {
    title: 'Số tiền',
    dataIndex: 'amount',
    key: 'amount',
    render: (amount: number, record: WalletTransaction) => (
      <div className={`text-lg font-bold ${record.direction === 'In' ? 'text-green-600' : 'text-red-600'}`}>
        {record.direction === 'In' ? '+' : '-'}
        {formatCurrency(Math.abs(amount))}
      </div>
    ),
  },
  {
    title: 'Loại giao dịch',
    dataIndex: 'direction',
    key: 'details',
    render: (direction: string, record: WalletTransaction) => {
      const icon = direction === 'In' ? 
        <PlusCircleOutlined className="text-green-500 mr-2" /> : 
        <DollarCircleOutlined className="text-red-500 mr-2" />;
      
      return (
        <div className="flex items-center">
          {icon}
          <div>
            <div className="font-medium">
              {direction === 'In' ? 'Nạp tiền' : 'Thanh toán'}
            </div>
            <div className="text-sm text-gray-500">
              {record.source}
            </div>
          </div>
        </div>
      );
    },
  },
  {
    title: 'Số dư sau',
    dataIndex: 'balanceAfter',
    key: 'balanceAfter',
    render: (balanceAfter: number) => (
      <div className="text-right">
        <div className="font-semibold text-blue-600">
          {formatCurrency(balanceAfter)}
        </div>
        <div className="text-xs text-gray-500">Tài khoản chính</div>
      </div>
    ),
  },
  {
    title: 'Khuyến mãi sau',
    dataIndex: 'promoAfter',
    key: 'promoAfter',
    render: (promoAfter: number) => (
      <div className="text-right">
        <div className="font-semibold text-orange-500">
          {formatCurrency(promoAfter)}
        </div>
        <div className="text-xs text-gray-500">Khuyến mãi</div>
      </div>
    ),
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
        message.success({
          content: `✅ Đã chuyển thành công ${formatCurrency(amount)} sang tài khoản chính!`,
          duration: 3,
        });
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
        message.error({
          content: `❌ ${msg}`,
          duration: 4,
        });
      },
    });

  // ====== MUTATION: Pay all debt ======
  const { mutateAsync: mutatePayAllDebt, isPending: isPayingDebt } =
    useMutation({
      mutationFn: () => payAllDebt(),
      onSuccess: (res) => {
        const msg =
          res?.message ||
          '✅ Đã thanh toán toàn bộ nợ thành công từ ví chính.';
        message.success({
          content: msg,
          duration: 3,
        });
        queryClient.invalidateQueries({ queryKey: ['wallet'] });
        queryClient.invalidateQueries({
          queryKey: ['walletTransactions'],
        });
      },
      onError: (error: any) => {
        const msg =
          error?.response?.data?.message ||
          error?.response?.data?.Message ||
          '❌ Thanh toán nợ không thành công.';
        message.error({
          content: msg,
          duration: 4,
        });
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
      <div className="flex flex-col justify-center items-center min-h-[60vh]">
        <Spin size="large" />
        <Text className="mt-4 text-gray-600">Đang tải thông tin ví...</Text>
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
      message.info({
        content: 'ℹ️ Bạn không có số dư khuyến mãi để chuyển.',
        duration: 3,
      });
      return;
    }

    setConvertAmount(maxAmount); // mặc định chuyển hết
    setIsConvertModalOpen(true);
  };

  // ====== HANDLE: xác nhận chuyển promo ======
  const handleConfirmConvert = async () => {
    const maxAmount = displayWallet.promoBalance;

    if (!convertAmount || convertAmount <= 0) {
      message.warning({
        content: '⚠️ Vui lòng nhập số tiền hợp lệ.',
        duration: 3,
      });
      return;
    }

    if (convertAmount > maxAmount) {
      message.warning({
        content: '⚠️ Số tiền chuyển không được vượt quá số dư hiện có.',
        duration: 3,
      });
      return;
    }

    await mutateConvertPromo(convertAmount);
  };

  // ====== HANDLE: thanh toán toàn bộ nợ ======
  const handlePayAllDebt = () => {
    if (displayWallet.totalDebt <= 0) {
      message.info({
        content: 'ℹ️ Bạn không có khoản nợ nào cần thanh toán.',
        duration: 3,
      });
      return;
    }

    modal.confirm({
      title: (
        <div className="flex items-center gap-2">
          <CreditCardOutlined className="text-red-500 text-xl" />
          <span className="text-lg font-semibold">Thanh toán toàn bộ nợ</span>
        </div>
      ),
      content: (
        <div className="space-y-3">
          <Text>Số tiền nợ cần thanh toán:</Text>
          <div className="text-2xl font-bold text-red-600 text-center">
            {formatCurrency(displayWallet.totalDebt)}
          </div>
          <Text type="secondary" className="text-sm">
            Hệ thống sẽ dùng số dư ví chính để thanh toán toàn bộ nợ.
          </Text>
        </div>
      ),
      okText: 'Xác nhận thanh toán',
      cancelText: 'Hủy',
      okButtonProps: { 
        loading: isPayingDebt,
        danger: true,
        icon: <CheckCircleOutlined />
      },
      cancelButtonProps: { type: 'default' },
      onOk: async () => {
        await mutatePayAllDebt();
      },
    });
  };

  return (
    <div className="min-h-screen bg-linear-to-b from-gray-50 to-gray-100">
      <div className="container mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center justify-between mb-4">
            <div className="flex items-center gap-3">
              <div className="p-3 bg-linear-to-r from-emerald-500 to-emerald-600 rounded-xl shadow-lg">
                <WalletOutlined className="text-2xl text-white" />
              </div>
              <div>
                <Title level={2} className="mb-1! text-gray-800">
                  Ví Của Tôi
                </Title>
                <Text type="secondary">
                  Quản lý số dư và lịch sử giao dịch
                </Text>
              </div>
            </div>
          </div>
        </div>

        {/* Wallet Summary Cards */}
        <Row gutter={[16, 16]} className="mb-8">
          <Col xs={24} md={8}>
            <Card 
              className="h-full border-0 shadow-lg rounded-2xl hover:shadow-xl transition-shadow duration-300"
              style={{
                background: 'linear-gradient(135deg, #10B981 0%, #059669 100%)',
              }}
            >
              <Statistic
                title={
                  <div className="flex items-center gap-2 text-white">
                    <WalletOutlined />
                    <span>Tài khoản chính</span>
                  </div>
                }
                value={displayWallet.balance}
                prefix="₫"
                valueStyle={{ 
                  fontSize: '32px',
                  color: '#FFFFFF',
                  fontWeight: 'bold'
                }}
                suffix={
                  <Text className="text-emerald-100 text-sm ml-2">
                    (Điểm = VND)
                  </Text>
                }
              />
              <div className="mt-4 pt-4 border-t border-white/20">
                <Text className="text-emerald-100 text-sm">
                  Số dư có thể sử dụng ngay
                </Text>
              </div>
            </Card>
          </Col>
          
          <Col xs={24} md={8}>
            <Card 
              className="h-full border-0 shadow-lg rounded-2xl hover:shadow-xl transition-shadow duration-300"
              style={{
                background: 'linear-gradient(135deg, #F59E0B 0%, #D97706 100%)',
              }}
            >
              <Statistic
                title={
                  <div className="flex items-center gap-2 text-white">
                    <GiftOutlined />
                    <span>Tài khoản khuyến mãi</span>
                  </div>
                }
                value={displayWallet.promoBalance}
                prefix="₫"
                valueStyle={{ 
                  fontSize: '32px',
                  color: '#FFFFFF',
                  fontWeight: 'bold'
                }}
                suffix={
                  <Text className="text-amber-100 text-sm ml-2">
                    (Khuyến mãi khả dụng)
                  </Text>
                }
              />
              <div className="mt-4 pt-4 border-t border-white/20">
                <Text className="text-amber-100 text-sm">
                  Có thể chuyển sang tài khoản chính
                </Text>
              </div>
            </Card>
          </Col>
          
          <Col xs={24} md={8}>
            <Card 
              className="h-full border-0 shadow-lg rounded-2xl hover:shadow-xl transition-shadow duration-300"
              style={{
                background: 'linear-gradient(135deg, #EF4444 0%, #DC2626 100%)',
              }}
            >
              <Statistic
                title={
                  <div className="flex items-center gap-2 text-white">
                    <CreditCardOutlined />
                    <span>Nợ cước</span>
                  </div>
                }
                value={displayWallet.totalDebt}
                prefix="₫"
                valueStyle={{ 
                  fontSize: '32px',
                  color: '#FFFFFF',
                  fontWeight: 'bold'
                }}
                suffix={
                  <Text className="text-red-100 text-sm ml-2">
                    (Vui lòng thanh toán)
                  </Text>
                }
              />
              <div className="mt-4 pt-4 border-t border-white/20">
                <Text className="text-red-100 text-sm">
                  Thanh toán để tiếp tục sử dụng dịch vụ
                </Text>
              </div>
            </Card>
          </Col>
        </Row>

        {/* Action Buttons */}
        <Card className="mb-8 border-0 shadow-lg rounded-2xl">
          <Title level={4} className="mb-6! flex items-center gap-2">
            <HistoryOutlined />
            Thao tác với ví
          </Title>
          <Row gutter={[16, 16]}>
            <Col xs={24} md={8}>
              <Button
                type="primary"
                size="large"
                icon={<WalletOutlined />}
                onClick={() => navigate('/top-up')}
                className="w-full h-16 bg-linear-to-r from-emerald-500 to-emerald-600 border-0 hover:from-emerald-600 hover:to-emerald-700"
                style={{
                  fontSize: '16px',
                  fontWeight: '600'
                }}
              >
                <div className="flex flex-col items-center">
                  <span>Nạp Tiền</span>
                  <span className="text-xs font-normal opacity-90">Tăng số dư</span>
                </div>
              </Button>
            </Col>
            
            <Col xs={24} md={8}>
              <Button
                type="primary"
                size="large"
                icon={<ArrowRightOutlined />}
                onClick={handleConvertPromo}
                disabled={displayWallet.promoBalance <= 0 || isConverting}
                loading={isConverting}
                className={`w-full h-16 ${
                  displayWallet.promoBalance <= 0 || isConverting
                    ? 'bg-gray-300 border-0 cursor-not-allowed'
                    : 'bg-linear-to-r from-amber-500 to-amber-600 border-0 hover:from-amber-600 hover:to-amber-700'
                }`}
                style={{
                  fontSize: '16px',
                  fontWeight: '600'
                }}
              >
                <div className="flex flex-col items-center">
                  <span>Chuyển khuyến mãi</span>
                  <span className="text-xs font-normal opacity-90">
                    {formatCurrency(displayWallet.promoBalance)}
                  </span>
                </div>
              </Button>
            </Col>
            
            <Col xs={24} md={8}>
              <Button
                type="primary"
                size="large"
                icon={<CheckCircleOutlined />}
                onClick={handlePayAllDebt}
                disabled={displayWallet.totalDebt <= 0 || isPayingDebt}
                loading={isPayingDebt}
                danger
                className={`w-full h-16 ${
                  displayWallet.totalDebt <= 0 || isPayingDebt
                    ? 'bg-gray-300 border-0 cursor-not-allowed'
                    : 'bg-linear-to-r from-red-500 to-red-600 border-0 hover:from-red-600 hover:to-red-700'
                }`}
                style={{
                  fontSize: '16px',
                  fontWeight: '600'
                }}
              >
                <div className="flex flex-col items-center">
                  <span>Thanh toán nợ</span>
                  <span className="text-xs font-normal opacity-90">
                    {formatCurrency(displayWallet.totalDebt)}
                  </span>
                </div>
              </Button>
            </Col>
          </Row>
        </Card>

        {/* Transaction History */}
        <Card className="border-0 shadow-lg rounded-2xl">
          <div className="flex flex-col md:flex-row md:items-center justify-between mb-6">
            <div className="flex items-center gap-3 mb-4 md:mb-0">
              <div className="p-2 bg-blue-100 rounded-lg">
                <HistoryOutlined className="text-blue-600 text-xl" />
              </div>
              <div>
                <Title level={4} className="mb-0!">Lịch sử giao dịch</Title>
                <Text type="secondary">Chi tiết các giao dịch trong ví</Text>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Text type="secondary" className="text-sm">
                Hiển thị {transactionsData?.items?.length || 0} giao dịch
              </Text>
            </div>
          </div>

          <Divider className="my-2" />

          <Table
            columns={getColumns()}
            dataSource={transactionsData?.items || []}
            rowKey="id"
            loading={isLoadingTransactions}
            pagination={{
              current: transactionsData?.pageNumber,
              pageSize: transactionsData?.pageSize,
              total: transactionsData?.totalCount,
              showSizeChanger: true,
              showQuickJumper: true,
              showTotal: (total, range) => `${range[0]}-${range[1]} của ${total} giao dịch`,
              className: 'mt-4'
            }}
            onChange={handleTableChange}
            scroll={{ x: true }}
            className="custom-table"
          />
        </Card>

        {/* Modal Chuyển khuyến mãi */}
        <Modal
          title={
            <div className="flex items-center gap-3">
              <div className="p-2 bg-amber-100 rounded-lg">
                <GiftOutlined className="text-amber-600 text-xl" />
              </div>
              <div>
                <div className="text-lg font-semibold">Chuyển khuyến mãi</div>
                <div className="text-sm text-gray-500">
                  Chuyển từ tài khoản khuyến mãi sang tài khoản chính
                </div>
              </div>
            </div>
          }
          open={isConvertModalOpen}
          onCancel={() => setIsConvertModalOpen(false)}
          onOk={handleConfirmConvert}
          okText="Xác nhận chuyển"
          cancelText="Hủy"
          confirmLoading={isConverting}
          okButtonProps={{
            icon: <ArrowRightOutlined />,
            className: 'bg-amber-500 hover:bg-amber-600 border-amber-500'
          }}
          width={500}
          className="rounded-lg"
        >
          <div className="space-y-6 py-4">
            <div className="p-4 bg-amber-50 rounded-lg border border-amber-200">
              <div className="flex justify-between items-center">
                <Text strong>Số dư khuyến mãi hiện tại:</Text>
                <Text strong className="text-2xl text-amber-600">
                  {formatCurrency(displayWallet.promoBalance)}
                </Text>
              </div>
            </div>

            <div>
              <div className="mb-3">
                <Text strong className="block mb-2">Số tiền muốn chuyển:</Text>
                <InputNumber
                  style={{ width: '100%' }}
                  size="large"
                  min={1}
                  max={displayWallet.promoBalance}
                  value={convertAmount}
                  onChange={(val) => setConvertAmount(Number(val || 0))}
                  formatter={(value) =>
                    `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
                  }
                  parser={(value) =>
                    value ? Number(value.replace(/,/g, '')) : 0
                  }
                  addonBefore="₫"
                  className="rounded-lg"
                />
              </div>
              
              <div className="flex gap-2">
                <Button 
                  size="small" 
                  onClick={() => setConvertAmount(Math.floor(displayWallet.promoBalance * 0.25))}
                  className="flex-1"
                >
                  25%
                </Button>
                <Button 
                  size="small" 
                  onClick={() => setConvertAmount(Math.floor(displayWallet.promoBalance * 0.5))}
                  className="flex-1"
                >
                  50%
                </Button>
                <Button 
                  size="small" 
                  onClick={() => setConvertAmount(Math.floor(displayWallet.promoBalance * 0.75))}
                  className="flex-1"
                >
                  75%
                </Button>
                <Button 
                  size="small" 
                  onClick={() => setConvertAmount(displayWallet.promoBalance)}
                  className="flex-1 bg-amber-100 text-amber-700 border-amber-300"
                >
                  Tất cả
                </Button>
              </div>
            </div>

            <Divider className="my-2" />

            <div className="p-4 bg-emerald-50 rounded-lg border border-emerald-200">
              <div className="flex justify-between items-center">
                <Text>Số dư chính sau khi chuyển:</Text>
                <Text strong className="text-lg text-emerald-600">
                  {formatCurrency(displayWallet.balance + convertAmount)}
                </Text>
              </div>
            </div>
          </div>
        </Modal>
      </div>
    </div>
  );
};

export default WalletPage;