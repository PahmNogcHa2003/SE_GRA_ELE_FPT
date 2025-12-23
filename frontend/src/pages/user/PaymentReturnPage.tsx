import React, { useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useAuth } from '../../features/auth/context/authContext';
import { processVnPayReturn } from '../../services/payment.service';
import { Result, Button, Spin } from 'antd';
import InvoiceCard from '../../components/payment/InvoiceCard';

const PaymentReturnPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { token } = useAuth();
  const queryClient = useQueryClient();
  const queryString = searchParams.toString();

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ['vnPayReturn', queryString],
    queryFn: () => processVnPayReturn(queryString),
    enabled: !!queryString && !!token,
    retry: false,
  });

  useEffect(() => {
    if (data?.rspCode === '00') {
      queryClient.invalidateQueries({ queryKey: ['wallet'] });
      queryClient.invalidateQueries({ queryKey: ['walletTransactions'] });
    }
  }, [data, queryClient]);

  if (isLoading) {
    return (
      <div className="flex flex-col justify-center items-center h-screen">
        <Spin size="large" />
        <p className="text-xl text-eco-green-dark mt-4">
          Đang xác nhận thanh toán, vui lòng chờ...
        </p>
      </div>
    );
  }

  if (isError) {
    return (
      <Result
        status="error"
        title="Xác nhận thanh toán thất bại"
        subTitle={(error as Error).message || "Đã có lỗi xảy ra. Vui lòng liên hệ hỗ trợ."}
        extra={[
          <Button type="primary" key="wallet" onClick={() => navigate('/wallet')}>
            Về trang Ví
          </Button>,
          <Button key="retry" onClick={() => navigate('/top-up')}>
            Thử nạp lại
          </Button>,
        ]}
      />
    );
  }

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50">
      {data?.rspCode === '00' ? (
        <InvoiceCard
          message={data.message}
          order={data.order}
          transaction={
            (data.transaction as any) || {
              amount: 0,
              balanceAfter: 0,
              source: '',
              createdAt: new Date().toISOString(),
            }
          }
        />
      ) : data?.rspCode === '02' ? (
        <Result
          status="info"
          title="Giao dịch đã được xử lý"
          subTitle="Hoá đơn này đã được xác nhận trước đó. Số dư ví của bạn không thay đổi."
          extra={[
            <Button
              key="wallet"
              type="primary"
              onClick={() => navigate('/wallet')}
              className="bg-eco-green hover:bg-eco-green-dark"
            >
              Về trang Ví
            </Button>,
          ]}
        />
      ) : (
        <Result
          status="warning"
          title="Giao dịch chưa hoàn tất"
          subTitle={data?.message || "Giao dịch đã bị hủy hoặc xảy ra lỗi. Tiền chưa bị trừ."}
          extra={[
            <Button
              type="primary"
              key="wallet"
              onClick={() => navigate('/wallet')}
              className="bg-eco-green hover:bg-eco-green-dark"
            >
              Về trang Ví
            </Button>,
            <Button key="retry" onClick={() => navigate('/top-up')}>
              Thử nạp lại
            </Button>,
          ]}
        />
      )}
    </div>
  );
};

export default PaymentReturnPage;