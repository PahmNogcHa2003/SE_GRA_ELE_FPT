import http from './http'; // Axios instance đã được cấu hình

// Import tất cả các types và type aliases cần thiết từ file station.ts
import {
  type StationDTO,
  type GetStationsParams,
  type StationApiResponse,
  type StationPagedApiResponse,
} from '../types/station';

// Import ApiResponse để dùng cho các response không trả về StationDTO
import type {  ApiResponse } from '../types/api';

const BASE_URL = 'stations'; // Endpoint của API

/**
 * Lấy danh sách trạm có phân trang, tìm kiếm và lọc.
 * @param params Các tham số query.
 * @returns Promise chứa dữ liệu các trạm đã được phân trang.
 */
export const getStations = async (
  params: GetStationsParams
): Promise<StationPagedApiResponse> => {
  const response = await http.get<StationPagedApiResponse>(BASE_URL, { params });
  return response.data;
};

/**
 * Lấy thông tin chi tiết của một trạm theo ID.
 * @param id ID của trạm cần lấy.
 * @returns Promise chứa dữ liệu của một trạm.
 */
export const getStationById = async (id: number): Promise<StationApiResponse> => {
  const response = await http.get<StationApiResponse>(`${BASE_URL}/${id}`);
  return response.data;
};

/**
 * Tạo một trạm xe mới.
 * @param stationData Dữ liệu của trạm mới (không bao gồm ID).
 * @returns Promise chứa dữ liệu của trạm vừa được tạo.
 */
export const createStation = async (
  stationData: Omit<StationDTO, 'id'>
): Promise<StationApiResponse> => {
  const response = await http.post<StationApiResponse>(BASE_URL, stationData);
  return response.data;
};

/**
 * Cập nhật thông tin của một trạm đã có.
 * @param id ID của trạm cần cập nhật.
 * @param stationData Dữ liệu mới của trạm.
 * @returns Promise chứa object rỗng và message thành công.
 */
export const updateStation = async (
  id: number,
  stationData: StationDTO
): Promise<ApiResponse<object>> => {
  const response = await http.put<ApiResponse<object>>(`${BASE_URL}/${id}`, stationData);
  return response.data;
};

/**
 * Xóa một trạm theo ID.
 * @param id ID của trạm cần xóa.
 * @returns Promise chứa object rỗng và message thành công.
 */
export const deleteStation = async (id: number): Promise<ApiResponse<object>> => {
  const response = await http.delete<ApiResponse<object>>(`${BASE_URL}/${id}`);
  return response.data;
};
/**
 * Lấy danh sách trạm gần vị trí đã cho trong một bán kính nhất định.
 * @param params Các tham số bao gồm lat, lng, radiusKm, page, pageSize.
 * @returns Promise chứa dữ liệu các trạm gần đó đã được phân trang.
 */
export const getNearbyStations = async (params: {
  lat: number; lng: number; radiusKm?: number; page?: number; pageSize?: number;
}): Promise<StationPagedApiResponse> => {
  const res = await http.get<StationPagedApiResponse>(`${BASE_URL}/nearby`, { params });
  return res.data;
};
