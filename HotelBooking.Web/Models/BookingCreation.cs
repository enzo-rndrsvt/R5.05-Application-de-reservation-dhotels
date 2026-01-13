using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models
{
    /// <summary>
 /// Modèle pour créer une nouvelle réservation
    /// </summary>
    public class BookingCreation
    {
        [Required(ErrorMessage = "La date de début est obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "Date d'arrivée")]
 public DateTime StartingDate { get; set; } = DateTime.Today.AddDays(1);

  [Required(ErrorMessage = "La date de fin est obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de départ")]
   public DateTime EndingDate { get; set; } = DateTime.Today.AddDays(2);

        [Required(ErrorMessage = "Veuillez sélectionner une chambre")]
   [Display(Name = "Chambre")]
        public Guid RoomId { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// Prix calculé (lecture seule)
        /// </summary>
        public decimal? EstimatedPrice { get; set; }

        /// <summary>
        /// Nombre de nuits calculé
 /// </summary>
        public int NumberOfNights => (EndingDate - StartingDate).Days;

/// <summary>
        /// Validation personnalisée
      /// </summary>
  public bool IsValid => StartingDate >= DateTime.Today && EndingDate > StartingDate;
}
}