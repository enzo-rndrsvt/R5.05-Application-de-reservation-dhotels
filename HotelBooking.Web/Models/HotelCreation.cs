namespace HotelBooking.Web.Models
{
    public class HotelCreation
    {
        public string Name { get; set; } = string.Empty;
        public string BriefDescription { get; set; } = string.Empty;
        public string FullDescription { get; set; } = string.Empty;
        public float StarRating { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string Geolocation { get; set; } = string.Empty;
        public Guid CityId { get; set; }
    }
}