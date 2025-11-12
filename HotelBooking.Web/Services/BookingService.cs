using System.Net.Http.Json;
using HotelBooking.Web.Models;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace HotelBooking.Web.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

  public BookingService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
      {
       _httpClient = httpClientFactory.CreateClient("HotelBookingAPI");
   _localStorage = localStorage;
   }

        public async Task<bool> CreateBookingAsync(BookingCreation booking)
        {
 await AddAuthenticationHeader();

try
  {
              var bookingData = new
      {
 StartingDate = booking.StartingDate,
     EndingDate = booking.EndingDate,
     RoomId = booking.RoomId
      };

      var json = JsonConvert.SerializeObject(bookingData);
   var content = new StringContent(json, Encoding.UTF8, "application/json");

         var response = await _httpClient.PostAsync("api/users/current-user/bookings", content);
     return response.IsSuccessStatusCode;
     }
  catch (Exception ex)
     {
                Console.WriteLine($"Erreur lors de la création de la réservation: {ex.Message}");
         return false;
            }
   }

   public async Task<IEnumerable<BookingForUser>> GetUserBookingsAsync(int pageNumber = 1, int pageSize = 10)
        {
            await AddAuthenticationHeader();

          try
   {
        var response = await _httpClient.GetAsync($"api/users/current-user/bookings?pageNumber={pageNumber}&pageSize={pageSize}");
 
        if (response.IsSuccessStatusCode)
        {
  var bookingsWithDetails = await response.Content.ReadFromJsonAsync<IEnumerable<BookingWithDetailsDTO>>();
 
       if (bookingsWithDetails != null)
    {
       return bookingsWithDetails.Select(MapToBookingForUser);
      }
        }
        
        return Enumerable.Empty<BookingForUser>();
    }
  catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors de la récupération des réservations: {ex.Message}");
    return Enumerable.Empty<BookingForUser>();
     }
    }

     public async Task<BookingForUser?> GetBookingByIdAsync(Guid bookingId)
        {
    await AddAuthenticationHeader();
    
         try
      {
  var response = await _httpClient.GetAsync($"api/users/current-user/bookings/{bookingId}");
        
      if (response.IsSuccessStatusCode)
     {
         var bookingWithDetails = await response.Content.ReadFromJsonAsync<BookingWithDetailsDTO>();
  
       if (bookingWithDetails != null)
      {
      return MapToBookingForUser(bookingWithDetails);
       }
        }
        
        return null;
 }
     catch (Exception ex)
     {
   Console.WriteLine($"Erreur lors de la récupération de la réservation: {ex.Message}");
     return null;
      }
        }

     public async Task<decimal?> CalculateEstimatedPriceAsync(Guid roomId, DateTime startDate, DateTime endDate)
        {
  await AddAuthenticationHeader();

            try
            {
   // Récupérer les informations de la chambre pour calculer le prix
     var response = await _httpClient.GetAsync($"api/rooms/{roomId}");
                
if (response.IsSuccessStatusCode)
         {
                 var room = await response.Content.ReadFromJsonAsync<RoomDetailsDTO>();
if (room != null)
      {
            int nights = (endDate - startDate).Days;
            // Le calcul doit être : prix par nuit × nombre de nuits
            // PAS multiplié par le nombre de personnes car c'est le prix de la chambre
  return room.PricePerNight * nights;
  }
     }
     
      return null;
 }
  catch (Exception ex)
    {
      Console.WriteLine($"Erreur lors du calcul du prix: {ex.Message}");
           return null;
            }
     }

     public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime startDate, DateTime endDate)
        {
   await AddAuthenticationHeader();

 try
            {
                var response = await _httpClient.GetAsync($"api/users/room-availability/{roomId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
                
                if (response.IsSuccessStatusCode)
                {
                    var availability = await response.Content.ReadFromJsonAsync<RoomAvailabilityInfo>();
                    return availability?.IsAvailable ?? false;
                }
                
                Console.WriteLine($"Erreur API lors de la vérification de disponibilité: {response.StatusCode}");
                return false;
       }
   catch (Exception ex)
         {
                Console.WriteLine($"Erreur lors de la vérification de disponibilité: {ex.Message}");
    return false;
     }
        }

        /// <summary>
        /// Obtenir des informations détaillées sur la disponibilité d'une chambre
        /// </summary>
        public async Task<RoomAvailabilityInfo?> GetRoomAvailabilityInfoAsync(Guid roomId, DateTime startDate, DateTime endDate)
        {
            await AddAuthenticationHeader();

            try
            {
                var response = await _httpClient.GetAsync($"api/users/room-availability/{roomId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RoomAvailabilityInfo>();
                }
                
                Console.WriteLine($"Erreur API lors de la récupération des infos de disponibilité: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des infos de disponibilité: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            await AddAuthenticationHeader();

            try
            {
                var response = await _httpClient.DeleteAsync($"api/users/current-user/bookings/{bookingId}");
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Réservation {bookingId} annulée avec succès");
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Réservation {bookingId} non trouvée ou n'appartient pas à l'utilisateur");
                    return false;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Impossible d'annuler la réservation {bookingId}: {errorContent}");
                    return false;
                }
                else
                {
                    Console.WriteLine($"Erreur lors de l'annulation de la réservation {bookingId}: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'annulation de la réservation {bookingId}: {ex.Message}");
                return false;
            }
        }

        private async Task AddAuthenticationHeader()
      {
     var token = await _localStorage.GetItemAsync<string>("authToken");

 if (!string.IsNullOrEmpty(token))
    {
     _httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        }

        /// <summary>
  /// Mapper un BookingWithDetailsDTO vers BookingForUser
        /// </summary>
    private static BookingForUser MapToBookingForUser(BookingWithDetailsDTO dto)
        {
    return new BookingForUser
    {
    Id = dto.Id,
    CreationDate = dto.CreationDate,
      StartingDate = dto.StartingDate,
      EndingDate = dto.EndingDate,
   Price = dto.Price,
      RoomId = dto.RoomId,
  RoomNumber = dto.RoomNumber,
     RoomType = dto.RoomType,
      RoomDescription = dto.RoomDescription,
  PricePerNight = dto.PricePerNight,
 HotelId = dto.HotelId,
    HotelName = dto.HotelName,
    HotelDescription = dto.HotelDescription,
   HotelStarRating = dto.HotelStarRating,
   OwnerName = dto.OwnerName
   };
      }
    }
}