using HotelBooking.Web.Models;

namespace HotelBooking.Web.Services
{
    public interface IBookingService
    {
 /// <summary>
        /// Créer une nouvelle réservation
   /// </summary>
   Task<bool> CreateBookingAsync(BookingCreation booking);
     
        /// <summary>
        /// Récupérer les réservations de l'utilisateur actuel
      /// </summary>
        Task<IEnumerable<BookingForUser>> GetUserBookingsAsync(int pageNumber = 1, int pageSize = 10);
     
    /// <summary>
        /// Récupérer une réservation spécifique par ID
        /// </summary>
        Task<BookingForUser?> GetBookingByIdAsync(Guid bookingId);
        
        /// <summary>
        /// Calculer le prix estimé pour une réservation
     /// </summary>
        Task<decimal?> CalculateEstimatedPriceAsync(Guid roomId, DateTime startDate, DateTime endDate);
        
        /// <summary>
/// Vérifier si une chambre est disponible pour une période donnée
        /// </summary>
        Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Annuler une réservation (si c'est possible selon les règles métier)
     /// </summary>
Task<bool> CancelBookingAsync(Guid bookingId);
    }
}