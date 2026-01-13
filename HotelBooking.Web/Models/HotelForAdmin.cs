namespace HotelBooking.Web.Models
{
    public class HotelForAdmin
    {
 public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float StarRating { get; set; }
      public string OwnerName { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
   public DateTime ModificationDate { get; set; }
        public int NumberOfRooms { get; set; }
    }
}