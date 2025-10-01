namespace Domain.Entities.Models
{
    public class Tag
    {
        public long Id { get; private set; }
        public string Name { get; private set; } = null!;

        private readonly List<News> _news = new();
        public IReadOnlyCollection<News> News => _news.AsReadOnly();

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

        // Gắn News vào Tag
        public void AddNews(News news)
        {
            if (!_news.Contains(news))
            {
                _news.Add(news);
            }
        }
    }
}
