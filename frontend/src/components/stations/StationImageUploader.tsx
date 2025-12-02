    // src/components/station/StationImageUploader.tsx
    import React, { useState } from "react";
    import { App, Button, Upload, Tooltip, Image } from "antd";
    import type { UploadProps } from "antd";
    import { PlusOutlined, LoadingOutlined, InfoCircleOutlined } from "@ant-design/icons";
    import { uploadImageStation } from "../../services/photo.service";
    import type { StationDTO } from "../../types/station";

    interface Props {
    station: StationDTO;
    onUploaded?: (newUrl: string) => void;
    }
    const MAX_MB = 5;
    const ACCEPT = ["image/jpeg", "image/png", "image/webp"];
    const StationImageUploader: React.FC<Props> = ({ station, onUploaded }) => {
    const { message } = App.useApp();
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
    const customRequest: UploadProps["customRequest"] = async ({
        file,
        onError,
        onSuccess,
    }) => {
        try {
        setUploading(true);
        const updated = await uploadImageStation(station.id, file as File);
        const newUrl = updated.image ?? null;
        onSuccess && onSuccess(updated as any);
        onUploaded && newUrl && onUploaded(newUrl);
        message.success("Cập nhật ảnh trạm thành công!");
        } catch (e: any) {
        console.error(e);
        message.error(e?.message || "Upload ảnh trạm thất bại");
        onError && onError(e);
        } finally {
        setUploading(false);
        }
    };
    return (
        <div className="flex items-center gap-3">
        <Image
            src={station.image || undefined}
            width={96}
            height={60}
            style={{ objectFit: "cover", borderRadius: 6 }}
            fallback="https://via.placeholder.com/96x60?text=No+Image"
        />
        <div className="flex flex-col gap-1">
            <Upload
            accept={ACCEPT.join(",")}
            showUploadList={false}
            beforeUpload={beforeUpload}
            customRequest={customRequest}
            >
            <Button
                type="primary"
                size="small"
                icon={uploading ? <LoadingOutlined /> : <PlusOutlined />}
            >
                {uploading ? "Đang tải..." : "Đổi ảnh"}
            </Button>
            </Upload>
            <Tooltip title={`JPG/PNG/WebP • ≤ ${MAX_MB}MB`}>
            <div className="text-xs text-gray-500 flex items-center gap-1">
                <InfoCircleOutlined /> Ảnh trạm hiển thị ở trang người dùng
            </div>
            </Tooltip>
        </div>
        </div>
    );
    };
    export default StationImageUploader;
