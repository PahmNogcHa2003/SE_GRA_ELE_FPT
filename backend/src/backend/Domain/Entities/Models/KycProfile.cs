namespace Domain.Entities.Models
{
    public class KycProfile : BaseEntity<long>
    {
        public long SubmissionId { get; private set; }
        public string VerifiedName { get; private set; } = null!;
        public DateOnly? VerifiedDob { get; private set; }
        public string? VerifiedGender { get; private set; }
        public DateTimeOffset VerifiedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }

        private KycProfile() { } // cho EF/AutoMapper

        // Factory method: tạo hồ sơ KYC đã xác minh
        public static KycProfile Create(long submissionId, string name, DateOnly? dob, string? gender)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Verified name cannot be empty.");

            return new KycProfile
            {
                SubmissionId = submissionId,
                VerifiedName = name,
                VerifiedDob = dob,
                VerifiedGender = gender,
                VerifiedAt = DateTimeOffset.UtcNow
            };
        }

        // Business method: cập nhật thông tin xác minh
        public void UpdateVerification(string name, DateOnly? dob, string? gender)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Verified name cannot be empty.");

            VerifiedName = name;
            VerifiedDob = dob;
            VerifiedGender = gender;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
