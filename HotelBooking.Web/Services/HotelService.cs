using System.Net.Http.Json;
using HotelBooking.Web.Models;
using System.Net;

namespace HotelBooking.Web.Services
{
    public class HotelService : IHotelService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public HotelService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClient = httpClientFactory.CreateClient("HotelBookingAPI");
            _localStorage = localStorage;
        }

        public async Task<IEnumerable<Hotel>> GetHotelsAsync(int pageNumber, int pageSize)
        {
            await AddAuthenticationHeader();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<Hotel>>($"api/hotels?pageNumber={pageNumber}&pageSize={pageSize}");
                return response ?? Enumerable.Empty<Hotel>();
            }
            catch
            {
                return Enumerable.Empty<Hotel>();
            }
        }

        public async Task<Hotel> GetHotelByIdAsync(Guid id)
        {
            await AddAuthenticationHeader();
            
            try
            {
                return await _httpClient.GetFromJsonAsync<Hotel>($"api/hotels/{id}") ?? new Hotel();
            }
            catch
            {
                return new Hotel();
            }
        }

        public async Task<IEnumerable<HotelForUser>> GetAllHotelsAsync()
        {
            // Utiliser l'endpoint public qui retourne HotelForAdminDTO
            try
            {
                var response = await _httpClient.GetAsync("api/public/hotels");
                
                if (response.IsSuccessStatusCode)
                {
                    var hotelsAdmin = await response.Content.ReadFromJsonAsync<IEnumerable<HotelForAdmin>>();
                    
                    // Convertir HotelForAdmin en HotelForUser
                    if (hotelsAdmin != null)
                    {
                        var hotelForUser = hotelsAdmin.Select(h => new HotelForUser
                        {
                            Id = h.Id,
                            Name = h.Name,
                            BriefDescription = "L'hôtel des pirates.", // Valeur par défaut
                            OwnerName = h.OwnerName,
                            StarRating = h.StarRating,
                            CreationDate = h.CreationDate,
                            ModificationDate = h.ModificationDate,
                            NumberOfRooms = h.NumberOfRooms
                        });
                        
                        return hotelForUser;
                    }
                }
                
                Console.WriteLine($"Erreur API: {response.StatusCode} - {response.ReasonPhrase}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Contenu erreur: {errorContent}");
                return Enumerable.Empty<HotelForUser>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des hôtels: {ex.Message}");
                return Enumerable.Empty<HotelForUser>();
            }
        }

        public async Task<IEnumerable<Room>> GetHotelRoomsAsync(Guid hotelId, int pageNumber = 1, int pageSize = 20)
        {
            await AddAuthenticationHeader();
     
            try
            {
                var response = await _httpClient.GetAsync($"api/hotels/{hotelId}/rooms/available?pageNumber={pageNumber}&pageSize={pageSize}");
                
                if (response.IsSuccessStatusCode)
                {
                    var rooms = await response.Content.ReadFromJsonAsync<IEnumerable<Room>>();
                    return rooms ?? Enumerable.Empty<Room>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || 
                         response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("L'utilisateur n'est pas autorisé à accéder aux chambres");
                    return Enumerable.Empty<Room>();
                }
      
                return Enumerable.Empty<Room>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des chambres: {ex.Message}");
                return Enumerable.Empty<Room>();
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
    }
}