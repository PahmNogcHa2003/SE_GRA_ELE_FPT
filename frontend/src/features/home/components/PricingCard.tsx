// src/features/home/components/PricingCard.tsx
import React from 'react';


// Định nghĩa kiểu dữ liệu cho props
interface PricingCardProps {
  title: string;
  price: string;
  period: string;
  features: string[];
  isPopular?: boolean;
}

const PricingCard: React.FC<PricingCardProps> = ({
  title,
  price,
  period,
  features,
  isPopular = false,
}) => {
  return (
    <div className={`bg-white rounded-xl shadow-lg p-8 relative overflow-hidden transition-transform duration-300 hover:scale-105 ${isPopular ? 'border-4 border-eco-green' : 'border-4 border-transparent'}`}>
      {isPopular && (
        <div className="absolute top-0 right-0 bg-eco-green text-white text-xs font-bold px-4 py-1"
             style={{ transform: 'rotate(45deg) translate(25%, -50%)', transformOrigin: 'top right' }}>
          Phổ biến
        </div>
      )}
      
      <h3 className="text-2xl font-semibold text-gray-800 text-center mb-2">{title}</h3>
      <div className="text-center mb-6">
        <span className="text-5xl font-bold text-eco-green">{price}</span>
        <span className="text-gray-500">{period}</span>
      </div>
      
      <ul className="space-y-3 mb-8 text-sm text-gray-600">
        {features.map((feature, index) => (
          <li key={index} className="flex items-start">
            <svg className="w-5 h-5 text-eco-green mr-2 shrink-0" fill="currentColor" viewBox="0 0 20 20"><path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd"></path></svg>
            <span>{feature}</span>
          </li>
        ))}
      </ul>
      

    </div>
  );
};

export default PricingCard;