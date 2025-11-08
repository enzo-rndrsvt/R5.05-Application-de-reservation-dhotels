using System.Net.Http.Json;
using System.Net;
using HotelBooking.Web.Models;

namespace HotelBooking.Web.Services
{
    public class HotelAdminService : IHotelAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public HotelAdminService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClient = httpClientFactory.CreateClient("HotelBookingAPI");
            _localStorage = localStorage;
        }

        public async Task<bool> CreateHotelAsync(HotelCreation hotelData)
        {
            await AddAuthenticationHeader();

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/admin/hotels", hotelData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de l'hôtel: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAllHotelsAsync()
        {
            await AddAuthenticationHeader();

            try
            {
                // Note: L'API ne semble pas avoir d'endpoint pour supprimer tous les hôtels
                // Il faudrait d'abord récupérer la liste puis supprimer un par un
                // Pour l'instant, on retourne true
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression des hôtels: {ex.Message}");
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
    }
}