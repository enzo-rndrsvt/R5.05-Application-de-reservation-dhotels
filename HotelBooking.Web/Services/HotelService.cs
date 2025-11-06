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
                // Si on a une erreur 401 ou 403, c'est que l'utilisateur n'est pas authentifié ou n'a pas les droits
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
            // Cette méthode utilise un endpoint protégé maintenant
            await AddAuthenticationHeader();
            
            try
            {
                var response = await _httpClient.GetAsync("api/hotels/featured");
                
                if (response.IsSuccessStatusCode)
                {
                    var hotels = await response.Content.ReadFromJsonAsync<IEnumerable<HotelForUser>>();
                    return hotels ?? Enumerable.Empty<HotelForUser>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || 
                         response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("L'utilisateur n'est pas autorisé à accéder aux hôtels");
                    return Enumerable.Empty<HotelForUser>();
                }
                
                return Enumerable.Empty<HotelForUser>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des hôtels: {ex.Message}");
                return Enumerable.Empty<HotelForUser>();
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