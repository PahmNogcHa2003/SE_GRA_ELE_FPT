export interface AvailableVoucherDTO {
  id: number;
  code: string;
  description: string;
  value: number;
  isPercentage: boolean;
  endDate: string;

  minOrderAmount?: number;
  isApplicable: boolean;
  notApplicableReason?: string;

  displayValue: string;
}
