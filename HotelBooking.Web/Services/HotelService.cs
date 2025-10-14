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
    }
}