import React from "react";
import { FaCheckCircle } from "react-icons/fa";
import { Button } from "antd";
import { useNavigate } from "react-router-dom";
import { formatUtcToVN, currencyVN } from "../../utils/datetime";

interface InvoiceCardProps {
  order?: {
    orderNo: string;
    total: number;
    paidAt?: string;
    createdAt?: string;
    status: string;
  };
  transaction?: {
    amount: number;
    balanceAfter: number;
    source: string;
    createdAt: string;
  };
  message?: string;
}

const InvoiceCard: React.FC<InvoiceCardProps> = ({ order, transaction, message }) => {
  const navigate = useNavigate();
  if (!order && !transaction) return null;

  return (
    <div className="relative bg-white rounded-2xl shadow-lg border border-eco-green/40 p-8 max-w-xl w-full mx-auto text-gray-800 overflow-hidden">
      <div className="absolute inset-0 bg-linear-to-br from-eco-green/5 via-transparent to-eco-green/10 pointer-events-none" />
      <div className="relative flex flex-col items-center text-center mb-6">
        <FaCheckCircle className="text-eco-green text-5xl mb-3" />
        <h2 className="text-2xl font-bold text-eco-green-dark">Thanh toán thành công!</h2>
        <p className="text-gray-600 mt-2">
          {message || "Giao dịch đã được ghi nhận vào tài khoản của bạn."}
        </p>
      </div>

      <div className="relative bg-gray-50 rounded-xl p-5 border border-eco-green/20 mb-6">
        <h3 className="text-lg font-semibold text-eco-green mb-4 text-center">🧾 Chi tiết hóa đơn</h3>
        <div className="grid grid-cols-2 gap-x-4 gap-y-2 text-sm sm:text-base text-gray-700">
          {order && (
            <>
              <p className="text-gray-500">Mã đơn hàng</p>
              <p className="font-medium">{order.orderNo}</p>

              <p className="text-gray-500">Tổng tiền</p>
              <p className="font-semibold text-eco-green-dark">{currencyVN(order.total)}</p>

              <p className="text-gray-500">Ngày tạo</p>
              <p>{formatUtcToVN(order.createdAt)}</p>

              {order.paidAt && (
                <>
                  <p className="text-gray-500">Ngày thanh toán</p>
                  <p>{formatUtcToVN(order.paidAt)}</p>
                </>
              )}

              <p className="text-gray-500">Trạng thái</p>
              <p className="font-medium">{order.status}</p>
            </>
          )}

          {transaction && (
            <>
              <p className="text-gray-500">Nguồn thanh toán</p>
              <p className="font-medium">{transaction.source}</p>

              <p className="text-gray-500">Số dư sau giao dịch</p>
              <p>{currencyVN(transaction.balanceAfter)}</p>

            </>
          )}
        </div>
      </div>

      <div className="relative flex justify-center">
        <Button
          type="primary"
          onClick={() => navigate("/wallet")}
          className="bg-eco-green hover:bg-eco-green-dark text-white px-8 py-2 rounded-lg font-semibold"
        >
          Về trang Ví
        </Button>
      </div>

      <p className="text-center text-sm text-gray-500 mt-4">
        Cần hỗ trợ? Liên hệ{" "}
        <a href="/contact" className="text-eco-green hover:underline">
          bộ phận CSKH
        </a>.
      </p>
    </div>
  );
};

export default InvoiceCard;
