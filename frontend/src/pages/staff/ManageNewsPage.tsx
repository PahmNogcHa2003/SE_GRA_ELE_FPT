// src/pages/staff/ManageNewsPage.tsx

import React, { useState } from 'react';
import { Table, Button, Modal, Space, Typography, Input, Tag, Row, Col, Select } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { TableProps } from 'antd';
import Swal from 'sweetalert2';
import * as newsService from '../../services/news.service';
import type { NewsDTO, GetNewsParams } from '../../types/news';
import NewsForm from '../../features/news/NewsForm';

const { Title } = Typography;
const { Search } = Input;

const ManageNewsPage: React.FC = () => {
    const queryClient = useQueryClient();
    const [queryParams, setQueryParams] = useState<GetNewsParams>({ page: 1, pageSize: 10 });
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingNews, setEditingNews] = useState<NewsDTO | null>(null);

    const { data: newsResponse, isLoading } = useQuery({
        queryKey: ['news', queryParams],
        queryFn: () => newsService.getNews(queryParams),
    });
    const newsData = newsResponse?.data;
    
    // Fetch a single news item when the modal opens for editing
    const { data: singleNewsData, isFetching: isFetchingSingleNews } = useQuery({
        queryKey: ['news', editingNews?.id],
        queryFn: () => newsService.getNewsById(editingNews!.id),
        enabled: !!editingNews?.id, // Only run this query when editingNews.id is available
        select: (res) => res.data.data
    });

    const createMutation = useMutation({ mutationFn: newsService.createNews, /* ... onSuccess, onError ... */ });
    const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number, data: any }) => newsService.updateNews(id, data), /* ... onSuccess, onError ... */ });
    const deleteMutation = useMutation({ mutationFn: newsService.deleteNews, /* ... onSuccess, onError ... */ });

    const handleOpenCreateModal = () => { setEditingNews(null); setIsModalOpen(true); };
    const handleOpenEditModal = (news: NewsDTO) => { setEditingNews(news); setIsModalOpen(true); };
    const handleCancelModal = () => setIsModalOpen(false);
    
    const handleFormSubmit = (values: any) => {
        if (editingNews) {
            updateMutation.mutate({ id: editingNews.id, data: { ...editingNews, ...values } });
        } else {
            createMutation.mutate(values);
        }
    };

    const handleDeleteConfirm = (id: number) => {
        Swal.fire({ title: 'Bạn có chắc?', icon: 'warning', showCancelButton: true }).then(result => {
            if (result.isConfirmed) deleteMutation.mutate(id);
        });
    };

    const handleTableChange: TableProps<NewsDTO>['onChange'] = (pagination) => {
        setQueryParams(prev => ({ ...prev, page: pagination.current, pageSize: pagination.pageSize }));
    };
    const handleSearch = (value: string) => setQueryParams(prev => ({ ...prev, page: 1, search: value.trim() }));
    const handleFilterChange = (value: string) => {
        const newParams: GetNewsParams = { ...queryParams, page: 1 };
        if (value === 'all') { delete newParams.filterField; delete newParams.filterValue; }
        else { newParams.filterField = 'isActive'; newParams.filterValue = value; }
        setQueryParams(newParams);
    };

    const columns: TableProps<NewsDTO>['columns'] = [
        { title: 'ID', dataIndex: 'id', key: 'id' },
        { title: 'Tiêu đề', dataIndex: 'title', key: 'title' },
        { title: 'Tác giả ID', dataIndex: 'authorId', key: 'authorId' },
        { title: 'Ngày tạo', dataIndex: 'createdAt', key: 'createdAt', render: (date) => new Date(date).toLocaleDateString('vi-VN') },
        { title: 'Trạng thái', dataIndex: 'isActive', key: 'isActive', render: (isActive) => <Tag color={isActive ? 'green' : 'red'}>{isActive ? 'CÔNG KHAI' : 'BẢN NHÁP'}</Tag> },
        { title: 'Hành động', key: 'action', align: 'center', render: (_, record) => (
            <Space>
                <Button icon={<EditOutlined />} onClick={() => handleOpenEditModal(record)} />
                <Button icon={<DeleteOutlined />} danger onClick={() => handleDeleteConfirm(record.id)} />
            </Space>
        )},
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
                        <Search placeholder="Tìm theo tiêu đề, nội dung..." onSearch={handleSearch} style={{ width: 300 }} />
                        <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenCreateModal}>Viết bài mới</Button>
                    </Space>
                </Col>
            </Row>
            <Table
                columns={columns}
                dataSource={newsData?.data.items}
                loading={isLoading}
                rowKey="id"
                pagination={{ current: newsData?.data.pageNumber, pageSize: newsData?.data.pageSize, total: newsData?.data. totalCount }}
                onChange={handleTableChange}
            />
            <Modal title={editingNews ? 'Chỉnh sửa bài viết' : 'Viết bài mới'} open={isModalOpen} onCancel={handleCancelModal} footer={null} destroyOnClose width={800}>
                <NewsForm initialValues={editingNews ? singleNewsData : null} onSubmit={handleFormSubmit} onCancel={handleCancelModal} isLoading={createMutation.isPending || updateMutation.isPending || isFetchingSingleNews} />
            </Modal>
        </div>
    );
};

export default ManageNewsPage;