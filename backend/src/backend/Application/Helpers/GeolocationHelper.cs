using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class GeolocationHelper
    {
        private const double EarthRadiusInMeters = 6371000;

        /// <summary>
        /// Tính toán khoảng cách giữa hai điểm trên Trái Đất theo mét bằng công thức Haversine.
        /// </summary>
        /// <param name="lat1">Vĩ độ của điểm 1</param>
        /// <param name="lon1">Kinh độ của điểm 1</param>
        /// <param name="lat2">Vĩ độ của điểm 2</param>
        /// <param name="lon2">Kinh độ của điểm 2</param>
        /// <returns>Khoảng cách theo mét.</returns>
        public static double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = EarthRadiusInMeters * c;

            return distance;
        }

        private static double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
