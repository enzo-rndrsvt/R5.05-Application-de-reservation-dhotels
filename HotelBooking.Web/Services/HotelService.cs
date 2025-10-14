using System.Net.Http.Json;
using HotelBooking.Web.Models;

namespace HotelBooking.Web.Services
{
    public class HotelService : IHotelService
    {
        private readonly HttpClient _httpClient;

        public HotelService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HotelBookingAPI");
        }

        public async Task<IEnumerable<Hotel>> GetHotelsAsync(int pageNumber, int pageSize)
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Hotel>>($"api/hotels?pageNumber={pageNumber}&pageSize={pageSize}");
            return response ?? Enumerable.Empty<Hotel>();
        }

        public async Task<Hotel> GetHotelByIdAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Hotel>($"api/hotels/{id}") ?? new Hotel();
        }

        public async Task<IEnumerable<HotelForUser>> GetAllHotelsAsync()
        {
            try
            {
                // Utiliser l'endpoint public qui ne nécessite pas d'authentification
                var hotels = await _httpClient.GetFromJsonAsync<IEnumerable<HotelForUser>>(
                    "api/public/hotels?pageNumber=1&pageSize=1000");
                
                return hotels ?? Enumerable.Empty<HotelForUser>();
            }
            catch (Exception)
            {
                // Log l'erreur si nécessaire
                return Enumerable.Empty<HotelForUser>();
            }
        }
    }
}