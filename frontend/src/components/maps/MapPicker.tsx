import React, { useState, useEffect, useCallback } from 'react';
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from 'react-leaflet';
import L from 'leaflet';

import 'leaflet/dist/leaflet.css';
import 'leaflet-control-geocoder/dist/Control.Geocoder.css';
import 'leaflet-control-geocoder';

    delete (L.Icon.Default.prototype as any)._getIconUrl;
    L.Icon.Default.mergeOptions({
    iconRetinaUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon-2x.png',
    iconUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png',
    shadowUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png',
    });
    export interface MapPickerValue {
    lat: number;
    lng: number;
    }

    interface MapPickerProps {
    value?: MapPickerValue; 
    onChange?: (value: MapPickerValue) => void;
    height?: string;
    }
    const HOA_LAC_FPT: L.LatLngTuple = [21.01335, 105.5277];
    const MapEventHandlers: React.FC<{ onSelectPosition: (lat: number, lng: number) => void }> = ({
    onSelectPosition,
    }) => {
    useMapEvents({
        click(e) {
        onSelectPosition(e.latlng.lat, e.latlng.lng);
        },
    });
    return null;
    };
    const GeocoderControl: React.FC<{ onSelectPosition: (lat: number, lng: number) => void }> = ({
    onSelectPosition,
    }) => {
    const map = useMap();
    useEffect(() => {
        if (!(L.Control as any).geocoder) return;
        const geocoder = (L.Control as any)
        .geocoder({
            defaultMarkGeocode: true,
            placeholder: 'Tìm kiếm địa điểm (FPT, Hòa Lạc...)',
            collapsed: true,
        })
        .on('markgeocode', function (e: any) {
            const latlng = e.geocode.center;
            map.setView(latlng, 16);
            onSelectPosition(latlng.lat, latlng.lng);
        })
        .addTo(map);
        return () => {
        geocoder.remove();
        };
    }, [map, onSelectPosition]);

    return null;
    };
    const MapPicker: React.FC<MapPickerProps> = ({ value, onChange, height = '350px' }) => {
    const [position, setPosition] = useState<L.LatLngTuple>(
        value ? [value.lat, value.lng] : HOA_LAC_FPT
    );
    useEffect(() => {
        if (value) {
        setPosition([value.lat, value.lng]);
        }
    }, [value]);

    const handleSelectPosition = useCallback(
        (lat: number, lng: number) => {
        setPosition([lat, lng]);
        onChange?.({ lat, lng });
        },
        [onChange]
    );
    return (
        <div
        style={{
            height,
            width: '100%',
            borderRadius: 8,
            overflow: 'hidden',
            border: '1px solid #d9d9d9',
        }}
        >
        <MapContainer
            center={position}
            zoom={15}
            style={{ height: '100%', width: '100%' }}
            scrollWheelZoom
        >
            <TileLayer
            attribution='© OpenStreetMap contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <MapEventHandlers onSelectPosition={handleSelectPosition} />
            <GeocoderControl onSelectPosition={handleSelectPosition} />
            <Marker position={position} />
        </MapContainer>
        </div>
    );
    };
    export default MapPicker;
