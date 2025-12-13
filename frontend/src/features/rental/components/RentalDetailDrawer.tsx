import React from 'react';
import { Drawer, Descriptions, Tag, Timeline, Card, Statistic, Row, Col, Divider, Spin } from 'antd';
import { 
  ClockCircleOutlined, 
  EnvironmentOutlined, 
  UserOutlined, 
  CarOutlined,
  HistoryOutlined 
} from '@ant-design/icons';
import type { RentalDetailDTO } from '../../../types/rental';
import { currencyVN, formatUtcToVN } from '../../../utils/datetime'; 

interface Props {
  open: boolean;
  onClose: () => void;
  data?: RentalDetailDTO;
  isLoading: boolean;
}

const RentalDetailDrawer: React.FC<Props> = ({ open, onClose, data, isLoading }) => {
  
  const getStatusColor = (status: string) => {
    switch (status?.toLowerCase()) {
      case 'ongoing': return 'processing';
      case 'end': return 'success';
      case 'overdue': return 'error';
      default: return 'default';
    }
  };

  return (
    <Drawer
      title={data ? `Chi tiết chuyến đi #${data.id} - ${data.bikeCode}` : 'Chi tiết chuyến đi'}
      width={720}
      onClose={onClose}
      open={open}
      extra={data && <Tag color={getStatusColor(data.status)}>{data.status.toUpperCase()}</Tag>}
    >
      {isLoading || !data ? (
        <div className="flex justify-center mt-10"><Spin size="large" /></div>
      ) : (
        <div className="flex flex-col gap-6">
          {/* 1. Tổng quan Metrics */}
          <div className="bg-gray-50 p-4 rounded-lg border border-gray-100">
            <Row gutter={16}>
              <Col span={8}>
                <Statistic 
                  title="Thời gian đi" 
                  value={data.durationMinutes || 0} 
                  suffix="phút" 
                  prefix={<ClockCircleOutlined />} 
                />
              </Col>
              <Col span={8}>
                <Statistic 
                  title="Quãng đường" 
                  value={data.distanceKm || 0} 
                  precision={2} 
                  suffix="km" 
                  prefix={<EnvironmentOutlined />} 
                />
              </Col>
              <Col span={8}>
                <Statistic 
                  title="Phí phát sinh" 
                  value={data.overusedFee || 0} 
                  precision={0} 
                  suffix="đ" 
                  valueStyle={{ color: (data.overusedFee || 0) > 0 ? '#cf1322' : '#3f8600' }}
                />
              </Col>
            </Row>
          </div>

          {/* 2. Thông tin chính */}
          <Row gutter={24}>
            <Col span={12}>
               <Card size="small" title={<><UserOutlined /> Khách hàng</>} bordered={false} className="shadow-sm">
                 <p><strong>Họ tên:</strong> {data.userFullName}</p>
                 <p><strong>Email:</strong> {data.userEmail}</p>
                 <p><strong>SĐT:</strong> {data.userPhone}</p>
                 <p><strong>Vé sử dụng:</strong> <Tag color="blue">{data.ticketPlanName}</Tag></p>
               </Card>
            </Col>
            <Col span={12}>
               <Card size="small" title={<><CarOutlined /> Phương tiện & Lộ trình</>} bordered={false} className="shadow-sm">
                 <p><strong>Xe:</strong> {data.bikeCode} ({data.vehicleType})</p>
                 <p><strong>Bắt đầu:</strong> {data.startStationName}</p>
                 <p className="text-gray-500 text-xs">{formatUtcToVN(data.startTimeUtc)}</p>
                 <Divider style={{ margin: '8px 0' }} />
                 <p><strong>Kết thúc:</strong> {data.endStationName || 'Chưa kết thúc'}</p>
                 <p className="text-gray-500 text-xs">{data.endTimeUtc ? formatUtcToVN(data.endTimeUtc) : '...'}</p>
               </Card>
            </Col>
          </Row>

          {/* 3. Cảnh báo nợ (Nếu có) */}
          {(data.overtimeDebtAmount || 0) > 0 && (
            <div className="bg-red-50 border border-red-200 rounded-md p-4">
              <h4 className="text-red-600 font-bold mb-2">⚠ Thông tin nợ quá giờ</h4>
              <Descriptions column={2} size="small">
                <Descriptions.Item label="Mã đơn nợ">{data.overtimeOrderNo}</Descriptions.Item>
                <Descriptions.Item label="Tổng nợ">{currencyVN(data.overtimeDebtAmount)}</Descriptions.Item>
                <Descriptions.Item label="Còn lại">{currencyVN(data.overtimeDebtRemaining)}</Descriptions.Item>
                <Descriptions.Item label="Trạng thái"><Tag color="red">{data.overtimeDebtStatus}</Tag></Descriptions.Item>
              </Descriptions>
            </div>
          )}

          {/* 4. Timeline Lịch sử */}
          <Card title={<><HistoryOutlined /> Hành trình chi tiết</>} bordered={false} className="shadow-sm">
            <Timeline
              mode="left"
              items={data.history.map(item => ({
                label: formatUtcToVN(item.timestampUtc),
                children: (
                  <>
                    <span className="font-semibold">{item.actionType}</span>
                    <p className="text-gray-500 text-sm m-0">{item.description}</p>
                    {item.distanceKm && <p className="text-xs text-blue-500">Odo: {item.distanceKm} km</p>}
                  </>
                ),
                color: item.actionType === 'RentalStarted' ? 'green' : (item.actionType === 'RentalEnded' ? 'red' : 'blue')
              }))}
            />
          </Card>
        </div>
      )}
    </Drawer>
  );
};

export default RentalDetailDrawer;