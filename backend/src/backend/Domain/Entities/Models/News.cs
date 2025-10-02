namespace Domain.Entities.Models
{
    public class News : BaseEntity<long>
    {
        public string Title { get; private set; } = null!;
        public string Content { get; private set; } = null!;
        public long AuthorId { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<Tag> _tags = new();
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        private News() { } // Cho EF/AutoMapper

        // Factory method: tạo news mới
        public static News Create(string title, string content, long authorId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.");

            return new News
            {
                Title = title,
                Content = content,
                AuthorId = authorId,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            };
        }

        // Business methods
        public void UpdateContent(string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.");

            Title = title;
            Content = content;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;

        public void AddTag(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            if (_tags.Contains(tag))
                return;

            _tags.Add(tag);
        }

        public void RemoveTag(Tag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            _tags.Remove(tag);
        }
    }
}
