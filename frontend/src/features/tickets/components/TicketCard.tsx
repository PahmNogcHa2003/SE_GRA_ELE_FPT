// src/features/tickets/components/TicketCard.tsx
import React from "react";
import { Button, Tag, Tooltip } from "antd";
import { FaClock, FaCalendarAlt, FaCheckCircle, FaSpinner, FaBolt, FaTicketAlt, FaChargingStation, FaBicycle } from "react-icons/fa";
import type { UserTicket } from "../../../types/user.ticket";
import { effectiveExpiry, formatUtcToVN } from "../../../utils/datetime";

// map activationMode sang chuỗi dễ đọc
const mapMode = (m: number | string | undefined): "IMMEDIATE" | "ON_FIRST_USE" =>
  m === 1 || m === "ON_FIRST_USE" ? "ON_FIRST_USE" : "IMMEDIATE";

// style theo status
const getStatusStyles = (status?: UserTicket["status"]) => {
  switch (status) {
    case "Active":
      return {
        bgColor: "bg-blue-500",
        textColor: "text-blue-600",
        chipColor: "blue",
        text: "ĐANG HOẠT ĐỘNG",
        icon: <FaSpinner className="animate-spin" />,
      };
    case "Ready":
      return {
        bgColor: "bg-eco-green",
        textColor: "text-eco-green",
        chipColor: "green",
        text: "SẴN SÀNG",
        icon: <FaCheckCircle />,
      };
    case "Expired":
      return {
        bgColor: "bg-gray-400",
        textColor: "text-gray-500",
        chipColor: "default",
        text: "HẾT HẠN",
        icon: <FaCheckCircle />,
      };
    case "Used":
      return {
        bgColor: "bg-gray-400",
        textColor: "text-gray-500",
        chipColor: "default",
        text: "ĐÃ SỬ DỤNG",
        icon: <FaCheckCircle />,
      };
    default:
      return {
        bgColor: "bg-gray-400",
        textColor: "text-gray-500",
        chipColor: "default",
        text: "KHÔNG XÁC ĐỊNH",
        icon: <FaCheckCircle />,
      };
  }
};
// Lấy ngày hết hạn hiệu lực (ưu tiên ValidTo hơn ExpiresAt)
const getEffectiveExpiry = (t: UserTicket) => effectiveExpiry({ validTo: (t as any).validTo, expiresAt: (t as any).expiresAt });

const isSubscriptionTicket = (t: UserTicket) => {
  const validFrom = (t as any).validFrom as string | undefined | null;
  const validTo = (t as any).validTo as string | undefined | null;
  const expiresAt = (t as any).expiresAt as string | undefined | null;
  // Nếu có ValidTo hoặc ExpiresAt => coi là gói theo thời gian
  return !!(validTo || expiresAt || validFrom);
};

interface TicketCardProps {
  ticket: UserTicket;
}

