namespace Domain.Entities.Models
{
    public class Tag : BaseEntity<long>
    {
        public string Name { get; private set; } = null!;

        private readonly List<NewsTag> _newsTags = new();
        public IReadOnlyCollection<NewsTag> NewsTags => _newsTags.AsReadOnly();

        private Tag() { } // constructor rỗng cho EF/AutoMapper

        // Factory method
        public static Tag Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tag name cannot be empty.", nameof(name));

            return new Tag
            {
                Name = name
            };
        }

        // Business method: đổi tên tag
        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Tag name cannot be empty.", nameof(newName));

            Name = newName;
        }

        // Thêm quan hệ với News
        public void AddNews(News news)
        {
            if (_newsTags.Any(x => x.NewsId == news.Id)) return;

            var newsTag = NewsTag.Create(news.Id, this.Id);
            _newsTags.Add(newsTag);
        }

        // Xoá quan hệ với News
        public void RemoveNews(News news)
        {
            var relation = _newsTags.FirstOrDefault(x => x.NewsId == news.Id);
            if (relation != null)
                _newsTags.Remove(relation);
        }
    }

    // Entity trung gian (Many-to-Many)
    public class NewsTag
    {
        public long NewsId { get; private set; }
        public long TagId { get; private set; }

        public News? News { get; private set; }
        public Tag? Tag { get; private set; }

        private NewsTag() { }

        public static NewsTag Create(long newsId, long tagId)
        {
            return new NewsTag
            {
                NewsId = newsId,
                TagId = tagId
            };
        }
    }
}
