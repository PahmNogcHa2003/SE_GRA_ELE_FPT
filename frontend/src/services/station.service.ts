import { httpAdmin, httpUser } from './http'; 

import {
  type StationDTO,
  type GetStationsParams,
  type StationApiResponse,
  type StationPagedApiResponse,
} from '../types/station';
import type {  ApiResponse } from '../types/api';

const BASE_URL = 'stations'; 

//Api dành cho user
export const getStations = async (
  params: GetStationsParams
): Promise<StationPagedApiResponse> => {
  const response = await httpUser.get<StationPagedApiResponse>(BASE_URL, { params });
  return response.data;
};
export const getStationById = async (id: number): Promise<StationApiResponse> => {
  const response = await httpUser.get<StationApiResponse>(`${BASE_URL}/${id}`);
  return response.data;
};
export const getNearbyStations = async (params: {
  lat: number; lng: number; radiusKm?: number; page?: number; pageSize?: number;
}): Promise<StationPagedApiResponse> => {
  const res = await httpUser.get<StationPagedApiResponse>(`${BASE_URL}/nearby`, { params });
  return res.data;
};

//Api dành cho admin

export const getStationsAdmin = async (
  params: GetStationsParams
): Promise<StationPagedApiResponse> => {
  const response = await httpAdmin.get<StationPagedApiResponse>(BASE_URL, { params });
  return response.data;
};

export const getStationByIdAdmin = async (id: number): Promise<StationApiResponse> => {
  const response = await httpAdmin.get<StationApiResponse>(`${BASE_URL}/${id}`);
  return response.data;
};

export const createStation = async (
  stationData: Omit<StationDTO, 'id'>
): Promise<StationApiResponse> => {
  const response = await httpAdmin.post<StationApiResponse>(BASE_URL, stationData);
  return response.data;
};

export const updateStation = async (
  id: number,
  stationData: StationDTO
): Promise<ApiResponse<object>> => {
  const response = await httpAdmin.put<ApiResponse<object>>(`${BASE_URL}/${id}`, stationData);
  return response.data;
};

export const deleteStation = async (id: number): Promise<ApiResponse<object>> => {
  const response = await httpAdmin.delete<ApiResponse<object>>(`${BASE_URL}/${id}`);
  return response.data;
};

