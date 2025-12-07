// src/pages/user/TopUpPage.tsx
import React, { useMemo, useState, useEffect, useRef } from 'react';
import { useAuth } from '../../features/auth/context/authContext';
import { useMutation } from '@tanstack/react-query';
import vnpaylogo from '../../assets/images/vnpay-logo.png';
import { createPaymentUrl } from '../../services/payment.service';
import {
  Form,
  InputNumber,
  Button,
  Alert,
  Radio,
  Spin,
  Tooltip,
  Card,
} from 'antd';
import { motion } from 'framer-motion';
import {
  FaExclamationTriangle,
  FaGift,
  FaShieldAlt,
  FaCreditCard,
  FaBolt,
  FaLeaf,
} from 'react-icons/fa';
import type { ApiResponse } from '../../types/api';
import type {
  CreatePaymentPayload,
  PaymentUrlResponse,
} from '../../types/payment';

// ===== Utility =====
const presetAmounts = [20000, 50000, 100000, 200000, 300000, 500000];
const formatCurrency = (amount: number) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
const calculateBonus = (amount: number) => {
  if (amount >= 500000) return amount * 0.2;
  if (amount >= 200000) return amount * 0.15;
  if (amount >= 100000) return amount * 0.1;
  return 0;
};

// ===== Fireworks Canvas =====
const Fireworks: React.FC<{ trigger: boolean }> = ({ trigger }) => {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    if (!trigger) return;
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d')!;
    const particles: any[] = [];
    const colors = ['#FFD700', '#FF69B4', '#00FFFF', '#FF4500', '#ADFF2F'];

    const createParticle = (x: number, y: number) => {
      for (let i = 0; i < 80; i++) {
        const angle = (Math.PI * 2 * i) / 80;
        const speed = Math.random() * 3 + 2;
        particles.push({
          x,
          y,
          dx: Math.cos(angle) * speed,
          dy: Math.sin(angle) * speed,
          life: 60 + Math.random() * 40,
          color: colors[Math.floor(Math.random() * colors.length)],
        });
      }
    };

    const render = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      particles.forEach((p, i) => {
        p.x += p.dx;
        p.y += p.dy;
        p.dy += 0.05;
        p.life -= 1;
        if (p.life <= 0) particles.splice(i, 1);
        ctx.beginPath();
        ctx.arc(p.x, p.y, 2, 0, Math.PI * 2);
        ctx.fillStyle = p.color;
        ctx.fill();
      });
      if (particles.length > 0) requestAnimationFrame(render);
    };

    // Khởi tạo nổ tại 3 vị trí
    const width = canvas.width = canvas.offsetWidth;
    const height = canvas.height = canvas.offsetHeight;
    createParticle(width / 2, height / 2);
    setTimeout(() => createParticle(width / 3, height / 1.5), 400);
    setTimeout(() => createParticle((width * 2) / 3, height / 3), 800);
    render();

    const timeout = setTimeout(() => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
    }, 3500);

    return () => clearTimeout(timeout);
  }, [trigger]);

  return (
    <canvas
      ref={canvasRef}
      className="absolute inset-0 w-full h-full pointer-events-none"
    />
  );
};

