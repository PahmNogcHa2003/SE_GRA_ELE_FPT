export default function App() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500 text-white">
      {/* Card */}
      <div className="bg-white text-gray-800 rounded-2xl shadow-xl p-8 w-96">
        <h1 className="text-3xl font-bold text-center mb-4 text-indigo-600">
          🚀 Hello Tailwind v4
        </h1>
        <p className="text-center text-gray-600 mb-6">
          Đây là ví dụ minh họa cho Tailwind CSS.  
          Bạn có thể thay đổi class để thấy sự khác biệt ngay lập tức.
        </p>

        {/* Buttons */}
        <div className="flex gap-4 justify-center">
          <button className="px-4 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-700 text-white font-semibold transition">
            Primary
          </button>
          <button className="px-4 py-2 rounded-lg bg-gray-200 hover:bg-gray-300 text-gray-800 font-medium transition">
            Secondary
          </button>
        </div>
      </div>
    </div>
  );
}
