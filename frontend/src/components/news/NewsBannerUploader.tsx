    // src/components/news/NewsBannerUploader.tsx
    import React, { useState } from "react";
    import { App, Button, Upload, Tooltip, Image } from "antd";
    import type { UploadProps } from "antd";
    import { PlusOutlined, LoadingOutlined, InfoCircleOutlined } from "@ant-design/icons";
    import { uploadBannerNews } from "../../services/photo.service";
    import type { NewsDTO } from "../../types/news";

    interface Props {
    news: NewsDTO;
    onUploaded?: (newUrl: string) => void;
    }

    const MAX_MB = 5;
    const ACCEPT = ["image/jpeg", "image/png", "image/webp"];

    const NewsBannerUploader: React.FC<Props> = ({ news, onUploaded }) => {
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

        // 👇 NHẬN TRỰC TIẾP NewsDTO
        const updated = await uploadBannerNews(news.id, file as File);
        const newUrl = updated.banner ?? null;

        onSuccess && onSuccess(updated as any);
        onUploaded && newUrl && onUploaded(newUrl);

        message.success("Cập nhật banner thành công!");
        } catch (e: any) {
        console.error(e);
        message.error(e?.message || "Upload banner thất bại");
        onError && onError(e);
        } finally {
        setUploading(false);
        }
    };

    return (
        <div className="flex items-center gap-3">
        <Image
            src={news.banner || undefined}
            width={140}
            height={70}
            style={{ objectFit: "cover", borderRadius: 6 }}
            fallback="https://via.placeholder.com/140x70?text=No+Banner"
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
                {uploading ? "Đang tải..." : "Đổi banner"}
            </Button>
            </Upload>
            <Tooltip title={`JPG/PNG/WebP • ≤ ${MAX_MB}MB`}>
            <div className="text-xs text-gray-500 flex items-center gap-1">
                <InfoCircleOutlined /> Banner hiển thị trên trang tin tức
            </div>
            </Tooltip>
        </div>
        </div>
    );
    };

    export default NewsBannerUploader;
