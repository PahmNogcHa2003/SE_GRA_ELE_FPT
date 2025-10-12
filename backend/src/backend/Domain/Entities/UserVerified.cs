using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserVerified
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? Type { get; set; }
        public string? Number { get; set; }
        public string? FullName { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public DateTimeOffset? IssuedDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string? IssuedBy { get; set; }
        public long? VerifiedBy { get; set; }
        public DateTimeOffset? VerifiedAt { get; set; }
        public string? VerificationMethod { get; set; }
        public long? SubmissionId { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public long? CreatedId { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public long? UpdatedId { get; set; }
        public virtual AspNetUser User { get; set; } = null!;
        public virtual AspNetUser? VerifiedByUser { get; set; }
        public virtual KycSubmission? Submission { get; set; }
    }
}
