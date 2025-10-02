namespace Domain.Entities.Models
{
    public class KycSubmission : BaseEntity<long>
    {
        public long UserId { get; private set; }
        public string Status { get; private set; } = null!;
        public DateTimeOffset SubmittedAt { get; private set; }
        public DateTimeOffset? ReviewedAt { get; private set; }

        private readonly List<KycDocument> _kycDocuments = new();
        public IReadOnlyCollection<KycDocument> KycDocuments => _kycDocuments.AsReadOnly();

        private readonly List<KycProfile> _kycProfiles = new();
        public IReadOnlyCollection<KycProfile> KycProfiles => _kycProfiles.AsReadOnly();

        private KycSubmission() { } // Cho EF/AutoMapper

        // Factory method: tạo submission mới
        public static KycSubmission Create(long userId)
        {
            return new KycSubmission
            {
                UserId = userId,
                Status = KycSubmissionStatus.Pending,
                SubmittedAt = DateTimeOffset.UtcNow
            };
        }

        // Business methods
        public void Review(string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Status cannot be empty.");

            if (Status != KycSubmissionStatus.Pending)
                throw new InvalidOperationException("Only pending submissions can be reviewed.");

            Status = newStatus;
            ReviewedAt = DateTimeOffset.UtcNow;
        }

        public void AddDocument(KycDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            _kycDocuments.Add(document);
        }

        public void AddProfile(KycProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            _kycProfiles.Add(profile);
        }
    }

    // Domain constants / có thể thay bằng enum
    public static class KycSubmissionStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}
