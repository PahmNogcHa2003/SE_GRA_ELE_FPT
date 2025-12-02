import dayjs from "dayjs";
import utc from "dayjs/plugin/utc";
import timezone from "dayjs/plugin/timezone";

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.tz.setDefault("Asia/Ho_Chi_Minh");

/**
 * Convert 1 ISO/UTC string -> giờ Việt Nam, định dạng “DD/MM/YYYY HH:mm:ss”
 */
export const formatUtcToVN = (iso?: string | null) => {
  if (!iso) return "—";
  return dayjs.utc(iso).tz("Asia/Ho_Chi_Minh").format("DD/MM/YYYY HH:mm:ss");
};

/**
 * Hạn thực tế của vé: ưu tiên ValidTo; nếu không có thì dùng ExpiresAt
 */
export const effectiveExpiry = (t: { validTo?: string | null; expiresAt?: string | null }) =>
  t?.validTo ?? t?.expiresAt ?? null;

/**
 * Hiển thị VNĐ
 */
export const currencyVN = (n?: number | null) =>
  `${(n ?? 0).toLocaleString("vi-VN")} đ`;
