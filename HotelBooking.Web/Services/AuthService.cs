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
                    Message = "Arrgh ! Ces identifiants ne correspondent à aucun membre de l'équipage !" 
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var friendlyMessage = ProcessLoginErrors(errorContent);
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = friendlyMessage
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
                return new AuthResponse { Success = true, Message = "Bienvenue à bord, matelot ! Votre inscription est réussie !" };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var friendlyMessage = ProcessRegistrationErrors(errorContent);
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = friendlyMessage
                };
            }
        }

        private string ProcessLoginErrors(string errorContent)
        {
            try
            {
                var errors = JsonSerializer.Deserialize<JsonElement[]>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (errors != null && errors.Length > 0)
                {
                    var firstError = errors[0];
                    
                    if (firstError.TryGetProperty("errorMessage", out var errorMessage))
                    {
                        var message = errorMessage.GetString() ?? "";
                        
                        // Transformer les messages d'erreur techniques en messages conviviaux
                        return message.ToLower() switch
                        {
                            var msg when msg.Contains("username") && msg.Contains("invalid format") => 
                                "Votre nom d'utilisateur contient des caractères interdits ! Utilisez uniquement des lettres, chiffres et tirets.",
                            var msg when msg.Contains("password") && msg.Contains("length") => 
                                "Votre mot de passe ne respecte pas les exigences de sécurité du navire !",
                            _ => "Une erreur inattendue s'est produite lors de la connexion."
                        };
                    }
                }
            }
            catch
            {
                // Si le parsing échoue, retourner un message générique
            }
            
            return "Une erreur s'est produite lors de la connexion. Vérifiez vos identifiants !";
        }

        private string ProcessRegistrationErrors(string errorContent)
        {
            try
            {
                var errors = JsonSerializer.Deserialize<JsonElement[]>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (errors != null && errors.Length > 0)
                {
                    var firstError = errors[0];
                    
                    if (firstError.TryGetProperty("errorMessage", out var errorMessage) && 
                        firstError.TryGetProperty("propertyName", out var propertyName))
                    {
                        var message = errorMessage.GetString() ?? "";
                        var property = propertyName.GetString() ?? "";
                        
                        // Transformer les messages d'erreur selon la propriété et le message
                        return (property.ToLower(), message.ToLower()) switch
                        {
                            ("username", var msg) when msg.Contains("already exists") => 
                                "Arrgh ! Ce nom d'utilisateur est déjà pris par un autre matelot ! Choisissez-en un autre.",
                            ("username", var msg) when msg.Contains("invalid format") => 
                                "Votre nom d'utilisateur contient des caractères interdits ! Utilisez uniquement des lettres, chiffres et tirets.",
                            ("email", var msg) when msg.Contains("emailaddress") => 
                                "Cette adresse email n'est pas valide ! Vérifiez le format (ex: capitaine@skullking.com).",
                            ("firstname", var msg) when msg.Contains("invalid format") => 
                                "Votre prénom contient des caractères non autorisés ! Utilisez uniquement des lettres.",
                            ("lastname", var msg) when msg.Contains("invalid format") => 
                                "Votre nom contient des caractères non autorisés ! Utilisez uniquement des lettres.",
                            ("password", var msg) when msg.Contains("length") => 
                                "Votre mot de passe ne respecte pas les exigences de sécurité du navire ! Il doit faire entre 6 et 100 caractères.",
                            _ => $"Erreur avec le champ {property} : {message}"
                        };
                    }
                }
            }
            catch
            {
                // Si le parsing échoue, retourner un message générique
            }
            
            return "Une erreur s'est produite lors de l'inscription. Vérifiez vos informations !";
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