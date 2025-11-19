import React, { useMemo } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Skeleton, Tag, Empty } from 'antd';
import dayjs from 'dayjs';
import { getNewsById, getRelatedNews } from '../../services/news.service';
import type { NewsDTO } from '../../types/news';

const NewsDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const newsId = Number(id);

  const { data: news, isLoading } = useQuery({
    queryKey: ['news-detail', newsId],
    queryFn: async () => {
      const res = await getNewsById(newsId);
      return res.data.data as NewsDTO;
    },
    enabled: !!newsId,
  });

  const tagIds = useMemo(() => news?.tagIds ?? [], [news]);
  const { data: related } = useQuery({
    queryKey: ['news-related', tagIds],
    queryFn: async () => {
      const res = await getRelatedNews(newsId, 4);
      return res.data.data as NewsDTO[];
    },
    enabled: tagIds.length > 0,
  });

  const publishedDate = news?.publishedAt || news?.createdAt;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <section
        className="relative h-56 md:h-64 bg-cover bg-center"
        style={{
          backgroundImage:
            'linear-gradient(to right, rgba(0,0,0,0.65), rgba(0,0,0,0.45)), url(/images/news-hero.jpg)',
        }}
      >
        <div className="absolute inset-0 flex flex-col justify-center px-4">
          <div className="container mx-auto text-white">
            {isLoading ? (
              <Skeleton active title paragraph={false} />
            ) : (
              <>
                <p className="tracking-[0.35em] text-xs uppercase opacity-80">Tin tức</p>
                <h1 className="mt-3 text-2xl md:text-3xl font-semibold">{news?.title}</h1>
                {publishedDate && (
                  <p className="mt-1 text-sm text-gray-200">
                    {dayjs(publishedDate).format('DD/MM/YYYY')}
                  </p>
                )}
              </>
            )}
          </div>
        </div>
      </section>

      {/* Nội dung */}
      <section className="container mx-auto px-4 py-10 md:py-12 grid grid-cols-1 lg:grid-cols-[minmax(0,2.5fr)_minmax(280px,1fr)] gap-8">
        <article className="bg-white rounded-2xl shadow-sm p-6 md:p-8 space-y-6">
          {isLoading ? (
            <Skeleton active paragraph={{ rows: 8 }} />
          ) : (
            <>
              {news?.banner && (
                <img
                  src={news.banner}
                  alt={news.title}
                  className="rounded-2xl w-full max-h-[380px] object-cover mb-4"
                />
              )}
              <div className="flex flex-wrap gap-2">
                {news?.tagNames?.map((t) => (
                  <Tag key={t} color="gold">
                    {t}
                  </Tag>
                ))}
              </div>
              <div
                className="prose max-w-none prose-p:text-gray-700 prose-headings:text-gray-900"
                dangerouslySetInnerHTML={{ __html: news?.content || '' }}
              />
            </>
          )}
        </article>

        {/* Bài viết liên quan */}
        <aside className="lg:pl-4">
          <div className="bg-white rounded-2xl shadow-sm p-5 sticky top-24">
            <h3 className="text-base md:text-lg font-semibold text-gray-900 mb-4">
              Bài viết liên quan
            </h3>
            {!related || related.length === 0 ? (
              <Empty description="Không có bài viết liên quan" image={Empty.PRESENTED_IMAGE_SIMPLE} />
            ) : (
              <div className="space-y-4">
                {related.map((item) => (
                  <Link
                    key={item.id}
                    to={`/news/${item.id}`}
                    className="flex gap-3 group"
                  >
                    <div className="h-16 w-20 overflow-hidden rounded-lg">
                      {item.banner ? (
                        <img
                          src={item.banner}
                          alt={item.title}
                          className="h-full w-full object-cover group-hover:scale-105 transition-transform"
                        />
                      ) : (
                        <div className="h-full w-full bg-emerald-500 text-white text-xs flex items-center justify-center">
                          Eco
                        </div>
                      )}
                    </div>
                    <div>
                      <h4 className="text-sm font-medium text-gray-900 line-clamp-2 group-hover:text-emerald-600">
                        {item.title}
                      </h4>
                      <p className="text-xs text-gray-500">
                        {dayjs(item.publishedAt || item.createdAt).format('DD/MM/YYYY')}
                      </p>
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </div>
        </aside>
      </section>
    </div>
  );
};

export default NewsDetailPage;
