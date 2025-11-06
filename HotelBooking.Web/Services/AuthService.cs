using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HotelBooking.Web.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace HotelBooking.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(IHttpClientFactory httpClientFactory, 
                          AuthenticationStateProvider authStateProvider,
                          ILocalStorageService localStorage)
        {
            _httpClient = httpClientFactory.CreateClient("HotelBookingAPI");
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task<AuthResponse> Login(LoginRequest loginRequest)
        {
            var apiLoginModel = new
            {
                Username = loginRequest.Username,
                Password = loginRequest.Password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/user-login", apiLoginModel);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Nom d'utilisateur ou mot de passe incorrect" 
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = $"Erreur de connexion au serveur: {errorContent}" 
                };
            }

            var token = await response.Content.ReadAsStringAsync();
            
            if (!string.IsNullOrEmpty(token))
            {
                token = token.Trim('"');
                
                // Stocker les informations d'authentification
                await _localStorage.SetItemAsync("authToken", token);
                await _localStorage.SetItemAsync("tokenExpiration", DateTime.UtcNow.AddHours(24));
                await _localStorage.SetItemAsync("currentUsername", loginRequest.Username);
                
                // Configurer l'en-tête d'autorisation
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);

                // Notifier le changement d'état d'authentification
                if (_authStateProvider is CustomAuthStateProvider customProvider)
                {
                    customProvider.NotifyUserAuthentication(token, loginRequest.Username);
                }

                return new AuthResponse { Success = true, Token = token };
            }

            return new AuthResponse { Success = false, Message = "Token invalide reçu" };
        }

        public async Task<AuthResponse> Register(RegisterRequest registerRequest)
        {
            var apiRegisterModel = new
            {
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                Username = registerRequest.Username,
                Password = registerRequest.Password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/user-register", apiRegisterModel);
            
            if (response.IsSuccessStatusCode)
            {
                return new AuthResponse { Success = true, Message = "Inscription réussie" };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = !string.IsNullOrEmpty(errorContent) ? errorContent : "Erreur lors de l'inscription" 
                };
            }
        }

        public async Task Logout()
        {
            // Nettoyer le stockage local
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("tokenExpiration");
            await _localStorage.RemoveItemAsync("currentUsername");
            
            // Supprimer l'en-tête d'autorisation
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
            // Notifier la déconnexion
            if (_authStateProvider is CustomAuthStateProvider customProvider)
            {
                customProvider.NotifyUserLogout();
            }
        }

        public async Task<UserInfo?> GetUserInfo()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            var username = await _localStorage.GetItemAsync<string>("currentUsername");
            
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var claims = ParseClaimsFromJwt(token);
                var userInfo = new UserInfo
                {
                    Username = username ?? "Utilisateur",
                    Email = claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "Unknown",
                    Roles = claims.Where(c => c.Type.Contains("role")).Select(c => c.Value).ToList()
                };

                return userInfo;
            }
            catch
            {
                await Logout();
                return null;
            }
        }

        public bool IsAuthenticated()
        {
            try 
            {
                var token = _localStorage.GetItemAsync<string>("authToken").Result;
                return !string.IsNullOrEmpty(token);
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<System.Security.Claims.Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<System.Security.Claims.Claim>();
            try
            {
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs == null)
                    return claims;

                foreach (var kvp in keyValuePairs)
                {
                    claims.Add(new System.Security.Claims.Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
                }
            }
            catch
            {
                // En cas d'erreur, retourner une liste vide
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}