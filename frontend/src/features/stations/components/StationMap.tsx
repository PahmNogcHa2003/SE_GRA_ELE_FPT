import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { GoogleMap, InfoWindowF, MarkerF, useJsApiLoader, CircleF } from '@react-google-maps/api';
import { Spin } from 'antd';
import type { StationDTO } from '../../../types/station';

const containerStyle = { width: '100%', height: '100%' };
const defaultCenter = { lat: 21.0278, lng: 105.8342 };

interface StationMapProps {
  stations: StationDTO[];
  selectedStation: StationDTO | null;
  onMarkerClick: (s: StationDTO | null) => void;
  userPosition?: { lat: number; lng: number } | null;
  radiusKm?: number;
}

const StationMap: React.FC<StationMapProps> = ({
  stations,
  selectedStation,
  onMarkerClick,
  userPosition,
  radiusKm,
}) => {
  const apiKey = import.meta.env.VITE_GOOGLE_MAPS_API_KEY || '';
  const { isLoaded } = useJsApiLoader({ id: 'google-map-script', googleMapsApiKey: apiKey });

  const [map, setMap] = useState<google.maps.Map | null>(null);

  const center = useMemo(() => {
    if (selectedStation) return { lat: Number(selectedStation.lat), lng: Number(selectedStation.lng) };
    if (userPosition) return userPosition;
    return defaultCenter;
  }, [selectedStation, userPosition]);

  const onLoad = useCallback((m: google.maps.Map) => setMap(m), []);
  const onUnmount = useCallback(() => setMap(null), []);

  // Fit bounds để nhìn thấy cả user + các trạm
  useEffect(() => {
    if (!map) return;
    const b = new google.maps.LatLngBounds();
    let hasAny = false;

    if (userPosition) {
      b.extend(userPosition);
      hasAny = true;
    }
    stations.forEach(s => {
      b.extend({ lat: Number(s.lat), lng: Number(s.lng) });
      hasAny = true;
    });

    if (hasAny) map.fitBounds(b, 80);
  }, [map, stations, userPosition]);

  if (!isLoaded) {
    return <div className="h-full flex items-center justify-center"><Spin size="large" /></div>;
  }

  return (
    <GoogleMap
      mapContainerStyle={containerStyle}
      center={center}
      zoom={selectedStation ? 15 : 12}
      onLoad={onLoad}
      onUnmount={onUnmount}
      options={{
        streetViewControl: false,
        mapTypeControl: false,
        fullscreenControl: false,
      }}
    >
      {/* Marker trạm */}
      {stations.map(s => (
        <MarkerF
          key={s.id}
          position={{ lat: Number(s.lat), lng: Number(s.lng) }}
          onClick={() => onMarkerClick(s)}
        />
      ))}

      {/* Marker người dùng */}
      {userPosition && (
        <MarkerF
          position={userPosition}
          icon={{
            url: 'https://maps.google.com/mapfiles/ms/icons/blue-dot.png',
            scaledSize: new google.maps.Size(40, 40),
          }}
        />
      )}

      {/* Vòng tròn bán kính */}
      {userPosition && radiusKm && radiusKm > 0 && (
        <CircleF
          center={userPosition}
          radius={radiusKm * 1000}
          options={{ strokeOpacity: 0.6, strokeWeight: 1, fillOpacity: 0.08 }}
        />
      )}

      {/* InfoWindow */}
      {selectedStation && (
        <InfoWindowF
          position={{ lat: Number(selectedStation.lat), lng: Number(selectedStation.lng) }}
          onCloseClick={() => onMarkerClick(null)}
        >
          <div className="p-2 max-w-[260px]">
            {selectedStation.image && (
              <img
                src={selectedStation.image}
                alt={selectedStation.name}
                className="w-full h-28 object-cover rounded-md mb-2 shadow-sm"
                onError={(e) => { (e.target as HTMLImageElement).src = '/assets/images/station-fallback.jpg'; }}
              />
            )}
            <h4 className="font-semibold text-base text-eco-green-dark mb-1">
              {selectedStation.name}
            </h4>
            <p className="text-sm text-gray-700">{selectedStation.location}</p>

            {typeof selectedStation.vehicleAvailable === 'number' && typeof selectedStation.capacity === 'number' && (
              <p className="text-xs text-gray-600 mt-1">🚲 {selectedStation.vehicleAvailable} / {selectedStation.capacity} xe</p>
            )}

            {typeof selectedStation.distanceKm === 'number' && (
              <p className="text-xs text-gray-500 mt-1">📍 Cách bạn ~{selectedStation.distanceKm.toFixed(2)} km</p>
            )}

            <p className={`text-xs font-medium mt-1 ${selectedStation.isActive ? 'text-green-600' : 'text-red-500'}`}>
              {selectedStation.isActive ? 'Đang hoạt động' : 'Tạm dừng'}
            </p>
          </div>
        </InfoWindowF>
      )}
    </GoogleMap>
  );
};

export default StationMap;
