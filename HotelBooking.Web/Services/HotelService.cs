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