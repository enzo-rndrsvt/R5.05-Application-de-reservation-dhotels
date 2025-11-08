using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models
{
    public class RoomCreation
    {
        [Required(ErrorMessage = "Le numéro de chambre est requis")]
        [Range(1, 9999, ErrorMessage = "Le numéro de chambre doit être entre 1 et 9999")]
        public double Number { get; set; }

        [Required(ErrorMessage = "Le type de chambre est requis")]
      [StringLength(50, MinimumLength = 1, ErrorMessage = "Le type doit avoir entre 1 et 50 caractères")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "La capacité adultes est requise")]
        [Range(0, 50, ErrorMessage = "La capacité adultes doit être entre 0 et 50")]
   public int AdultsCapacity { get; set; }

  [Range(0, 50, ErrorMessage = "La capacité enfants doit être entre 0 et 50")]
        public int ChildrenCapacity { get; set; }

        [Required(ErrorMessage = "La description est requise")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "La description doit avoir entre 1 et 150 caractères")]
        public string BriefDescription { get; set; } = string.Empty;

   [Required(ErrorMessage = "Le prix est requis")]
        [Range(0, 999999, ErrorMessage = "Le prix ne peut pas être négatif")]
   public decimal PricePerNight { get; set; }

 [Required(ErrorMessage = "L'ID de l'hôtel est requis")]
        public Guid HotelId { get; set; }
    }
}