namespace Domain.Entities.Models
{
    public class KycSubmission
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public string Status { get; private set; }
        public DateTimeOffset SubmittedAt { get; private set; }
        public DateTimeOffset? ReviewedAt { get; private set; }

        private readonly List<KycDocument> _kycDocuments = new();
        public IReadOnlyCollection<KycDocument> KycDocuments => _kycDocuments.AsReadOnly();

        private readonly List<KycProfile> _kycProfiles = new();
        public IReadOnlyCollection<KycProfile> KycProfiles => _kycProfiles.AsReadOnly();

        private KycSubmission() { } // constructor rỗng cho EF/AutoMapper

        // Factory method: tạo submission mới
        public static KycSubmission Create(long userId)
        {
            return new KycSubmission
            {
                UserId = userId,
                Status = "Pending",
                SubmittedAt = DateTimeOffset.UtcNow
            };
        }

        // Business methods
        public void Review(string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Status cannot be empty.");

            Status = newStatus;
            ReviewedAt = DateTimeOffset.UtcNow;
        }

        public void AddDocument(KycDocument document)
        {
            _kycDocuments.Add(document);
        }

        public void AddProfile(KycProfile profile)
        {
            _kycProfiles.Add(profile);
        }
    }
}
