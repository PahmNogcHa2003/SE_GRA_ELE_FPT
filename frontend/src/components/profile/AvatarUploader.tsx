// src/components/profile/AvatarUploader.tsx
import React, { useState } from "react";
import { App, Avatar, Button, Upload, Tooltip } from "antd";
import type { UploadProps } from "antd";
import { PlusOutlined, LoadingOutlined, InfoCircleOutlined } from "@ant-design/icons";
import { uploadAvatar } from "../../services/photo.service";
import { useAuth } from "../../features/auth/context/authContext";

interface Props {
  value?: string | null;
  onChange?: (url: string | null) => void;
  size?: number;
}

const MAX_MB = 5; 
const ACCEPT = ["image/jpeg", "image/png", "image/webp"];

const AvatarUploader: React.FC<Props> = ({ value, onChange, size = 112 }) => {
  const { message } = App.useApp();
  const { updateUser } = useAuth();     
  const [uploading, setUploading] = useState(false);

  const beforeUpload: UploadProps["beforeUpload"] = (file) => {
    const isAllowed = ACCEPT.includes(file.type);
    if (!isAllowed) {
      message.error("Chỉ hỗ trợ JPG/PNG/WebP");
      return Upload.LIST_IGNORE;
    }

    const isLtMax = file.size / 1024 / 1024 < MAX_MB;
    if (!isLtMax) {
      message.error(`Ảnh phải nhỏ hơn ${MAX_MB}MB`);
      return Upload.LIST_IGNORE;
    }

    return true;
  };

  const customRequest: UploadProps["customRequest"] = async ({ file, onError, onSuccess }) => {
  try {
    setUploading(true);
    const profile = await uploadAvatar(file as File);
    const newUrl = profile.avatarUrl || null;

    onSuccess && onSuccess(profile as any);
    onChange && onChange(newUrl);
    updateUser(prev => (prev ? { ...prev, avatarUrl: newUrl || undefined } : prev));

    message.success("Cập nhật ảnh đại diện thành công!");
  } catch (e: any) {
    console.error(e);
    message.error(e?.response?.data?.message || "Upload thất bại");
    onError && onError(e);
  } finally {
    setUploading(false);
  }
};


  return (
    <div className="flex items-center gap-5">
      <div className="relative">
        <Avatar
          src={value || undefined}
          size={size}
          className="shadow ring-2 ring-emerald-100"
        />
        <div className="absolute -bottom-2 -right-2">
          <Upload
            accept={ACCEPT.join(",")}
            showUploadList={false}
            beforeUpload={beforeUpload}
            customRequest={customRequest}
          >
            <Button
              type="primary"
              shape="round"
              size="small"
              icon={uploading ? <LoadingOutlined /> : <PlusOutlined />}
            >
              {uploading ? "Đang tải" : "Đổi ảnh"}
            </Button>
          </Upload>
        </div>
      </div>
      <Tooltip title="Ảnh giúp nhân viên nhận diện nhanh hơn khi hỗ trợ tại trạm">
        <div className="text-gray-500 text-sm flex items-center gap-2">
          <InfoCircleOutlined /> JPG/PNG/WebP • ≤ {MAX_MB}MB
        </div>
      </Tooltip>
    </div>
  );
};

export default AvatarUploader;
