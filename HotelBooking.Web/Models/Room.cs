namespace HotelBooking.Web.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public double Number { get; set; }
        public string Type { get; set; } = string.Empty;
        public int AdultsCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public string BriefDescription { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public Guid HotelId { get; set; }

        /// <summary>
        /// Current discount information
        /// </summary>
        public DiscountInfo? CurrentDiscount { get; set; }

        /// <summary>
        /// Primary image URL for backward compatibility
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Collection of image URLs (up to 5 images)
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();

        /// <summary>
        /// Get the primary image (first image or ImageUrl if ImageUrls is empty)
        /// </summary>
        public string? PrimaryImageUrl => ImageUrls.FirstOrDefault() ?? ImageUrl;

        /// <summary>
        /// Get all available images (ImageUrls parsed from database, no duplicates)
        /// </summary>
        public List<string> AllImageUrls
        {
            get
            {
                // Si ImageUrls existe et contient des images valides, les utiliser
                if (ImageUrls?.Any() == true)
                {
                    var validUrls = ImageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url) && url.Trim().Length > 3)
                        .Select(url => url.Trim())
                        .Distinct() // Éviter les doublons
                        .Take(5) // Maximum 5 images
                        .ToList();
                    
                    if (validUrls.Any())
                    {
                        Console.WriteLine($"Room {Number}: Using ImageUrls - Count={validUrls.Count}");
                        return validUrls;
                    }
                }

                // Fallback: utiliser ImageUrl si ImageUrls est vide
                if (!string.IsNullOrWhiteSpace(ImageUrl) && ImageUrl.Trim().Length > 3)
                {
                    Console.WriteLine($"Room {Number}: Fallback to single ImageUrl");
                    return new List<string> { ImageUrl.Trim() };
                }

                Console.WriteLine($"Room {Number}: No valid images found");
                return new List<string>();
            }
        }

        /// <summary>
        /// Check if room has any images
        /// </summary>
        public bool HasImages => !string.IsNullOrEmpty(ImageUrl) || (ImageUrls?.Any() == true);

        /// <summary>
        /// Get image count for display
        /// </summary>
        public int ImageCount => AllImageUrls.Count;
    }

    /// <summary>
    /// Simple discount info for display
    /// </summary>
    public class DiscountInfo
    {
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}