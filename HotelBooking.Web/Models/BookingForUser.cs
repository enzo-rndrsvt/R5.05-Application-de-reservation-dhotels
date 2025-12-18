namespace HotelBooking.Web.Models
{
    /// <summary>
    /// Modèle pour afficher une réservation
    /// </summary>
  public class BookingForUser
    {
 public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartingDate { get; set; }
    public DateTime EndingDate { get; set; }
        public decimal Price { get; set; }
  
        // Informations sur la chambre
        public Guid RoomId { get; set; }
      public double RoomNumber { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public string RoomDescription { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public string? RoomImageUrl { get; set; } = string.Empty; // Ajout pour la maquette
        
        // Informations sur l'hôtel
   public Guid HotelId { get; set; }
     public string HotelName { get; set; } = string.Empty;
        public string HotelDescription { get; set; } = string.Empty;
        public float HotelStarRating { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        
        /// <summary>
   /// Nombre de nuits
        /// </summary>
   public int NumberOfNights => (EndingDate - StartingDate).Days;
   
        /// <summary>
        /// Statut de la réservation basé sur les dates
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
 
        /// <summary>
  /// Couleur du badge de statut pour l'affichage
     /// </summary>
      public string StatusBadgeClass => Status switch
  {
      "À venir" => "bg-primary",
  "En cours" => "bg-success",
            "Terminée" => "bg-secondary",
 _ => "bg-light"
        };
    }
}