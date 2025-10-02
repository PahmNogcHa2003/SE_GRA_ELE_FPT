namespace Domain.Entities.Models
{
    public class KycDocument : BaseEntity<long>
    {
        public long SubmissionId { get; private set; }
        public string DocType { get; private set; } = null!;
        public string DocPath { get; private set; } = null!;
        public DateTimeOffset UploadedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }

        private KycDocument() { } // Cho EF/AutoMapper

        // Factory method: tạo document mới
        public static KycDocument Create(long submissionId, string docType, string docPath)
        {
            if (string.IsNullOrWhiteSpace(docType))
                throw new ArgumentException("Document type cannot be empty.");
            if (string.IsNullOrWhiteSpace(docPath))
                throw new ArgumentException("Document path cannot be empty.");

            return new KycDocument
            {
                SubmissionId = submissionId,
                DocType = docType,
                DocPath = docPath,
                UploadedAt = DateTimeOffset.UtcNow
            };
        }

        // Business method: cập nhật đường dẫn document
        public void UpdateDocPath(string newPath)
        {
            if (string.IsNullOrWhiteSpace(newPath))
                throw new ArgumentException("Document path cannot be empty.");

            DocPath = newPath;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
