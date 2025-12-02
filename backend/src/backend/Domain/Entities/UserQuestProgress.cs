using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Table("UserQuestProgress")]
    [Index(nameof(UserId), nameof(QuestId), Name = "UQ_UserQuest", IsUnique = true)]
    public class UserQuestProgress : BaseEntity<long>
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public long QuestId { get; set; }
        public int CurrentDurationMinutes { get; set; } = 0;
        public int CurrentTrips { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentDistanceKm { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;

        public DateTimeOffset? CompletedAt { get; set; }

        public DateTimeOffset? RewardClaimedAt { get; set; }

        public DateTimeOffset LastUpdatedAt { get; set; }

        [ForeignKey(nameof(QuestId))]
        public virtual Quest Quest { get; set; } = null!;
    }

}
