// src/pages/staff/ManageTagsPage.tsx

import React, { useState } from 'react';
import { Table, Button, Modal, Space, Typography, Input } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import type { TableProps } from 'antd';
import Swal from 'sweetalert2';
import * as tagService from '../../services/tag.service';
import type { TagDTO, GetTagsParams } from '../../types/tag';
import TagForm from '../../features/tags/TagForm';

const { Title } = Typography;
const { Search } = Input;

const ManageTagsPage: React.FC = () => {
  const queryClient = useQueryClient();
  const [queryParams, setQueryParams] = useState<GetTagsParams>({ page: 1, pageSize: 10 });
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingTag, setEditingTag] = useState<TagDTO | null>(null);

  const { data: tagsResponse, isLoading } = useQuery({
    queryKey: ['tags', queryParams],
    queryFn: () => tagService.getTags(queryParams),
  });
  const tagsData = tagsResponse?.data;

  const createMutation = useMutation({
    mutationFn: tagService.createTag,
    onSuccess: () => {
      Swal.fire('Thành công!', 'Tạo thẻ mới thành công!', 'success');
      queryClient.invalidateQueries({ queryKey: ['tags'] });
      setIsModalOpen(false);
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi tạo thẻ.', 'error'),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: TagDTO }) => tagService.updateTag(id, data),
    onSuccess: () => {
      Swal.fire('Thành công!', 'Cập nhật thẻ thành công!', 'success');
      queryClient.invalidateQueries({ queryKey: ['tags'] });
      setIsModalOpen(false);
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi cập nhật.', 'error'),
  });

  const deleteMutation = useMutation({
    mutationFn: tagService.deleteTag,
    onSuccess: () => {
      Swal.fire('Đã xóa!', 'Thẻ đã được xóa.', 'success');
      queryClient.invalidateQueries({ queryKey: ['tags'] });
    },
    onError: () => Swal.fire('Lỗi!', 'Có lỗi xảy ra khi xóa.', 'error'),
  });

  const handleOpenCreateModal = () => { setEditingTag(null); setIsModalOpen(true); };
  const handleOpenEditModal = (tag: TagDTO) => { setEditingTag(tag); setIsModalOpen(true); };
  const handleCancelModal = () => setIsModalOpen(false);
  const handleFormSubmit = (values: Omit<TagDTO, 'id'>) => {
    if (editingTag) {
      updateMutation.mutate({ id: editingTag.id, data: { ...editingTag, ...values } });
    } else {
      createMutation.mutate(values);
    }
  };
  const handleDeleteConfirm = (id: number) => {
    Swal.fire({
      title: 'Bạn có chắc không?', text: "Hành động này không thể hoàn tác!", icon: 'warning',
      showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Vâng, xóa nó!', cancelButtonText: 'Hủy'
    }).then((result) => result.isConfirmed && deleteMutation.mutate(id));
  };
  const handleTableChange: TableProps<TagDTO>['onChange'] = (pagination) => {
    setQueryParams(prev => ({ ...prev, page: pagination.current, pageSize: pagination.pageSize }));
  };
  const handleSearch = (value: string) => {
    setQueryParams(prev => ({ ...prev, page: 1, search: value.trim() }));
  };

  const columns: TableProps<TagDTO>['columns'] = [
    { title: 'ID', dataIndex: 'id', key: 'id', sorter: true },
    { title: 'Tên thẻ', dataIndex: 'name', key: 'name', sorter: true },
    {
      title: 'Hành động', key: 'action', align: 'center',
      render: (_, record) => (
        <Space size="middle">
          <Button icon={<EditOutlined />} onClick={() => handleOpenEditModal(record)} />
          <Button icon={<DeleteOutlined />} danger onClick={() => handleDeleteConfirm(record.id)} loading={deleteMutation.isPending && deleteMutation.variables === record.id} />
        </Space>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <Space direction="vertical" style={{ width: '100%' }}>
        <Title level={2}>Quản lý Thẻ (Tags)</Title>
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
          <Search placeholder="Tìm theo tên thẻ..." onSearch={handleSearch} style={{ width: 300 }} allowClear enterButton />
          <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenCreateModal}>Thêm thẻ mới</Button>
        </div>
        <Table
          columns={columns}
          dataSource={tagsData?.data?.items}
          loading={isLoading}
          rowKey="id"
          pagination={{ current: tagsData?.data?.pageNumber, pageSize: tagsData?.data?.pageSize, total: tagsData?.data?.totalCount, showSizeChanger: true }}
          onChange={handleTableChange}
        />
      </Space>
      <Modal title={editingTag ? 'Chỉnh sửa thẻ' : 'Tạo thẻ mới'} open={isModalOpen} onCancel={handleCancelModal} footer={null} destroyOnClose>
        <TagForm initialValues={editingTag} onSubmit={handleFormSubmit} onCancel={handleCancelModal} isLoading={createMutation.isPending || updateMutation.isPending} />
      </Modal>
    </div>
  );
};

export default ManageTagsPage;