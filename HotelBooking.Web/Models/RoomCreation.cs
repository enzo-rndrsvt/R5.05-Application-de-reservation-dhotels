using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

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
        
        /// <summary>
        /// URL de l'image principale (pour la compatibilité ascendante)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Collection des URL d'images téléchargées (jusqu'à 5 images)
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();

        /// <summary>
        /// Fichier d'image principal à télécharger (compatibilité ascendante)
        /// </summary>
        public IBrowserFile? RoomImage { get; set; }

        /// <summary>
        /// Fichiers d'images multiples à télécharger (jusqu'à 5 images)
        /// </summary>
        public List<IBrowserFile> RoomImages { get; set; } = new();

        /// <summary>
        /// Obtenir toutes les URL d'images téléchargées
        /// </summary>
        public List<string> AllImageUrls
        {
            get
            {
                var allImages = new List<string>();
                
                if (ImageUrls?.Any() == true)
                {
                    allImages.AddRange(ImageUrls.Where(url => !string.IsNullOrEmpty(url)));
                }
                
                if (!string.IsNullOrEmpty(ImageUrl) && !allImages.Contains(ImageUrl))
                {
                    allImages.Insert(0, ImageUrl);
                }
                
                return allImages.Take(5).ToList();
            }
        }

        /// <summary>
        /// Vérifier si la chambre a des images téléchargées
        /// </summary>
        public bool HasImages => !string.IsNullOrEmpty(ImageUrl) || (ImageUrls?.Any() == true);

        /// <summary>
        /// Obtenir le nombre total d'images
        /// </summary>
        public int ImageCount => AllImageUrls.Count;

        /// <summary>
        /// Vérifier si nous pouvons ajouter plus d'images (max 5)
        /// </summary>
        public bool CanAddMoreImages => ImageCount < 5;
    }
}