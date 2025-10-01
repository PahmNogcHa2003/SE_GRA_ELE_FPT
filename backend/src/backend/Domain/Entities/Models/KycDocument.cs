namespace Domain.Entities.Models
{
    public class KycDocument
    {
        public long Id { get; private set; }
        public long SubmissionId { get; private set; }
        public string? DocType { get; private set; }
        public string? DocPath { get; private set; }
        public DateTimeOffset UploadedAt { get; private set; }

        private KycDocument() { } // constructor rỗng cho EF/AutoMapper

        // Factory method: tạo document mới
        public static KycDocument Create(long submissionId, string? docType, string? docPath)
        {
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
        }
    }
}
