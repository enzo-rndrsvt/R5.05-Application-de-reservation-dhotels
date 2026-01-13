namespace HotelBooking.Domain.Models
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

        /// <summary>
        /// Create an available room info
        /// </summary>
        public static RoomAvailabilityInfo Available(Guid roomId)
        {
            return new RoomAvailabilityInfo
            {
                RoomId = roomId,
                IsAvailable = true,
                Message = "Chambre disponible pour la période demandée"
            };
        }

        /// <summary>
        /// Create an unavailable room info with conflicts
        /// </summary>
        public static RoomAvailabilityInfo Unavailable(Guid roomId, string message, List<BookingConflict> conflicts, DateTime? nextAvailable = null)
        {
            return new RoomAvailabilityInfo
            {
                RoomId = roomId,
                IsAvailable = false,
                Message = message,
                Conflicts = conflicts,
                NextAvailableDate = nextAvailable
            };
        }
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