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
            
            Console.WriteLine("=== DEBUG HotelAdminService.CreateRoomAsync ===");

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

                Console.WriteLine($"Données à envoyer: {System.Text.Json.JsonSerializer.Serialize(roomToSend)}");
                Console.WriteLine($"URL de l'API: {_httpClient.BaseAddress}api/admin/rooms");
                
                // Vérifier les headers d'auth
                var authHeader = _httpClient.DefaultRequestHeaders.Authorization;
                Console.WriteLine($"Header d'auth: {authHeader?.Scheme} {(authHeader?.Parameter != null ? authHeader.Parameter.Substring(0, Math.Min(10, authHeader.Parameter.Length)) : "NULL")}...");

                var response = await _httpClient.PostAsJsonAsync("api/admin/rooms", roomToSend);
                
                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Is Success: {response.IsSuccessStatusCode}");
                Console.WriteLine($"Reason Phrase: {response.ReasonPhrase}");
                
                // TOUJOURS lire le contenu de la réponse pour avoir les détails
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Contenu de la réponse: {errorContent}");
                
                // Si c'est un 400 Bad Request, analyser les erreurs de validation
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    Console.WriteLine("=== ERREUR VALIDATION DÉTECTÉE ===");
                    Console.WriteLine($"Détails de validation: {errorContent}");
                }
                
                // Si c'est un 401/403, problème d'autorisation
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("=== ERREUR AUTORISATION - 401 UNAUTHORIZED ===");
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("=== ERREUR AUTORISATION - 403 FORBIDDEN ===");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception dans CreateRoomAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
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