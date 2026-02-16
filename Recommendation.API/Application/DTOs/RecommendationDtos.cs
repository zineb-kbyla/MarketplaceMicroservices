using System.ComponentModel.DataAnnotations;

namespace Recommendation.API.Application.DTOs
{
    public class RecommendedProductDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public decimal Score { get; set; }
        public string Reason { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    public class RecordViewDto
    {
        [Required]
        [MinLength(1)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string ProductId { get; set; } = string.Empty;

        public int Duration { get; set; }
        public string Source { get; set; } = "web";
    }

    public class RecordPurchaseDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<PurchaseItemDto> Items { get; set; } = new();
    }

    public class PurchaseItemDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }

    public class UserHistoryDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class TrendingProductDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int RecentPurchases { get; set; }
        public double TrendScore { get; set; }
    }

    public class SimilarProductDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double SimilarityScore { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
