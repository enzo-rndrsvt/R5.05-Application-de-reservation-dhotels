namespace HotelBooking.Web.Models
{
    /// <summary>
    /// Modèle de réponse pour les détails d'une chambre depuis l'API
    /// </summary>
    public class RoomDetailsDTO
    {
        public Guid Id { get; set; }
        public double Number { get; set; }
        public string Type { get; set; } = string.Empty;
        public int AdultsCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public string BriefDescription { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public Guid HotelId { get; set; }
    }
}