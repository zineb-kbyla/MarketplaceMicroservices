namespace Recommendation.API.Domain.Entities
{
    public class UserNode
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime JoinedDate { get; set; }
        public DateTime LastActive { get; set; }
        public List<string> PreferredCategories { get; set; } = new();
    }
}
