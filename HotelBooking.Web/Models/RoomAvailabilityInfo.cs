namespace HotelBooking.Web.Models
{
    /// <summary>
    /// Information about room availability for a specific period
    /// </summary>
    public class RoomAvailabilityInfo
    {
        public Guid RoomId { get; set; }
        public bool IsAvailable { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? NextAvailableDate { get; set; }
        public List<BookingConflict> Conflicts { get; set; } = new();
    }

    /// <summary>
    /// Information about a booking conflict
    /// </summary>
    public class BookingConflict
    {
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public Guid BookingId { get; set; }

        public string Period => $"{StartingDate:dd/MM/yyyy} au {EndingDate:dd/MM/yyyy}";
    }
}