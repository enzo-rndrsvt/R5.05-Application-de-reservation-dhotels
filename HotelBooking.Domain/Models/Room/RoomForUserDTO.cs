using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Models.Room
{
    /// <summary>
    /// Room model to view for user.
    /// </summary>
    public class RoomForUserDTO : Entity
    {
        /// <inheritdoc cref="RoomDTO.Number"/>
        public double Number { get; set; }

        /// <inheritdoc cref="RoomDTO.Type"/>
        public string Type { get; set; }

        /// <inheritdoc cref="RoomDTO.AdultsCapacity"/>
        public int AdultsCapacity { get; set; }

        /// <inheritdoc cref="RoomDTO.ChildrenCapacity"/>
        public int ChildrenCapacity { get; set; }

        /// <inheritdoc cref="RoomDTO.BriefDescription"/>
        public string BriefDescription { get; set; }

        /// <inheritdoc cref="RoomDTO.PricePerNight"/>
        public decimal PricePerNight { get; set; }

        /// <summary>
        /// The highest available discount at the moment.
        /// </summary>
        public DiscountDTO CurrentDiscount { get; set; }

        /// <inheritdoc cref="RoomDTO.ImageUrl"/>
        public string? ImageUrl { get; set; }

        /// <inheritdoc cref="RoomDTO.ImageUrls"/>
        public List<string> ImageUrls { get; set; } = new();

        /// <summary>
        /// Get the primary image (first image or ImageUrl if ImageUrls is empty)
        /// </summary>
        public string? PrimaryImageUrl => ImageUrls.FirstOrDefault() ?? ImageUrl;

        /// <summary>
        /// Get all available images (combines ImageUrl and ImageUrls)
        /// </summary>
        public List<string> AllImageUrls
        {
            get
            {
                var allImages = new List<string>();
                
                if (ImageUrls?.Any() == true)
                {
                    allImages.AddRange(ImageUrls.Where(url => !string.IsNullOrEmpty(url)));
                }
                
                if (!string.IsNullOrEmpty(ImageUrl) && !allImages.Contains(ImageUrl))
                {
                    allImages.Insert(0, ImageUrl);
                }
                
                return allImages.Take(5).ToList();
            }
        }

        /// <summary>
        /// Check if room has any images
        /// </summary>
        public bool HasImages => !string.IsNullOrEmpty(ImageUrl) || (ImageUrls?.Any() == true);
    }
}
