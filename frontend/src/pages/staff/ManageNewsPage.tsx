    // src/pages/staff/ManageNewsPage.tsx

    import React, { useRef, useState } from 'react';
    import {
    Table,
    Button,
    Modal,
    Space,
    Typography,
    Input,
    Tag,
    Row,
    Col,
    Select,
    } from 'antd';
    import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
    import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
    import type { TableProps } from 'antd';
    import Swal from 'sweetalert2';

    import * as photoService from '../../services/photo.service';
    import * as newsService from '../../services/news.service';
    import type { NewsDTO, GetNewsParams } from '../../types/news';
    import NewsForm from '../../features/news/NewsForm';
    import NewsBannerUploader from '../../components/news/NewsBannerUploader';

    const { Title } = Typography;
    const { Search } = Input;

    const ManageNewsPage: React.FC = () => {
    const queryClient = useQueryClient();
    const [queryParams, setQueryParams] = useState<GetNewsParams>({ pageNumber: 1, pageSize: 10 });
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingNews, setEditingNews] = useState<NewsDTO | null>(null);

    // input file ẩn cho banner
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [newsIdForBanner /*setNewsIdForBanner*/] = useState<number | null>(null);

    const { data: newsResponse, isLoading } = useQuery({
        queryKey: ['news', queryParams],
        queryFn: () => newsService.getNews(queryParams),
    });

    const newsData = newsResponse?.data; // ApiResponse<PagedResult<NewsDTO>> hoặc tương tự

    const { data: singleNewsData, isFetching: isFetchingSingleNews } = useQuery({
        queryKey: ['news', editingNews?.id],
        queryFn: () => newsService.getNewsById(editingNews!.id),
        enabled: !!editingNews?.id,
        select: (res) => res.data.data,
    });

    const createMutation = useMutation({
        mutationFn: newsService.createNews,
        onSuccess: () => {
        Swal.fire('Thành công!', 'Tạo bài viết mới thành công.', 'success');
        queryClient.invalidateQueries({ queryKey: ['news'] });
        setIsModalOpen(false);
        setEditingNews(null);
        },
        onError: () => {
        Swal.fire('Lỗi!', 'Không thể tạo bài viết.', 'error');
        },
    });

    const updateMutation = useMutation({
        mutationFn: ({ id, data }: { id: number; data: any }) =>
        newsService.updateNews(id, data),
        onSuccess: () => {
        Swal.fire('Thành công!', 'Cập nhật bài viết thành công.', 'success');
        queryClient.invalidateQueries({ queryKey: ['news'] });
        setIsModalOpen(false);
        setEditingNews(null);
        },
        onError: () => {
        Swal.fire('Lỗi!', 'Không thể cập nhật bài viết.', 'error');
        },
    });

    const deleteMutation = useMutation({
        mutationFn: newsService.deleteNews,
        onSuccess: () => {
        Swal.fire('Đã xóa!', 'Bài viết đã được xóa.', 'success');
        queryClient.invalidateQueries({ queryKey: ['news'] });
        },
        onError: () => {
        Swal.fire('Lỗi!', 'Không thể xóa bài viết.', 'error');
        },
    });

    const uploadBannerMutation = useMutation({
        mutationFn: ({ id, file }: { id: number; file: File }) =>
        photoService.uploadBannerNews(id, file),
        onSuccess: () => {
        Swal.fire('Thành công!', 'Cập nhật banner thành công.', 'success');
        queryClient.invalidateQueries({ queryKey: ['news'] });
        },
        onError: () => {
        Swal.fire('Lỗi!', 'Upload banner thất bại.', 'error');
        },
    });

    const handleOpenCreateModal = () => {
        setEditingNews(null);
        setIsModalOpen(true);
    };

    const handleOpenEditModal = (news: NewsDTO) => {
        setEditingNews(news);
        setIsModalOpen(true);
    };

    const handleCancelModal = () => {
        setIsModalOpen(false);
        setEditingNews(null);
    };

    const handleFormSubmit = (values: any) => {
        if (editingNews) {
        updateMutation.mutate({ id: editingNews.id, data: { ...editingNews, ...values } });
        } else {
        createMutation.mutate(values);
        }
    };

    const handleDeleteConfirm = (id: number) => {
        Swal.fire({
        title: 'Bạn có chắc?',
        text: 'Bài viết sẽ bị xóa vĩnh viễn.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy',
        }).then((result) => {
        if (result.isConfirmed) {
            deleteMutation.mutate(id);
        }
        });
    };

    // // click nút đổi banner
    // const handleClickChangeBanner = (id: number) => {
    //     setNewsIdForBanner(id);
    //     if (fileInputRef.current) {
    //     fileInputRef.current.value = '';
    //     fileInputRef.current.click();
    //     }
    // };

    // khi chọn file banner
    const handleFileChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        const file = e.target.files?.[0];
        if (!file || newsIdForBanner == null) return;

        Swal.fire({
        title: 'Cập nhật banner?',
        text: 'Banner cũ sẽ bị thay thế.',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Hủy',
        }).then((result) => {
        if (result.isConfirmed) {
            uploadBannerMutation.mutate({ id: newsIdForBanner, file });
        }
        });
    };

    const handleTableChange: TableProps<NewsDTO>['onChange'] = (pagination) => {
        setQueryParams((prev) => ({
        ...prev,
        page: pagination.current,
        pageSize: pagination.pageSize,
        }));
    };

    const handleSearch = (value: string) =>
        setQueryParams((prev) => ({ ...prev, page: 1, search: value.trim() }));

    const handleFilterChange = (value: string) => {
        const newParams: GetNewsParams = { ...queryParams, pageNumber: 1 };
        if (value === 'all') {
        delete newParams.filterField;
        delete newParams.filterValue;
        } else {
        newParams.filterField = 'isActive';
        newParams.filterValue = value;
        }
        setQueryParams(newParams);
    };

    const columns: TableProps<NewsDTO>['columns'] = [
        { title: 'ID', dataIndex: 'id', key: 'id' },
        {
        title: 'Banner',
        dataIndex: 'banner',
        key: 'banner',
        render: (_, record) => (
            <NewsBannerUploader
            news={record}
            onUploaded={() =>
                queryClient.invalidateQueries({ queryKey: ['news', queryParams] })
            }
            />
        ),
        },
        { title: 'Tiêu đề', dataIndex: 'title', key: 'title' },
        {
        title: 'Tác giả ID',
        dataIndex: 'userId',
        key: 'userId',
        },
        {
        title: 'Ngày tạo',
        dataIndex: 'createdAt',
        key: 'createdAt',
        render: (date) =>
            date ? new Date(date).toLocaleDateString('vi-VN') : '',
        },
        {
        title: 'Trạng thái',
        dataIndex: 'status',
        key: 'status',
        render: (status: string) => {
            const up = status?.toLowerCase();
            const isPublished = up === 'published';

            return (
            <Tag color={isPublished ? 'green' : 'red'}>
                {isPublished ? 'CÔNG KHAI' : 'BẢN NHÁP'}
            </Tag>
            );
        },
        },
        {
        title: 'Hành động',
        key: 'action',
        align: 'center',
        render: (_, record) => (
            <Space>
            <Button icon={<EditOutlined />} onClick={() => handleOpenEditModal(record)} />
            <Button
                icon={<DeleteOutlined />}
                danger
                onClick={() => handleDeleteConfirm(record.id)}
            />
            </Space>
        ),
        },
    ];

    return (
        <div style={{ padding: '24px' }}>
        <Title level={2}>Quản lý Tin tức</Title>
        <Row justify="space-between" style={{ marginBottom: 24 }} gutter={16}>
            <Col>
            <Select defaultValue="all" style={{ width: 150 }} onChange={handleFilterChange}>
                <Select.Option value="all">Tất cả</Select.Option>
                <Select.Option value="true">Công khai</Select.Option>
                <Select.Option value="false">Bản nháp</Select.Option>
            </Select>
            </Col>
            <Col>
            <Space>
                <Search
                placeholder="Tìm theo tiêu đề, nội dung..."
                onSearch={handleSearch}
                style={{ width: 300 }}
                allowClear
                enterButton
                />
                <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenCreateModal}>
                Viết bài mới
                </Button>
            </Space>
            </Col>
        </Row>
        <Table
            columns={columns}
            dataSource={newsData?.data?.items}
            loading={isLoading}
            rowKey="id"
            pagination={{
            current: newsData?.data?.pageNumber,
            pageSize: newsData?.data?.pageSize,
            total: newsData?.data?.totalCount,
            }}
            onChange={handleTableChange}
        />
        <Modal
            title={editingNews ? 'Chỉnh sửa bài viết' : 'Viết bài mới'}
            open={isModalOpen}
            onCancel={handleCancelModal}
            footer={null}
            destroyOnClose
            width={800}
        >
            <NewsForm
            initialValues={editingNews ? singleNewsData : null}
            onSubmit={handleFormSubmit}
            onCancel={handleCancelModal}
            isLoading={
                createMutation.isPending ||
                updateMutation.isPending ||
                isFetchingSingleNews
            }
            />
        </Modal>

        {/* input file ẩn cho upload banner */}
        <input
            type="file"
            accept="image/*"
            style={{ display: 'none' }}
            ref={fileInputRef}
            onChange={handleFileChange}
        />
        </div>
    );
    };

    export default ManageNewsPage;
