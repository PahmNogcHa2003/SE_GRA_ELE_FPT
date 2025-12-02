
import React, { useState, useMemo } from 'react';
import { Row, Col, Input, Pagination, Skeleton, Empty } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';


import * as newsService from '../../services/news.service';
import type {  GetNewsParams } from '../../types/news';
import NewsCard from '../../components/news/NewsCard';

const { Search } = Input;

const NewsListPage: React.FC = () => {
  const [queryParams, setQueryParams] = useState<GetNewsParams>({
    pageNumber: 1,
    pageSize: 6,
  });


  const [searchText, setSearchText] = useState('');

  const finalParams: GetNewsParams = useMemo(
    () => ({
      ...queryParams,
      search: searchText.trim() || undefined,
      sortOrder: 'PublishedAt', 
    }),
    [queryParams, searchText]
  );

  const {
    data: newsResponse,
    isLoading,
    isFetching,
  } = useQuery({
    queryKey: ['news', finalParams],
    queryFn: () => newsService.getNews(finalParams),
    select: (res) => res.data, 
    placeholderData: (prev) => prev,
  });

  const paged = newsResponse?.data; 

  const handleSearch = (value: string) => {
    setSearchText(value);
    setQueryParams((prev) => ({
      ...prev,
      page: 1,
    }));
  };

  const handlePageChange = (page: number, pageSize?: number) => {
    setQueryParams((prev) => ({
      ...prev,
      page,
      pageSize: pageSize ?? prev.pageSize,
    }));
  };

  const isFirstLoading = isLoading && !paged;

  return (
    <div className="min-h-screen bg-linear-to-b from-emerald-50 via-white to-emerald-50">
      {/* BANNER / HERO */}
      <section
        className="relative h-64 md:h-80 bg-cover bg-center overflow-hidden rounded-b-4xl shadow-sm"
        style={{
          backgroundImage:
            'linear-gradient(to right, rgba(0,0,0,0.6), rgba(0,0,0,0.35)), url(/images/news-hero.jpg)',
        }}
      >
        <div className="absolute inset-0 flex flex-col items-center justify-center text-center text-white px-4">
          <p className="tracking-[0.35em] text-xs md:text-sm uppercase opacity-80">
            Eco Journey
          </p>
          <h1 className="mt-2 text-3xl md:text-4xl font-semibold tracking-wide">
            TIN TỨC &amp; SỰ KIỆN
          </h1>
          <div className="mt-3 h-1 w-24 bg-emerald-400 rounded-full" />
          <p className="mt-4 max-w-xl text-sm md:text-base text-gray-100">
            Cập nhật các ưu đãi, hoạt động cộng đồng và câu chuyện thú vị cùng Eco Journey.
          </p>
        </div>
      </section>

      {/* NỘI DUNG */}
      <section className="container mx-auto px-4 py-10 md:py-12">
        {/* SEARCH */}
        <div className="flex justify-center mb-8">
          <Search
            placeholder="Tìm kiếm theo tiêu đề, nội dung..."
            allowClear
            enterButton="Tìm kiếm"
            size="large"
            value={searchText}
            prefix={<SearchOutlined className="text-gray-400" />}
            onChange={(e) => setSearchText(e.target.value)}
            onSearch={handleSearch}
            className="max-w-xl w-full"
          />
        </div>

        {isFirstLoading ? (
          // Lần đầu load
          <Row gutter={[24, 24]}>
            {Array.from({ length: 6 }).map((_, idx) => (
              <Col xs={24} sm={12} lg={8} key={idx} className="flex">
                <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-4 w-full">
                  <Skeleton.Image active style={{ width: '100%', height: 180 }} />
                  <div className="mt-4">
                    <Skeleton active paragraph={{ rows: 3 }} />
                  </div>
                </div>
              </Col>
            ))}
          </Row>
        ) : !paged || paged.items.length === 0 ? (
          // Không có dữ liệu
          <div className="py-16">
            <Empty description="Chưa có tin tức nào" />
          </div>
        ) : (
          <>
            {isFetching && (
              <div className="mb-3 text-xs text-gray-400 text-center">
                Đang tải dữ liệu...
              </div>
            )}

            {/* GRID NEWS */}
            <Row gutter={[24, 24]} className="mb-10">
              {paged.items.map((news) => (
                <Col xs={24} sm={12} lg={8} key={news.id} className="flex">
                  <NewsCard news={news} />
                </Col>
              ))}
            </Row>

            {/* PAGINATION */}
            <div className="flex justify-center">
              <Pagination
                current={paged.pageNumber}
                pageSize={paged.pageSize}
                total={paged.totalCount}
                onChange={handlePageChange}
                showSizeChanger={false}
              />
            </div>
          </>
        )}
      </section>
    </div>
  );
};

export default NewsListPage;
