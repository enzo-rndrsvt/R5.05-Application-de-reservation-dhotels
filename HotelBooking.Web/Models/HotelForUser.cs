namespace HotelBooking.Web.Models
{
    public class HotelForUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BriefDescription { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public float StarRating { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public int NumberOfRooms { get; set; }
    }
}
