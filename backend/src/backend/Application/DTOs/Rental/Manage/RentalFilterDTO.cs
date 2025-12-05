using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Rental.Manage
{
    public class RentalFilterDTO
    {
        public string? Status { get; set; }     
        public long? UserId { get; set; }
        public long? VehicleId { get; set; }
        public long? StartStationId { get; set; }
        public long? EndStationId { get; set; }
        public DateTimeOffset? FromStartTimeUtc { get; set; }
        public DateTimeOffset? ToStartTimeUtc { get; set; }
        public DateTimeOffset? FromEndTimeUtc { get; set; }
        public DateTimeOffset? ToEndTimeUtc { get; set; }
        public string? Keyword { get; set; }   
    }
}
