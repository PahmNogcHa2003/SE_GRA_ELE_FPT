// src/pages/user/TopUpPage.tsx
import React, { useMemo, useState } from 'react';
import { useAuth } from '../../features/auth/context/authContext';
import { useMutation } from '@tanstack/react-query';
import { createPaymentUrl } from '../../services/payment.service';
import { Form, InputNumber, Button, Alert, Radio, Spin, Divider, Tooltip } from 'antd';
import { FaExclamationTriangle } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import type { ApiResponse } from '../../types/api';
import type { CreatePaymentPayload, PaymentUrlResponse } from '../../types/payment';

const presetAmounts = [20000, 50000, 100000, 200000, 300000, 400000];

const formatCurrency = (amount: number) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);

const formatPlain = (amount: number) =>
  new Intl.NumberFormat('vi-VN').format(amount);

const TopUpPage: React.FC = () => {
  const [form] = Form.useForm();
  const amount = Form.useWatch('amount', form) as number | undefined;
  const { user } = useAuth();
  const [paymentMethod, setPaymentMethod] = useState<'VNPAY' | 'ZALOPAY'>('VNPAY');
  const navigate = useNavigate();

  const { mutate, isPending, error } = useMutation<
    PaymentUrlResponse,
    Error | ApiResponse<null>,
    { payload: CreatePaymentPayload }
  >({
    mutationFn: ({ payload }) => createPaymentUrl(payload),
    onSuccess: (response) => {
      if (response.paymentUrl) window.location.href = response.paymentUrl;
      else console.error('Không nhận được URL thanh toán');
    },
    onError: (err) => console.error('Lỗi khi tạo URL:', err),
  });
  const MIN_TOPUP = 10000;
  const formatThousands = (v?: number | string) =>
    v === undefined || v === null || v === ''
      ? ''
      : v.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');

  const parseThousands = (v?: string) =>
    v ? Number(v.replace(/\./g, '').replace(/\D/g, '')) : 0;

  const onFinish = (values: { amount: number }) => {
    if (!user) return;
    mutate({ payload: { amount: values.amount } });
  };

  const handlePresetClick = (value: number) => {
    form.setFieldsValue({ amount: value });
  };

  const canSubmit = useMemo(
    () => !!amount && amount >= 10000 && !isPending,
    [amount, isPending]
  );

  return (
    <div className="bg-linear-to-b from-emerald-50 via-white to-white min-h-screen py-10">
      <div className="container mx-auto px-4 max-w-3xl">
        {/* Header */}
        <div className="rounded-2xl overflow-hidden shadow-lg">
          <div className="w-full bg-eco-green-dark text-white text-center p-6 md:p-8">
            <h1 className="text-3xl md:text-4xl font-extrabold tracking-tight">NẠP ĐIỂM</h1>
            <p className="mt-2 text-white/80">
              Tăng số dư để thuê xe nhanh chóng trên Eco Journey
            </p>
          </div>

          {/* Card body */}
          <div className="bg-white p-6 md:p-10">
            <Spin spinning={isPending} tip="Đang tạo yêu cầu...">
              <Form
                form={form}
                onFinish={onFinish}
                layout="vertical"
                className="space-y-6"
                initialValues={{ amount: 10000 }}
              >
                {/* Số tiền */}
                <div>
                  <div className="flex items-center justify-between mb-2">
                    <label className="font-semibold text-gray-800">
                      Số tiền muốn nạp (VND)
                    </label>
                    {amount ? (
                      <span className="text-sm text-gray-500">
                        Tương đương:{' '}
                        <span className="font-semibold">
                          {formatPlain(amount)} điểm
                        </span>
                      </span>
                    ) : null}
                  </div>

                  <Form.Item
                    name="amount"
                    rules={[
                      { required: true, message: 'Vui lòng nhập số tiền!' },
                      {
                        validator: (_, v) =>
                          parseThousands(v?.toString()) >= MIN_TOPUP
                            ? Promise.resolve()
                            : Promise.reject(`Tối thiểu ${MIN_TOPUP.toLocaleString('vi-VN')}đ`),
                      },
                    ]}
                    >
                    <InputNumber<number>  // 👈 ép generic là number
                      className="w-full h-10 text-base px-3 rounded-lg"
                      style={{ width: '100%', height: 40, lineHeight: '38px', marginBottom: '1.2rem' }}
                      precision={0}
                      formatter={(v) => (v === undefined || v === null ? '' : formatThousands(String(v)))}
                      parser={(v) => parseThousands(v)}
                      min={MIN_TOPUP}     // hoặc min={10000 as number}
                      step={1000}
                      inputMode="numeric"
                    />
                  </Form.Item>

                  {/* Quick picks */}
                  <div className="grid grid-cols-2 sm:grid-cols-3 gap-3 mb-6">
                    {presetAmounts.map((v) => {
                      const active = amount === v;
                      return (
                        <button
                          key={v}
                          type="button"
                          onClick={() => handlePresetClick(v)}
                          className={[
                            'rounded-xl border px-4 py-3 text-center transition-all',
                            active
                              ? 'bg-eco-green text-white border-eco-green shadow'
                              : 'bg-emerald-50 text-emerald-700 border-emerald-200 hover:bg-emerald-100',
                          ].join(' ')}
                        >
                          {formatPlain(v)} đ
                        </button>
                      );
                    })}
                  </div>
                </div>

                {/* Cảnh báo */}
                <Alert
                  message={
                    <span className="font-semibold text-gray-800">
                      Số tiền nạp tối thiểu 10.000đ. Tiền nạp điểm vào tài khoản sẽ
                      <u className="font-semibold text-red-500 underline underline-offset-2">
                        {' '}không hoàn lại
                      </u>.
                    </span>
                  }
                  type="warning"
                  showIcon
                  icon={<FaExclamationTriangle />}
                  className="mb-6"
                />

                <Divider className="my-2" />

                {/* Phương thức thanh toán */}
                <div>
                  <h3 className="font-semibold text-gray-800 mb-2">
                    Chọn phương thức thanh toán
                  </h3>

                  <div className="rounded-2xl border border-gray-200 p-4 md:p-5">
                    <Radio.Group
                      onChange={(e) => setPaymentMethod(e.target.value)}
                      value={paymentMethod}
                      className="flex flex-wrap gap-3"
                    >
                      <Radio.Button value="VNPAY" className="px-4 py-2">
                        <div className="flex items-center gap-3">
                          <img
                            src="https://upload.wikimedia.org/wikipedia/commons/4/4e/VNPAY_logo.svg"
                            className="h-6"
                            alt="VNPay"
                          />
                          <span className="font-medium">VNPay</span>
                        </div>
                      </Radio.Button>

                      <Tooltip title="Sắp ra mắt">
                        <Radio.Button
                          value="ZALOPAY"
                          disabled
                          className="px-4 py-2"
                        >
                          ZaloPay
                        </Radio.Button>
                      </Tooltip>
                    </Radio.Group>

                    <p className="text-xs text-gray-500 mt-3">
                      * Thanh toán được bảo mật và xử lý qua cổng {paymentMethod}.
                    </p>
                  </div>
                </div>

                {/* Lỗi */}
                {error && (
                  <Alert
                    message="Đã xảy ra lỗi"
                    description={
                      (error as any).message ||
                      'Không thể tạo yêu cầu. Vui lòng thử lại.'
                    }
                    type="error"
                    showIcon
                  />
                )}

                {/* CTA */}
                <Form.Item className="mb-0">
                  <Button
                    type="primary"
                    htmlType="submit"
                    className="w-full h-14 bg-eco-green-dark text-lg font-bold hover:bg-eco-green-dark/90 rounded-xl"
                    loading={isPending}
                    disabled={!canSubmit}
                  >
                    {amount ? `Nạp ${formatCurrency(amount)}` : 'Nạp điểm'}
                  </Button>
                </Form.Item>
              </Form>

              {/* Footer note */}
              <div className="mt-8 rounded-xl bg-emerald-50 p-4 text-emerald-800 text-sm">
                <p className="font-semibold">Mẹo an toàn</p>
                <ul className="list-disc ml-5 mt-1 space-y-1">
                  <li>
                    Chỉ nạp qua cổng thanh toán chính thức của Eco Journey.
                  </li>
                  <li>
                    Không chia sẻ mã OTP hoặc thông tin thẻ cho bất kỳ ai.
                  </li>
                  <li>
                    Nếu gặp sự cố, liên hệ hỗ trợ trong mục “Trợ giúp”.
                  </li>
                </ul>
              </div>
            </Spin>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TopUpPage;
