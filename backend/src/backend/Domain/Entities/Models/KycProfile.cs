namespace Domain.Entities.Models
{
    public class KycProfile
    {
        public long Id { get; private set; }
        public long SubmissionId { get; private set; }
        public string? VerifiedName { get; private set; }
        public DateOnly? VerifiedDob { get; private set; }
        public string? VerifiedGender { get; private set; }
        public DateTimeOffset? VerifiedAt { get; private set; }

        private KycProfile() { } // constructor rỗng cho EF/AutoMapper

        // Factory method: tạo hồ sơ KYC đã xác minh
        public static KycProfile Create(long submissionId, string? name, DateOnly? dob, string? gender)
        {
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
        public void UpdateVerification(string? name, DateOnly? dob, string? gender)
        {
            VerifiedName = name;
            VerifiedDob = dob;
            VerifiedGender = gender;
            VerifiedAt = DateTimeOffset.UtcNow;
        }
    }
}
