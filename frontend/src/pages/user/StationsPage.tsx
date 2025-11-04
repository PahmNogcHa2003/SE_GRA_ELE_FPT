import React, { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Input, Spin, Alert, Empty, Badge, Tooltip, message, Segmented } from 'antd';
import { EnvironmentOutlined, SearchOutlined, AimOutlined } from '@ant-design/icons';
import StationMap from '../../features/stations/components/StationMap';
import { getStations, getNearbyStations } from '../../services/station.service';
import type { StationDTO, StationPagedApiResponse } from '../../types/station';

const { Search } = Input;

/** ===== bạn có thể tinh chỉnh 4 con số này nếu cần ===== */
const NAV_H = 32;        // chiều cao navbar (không đổi nav, chỉ để tính chiều cao nội dung)
const BANNER_H = 190;    // chiều cao banner
const TOP_BOTTOM_GAP = 12; // khoảng cách trên/dưới phần card (nhỏ thôi)
const FOOTER_GUARD = 12; // khoảng cách tối thiểu tới footer (nhỏ thôi)

const calcPanelHeight = `calc(100vh - ${NAV_H + TOP_BOTTOM_GAP + FOOTER_GUARD}px - ${BANNER_H}px)`;

/** debounce nhỏ gọn */
function useDebouncedValue<T>(value: T, delay = 350) {
  const [debounced, setDebounced] = React.useState(value);
  React.useEffect(() => { const t = setTimeout(() => setDebounced(value), delay); return () => clearTimeout(t); }, [value, delay]);
  return debounced;
}

const StationsPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedStation, setSelectedStation] = useState<StationDTO | null>(null);

  const [userPosition, setUserPosition] = useState<{ lat: number; lng: number } | null>(null);
  const [nearbyMode, setNearbyMode] = useState(false);
  const [radiusKm, setRadiusKm] = useState<number>(5);
  const [nearbyItems, setNearbyItems] = useState<StationDTO[] | null>(null);

  // lấy list đầy đủ (cho chế độ thường)
  const { data: allPaged, isLoading, isError, error } = useQuery<
    StationPagedApiResponse, Error, StationDTO[]
  >({
    queryKey: ['stations'],
    queryFn: () => getStations({ page: 1, pageSize: 9999 }),
    select: (res) => res.data?.items ?? [],
  });

  const debouncedSearch = useDebouncedValue(searchTerm, 350);

  const baseFiltered = useMemo(() => {
    const src = allPaged ?? [];
    if (!debouncedSearch.trim()) return src;
    const kw = debouncedSearch.toLowerCase();
    return src.filter(s =>
      s.name.toLowerCase().includes(kw) ||
      (s.location ?? '').toLowerCase().includes(kw)
    );
  }, [allPaged, debouncedSearch]);

  const stationsToShow = nearbyMode ? (nearbyItems ?? []) : baseFiltered;

  const handleFindNearby = async () => {
    if (!navigator.geolocation) { message.error('Trình duyệt không hỗ trợ định vị!'); return; }
    message.loading({ content: 'Đang xác định vị trí của bạn...', key: 'locate' });

    navigator.geolocation.getCurrentPosition(async (pos) => {
      const { latitude, longitude } = pos.coords;
      setUserPosition({ lat: latitude, lng: longitude });

      try {
        const res = await getNearbyStations({ lat: latitude, lng: longitude, radiusKm, page: 1, pageSize: 100 });
        const items = res.data?.items ?? [];
        setNearbyItems(items);
        setNearbyMode(true);

        if (items.length === 0) {
          message.warning({ content: `Không có trạm nào trong bán kính ${radiusKm} km`, key: 'locate' });
        } else {
          message.success({ content: `Đã tìm thấy ${items.length} trạm gần bạn`, key: 'locate', duration: 2 });
        }
      } catch (e: any) {
        message.error({ content: e?.message || 'Không thể lấy danh sách trạm gần bạn', key: 'locate' });
      }
    }, () => {
      message.error({ content: 'Không thể lấy vị trí. Hãy bật GPS!', key: 'locate' });
    });
  };

  const clearNearby = () => { setNearbyMode(false); setNearbyItems(null); };

  return (
    <div className="bg-gray-50">
      {/* ===== Banner mỏng, sát nav nhưng vẫn thoáng ===== */}
      <div
        className="relative text-white text-center overflow-hidden"
        style={{
          backgroundImage: "url('src/assets/images/about_us.png')",
          backgroundSize: 'cover',
          backgroundPosition: 'center',
          height: BANNER_H,
        }}
      >
        <div className="absolute inset-0 bg-linear-to-b from-black/60 via-black/40 to-black/40" />
        <div className="relative z-10 px-3 md:px-4 h-full flex flex-col items-center justify-center">
          <h1 className="text-2xl md:text-4xl font-extrabold tracking-wide drop-shadow">
            DANH SÁCH TRẠM
          </h1>
          <div className="w-24 md:w-36 h-1 bg-eco-green mt-2 rounded-full" />
          <p className="mt-2 opacity-90 text-xs md:text-sm">
            Tìm trạm gần bạn, xem trạng thái hoạt động và số xe hiện có.
          </p>
        </div>
      </div>

      {/* ===== Nội dung: gần banner, gần 2 mép, gần footer nhưng vẫn có khoảng nhỏ ===== */}
      <section className="px-2 md:px-4 lg:px-6 py-3" /* mép trái/phải rất mỏng, trên/dưới mỏng */>
        <div className="rounded-xl shadow-md border bg-white overflow-hidden">
          <div className="flex flex-col md:flex-row">
            {/* === Panel List (trái) === */}
            <aside
              className="w-full md:w-[40%] lg:w-[33%] xl:w-[30%] border-b md:border-b-0 md:border-r flex flex-col"
              style={{ height: calcPanelHeight }}
            >
              {/* Header tìm kiếm sticky, top rất nhỏ để không sát mép */}
              <div className="sticky top-2 z-10 bg-white/90 backdrop-blur px-3 pt-3 pb-2 border-b md:border-b-0">
                <div className="mb-1 text-[11px] font-medium text-gray-500 uppercase tracking-wider">
                  Tìm kiếm
                </div>
                <Search
                  placeholder="Tìm trạm theo tên hoặc địa chỉ…"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  allowClear
                  size="large"
                  prefix={<SearchOutlined className="text-gray-400" />}
                />
                <div className="mt-2 flex items-center justify-between text-[12px] text-gray-500">
                  <span>Kết quả: <b>{stationsToShow.length}</b> / {allPaged?.length ?? 0}</span>
                  <div className="flex items-center gap-2">
                    <Segmented
                      size="small"
                      value={radiusKm}
                      onChange={(v) => setRadiusKm(Number(v))}
                      options={[2, 5, 10].map(n => ({ label: `${n} km`, value: n }))}
                    />
                    {!nearbyMode ? (
                      <Tooltip title="Tìm trạm gần vị trí của bạn">
                        <button
                          onClick={handleFindNearby}
                          className="inline-flex items-center gap-1 font-medium text-gray-600 hover:text-eco-green transition"
                        >
                          <AimOutlined /> Gần tôi
                        </button>
                      </Tooltip>
                    ) : (
                      <button
                        onClick={clearNearby}
                        className="inline-flex items-center gap-1 font-medium text-eco-green"
                      >
                        ✕ Bỏ lọc
                      </button>
                    )}
                  </div>
                </div>
              </div>

              {/* Danh sách trạm: chiếm hết phần còn lại, cuộn độc lập */}
              <div className="flex-1 overflow-y-auto">
                {isLoading && <div className="h-full flex items-center justify-center"><Spin /></div>}
                {isError && (
                  <div className="p-3">
                    <Alert type="error" showIcon message="Không thể tải dữ liệu" description={error.message} />
                  </div>
                )}
                {!isLoading && !isError && stationsToShow.length === 0 && (
                  <div className="p-4">
                    <Empty description={nearbyMode ? `Không có trạm trong bán kính ${radiusKm} km` : 'Không có trạm phù hợp'} />
                  </div>
                )}

                {!isLoading && !isError && stationsToShow.length > 0 && (
                  <ul className="divide-y divide-gray-100">
                    {stationsToShow.map(s => (
                      <li
                        key={s.id}
                        onClick={() => setSelectedStation(s)}
                        className={[
                          'p-3 cursor-pointer transition-colors hover:bg-green-50',
                          selectedStation?.id === s.id ? 'bg-eco-green/10' : '',
                        ].join(' ')}
                      >
                        <div className="flex items-start gap-3">
                          <div className="w-12 h-12 rounded-lg overflow-hidden shrink-0 border">
                            {s.image ? (
                              <img
                                src={s.image}
                                alt={s.name}
                                className="w-full h-full object-cover"
                                onError={(e) => { (e.target as HTMLImageElement).src = '/assets/images/station-fallback.jpg'; }}
                              />
                            ) : (
                              <div className="w-full h-full grid place-items-center bg-gray-100">
                                <EnvironmentOutlined className="text-lg text-gray-500" />
                              </div>
                            )}
                          </div>

                          <div className="min-w-0 flex-1">
                            <div className="flex items-center gap-2 flex-wrap">
                              <p className="font-semibold text-eco-green-dark truncate">{s.name}</p>
                              <Badge color={s.isActive ? 'green' : 'red'} text={s.isActive ? 'Hoạt động' : 'Tạm dừng'} />
                            </div>
                            <p className="text-sm text-gray-600 mt-0.5 line-clamp-2">{s.location}</p>

                            <div className="mt-1 text-xs text-gray-600 flex items-center gap-3">
                              {typeof s.vehicleAvailable === 'number' && typeof s.capacity === 'number' && (
                                <span>🚲 {s.vehicleAvailable} / {s.capacity} xe</span>
                              )}
                              {typeof s.distanceKm === 'number' && (
                                <span>📍 {s.distanceKm.toFixed(2)} km</span>
                              )}
                            </div>
                          </div>
                        </div>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            </aside>

            {/* === Bản đồ (phải) — chiếm hết chiều cao đã tính, không dính footer === */}
            <section className="w-full md:flex-1" style={{ height: calcPanelHeight }}>
              <StationMap
                stations={stationsToShow}
                selectedStation={selectedStation}
                onMarkerClick={setSelectedStation}
                userPosition={userPosition}
                radiusKm={nearbyMode ? radiusKm : undefined}
              />
            </section>
          </div>
        </div>

        {/* Spacer nhỏ để không chạm footer */}
        <div className="h-6" />
      </section>
    </div>
  );
};

export default StationsPage;
