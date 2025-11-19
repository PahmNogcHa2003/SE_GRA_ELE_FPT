import React from "react";
import { useMutation } from "@tanstack/react-query";
import { Button, Card, Col, Form, Input, Row, Typography, message, Divider } from "antd";
import { createContact } from "../../services/contact.service";
import type { CreateContactDTO } from "../../types/contact";

const { Title, Paragraph, Text, Link } = Typography;
const { TextArea } = Input;

const MAX = {
  email: 255,
  phone: 50,
  title: 150,
  content: 4000,
};

const ContactPage: React.FC = () => {
  const [form] = Form.useForm<CreateContactDTO>();

  const { mutateAsync, isPending } = useMutation({
    mutationKey: ["create-contact"],
    mutationFn: createContact,
    onSuccess: (resp) => {
      message.success(resp?.message ?? "Gửi liên hệ thành công!");
      form.resetFields();
    },
    onError: (err: any) => {
      const serverMsg =
        err?.response?.data?.message ||
        err?.response?.data?.error ||
        err?.message ||
        "Gửi liên hệ thất bại. Vui lòng thử lại!";
      message.error(serverMsg);
    },
  });

  const onFinish = async (values: CreateContactDTO) => {
    const payload: CreateContactDTO = {
      email: values.email?.trim() || undefined,
      phoneNumber: values.phoneNumber?.trim() || undefined,
      title: values.title.trim(),
      content: values.content.trim(),
    };
    await mutateAsync(payload);
  };

  return (
    <div className="container mx-auto px-4 py-6">
      <Row gutter={[24, 24]}>
        {/* Left: Form */}
        <Col xs={24} lg={14}>
          <Card
            title={<Title level={3} style={{ margin: 0 }}>Liên hệ với HolaBike</Title>}
            bordered
            style={{ borderRadius: 16 }}
          >
            <Paragraph type="secondary" style={{ marginTop: 8 }}>
              Bạn có thắc mắc, góp ý hay cần hỗ trợ? Hãy để lại thông tin, đội ngũ của chúng tôi sẽ phản hồi sớm nhất.
            </Paragraph>

            <Divider style={{ margin: "12px 0 24px" }} />

            <Form
              form={form}
              layout="vertical"
              onFinish={onFinish}
              scrollToFirstError
              requiredMark="optional"
            >
              <Form.Item
                label="Email"
                name="email"
                rules={[
                  { required: true, message: "Vui lòng nhập email" },
                  { type: "email", message: "Email không hợp lệ" },
                  { max: MAX.email, message: `Tối đa ${MAX.email} ký tự` },
                ]}
              >
                <Input placeholder="nguyenvanA@gmail.com" allowClear />
              </Form.Item>

              <Form.Item
                label="Số điện thoại"
                name="phoneNumber"
                rules={[
                  { max: MAX.phone, message: `Tối đa ${MAX.phone} ký tự` },
                  {
                    pattern: /^[0-9+()\-\s]*$/,
                    message: "Số điện thoại chỉ gồm số và ký tự (+) ( ) - khoảng trắng",
                  },
                ]}
              >
                <Input placeholder="+84 912 345 678" allowClear />
              </Form.Item>

              <Form.Item
                label="Tiêu đề"
                name="title"
                rules={[
                  { required: true, message: "Vui lòng nhập tiêu đề" },
                  { max: MAX.title, message: `Tối đa ${MAX.title} ký tự` },
                ]}
              >
                <Input placeholder="Nội dung liên hệ..." showCount maxLength={MAX.title} />
              </Form.Item>

              <Form.Item
                label="Nội dung"
                name="content"
                rules={[
                  { required: true, message: "Vui lòng nhập nội dung" },
                  { max: MAX.content, message: `Tối đa ${MAX.content} ký tự` },
                ]}
              >
                <TextArea
                  placeholder="Mô tả chi tiết vấn đề bạn gặp phải hoặc góp ý..."
                  rows={6}
                  showCount
                  maxLength={MAX.content}
                />
              </Form.Item>

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  size="large"
                  loading={isPending}
                  style={{ borderRadius: 10 }}
                >
                  Gửi liên hệ
                </Button>
              </Form.Item>

              <Text type="secondary">
                * Dữ liệu của bạn sẽ được xử lý theo Chính sách Quyền riêng tư. Chúng tôi không chia sẻ thông tin liên hệ cho bên thứ ba.
              </Text>
            </Form>
          </Card>
        </Col>

        {/* Right: Info */}
        <Col xs={24} lg={10}>
          <Card bordered style={{ borderRadius: 16 }}>
            <Title level={4} style={{ marginTop: 0 }}>Thông tin hỗ trợ</Title>
            <Paragraph>
              <Text strong>Hotline:</Text> <Text>1900 1234</Text>
              <br />
              <Text strong>Email hỗ trợ:</Text> <Link href="mailto:support@holabike.vn">support@EcoJourney.vn</Link>
              <br />
              <Text strong>Thời gian:</Text> <Text>08:00 - 21:00 (T2 - CN)</Text>
            </Paragraph>

            <Divider />

            <Title level={5}>Văn phòng</Title>
            <Paragraph>
              Khu Công Nghệ Cao Hòa Lạc, km 29, Đại lộ, Thăng Long, Hà Nội
              <br />
              <Text type="secondary">Tìm chúng tôi trên bản đồ hoặc đặt lịch hẹn trước.</Text>
            </Paragraph>

            <div style={{ width: "100%", height: 220, borderRadius: 12, overflow: "hidden" }}>
                <iframe
                    title="Văn phòng EcoJourney"
                    src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1862.2518580667536!2d105.52402855769394!3d21.0125218422715!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135abc60e7d3f19%3A0x2be9d7d0b5abcbf4!2zVHLGsOG7nW5nIMSQ4bqhaSBo4buNYyBGUFQgSMOgIE7hu5lp!5e0!3m2!1svi!2s!4v1762022910180!5m2!1svi!2s"
                    width="100%"
                    height="100%"
                    style={{ border: 0 }}
                    allowFullScreen
                    loading="lazy"
                    referrerPolicy="no-referrer-when-downgrade"
                ></iframe>
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default ContactPage;
