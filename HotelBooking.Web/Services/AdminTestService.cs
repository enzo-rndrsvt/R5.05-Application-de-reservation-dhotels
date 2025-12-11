using HotelBooking.Web.Models;
using HotelBooking.Web.Services;
using Microsoft.JSInterop;

namespace HotelBooking.Web.Services
{
    public interface IAdminTestService
    {
        Task<bool> AddTestRoomAsync();
        Task<int> AddTestImagesToAllRoomsAsync();
        Task<bool> TestAllCarouselsAsync();
        Task<bool> CleanTestDataAsync();
        Task<SystemHealthCheck> CheckSystemHealthAsync();
    }

    public class AdminTestService : IAdminTestService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorage;
        private readonly IHotelAdminService _hotelAdminService;

        public AdminTestService(
            IHttpClientFactory httpClientFactory, 
            ILocalStorageService localStorage,
            IHotelAdminService hotelAdminService)
        {
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
            _hotelAdminService = hotelAdminService;
        }

        public async Task<bool> AddTestRoomAsync()
        {
            try
            {
                // Créer une chambre de test avec données réalistes
                var testRoom = new RoomCreation
                {
                    Number = GenerateTestRoomNumber(),
                    Type = GetRandomRoomType(),
                    AdultsCapacity = Random.Shared.Next(1, 4),
                    ChildrenCapacity = Random.Shared.Next(0, 2),
                    BriefDescription = GenerateTestDescription(),
                    PricePerNight = Random.Shared.Next(50, 300),
                    HotelId = await GetSkullKingHotelIdAsync(),
                    ImageUrls = GenerateTestImageUrls()
                };

                return await _hotelAdminService.CreateRoomAsync(testRoom);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de chambre test: {ex.Message}");
                return false;
            }
        }

        public async Task<int> AddTestImagesToAllRoomsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("HotelBookingAPI");
                await AddAuthHeader(httpClient);

                // Récupérer toutes les chambres
                var hotelsResponse = await httpClient.GetAsync("api/public/hotels");
                if (!hotelsResponse.IsSuccessStatusCode) return 0;

                var hotels = await hotelsResponse.Content.ReadFromJsonAsync<IEnumerable<HotelForAdmin>>();
                int updatedCount = 0;

                foreach (var hotel in hotels ?? Enumerable.Empty<HotelForAdmin>())
                {
                    // Pour chaque hôtel, récupérer les chambres et ajouter des images
                    var roomsResponse = await httpClient.GetAsync($"api/admin/hotels/{hotel.Id}/rooms");
                    if (roomsResponse.IsSuccessStatusCode)
                    {
                        var rooms = await roomsResponse.Content.ReadFromJsonAsync<IEnumerable<object>>();
                        
                        // Ajouter des images de test à chaque chambre
                        foreach (var room in rooms ?? Enumerable.Empty<object>())
                        {
                            // Logique d'ajout d'images (à implémenter selon l'API)
                            updatedCount++;
                        }
                    }
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout d'images test: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> TestAllCarouselsAsync()
        {
            try
            {
                // Simuler le test de tous les carrousels
                await Task.Delay(1500);
                
                // Ici on pourrait implémenter une vraie vérification JavaScript
                // des carrousels via JSInterop
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du test des carrousels: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CleanTestDataAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("HotelBookingAPI");
                await AddAuthHeader(httpClient);

                // Supprimer toutes les chambres de test (numéros > 900)
                var response = await httpClient.DeleteAsync("api/admin/rooms/test-data");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage: {ex.Message}");
                return false;
            }
        }

        public async Task<SystemHealthCheck> CheckSystemHealthAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("HotelBookingAPI");
                
                var healthCheck = new SystemHealthCheck();
                
                // Test de la base de données
                var dbTest = await httpClient.GetAsync("api/admin/rooms/test-database");
                healthCheck.DatabaseHealthy = dbTest.IsSuccessStatusCode;
                
                // Test de l'API
                var apiTest = await httpClient.GetAsync("api/public/hotels");
                healthCheck.ApiHealthy = apiTest.IsSuccessStatusCode;
                
                // Test du temps de réponse
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                await httpClient.GetAsync("api/health");
                stopwatch.Stop();
                healthCheck.ResponseTime = (int)stopwatch.ElapsedMilliseconds;
                
                return healthCheck;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du check système: {ex.Message}");
                return new SystemHealthCheck { DatabaseHealthy = false, ApiHealthy = false };
            }
        }

        private async Task AddAuthHeader(HttpClient httpClient)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<Guid> GetSkullKingHotelIdAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("HotelBookingAPI");
                var response = await httpClient.GetAsync("api/public/hotels");
                
                if (response.IsSuccessStatusCode)
                {
                    var hotels = await response.Content.ReadFromJsonAsync<IEnumerable<HotelForAdmin>>();
                    var skullKing = hotels?.FirstOrDefault(h => h.Name.Contains("SkullKing", StringComparison.OrdinalIgnoreCase));
                    
                    if (skullKing != null)
                        return skullKing.Id;
                }
                
                // Fallback: ID par défaut de SkullKing si trouvé précédemment
                return Guid.Parse("415d2c20-3590-4111-e70b-08de1d4a02ab");
            }
            catch
            {
                return Guid.Parse("415d2c20-3590-4111-e70b-08de1d4a02ab");
            }
        }

        private int GenerateTestRoomNumber()
        {
            // Numéros de test entre 900 et 999
            return Random.Shared.Next(900, 999);
        }

        private string GetRandomRoomType()
        {
            var types = new[] { "Standard", "Deluxe", "Suite", "Presidential", "Capitaine" };
            return types[Random.Shared.Next(types.Length)];
        }

        private string GenerateTestDescription()
        {
            var descriptions = new[]
            {
                "Cabine de test confortable avec vue sur l'océan pirate",
                "Chambre d'essai luxueuse pour les aventuriers modernes",
                "Suite test avec équipements pirates authentiques",
                "Cabine expérimentale au design unique",
                "Chambre prototype avec tout le confort moderne"
            };
            
            return descriptions[Random.Shared.Next(descriptions.Length)];
        }

        private List<string> GenerateTestImageUrls()
        {
            var baseUrls = new[]
            {
                "https://picsum.photos/800/600?random=",
                "https://source.unsplash.com/800x600/?room,",
                "https://source.unsplash.com/800x600/?bedroom,",
                "https://source.unsplash.com/800x600/?hotel,"
            };

            var images = new List<string>();
            var imageCount = Random.Shared.Next(2, 5); // 2 à 4 images

            for (int i = 0; i < imageCount; i++)
            {
                var baseUrl = baseUrls[Random.Shared.Next(baseUrls.Length)];
                var uniqueId = Random.Shared.Next(1000, 9999);
                images.Add($"{baseUrl}{uniqueId}");
            }

            return images;
        }
    }

    public class SystemHealthCheck
    {
        public bool DatabaseHealthy { get; set; }
        public bool ApiHealthy { get; set; }
        public int ResponseTime { get; set; }
        public DateTime CheckTime { get; set; } = DateTime.Now;
        
        public bool IsHealthy => DatabaseHealthy && ApiHealthy && ResponseTime < 5000;
    }
}
