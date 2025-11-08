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
        public Discount? CurrentDiscount { get; set; }
    }

    public class Discount
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}