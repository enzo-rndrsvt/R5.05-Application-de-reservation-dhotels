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
        
        // Images de la chambre (récupérées depuis la base)
        public string? RoomImageUrl { get; set; } = string.Empty;
        public List<string> RoomImageUrls { get; set; } = new();
        
        // Informations sur l'hôtel
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string HotelDescription { get; set; } = string.Empty;
        public float HotelStarRating { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        
        /// <summary>
        /// URL de l'image principale de la chambre (première image disponible)
        /// </summary>
        public string? PrimaryRoomImageUrl 
        {
            get
            {
                // Si on a une image en base, on l'utilise
                if (!string.IsNullOrEmpty(RoomImageUrl))
                    return RoomImageUrl;
                
                // Sinon, on utilise une image par défaut basée sur le numéro de chambre
                return GetDefaultRoomImage((int)RoomNumber);
            }
        }
        
        /// <summary>
        /// Toutes les images disponibles pour la chambre
        /// </summary>
        public List<string> AllRoomImageUrls 
        {
            get
            {
                var allImages = new List<string>();
                
                // Ajouter l'image principale si elle existe
                if (!string.IsNullOrEmpty(RoomImageUrl))
                    allImages.Add(RoomImageUrl);
                
                // Ajouter les autres images
                if (RoomImageUrls?.Any() == true)
                    allImages.AddRange(RoomImageUrls.Where(url => !string.IsNullOrEmpty(url) && url != RoomImageUrl));
                
                // Si aucune image, utiliser l'image par défaut
                if (!allImages.Any())
                    allImages.Add(GetDefaultRoomImage((int)RoomNumber));
                
                return allImages.Distinct().ToList();
            }
        }
        
        /// <summary>
        /// Vérifier si la chambre a des images en base de données (pas les images par défaut)
        /// </summary>
        public bool HasRoomImages => !string.IsNullOrEmpty(RoomImageUrl) || (RoomImageUrls?.Any() == true);
        
        /// <summary>
        /// Obtenir une image par défaut basée sur le numéro de chambre
        /// </summary>
        private string GetDefaultRoomImage(int roomNumber)
        {
            var roomImageUrls = new[]
            {
                "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=400&h=300&fit=crop&crop=center", // Chambre luxueuse 1
                "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=400&h=300&fit=crop&crop=center", // Chambre luxueuse 2  
                "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=400&h=300&fit=crop&crop=center", // Chambre d'hôtel moderne
                "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=400&h=300&fit=crop&crop=center",   // Chambre avec vue
                "https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=400&h=300&fit=crop&crop=center", // Suite d'hôtel
                "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=400&h=300&fit=crop&crop=center", // Chambre élégante
                "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=400&h=300&fit=crop&crop=center", // Chambre cosy
                "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=400&h=300&fit=crop&crop=center"  // Chambre moderne
            };
            
            return roomImageUrls[roomNumber % roomImageUrls.Length];
        }
        
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