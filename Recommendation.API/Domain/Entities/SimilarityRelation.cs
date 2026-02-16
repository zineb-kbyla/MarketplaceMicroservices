namespace Recommendation.API.Domain.Entities
{
    public class SimilarityRelation
    {
        public double SimilarityScore { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
