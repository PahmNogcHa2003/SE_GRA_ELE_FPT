import React, { useCallback, useEffect, useRef, useState } from "react";
import { GoogleMap, AdvancedMarker, PinElement, Autocomplete, useJsApiLoader } from "@react-google-maps/api";
import { Button } from "antd";

type LatLng = { lat: number; lng: number };
type ChangePayload = { lat: number; lng: number; address?: string };

type Props = {
  value?: LatLng | null;
  onChange?: (val: ChangePayload) => void;
  height?: number;
  showUseAddressButton?: boolean;
};

const containerStyle = (h: number) => ({
  width: "100%",
  height: `${h}px`,
  borderRadius: 8,
  overflow: "hidden",
});

const defaultCenter = { lat: 21.028511, lng: 105.804817 }; // Hà Nội

const LocationPickerMap: React.FC<Props> = ({ value, onChange, height = 260, showUseAddressButton = true }) => {
  const { isLoaded } = useJsApiLoader({
    googleMapsApiKey: import.meta.env.VITE_GOOGLE_MAPS_API_KEY as string,
    libraries: ["places", "marker"],
  });

  const [markerPos, setMarkerPos] = useState<LatLng | null>(value ?? null);
  const [address, setAddress] = useState<string>("");
  const mapRef = useRef<google.maps.Map | null>(null);
  const autoRef = useRef<google.maps.places.Autocomplete | null>(null);
  const geocoderRef = useRef<google.maps.Geocoder | null>(null);

  useEffect(() => {
    if (isLoaded && !geocoderRef.current) geocoderRef.current = new google.maps.Geocoder();
  }, [isLoaded]);

  const reverseGeocode = useCallback(async (pos: LatLng) => {
    if (!geocoderRef.current) return;
    try {
      const res = await geocoderRef.current.geocode({ location: pos });
      const addr = res.results?.[0]?.formatted_address || "";
      setAddress(addr);
      onChange?.({ ...pos, address: addr });
    } catch {
      onChange?.({ ...pos });
    }
  }, [onChange]);

  useEffect(() => {
    if (value) {
      setMarkerPos(value);
      reverseGeocode(value);
    }
  }, [value, reverseGeocode]);

  const onMapLoad = (map: google.maps.Map) => {
    mapRef.current = map;
    if (markerPos) {
      map.panTo(markerPos);
      map.setZoom(15);
    }
  };

  const handleSetPos = (pos: LatLng) => {
    setMarkerPos(pos);
    mapRef.current?.panTo(pos);
    reverseGeocode(pos);
  };

  const handleMapClick = (e: google.maps.MapMouseEvent) => {
    if (!e.latLng) return;
    handleSetPos({ lat: e.latLng.lat(), lng: e.latLng.lng() });
  };

  const onAutocompleteLoad = (ac: google.maps.places.Autocomplete) => (autoRef.current = ac);

  const onPlaceChanged = () => {
    const place = autoRef.current?.getPlace();
    const loc = place?.geometry?.location;
    if (!loc) return;
    const pos = { lat: loc.lat(), lng: loc.lng() };
    const addr = place?.formatted_address || place?.name || "";
    setAddress(addr);
    setMarkerPos(pos);
    onChange?.({ ...pos, address: addr });
    mapRef.current?.panTo(pos);
    mapRef.current?.setZoom(16);
  };

  if (!isLoaded) return <div style={{ ...containerStyle(height), background: "#f5f5f5" }} />;

  return (
    <div style={{ display: "grid", gap: 8 }}>
      <Autocomplete onLoad={onAutocompleteLoad} onPlaceChanged={onPlaceChanged}>
        <input
          type="text"
          placeholder="Tìm địa điểm (vd: Đại học FPT Hòa Lạc)…"
          style={{
            width: "100%", height: 40, padding: "0 12px",
            border: "1px solid #d9d9d9", borderRadius: 8,
          }}
        />
      </Autocomplete>

      <div style={containerStyle(height)}>
        <GoogleMap
          onLoad={onMapLoad}
          mapContainerStyle={{ width: "100%", height: "100%" }}
          center={markerPos || defaultCenter}
          zoom={markerPos ? 15 : 12}
          onClick={handleMapClick}
          options={{ fullscreenControl: false, streetViewControl: false, mapTypeControl: false }}
        >
          {markerPos && (
            <AdvancedMarker
              position={markerPos}
              draggable
              // @ts-expect-error AdvancedMarker drag event typing
              onDragEnd={(e) => {
                const { latLng } = e;
                if (!latLng) return;
                handleSetPos({ lat: latLng.lat(), lng: latLng.lng() });
              }}
            >
              <PinElement />
            </AdvancedMarker>
          )}
        </GoogleMap>
      </div>

      <div style={{ display: "flex", gap: 8, alignItems: "center", justifyContent: "space-between", flexWrap: "wrap" }}>
        <div style={{ fontSize: 12, color: "#555", flex: 1, minWidth: 200 }}>
          <div><b>Tọa độ:</b> {markerPos ? `${markerPos.lat.toFixed(6)}, ${markerPos.lng.toFixed(6)}` : "Chưa chọn"}</div>
          <div style={{ marginTop: 4 }}>
            <b>Địa chỉ:</b> {address || (markerPos ? "Đang xác định địa chỉ..." : "Chưa chọn")}
          </div>
        </div>
        {showUseAddressButton && (
          <Button size="small" onClick={() => { if (address && markerPos) onChange?.({ ...markerPos, address }); }}>
            Dùng địa chỉ này
          </Button>
        )}
      </div>
    </div>
  );
};

export default LocationPickerMap;
