namespace HotelBooking.Web.Models
{
    public class HotelSearchRequest
    {
        public string SearchQuery { get; set; } = string.Empty;
        public DateTime CheckinDate { get; set; } = DateTime.Today.AddDays(1);
        public DateTime CheckoutDate { get; set; } = DateTime.Today.AddDays(2);
        public int NumberOfAdults { get; set; } = 1;
        public int NumberOfChildren { get; set; } = 0;
        public int NumberOfRooms { get; set; } = 1;
        public decimal MinRoomPrice { get; set; } = 0;
        public decimal MaxRoomPrice { get; set; } = 10000;
        public string RoomsType { get; set; } = string.Empty;
    }
}
