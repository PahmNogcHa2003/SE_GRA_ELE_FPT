export const strongPasswordRules = [
  { required: true, message: 'Vui lòng nhập mật khẩu mới.' },
  {
    min: 8,
    message: 'Mật khẩu phải có ít nhất 8 ký tự.',
  },
  {
    validator: (_: any, value: string) => {
      if (!value) return Promise.resolve();

      if (!/[A-Z]/.test(value)) {
        return Promise.reject(
          'Mật khẩu phải chứa ít nhất 1 chữ cái viết hoa (A–Z).'
        );
      }
      if (!/[a-z]/.test(value)) {
        return Promise.reject(
          'Mật khẩu phải chứa ít nhất 1 chữ cái viết thường (a–z).'
        );
      }
      if (!/[0-9]/.test(value)) {
        return Promise.reject(
          'Mật khẩu phải chứa ít nhất 1 chữ số (0–9).'
        );
      }
      if (!/[!@#$%^&*()_\-+=\[\]{};:'",.<>/?\\|`~]/.test(value)) {
        return Promise.reject(
          'Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.'
        );
      }

      return Promise.resolve();
    },
  },
];
