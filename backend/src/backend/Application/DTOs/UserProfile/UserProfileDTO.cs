using Application.DTOs.JsonConvert;
using Application.DTOs.RentalHistory;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs.UserProfile
{
    public class UserProfileDTO
    {
        [Key]
        public long UserId { get; set; }
        public string? IsVerify { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        [StringLength(150)]
        public string? FullName { get; set; }

        [Column(TypeName = "date")]
        [JsonConverter(typeof(DateTimeToDateStringConverter))]
        public DateTime? Dob { get; set; }

        [StringLength(10)]
        [Unicode(false)]
        public string? Gender { get; set; }

        [StringLength(255)]
        public string? AvatarUrl { get; set; }
        public string? AvatarPublicId { get; set; }

        [StringLength(150)]
        public string? EmergencyName { get; set; }
            
        [Required]
        [StringLength(15)]
        public string EmergencyPhone { get; set; }

        public int? ProvinceCode { get; set; }
        [StringLength(50)]
        public string? ProvinceName { get; set; }
        public int? WardCode { get; set; }

        [StringLength(100)]
        public string? WardName { get; set; }

        public string? AddressDetail { get; set; }

        [Required]
        [StringLength(15)]
        public string NumberCard { get; set; } // ĐÃ SỬA: string

        [StringLength(150)]
        public string? PlaceOfOrigin { get; set; }

        [StringLength(150)]
        public string? PlaceOfResidence { get; set; }

        [Precision(7)]
        public DateTimeOffset? IssuedDate { get; set; }

        [Precision(7)]
        public DateTimeOffset? ExpiryDate { get; set; }

        [StringLength(100)]
        public string? IssuedBy { get; set; }

        [Required]
        [Precision(0)]
        public DateTimeOffset CreatedAt { get; set; }

        [Required]
        [Precision(0)]
        public DateTimeOffset UpdatedAt { get; set; }

        public List<RentalHistoryDTO> rentalHistories = new List<RentalHistoryDTO>();
    }
}