namespace HotelBooking.Web.Models
{
    /// <summary>
    /// Modèle pour recevoir les réservations avec détails depuis l'API
    /// </summary>
    public class BookingWithDetailsDTO
 {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public decimal Price { get; set; }
        public Guid UserId { get; set; }

 // Room information
        public Guid RoomId { get; set; }
        public double RoomNumber { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public string RoomDescription { get; set; } = string.Empty;
   public decimal PricePerNight { get; set; }
      public int AdultsCapacity { get; set; }
     public int ChildrenCapacity { get; set; }

        // Hotel information
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string HotelDescription { get; set; } = string.Empty;
    public float HotelStarRating { get; set; }
        public string OwnerName { get; set; } = string.Empty;

    /// <summary>
        /// Number of nights in the booking
        /// </summary>
        public int NumberOfNights => (EndingDate - StartingDate).Days;

        /// <summary>
     /// Booking status based on dates
     /// </summary>
        public string Status => GetBookingStatus();

      private string GetBookingStatus()
        {
        var now = DateTime.Now;
if (now < StartingDate)
       return "À venir";
         else if (now >= StartingDate && now <= EndingDate)
  return "En cours";
    else
          return "Terminée";
        }
    }
}