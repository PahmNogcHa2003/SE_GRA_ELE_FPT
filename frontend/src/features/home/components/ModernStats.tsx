// src/features/home/components/ModernStats.tsx
import React from 'react';

const stats = [
  { label: 'Trạm xe', value: '50+' },
  { label: 'Người dùng', value: '20k+' },
  { label: 'CO2 Giảm thải', value: '1.5T' },
  { label: 'Chuyến đi', value: '100k+' },
];

const ModernStats: React.FC = () => {
  return (
    <section className="py-16 bg-green-900 text-white relative overflow-hidden">
      <div className="absolute inset-0 opacity-10 bg-[url('https://www.transparenttextures.com/patterns/cubes.png')]"></div>
      <div className="container mx-auto px-4 relative z-10">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-8 text-center divide-x divide-white/10">
          {stats.map((stat, idx) => (
            <div key={idx} className="p-4">
              <div className="text-4xl md:text-5xl font-bold text-lime-400 mb-2">{stat.value}</div>
              <div className="text-gray-300 font-medium uppercase tracking-wider text-sm">{stat.label}</div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default ModernStats;