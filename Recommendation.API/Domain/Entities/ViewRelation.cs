namespace Recommendation.API.Domain.Entities
{
    public class ViewRelation
    {
        public DateTime ViewedAt { get; set; }
        public int Duration { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
