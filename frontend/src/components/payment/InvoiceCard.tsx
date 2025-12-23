import React from "react";
import { 
  FaCheckCircle, 
  FaWallet, 
  FaGift, 
  FaReceipt, 
  FaClock, 
  FaCreditCard, 
  FaArrowRight 
} from "react-icons/fa";
import { Button, Divider } from "antd";
import { useNavigate } from "react-router-dom";
import { formatUtcToVN, currencyVN } from "../../utils/datetime";

// --- 1. CẬP NHẬT INTERFACE ĐỂ KHỚP VỚI PAYMENT RESULT ---
interface InvoiceCardProps {
  order?: {
    orderNo: string;
    total: number;
    paidAt?: string;
    createdAt?: string;
    status: string;
  };
  // Sửa lại cấu trúc transaction để khớp với PaymentResult
  transaction?: {
    wallet?: {
      amount: number;
      balanceAfter: number;
      source: string;
      createdAt: string;
    };
    promo?: {
      amount: number;
      promoAfter: number;
      source: string;
      createdAt: string;
    } | null;
  };
  message?: string;
}

const InvoiceCard: React.FC<InvoiceCardProps> = ({ order, transaction, message }) => {
  const navigate = useNavigate();

  // Kiểm tra nếu không có dữ liệu quan trọng
  if (!order && !transaction) return null;

  // Lấy thông tin chung từ ví chính (nếu có)
  const walletData = transaction?.wallet;
  const promoData = transaction?.promo;

  // Xác định ngày hiển thị
  const displayDate = order?.paidAt || order?.createdAt || walletData?.createdAt || new Date().toISOString();
  
  // Kiểm tra có tiền khuyến mãi không
  const hasPromo = promoData && promoData.promoAfter !== undefined;

  return (
    <div className="w-full max-w-lg mx-auto">
      <div className="relative bg-white rounded-3xl shadow-2xl overflow-hidden border border-gray-100 font-sans">
        
        <div className="h-3 bg-linear-to-r from-emerald-400 via-eco-green to-teal-500" />

        <div className="p-8 pt-10 text-center">
          <div className="inline-flex items-center justify-center w-20 h-20 bg-emerald-50 rounded-full mb-6 ring-4 ring-emerald-50/50">
            <FaCheckCircle className="text-eco-green text-5xl drop-shadow-sm" />
          </div>

          <h2 className="text-2xl font-bold text-gray-800 mb-2">Thanh toán thành công!</h2>
          <p className="text-gray-500 text-sm max-w-xs mx-auto leading-relaxed">
            {message || "Giao dịch nạp điểm của bạn đã hoàn tất."}
          </p>

          <div className="mt-6 mb-8 py-4 bg-gray-50/50 rounded-2xl border border-gray-100 dashed-border">
            <p className="text-gray-400 text-xs font-bold uppercase tracking-wider mb-1">Tổng tiền nạp</p>
            <span className="text-4xl font-extrabold text-eco-green-dark tracking-tight">
              {order ? currencyVN(order.total) : "---"}
            </span>
          </div>

          {/* === PHẦN SỬA ĐỔI: TRUY CẬP VÀO WALLET VÀ PROMO === */}
          {transaction && (
            <div className="grid grid-cols-2 gap-4 mb-8">
              {/* Ví chính */}
              <div className="bg-blue-50/60 rounded-xl p-3 border border-blue-100 flex flex-col items-center justify-center">
                <div className="flex items-center gap-2 text-blue-600 text-xs font-bold uppercase mb-1">
                  <FaWallet /> Ví chính
                </div>
                <span className="font-bold text-gray-800 text-lg">
                  {/* Sửa: Truy cập qua walletData */}
                  {walletData ? currencyVN(walletData.balanceAfter) : "--"}
                </span>
              </div>

              {/* Ví khuyến mãi */}
              <div className={`rounded-xl p-3 border flex flex-col items-center justify-center ${
                hasPromo ? 'bg-pink-50/60 border-pink-100' : 'bg-gray-50 border-gray-100'
              }`}>
                <div className={`flex items-center gap-2 text-xs font-bold uppercase mb-1 ${
                  hasPromo ? 'text-pink-500' : 'text-gray-400'
                }`}>
                  <FaGift /> Ví Promo
                </div>
                <span className={`font-bold text-lg ${
                  hasPromo ? 'text-pink-600' : 'text-gray-400'
                }`}>
                  {/* Sửa: Truy cập qua promoData */}
                  {hasPromo ? currencyVN(promoData.promoAfter) : "--"}
                </span>
              </div>
            </div>
          )}

          <Divider dashed className="border-gray-300 my-6" />

          <div className="space-y-4 text-sm">
            <DetailRow 
              label="Mã giao dịch" 
              value={order?.orderNo} 
              icon={<FaReceipt className="text-gray-400" />} 
            />
            <DetailRow 
              label="Thời gian" 
              value={formatUtcToVN(displayDate)} 
              icon={<FaClock className="text-gray-400" />}
            />
            <DetailRow 
              label="Nguồn tiền" 
              // Sửa: Lấy source từ walletData
              value={walletData?.source || "VNPAY"} 
              icon={<FaCreditCard className="text-gray-400" />}
            />
            <DetailRow 
              label="Trạng thái" 
              value={
                <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                  {order?.status === "Success" ? "Đã thanh toán" : order?.status}
                </span>
              } 
            />
          </div>
        </div>

        <div className="bg-gray-50 p-6 border-t border-gray-100">
          <Button
            type="primary"
            size="large"
            onClick={() => navigate("/wallet")}
            className="w-full bg-eco-green hover:bg-eco-green-dark h-12 rounded-xl font-bold shadow-lg shadow-emerald-100 border-none flex items-center justify-center gap-2 transition-all transform hover:scale-[1.02]"
          >
            Về ví của tôi <FaArrowRight />
          </Button>
          
          <div className="text-center mt-4">
             <button onClick={() => navigate("/support")} className="text-xs text-gray-400 hover:text-eco-green underline transition-colors">
                Cần hỗ trợ về giao dịch này?
             </button>
          </div>
        </div>
      </div>
    </div>
  );
};

const DetailRow: React.FC<{ label: string; value: React.ReactNode; icon?: React.ReactNode }> = ({ label, value, icon }) => (
  <div className="flex justify-between items-center group">
    <span className="text-gray-500 flex items-center gap-2">
      {icon} {label}
    </span>
    <span className="font-semibold text-gray-700 text-right">
      {value || "---"}
    </span>
  </div>
);

export default InvoiceCard;