const TopUpPage: React.FC = () => {
  const [form] = Form.useForm();
  const amount = Form.useWatch('amount', form) as number | undefined;
  const { user } = useAuth();
  const [paymentMethod, setPaymentMethod] = useState<'VNPAY' | 'ZALOPAY'>('VNPAY');
  const [showFireworks, setShowFireworks] = useState(false);

  const { mutate, isPending, error } = useMutation<
    PaymentUrlResponse,
    Error | ApiResponse<null>,
    { payload: CreatePaymentPayload }
  >({
    mutationFn: ({ payload }) => createPaymentUrl(payload),
    onSuccess: (res) => {
      if (res.paymentUrl) window.location.href = res.paymentUrl;
    },
  });

  const onFinish = (values: { amount: number }) => {
    if (!user) return;
    mutate({ payload: { amount: values.amount } });
  };

  const bonus = useMemo(() => (amount ? calculateBonus(amount) : 0), [amount]);
  const canSubmit = useMemo(() => !!amount && amount >= 20000 && !isPending, [amount, isPending]);

  // 🎆 Hiệu ứng pháo hoa khi đạt mốc 500k
  useEffect(() => {
    if (amount && amount >= 500000) {
      setShowFireworks(true);
      const timer = setTimeout(() => setShowFireworks(false), 4000);
      return () => clearTimeout(timer);
    }
  }, [amount]);

  return (
    <div className="min-h-screen bg-linear-to-br from-emerald-50 via-white to-emerald-100 py-12 px-4 flex items-center justify-center">
      <div className="max-w-6xl w-full grid grid-cols-1 md:grid-cols-2 gap-10 items-center">
        {/* LEFT SIDE — PROMOTION PANEL */}
        <motion.div
          initial={{ opacity: 0, x: -40 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.6 }}
          className="relative bg-linear-to-br from-eco-green-dark via-emerald-700 to-emerald-500 rounded-3xl p-10 text-white shadow-2xl overflow-hidden flex flex-col justify-between h-full"
        >
          {/* Fireworks */}
          <Fireworks trigger={showFireworks} />

          {/* Background overlay */}
          <div className="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1518709268805-4e9042af9f23?auto=format&fit=crop&q=80&w=1400')] bg-cover bg-center opacity-15"></div>
          <div className="absolute inset-0 bg-linear-to-t from-black/20 via-transparent to-eco-green-dark/20"></div>

          {/* Content */}
          <div className="relative z-10 flex flex-col items-center text-center space-y-5">
            <motion.div
              initial={{ y: -10, opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              transition={{ delay: 0.2 }}
              className="flex items-center gap-3 text-3xl font-bold"
            >
              <FaLeaf className="text-yellow-300" />
              <span>EcoJourney Rewards</span>
            </motion.div>

            <motion.h1
              className="text-4xl md:text-5xl font-extrabold leading-snug tracking-tight drop-shadow-lg"
              initial={{ scale: 0.9 }}
              animate={{ scale: 1 }}
              transition={{ duration: 0.4 }}
            >
              🎉 ƯU ĐÃI NẠP ĐIỂM THÁNG NÀY!
            </motion.h1>

            <p className="text-lg text-white/90">
              Nạp càng nhiều – Nhận càng lớn 🎁  
              Cơ hội tăng điểm thưởng lên đến <b className="text-yellow-300">+20%</b>!
            </p>

            {/* Bonus tiers */}
            <div className="mt-6 grid grid-cols-1 sm:grid-cols-3 gap-4 w-full text-center">
              {[
                { percent: '+10%', desc: '≥ 100.000đ' },
                { percent: '+15%', desc: '≥ 200.000đ' },
                { percent: '+20%', desc: '≥ 500.000đ' },
              ].map((p, i) => (
                <motion.div
                  key={i}
                  whileHover={{ scale: 1.05 }}
                  className="bg-white/15 backdrop-blur-md rounded-xl p-4 border border-white/20 shadow-md"
                >
                  <h3 className="text-2xl font-bold text-yellow-300">{p.percent}</h3>
                  <p className="text-sm text-white/90">{p.desc}</p>
                </motion.div>
              ))}
            </div>

            {/* Highlight */}
            <motion.div
              className="mt-8 bg-yellow-300/20 border border-yellow-400/60 rounded-xl px-6 py-4 inline-flex items-center justify-center gap-3 shadow-lg backdrop-blur-md"
              animate={{ scale: [1, 1.05, 1] }}
              transition={{ duration: 2, repeat: Infinity }}
            >
              <FaGift className="text-yellow-300 text-3xl" />
              <span className="font-semibold text-lg">
                Nạp ngay để nhận thưởng tự động 🎁
              </span>
            </motion.div>
          </div>

          <div className="relative z-10 mt-10 text-sm text-white/80 text-center border-t border-white/20 pt-3">
            🌿 Đi xe xanh – Sống xanh cùng EcoJourney!
          </div>
        </motion.div>

        {/* RIGHT SIDE — TOP UP FORM */}
        <motion.div
          initial={{ opacity: 0, x: 40 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.6 }}
        >
          <Card className="shadow-2xl rounded-3xl border-none overflow-hidden backdrop-blur-lg bg-white/90">
            <Spin spinning={isPending} tip="Đang tạo yêu cầu..." size="large">
              <Form
                form={form}
                layout="vertical"
                onFinish={onFinish}
                initialValues={{ amount: 100000 }}
                className="space-y-6 p-6 md:p-8"
              >
                <div className="text-center mb-2">
                  <h3 className="font-bold text-2xl text-eco-green-dark mb-1">
                    💰 Nạp điểm EcoJourney
                  </h3>
                  <p className="text-gray-500 text-sm">
                    Số dư dùng để thuê xe, thanh toán nhanh & nhận thưởng khuyến mãi.
                  </p>
                </div>

                <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
                  {presetAmounts.map((v) => {
                    const active = amount === v;
                    return (
                      <motion.button
                        whileTap={{ scale: 0.95 }}
                        key={v}
                        type="button"
                        onClick={() => form.setFieldsValue({ amount: v })}
                        className={[
                          'rounded-xl py-4 font-semibold border text-lg transition-all shadow-sm',
                          active
                            ? 'bg-eco-green-dark text-white border-eco-green-dark shadow-md scale-105'
                            : 'bg-white hover:bg-emerald-50 text-emerald-700 border-emerald-200',
                        ].join(' ')}
                      >
                        {formatCurrency(v)}
                      </motion.button>
                    );
                  })}
                </div>

                <Form.Item
                  name="amount"
                  label={<span className="font-semibold text-gray-800">Hoặc nhập số tiền (VND)</span>}
                  rules={[
                    { required: true, message: 'Vui lòng nhập số tiền!' },
                    {
                      validator: (_, v) =>
                        (v ?? 0) >= 20000
                          ? Promise.resolve()
                          : Promise.reject('Tối thiểu 20.000đ'),
                    },
                  ]}
                >
                  <InputNumber
                    className="w-full h-10 text-base px-3 rounded-lg border border-emerald-300 focus:border-eco-green focus:ring-1 focus:ring-eco-green outline-none transition-all" 
                    style={{ width: '100%', fontWeight: 600 }}
                    precision={0}
                    min={20000}
                    step={1000}
                    inputMode="numeric"
                    formatter={(v) => `${v}`.replace(/\B(?=(\d{3})+(?!\d))/g, '.')}
                    parser={(v: string | undefined) => (v ? Number(v.replace(/\./g, '')) : 0)}
                  />
                </Form.Item>

                {bonus > 0 && (
                  <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    className="flex items-center justify-center gap-3 bg-yellow-50 border border-yellow-400 rounded-xl py-3 px-4 text-yellow-800 shadow-sm"
                  >
                    <FaBolt className="text-yellow-500 text-2xl" />
                    <span className="font-semibold text-lg">
                      + Nhận thưởng {formatCurrency(bonus)} khi nạp!
                    </span>
                  </motion.div>
                )}

                <Alert
                  message={
                    <span className="text-gray-800 font-medium">
                      Số tiền nạp tối thiểu <b>20.000đ</b>. Tiền nạp{' '}
                      <u className="text-red-500 font-semibold underline underline-offset-2">
                        không hoàn lại
                      </u>.
                    </span>
                  }
                  type="warning"
                  icon={<FaExclamationTriangle />}
                  showIcon
                  className="bg-yellow-50 border-yellow-300"
                />

                <div>
                  <h3 className="font-semibold text-gray-800 mb-2 text-lg">
                    Phương thức thanh toán
                  </h3>
                  <Radio.Group
                    onChange={(e) => setPaymentMethod(e.target.value)}
                    value={paymentMethod}
                    className="flex flex-col sm:flex-row gap-3"
                  >
                    <Radio.Button
                      value="VNPAY"
                      className="px-6 py-4 flex-1 rounded-xl border-2 border-emerald-200 hover:border-eco-green-dark transition-all"
                    >
                      <div className="flex items-center gap-3 justify-center">
                        <img src={vnpaylogo} className="h-6" alt="VNPay" />
                        <span className="font-medium text-emerald-800">VNPay</span>
                      </div>
                    </Radio.Button>

                    <Tooltip title="Sắp ra mắt">
                      <Radio.Button
                        value="ZALOPAY"
                        disabled
                        className="px-6 py-4 flex-1 rounded-xl border-2"
                      >
                        <div className="flex items-center justify-center gap-3 text-gray-400">
                          <FaCreditCard />
                          <span>ZaloPay</span>
                        </div>
                      </Radio.Button>
                    </Tooltip>
                  </Radio.Group>

                  <p className="text-xs text-gray-500 mt-3 text-center">
                    * Thanh toán được bảo mật bởi <b>{paymentMethod}</b> với chứng chỉ{' '}
                    <FaShieldAlt className="inline text-emerald-600" /> SSL.
                  </p>
                </div>

                <Form.Item className="mb-0">
                  <Button
                    type="primary"
                    htmlType="submit"
                    className="w-full h-14 bg-linear-to-r from-eco-green-dark to-emerald-500 text-lg font-bold hover:opacity-90 rounded-xl shadow-lg"
                    loading={isPending}
                    disabled={!canSubmit}
                  >
                    {amount ? `Nạp ${formatCurrency(amount)}` : 'Nạp điểm ngay'}
                  </Button>
                </Form.Item>

                {error && (
                  <Alert
                    message="Đã xảy ra lỗi"
                    description={
                      (error as any).message || 'Không thể tạo yêu cầu. Vui lòng thử lại.'
                    }
                    type="error"
                    showIcon
                    className="mt-4"
                  />
                )}
              </Form>
            </Spin>
          </Card>
        </motion.div>
      </div>
    </div>
  );
};

export default TopUpPage;