const TicketCard: React.FC<TicketCardProps> = ({ ticket }) => {
  const styles = getStatusStyles(ticket.status);
  const mode = mapMode((ticket as any).activationMode);
  const validFrom = (ticket as any).validFrom as string | undefined | null;
  const validTo = (ticket as any).validTo as string | undefined | null;
  const expiresAt = (ticket as any).expiresAt as string | undefined | null;
  const activationDeadline = (ticket as any).activationDeadline as string | undefined | null;
  const effectiveExp = getEffectiveExpiry(ticket);
  const isSub = isSubscriptionTicket(ticket);
  const vehicleType = (ticket as any).vehicleType as "Bike" | "EBike" | undefined;
  return (
    <div className="bg-white rounded-xl shadow-lg overflow-hidden flex transform transition-all duration-300 hover:shadow-2xl hover:-translate-y-1">
      <div className={`flex flex-col items-center justify-center w-24 p-4 ${styles.bgColor} text-white relative`}>
        <div className="absolute -right-3 top-1/2 -translate-y-1/2 w-6 h-6 bg-gray-100 rounded-full" />
        <div className="transform -rotate-90 whitespace-nowrap flex items-center gap-2">
          {styles.icon}
          <span className="text-base md:text-lg font-bold tracking-wider uppercase">{styles.text}</span>
        </div>
      </div>

      <div className="border-l-2 border-dashed border-gray-200" />
      <div className="p-5 flex-1 flex flex-col justify-between">
        <div>
          <div className="flex items-center justify-between gap-3 flex-wrap">
            <h3 className={`text-2xl font-bold ${styles.textColor}`}>{ticket.planName || "Gói vé"}</h3>
            <div className="flex items-center gap-2">
              <Tag color={styles.chipColor}>{ticket.status || "Unknown"}</Tag>
              <Tag color={mode === "ON_FIRST_USE" ? "purple" : "green"}>
                {mode === "ON_FIRST_USE" ? "Kích hoạt khi dùng lần đầu" : "Kích hoạt ngay"}
              </Tag>
              {isSub ? <Tag color="blue">Gói theo thời gian</Tag> : <Tag color="magenta">Vé lượt</Tag>}
              {vehicleType === "EBike" && (
                <Tag color="cyan">
                  Xe điện
                </Tag>
              )}
              {vehicleType === "Bike" && (
                <Tag  color="orange">
                  Xe đạp
                </Tag>
              )}
            </div>
          </div>

          <div className="mt-3 grid grid-cols-1 md:grid-cols-2 gap-3 text-gray-700">
            {ticket.activatedAt && (
              <div className="flex items-center">
                <FaBolt className="mr-3 text-amber-500" />
                <span>
                  Đã kích hoạt:&nbsp;
                  <strong>{formatUtcToVN(ticket.activatedAt)}</strong>
                </span>
              </div>
            )}

            {mode === "ON_FIRST_USE" && activationDeadline && (
              <div className="flex items-center">
                <FaCalendarAlt className="mr-3 text-violet-500" />
                <span>
                  Hạn kích hoạt:&nbsp;
                  <strong>{formatUtcToVN(activationDeadline)}</strong>
                </span>
              </div>
            )}

            {validFrom && (
              <div className="flex items-center">
                <FaCalendarAlt className="mr-3 text-eco-green" />
                <span>
                  Hiệu lực từ:&nbsp;
                  <strong>{formatUtcToVN(validFrom)}</strong>
                </span>
              </div>
            )}

            {(validTo || expiresAt) && (
              <div className="flex items-center">
                <FaCalendarAlt className="mr-3 text-rose-500" />
                <span>
                  Hết hạn:&nbsp;
                  <strong>{formatUtcToVN(effectiveExp || undefined)}</strong>
                </span>
              </div>
            )}

            {/* Thời lượng / lượt còn lại */}
            {typeof ticket.remainingMinutes === "number" && (
              <div className="flex items-center">
                <FaClock className="mr-3 text-gray-400" />
                <span>
                  Phút còn lại:&nbsp;<strong>{ticket.remainingMinutes}</strong>
                </span>
              </div>
            )}

            {typeof (ticket as any).remainingRides === "number" && (
              <div className="flex items-center">
                <FaTicketAlt className="mr-3 text-gray-400" />
                <span>
                  Lượt còn lại:&nbsp;<strong>{(ticket as any).remainingRides}</strong>
                </span>
              </div>
            )}

            {/* Serial & giá */}
            {ticket.serialCode && (
              <div className="flex items-center">
                <Tooltip title="Mã vé dùng để đối soát giao dịch khi cần.">
                  <span className="text-xs text-gray-500">Mã vé:</span>
                </Tooltip>
                <span className="ml-2 font-mono text-sm">{ticket.serialCode}</span>
              </div>
            )}
            {typeof ticket.purchasedPrice === "number" && (
              <div className="flex items-center">
                <span className="text-xs text-gray-500">Giá mua:</span>
                <span className="ml-2 font-semibold">
                  {ticket.purchasedPrice.toLocaleString("vi-VN")} đ
                </span>
              </div>
            )}

            {ticket.createdAt && (
              <div className="flex items-center">
                <span className="text-xs text-gray-500">Ngày tạo:</span>
                <span className="ml-2">{formatUtcToVN(ticket.createdAt)}</span>
              </div>
            )}
          </div>
        </div>

        {/* Nút hành động (chỉ thông tin/disabled) */}
        <div className="mt-6 text-right">
          {ticket.status === "Ready" && (
            <Button disabled className="bg-eco-green! text-white! border-eco-green! shadow-sm! cursor-default!">
              Sẵn sàng sử dụng
            </Button>
          )}

          {ticket.status === "Active" && (
            <Button disabled className="bg-eco-green! text-white! border-eco-green! shadow-sm! cursor-default!">
              Đang hoạt động
            </Button>
          )}

          {(ticket.status === "Expired" || ticket.status === "Used") && (
            <Button disabled className="bg-eco-green/60! text-white! border-eco-green! shadow-sm! cursor-not-allowed!">
              Đã sử dụng / Hết hạn
            </Button>
          )}
        </div>
      </div>
    </div>
  );
};

export default TicketCard;
