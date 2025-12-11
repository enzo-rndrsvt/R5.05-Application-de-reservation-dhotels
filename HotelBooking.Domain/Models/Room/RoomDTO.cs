using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Models.Room
{
    /// <inheritdoc cref="Entities.Room"/>
    public class RoomDTO : Entity
    {
        /// <inheritdoc cref="Entities.Room.Number"/>
        public double Number { get; set; }

        /// <inheritdoc cref="Entities.Room.Type"/>
        public string Type { get; set; }

        /// <inheritdoc cref="Entities.Room.AdultsCapacity"/>
        public int AdultsCapacity { get; set; }

        /// <inheritdoc cref="Entities.Room.ChildrenCapacity"/>
        public int ChildrenCapacity { get; set; }

        /// <inheritdoc cref="Entities.Room.BriefDescription"/>
        public string BriefDescription { get; set; }

        /// <inheritdoc cref="Entities.Room.PricePerNight"/>
        public decimal PricePerNight { get; set; }

        /// <inheritdoc cref="Entities.Room.CreationDate"/>
        public DateTime CreationDate { get; set; }

        /// <inheritdoc cref="Entities.Room.ModificationDate"/>
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Id of the hotel that contains the room.
        /// </summary>
        public Guid HotelId { get; set; }

        /// <summary>
        /// Primary image URL for the room (for backward compatibility)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Collection of image URLs for the room (up to 5 images)
        /// </summary>
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
                
                // Add images from ImageUrls first
                if (ImageUrls?.Any() == true)
                {
                    allImages.AddRange(ImageUrls.Where(url => !string.IsNullOrEmpty(url)));
                }
                
                // Add ImageUrl if it's not already in the list (backward compatibility)
                if (!string.IsNullOrEmpty(ImageUrl) && !allImages.Contains(ImageUrl))
                {
                    allImages.Insert(0, ImageUrl);
                }
                
                return allImages.Take(5).ToList(); // Limit to 5 images max
            }
        }
    }
}
