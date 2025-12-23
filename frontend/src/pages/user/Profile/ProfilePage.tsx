import React, { useEffect, useMemo, useState } from "react";
import {
  App,
  Badge,
  Card,
  Col,
  Form,
  Input,
  Row,
  Button,
  Skeleton,
  Alert,
  Tag,
  Divider,
  Space,
  Typography,
  DatePicker,
  Select,
  Tabs,
  ConfigProvider, // Thêm cái này để chỉnh style Tabs
} from "antd";
import { UserOutlined, TrophyOutlined, FlagOutlined } from "@ant-design/icons"; // Thêm icon cho đẹp
import type {
  UserProfileDTO,
  UpdateUserProfileBasicDTO,
} from "../../../types/userProfile";
import { getMyProfile, updateMyProfile } from "../../../services/profile.service";
import AvatarUploader from "../../../components/profile/AvatarUploader";
import { useAuth } from "../../../features/auth/context/authContext";
import ProfileQuestsTab from "./ProfileQuestsTab"; 
import dayjs from "dayjs";
import ProfileAchievementsTab from "./ProfileAchievementsTab";
import { useLocation } from "react-router-dom";

const { Item } = Form;
const { Title, Text } = Typography;

const ProfilePage: React.FC = () => {
  const { message } = App.useApp();
  const { isLoggedIn } = useAuth();
  const [form] = Form.useForm<Partial<UserProfileDTO>>();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [profile, setProfile] = useState<UserProfileDTO | null>(null);
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);
  const defaultTab = searchParams.get("tab") || "profile";
  // Trạng thái verify
  const verifyState = useMemo(() => {
    const raw = profile?.isVerify?.toString().toLowerCase() ?? "";
    const isVerified =
      raw === "approved" || raw === "verified" || raw === "true";
    const isPending = raw === "pending";
    const isRejected = raw === "rejected";

    let color: "green" | "gold" | "red" | "default" = "default";
    let label = "Chưa xác minh";

    if (isVerified) {
      color = "green";
      label = "ĐÃ XÁC MINH";
    } else if (isPending) {
      color = "gold";
      label = "ĐANG CHỜ DUYỆT";
    } else if (isRejected) {
      color = "red";
      label = "BỊ TỪ CHỐI";
    }

    return { isVerified, isPending, isRejected, color, label };
  }, [profile?.isVerify]);

  // Load profile
  useEffect(() => {
    (async () => {
      try {
        const data = await getMyProfile();
        setProfile(data);
        form.setFieldsValue({
          ...data,
          dob: data.dob ? dayjs(data.dob) : undefined,
        } as any);
      } catch (e) {
        console.error(e);
        message.error("Không tải được hồ sơ của bạn");
      } finally {
        setLoading(false);
      }
    })();
  }, [form, message]);

  // Khi avatar đổi
  const onAvatarChange = (url: string | null) => {
    form.setFieldValue("avatarUrl", url || undefined);
    setProfile((p) => (p ? { ...p, avatarUrl: url || undefined } : p));
  };

  const onFinish = async (values: any) => {
    const payload: UpdateUserProfileBasicDTO = {
      fullName: values.fullName ?? undefined,
      avatarUrl: values.avatarUrl ?? undefined,
      emergencyName: values.emergencyName!,
      emergencyPhone: values.emergencyPhone!,
      addressDetail: values.addressDetail ?? undefined,
      phoneNumber: values.phoneNumber ?? undefined,
      dob: values.dob ? values.dob.format("YYYY-MM-DD") : undefined,
      gender: values.gender ?? undefined,
    };

    try {
      setSaving(true);
      const updated = await updateMyProfile(payload);
      setProfile(updated);
      form.setFieldsValue({
        ...updated,
        dob: updated.dob ? dayjs(updated.dob) : undefined,
      } as any);
      message.success("Cập nhật hồ sơ thành công");
    } catch (e: any) {
      console.error(e);
      message.error(e?.response?.data?.message || "Cập nhật thất bại");
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-[60vh] flex items-center justify-center bg-gray-50">
        <div className="w-full max-w-3xl px-4">
          <Skeleton active paragraph={{ rows: 10 }} />
        </div>
      </div>
    );
  }

  if (!isLoggedIn) {
    return (
      <div className="min-h-[60vh] flex items-center justify-center bg-gray-50">
        <div className="w-full max-w-3xl px-4">
          <Card className="shadow-md rounded-2xl">
            <Alert
              type="warning"
              showIcon
              message="Bạn chưa đăng nhập"
              description="Vui lòng đăng nhập để xem và cập nhật trang cá nhân."
            />
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-emerald-50 py-8 min-h-screen">
      <div className="container mx-auto px-4 max-w-8xl">
        <Card className="shadow-md rounded-2xl border-0 bg-white/80 backdrop-blur">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
            <div className="space-y-3 flex-1">
              <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-emerald-50 text-emerald-700 text-xs font-medium">
                <span className="inline-block h-2 w-2 rounded-full bg-emerald-500" />
                Hồ sơ EcoJourney
              </div>

              <div>
                <Title level={3} className="mb-1!">
                  Trang cá nhân
                </Title>
                <Text type="secondary">
                  Cập nhật thông tin liên hệ và theo dõi trạng thái xác minh tài
                  khoản của bạn.
                </Text>
              </div>

              <Space size="small" direction="vertical">
                <div className="flex flex-wrap items-center gap-2 text-sm">
                  <span className="text-gray-500">Trạng thái xác minh:</span>
                  <Tag color={verifyState.color} className="font-semibold">
                    {verifyState.label}
                  </Tag>
                </div>
                {verifyState.isPending && (
                  <Text type="secondary" className="text-xs">
                    Hồ sơ KYC đang được hệ thống kiểm tra. Bạn sẽ nhận được
                    thông báo khi có kết quả.
                  </Text>
                )}
                {verifyState.isRejected && (
                  <Text type="danger" className="text-xs">
                    Hồ sơ KYC đã bị từ chối. Vui lòng liên hệ hỗ trợ để được
                    hướng dẫn cập nhật lại thông tin.
                  </Text>
                )}
              </Space>
            </div>

            <div className="flex justify-center md:justify-end flex-1">
              <AvatarUploader
                value={profile?.avatarUrl || null}
                onChange={onAvatarChange}
                size={120}
              />
            </div>
          </div>
        </Card>

        <style>{`
            /* 1. Tùy chỉnh thanh chứa Tabs (Container) */
            .custom-separated-tabs .ant-tabs-nav {
                margin-bottom: 0 !important; /* Xóa margin mặc định */
            }

            /* 2. Xóa đường gạch chân xám mặc định của Tabs */
            .custom-separated-tabs .ant-tabs-nav::before {
                display: none !important;
            }

            /* 3. Style cho từng Tab (Viên thuốc) */
            .custom-separated-tabs .ant-tabs-tab {
                background: #fff; /* Nền trắng */
                border: 1px solid #e5e7eb; /* Viền xám nhạt */
                border-radius: 99px !important; /* Bo tròn hoàn toàn */
                padding: 8px 24px !important;
                margin-right: 12px !important; /* Khoảng cách giữa các nút tab */
                transition: all 0.3s ease;
            }

            /* 4. Trạng thái khi Tab được chọn */
            .custom-separated-tabs .ant-tabs-tab-active {
                background: #10b981 !important; /* Nền xanh Eco */
                border-color: #10b981 !important;
                box-shadow: 0 4px 6px -1px rgba(16, 185, 129, 0.2); /* Bóng đổ nhẹ */
            }

            .custom-separated-tabs .ant-tabs-tab-active .ant-tabs-tab-btn {
                color: #fff !important; /* Chữ trắng */
                font-weight: 600;
            }

            /* 5. Ẩn thanh gạch chân chạy chạy (Ink Bar) */
            .custom-separated-tabs .ant-tabs-ink-bar {
                display: none !important;
            }

            /* 6. QUAN TRỌNG: Tách nội dung bên dưới ra xa thanh Tabs */
            .custom-separated-tabs .ant-tabs-content-holder {
                margin-top: 24px !important; /* Tạo khoảng cách lớn */
            }
        `}</style>
      <div className="mt-4">
          <ConfigProvider
            theme={{
                components: {
                    Tabs: {
                        itemColor: '#64748b', // Màu chữ khi chưa chọn
                        itemHoverColor: '#10b981',
                        fontSize: 15,
                    }
                }
            }}
            >
            <Tabs
            className="mt-8 custom-separated-tabs" 
            defaultActiveKey={defaultTab}
            items={[
                {
                key: "profile",
                label: <span className="flex items-center gap-2"><UserOutlined /> Hồ sơ cá nhân</span>,
                children: (
                    <Row gutter={[24, 24]} align="stretch">
                    {/* FORM BÊN TRÁI */}
                    <Col xs={24} lg={14}>
                        <Card className="shadow-sm rounded-2xl h-full border border-gray-100">
                        <div className="flex items-center justify-between mb-4">
                            <div>
                            <Title level={4} className="mb-0!">
                                Thông tin cơ bản
                            </Title>
                            <Text type="secondary" className="text-xs">
                                Những thông tin này dùng để liên hệ và hỗ trợ bạn
                                khi cần thiết.
                            </Text>
                            </div>
                        </div>

                        <Divider className="my-3!" />

                        <Form
                            form={form}
                            layout="vertical"
                            onFinish={onFinish}
                            className="space-y-1"
                        >
                            <Item label="Họ và tên" name="fullName">
                            <Input size="large" placeholder="Nguyễn Văn A" />
                            </Item>

                            <Row gutter={12}>
                            <Col span={12}>
                                <Item label="Ngày sinh" name="dob">
                                <DatePicker
                                    className="w-full"
                                    size="large"
                                    format="DD/MM/YYYY"
                                    placeholder="Chọn ngày sinh"
                                />
                                </Item>
                            </Col>

                            <Col span={12}>
                                <Item label="Giới tính" name="gender">
                                <Select
                                    size="large"
                                    placeholder="Chọn giới tính"
                                    options={[
                                    { label: "Nam", value: "Male" },
                                    { label: "Nữ", value: "Female" },
                                    { label: "Khác", value: "Other" },
                                    ]}
                                />
                                </Item>
                            </Col>
                            </Row>

                            <Row gutter={12}>
                            <Col span={12}>
                                <Item label="Email" name="email">
                                <Input
                                    size="large"
                                    maxLength={100}
                                    placeholder="VD: example@example.com"
                                    disabled
                                />
                                </Item>
                            </Col>

                            <Col span={12}>
                                <Item label="Số điện thoại" name="phoneNumber">
                                <Input
                                    size="large"
                                    maxLength={20}
                                    placeholder="VD: 0912345678"
                                />
                                </Item>
                            </Col>
                            </Row>

                            <Row gutter={12}>
                            <Col span={12}>
                                <Item
                                label="SĐT khẩn cấp"
                                name="emergencyPhone"
                                rules={[
                                    {
                                    required: true,
                                    message: "Vui lòng nhập SĐT khẩn cấp",
                                    },
                                ]}
                                >
                                <Input
                                    size="large"
                                    maxLength={15}
                                    placeholder="VD: 0912345678"
                                />
                                </Item>
                            </Col>
                            <Col span={12}>
                                <Item
                                label="Người liên hệ khẩn cấp"
                                name="emergencyName"
                                rules={[
                                    {
                                    required: true,
                                    message:
                                        "Vui lòng nhập tên người liên hệ khẩn cấp",
                                    },
                                ]}
                                >
                                <Input
                                    size="large"
                                    maxLength={50}
                                    placeholder="VD: Trần Thị B (mẹ)"
                                />
                                </Item>
                            </Col>
                            </Row>

                            <Item label="Địa chỉ chi tiết" name="addressDetail">
                            <Input.TextArea
                                rows={3}
                                placeholder="Số nhà, đường, phường/xã, quận/huyện, tỉnh/thành phố..."
                            />
                            </Item>

                            {/* Field ẩn giữ URL avatar */}
                            <Item name="avatarUrl" className="hidden">
                            <Input type="hidden" />
                            </Item>

                            <Divider className="my-4!" />

                            <div className="flex justify-end gap-2">
                            <Button
                                type="default"
                                size="large"
                                className="rounded-full px-6"
                                onClick={() =>
                                form.setFieldsValue({
                                    ...profile,
                                    dob: profile?.dob
                                    ? dayjs(profile.dob)
                                    : undefined,
                                } as any)
                                }
                            >
                                Hủy thay đổi
                            </Button>
                            <Button
                                type="primary"
                                size="large"
                                htmlType="submit"
                                loading={saving}
                                className="rounded-full px-8 bg-emerald-500 hover:bg-emerald-600"
                            >
                                Lưu thay đổi
                            </Button>
                            </div>
                        </Form>
                        </Card>
                    </Col>

                    {/* KYC BÊN PHẢI */}
                    <Col xs={24} lg={10}>
                        <Card className="shadow-sm rounded-2xl h-full border border-gray-100 flex flex-col">
                        <div className="flex items-center justify-between mb-3">
                            <div>
                            <Title level={4} className="mb-0!">
                                Thông tin CCCD / KYC
                            </Title>
                            <Text type="secondary" className="text-xs">
                                Dùng để xác minh danh tính và bảo vệ tài khoản của
                                bạn.
                            </Text>
                            </div>
                            <Badge
                            status={
                                verifyState.isVerified
                                ? "success"
                                : verifyState.isPending
                                ? "processing"
                                : verifyState.isRejected
                                ? "error"
                                : "default"
                            }
                            text={verifyState.label}
                            />
                        </div>

                        {!verifyState.isVerified ? (
                            <div className="flex-1 flex flex-col justify-between space-y-4 mt-2">
                            <div className="space-y-2 text-sm">
                                <Alert
                                type={
                                    verifyState.isRejected
                                    ? "error"
                                    : verifyState.isPending
                                    ? "info"
                                    : "warning"
                                }
                                showIcon
                                message={
                                    verifyState.isRejected
                                    ? "Hồ sơ KYC của bạn đã bị từ chối."
                                    : verifyState.isPending
                                    ? "Hồ sơ KYC đang được xem xét."
                                    : "Bạn chưa hoàn tất xác minh danh tính."
                                }
                                description={
                                    verifyState.isRejected
                                    ? "Vui lòng kiểm tra lại thông tin CCCD/CMND và liên hệ nhân viên hỗ trợ để được hướng dẫn cập nhật."
                                    : verifyState.isPending
                                    ? "Chúng tôi sẽ thông báo kết quả qua ứng dụng sau khi hoàn tất kiểm tra."
                                    : "Việc xác minh giúp bảo vệ tài khoản và tăng hạn mức sử dụng dịch vụ."
                                }
                                />
                            </div>

                            <div className="pt-2 border-t border-dashed border-gray-200 mt-4">
                                <Text
                                type="secondary"
                                className="text-xs block mb-2"
                                >
                                Khi xác minh, thông tin CCCD của bạn sẽ hiển thị
                                tại đây dưới dạng chỉ đọc.
                                </Text>
                                <Button
                                type="default"
                                size="middle"
                                className="rounded-full border-emerald-500 text-emerald-600 hover:bg-emerald-50"
                                // onClick={() => navigate("/kyc")}
                                >
                                Xem hướng dẫn xác minh
                                </Button>
                            </div>
                            </div>
                        ) : (
                            <div className="mt-3 flex-1 flex flex-col justify-between">
                            <div className="space-y-2 text-sm bg-emerald-50/60 rounded-xl p-3 border border-emerald-100">
                                <InfoRow
                                label="Số CCCD/CMND"
                                value={profile?.numberCard}
                                />
                                {/* <InfoRow
                                label="Cơ quan cấp"
                                value={profile?.issuedBy}
                                />
                                <InfoRow
                                label="Ngày cấp"
                                value={
                                    profile?.issuedDate
                                    ? new Date(
                                        profile.issuedDate
                                        ).toLocaleDateString("vi-VN")
                                    : undefined
                                }
                                />
                                <InfoRow
                                label="Ngày hết hạn"
                                value={
                                    profile?.expiryDate
                                    ? new Date(
                                        profile.expiryDate
                                        ).toLocaleDateString("vi-VN")
                                    : undefined
                                }
                                />
                                <InfoRow
                                label="Nguyên quán"
                                value={profile?.placeOfOrigin}
                                />
                                <InfoRow
                                label="Nơi cư trú"
                                value={profile?.placeOfResidence}
                                /> */}
                            </div>

                            <div className="mt-4">
                                <Alert
                                type="success"
                                showIcon
                                message="Thông tin đã được xác minh"
                                description="Thông tin KYC của bạn đang được bảo vệ. Nếu phát hiện sai lệch, vui lòng liên hệ bộ phận hỗ trợ."
                                />
                            </div>
                            </div>
                        )}
                        </Card>
                    </Col>
                    </Row>
                ),
                },
                {
                key: "quests",
                label: <span className="flex items-center gap-2"><FlagOutlined /> Nhiệm vụ đang diễn ra </span>,
                children: <ProfileQuestsTab />,
                },
                {
                key: "achievements",
                label: <span className="flex items-center gap-2"><TrophyOutlined /> Thành tích & Lịch sử hành trình</span>,
                children: <ProfileAchievementsTab profile={profile} />,
                },
            ]}
            />
          </ConfigProvider>
        </div>
      </div>
    </div>
  );
};

interface InfoRowProps {
  label: string;
  value?: string | null;
}

const InfoRow: React.FC<InfoRowProps> = ({ label, value }) => {
  return (
    <div className="flex justify-between gap-3 text-xs md:text-sm">
      <span className="text-gray-500 shrink-0">{label}:</span>
      <span className="font-medium text-gray-800 text-right wrap-break-word">
        {value && value.trim().length > 0 ? value : "—"}
      </span>
    </div>
  );
};

export default ProfilePage;