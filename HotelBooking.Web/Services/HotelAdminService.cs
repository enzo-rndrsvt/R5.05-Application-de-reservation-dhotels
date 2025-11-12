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

        public async Task<bool> CreateRoomAsync(RoomCreation roomData)
        {
            await AddAuthenticationHeader();

            try
            {
                // Créer l'objet à envoyer (sans l'image pour l'instant)
                var roomToSend = new
                {
                    roomData.Number,
                    roomData.Type,
                    roomData.AdultsCapacity,
                    roomData.ChildrenCapacity,
                    roomData.BriefDescription,
                    roomData.PricePerNight,
                    roomData.HotelId,
                    roomData.ImageUrl // L'URL de l'image déjà uploadée
                };

                var response = await _httpClient.PostAsJsonAsync("api/admin/rooms", roomToSend);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de la chambre: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<HotelForUser>> GetHotelsForRoomCreationAsync()
        {
            await AddAuthenticationHeader();

            // Utiliser l'endpoint public pour récupérer les hôtels
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
                return Enumerable.Empty<HotelForUser>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des hôtels: {ex.Message}");
                return Enumerable.Empty<HotelForUser>();
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