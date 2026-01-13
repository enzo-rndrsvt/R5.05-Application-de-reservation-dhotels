using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace HotelBooking.Web.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationState _anonymous;

        public CustomAuthStateProvider(
            ILocalStorageService localStorage,
            IHttpClientFactory httpClientFactory)
        {
            _localStorage = localStorage;
            _httpClient = httpClientFactory.CreateClient("HotelBookingAPI");
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                var username = await _localStorage.GetItemAsync<string>("currentUsername");

                if (string.IsNullOrEmpty(token))
                {
                    return _anonymous;
                }

                var expiration = await _localStorage.GetItemAsync<DateTime?>("tokenExpiration");
                
                if (expiration.HasValue && expiration.Value < DateTime.UtcNow)
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    await _localStorage.RemoveItemAsync("tokenExpiration");
                    await _localStorage.RemoveItemAsync("currentUsername");
                    return _anonymous;
                }
                    
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);

                var claims = ParseClaimsFromJwt(token, username ?? "Utilisateur");
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch
            {
                return _anonymous;
            }
        }

        public void NotifyUserAuthentication(string token, string username = "Utilisateur")
        {
            var claims = ParseClaimsFromJwt(token, username);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            
            var authState = new AuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public void NotifyUserLogout()
        {
            var authState = _anonymous;
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt, string displayName)
        {
            var claims = new List<Claim>();
            
            try
            {
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs == null)
                    return claims;

                foreach (var kvp in keyValuePairs)
                {
                    var key = kvp.Key;
                    var value = kvp.Value.ToString() ?? string.Empty;

                    switch (key)
                    {
                        case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name":
                            claims.Add(new Claim(ClaimTypes.NameIdentifier, value));
                            break;
                        case "http://schemas.microsoft.com/ws/2008/06/identity/claims/role":
                            claims.Add(new Claim(ClaimTypes.Role, value));
                            break;
                        default:
                            claims.Add(new Claim(key, value));
                            break;
                    }
                }

                // Ajouter le nom d'affichage
                claims.Add(new Claim(ClaimTypes.Name, displayName));
            }
            catch
            {
                claims.Add(new Claim(ClaimTypes.Name, displayName));
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