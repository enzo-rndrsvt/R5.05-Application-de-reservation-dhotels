using System.Net.Http.Json;
using System.Text.Json;
using HotelBooking.Web.Models;

namespace HotelBooking.Web.Services
{
  public class UserProfileService : IUserProfileService
   {
 private readonly HttpClient _httpClient;
 private readonly ILocalStorageService _localStorage;
      private readonly IAuthService _authService;

   public UserProfileService(IHttpClientFactory httpClientFactory, 
      ILocalStorageService localStorage,
    IAuthService authService)
      {
      _httpClient = httpClientFactory.CreateClient("HotelBookingAPI");
   _localStorage = localStorage;
         _authService = authService;
    }

      public async Task<UserProfile?> GetCurrentUserProfileAsync()
    {
       try
   {
 await AddAuthenticationHeader();
       
    var response = await _httpClient.GetAsync("api/users/current-user/profile");
    
   if (response.IsSuccessStatusCode)
     {
         var apiProfile = await response.Content.ReadFromJsonAsync<ApiUserProfile>();
    if (apiProfile != null)
    {
    return new UserProfile
        {
        Id = apiProfile.Id,
   FirstName = apiProfile.FirstName,
        LastName = apiProfile.LastName,
     Email = apiProfile.Email,
         Username = apiProfile.Username,
Roles = apiProfile.Roles
       };
        }
   }
    else
  {
      Console.WriteLine($"Erreur API: {response.StatusCode}");
     }

     return null;
    }
 catch (Exception ex)
   {
      Console.WriteLine($"Erreur lors de la récupération du profil: {ex.Message}");
  return null;
    }
        }

      public async Task<bool> UpdateUserProfileAsync(UserProfile userProfile)
   {
    try
   {
        await AddAuthenticationHeader();
          
     var updateRequest = new
  {
    FirstName = userProfile.FirstName,
      LastName = userProfile.LastName,
   Email = userProfile.Email
     };

    var response = await _httpClient.PutAsJsonAsync("api/users/current-user/profile", updateRequest);
       return response.IsSuccessStatusCode;
}
      catch (Exception ex)
           {
  Console.WriteLine($"Erreur lors de la mise à jour du profil: {ex.Message}");
  return false;
  }
      }

      public async Task<bool> ChangePasswordAsync(string newPassword)
        {
  try
   {
  await AddAuthenticationHeader();
         
     var passwordChangeRequest = new { NewPassword = newPassword };
         
     var response = await _httpClient.PutAsJsonAsync("api/users/current-user/password", passwordChangeRequest);
      return response.IsSuccessStatusCode;
       }
          catch (Exception ex)
{
        Console.WriteLine($"Erreur lors du changement de mot de passe: {ex.Message}");
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

  // Classe pour désérialiser la réponse API
    private class ApiUserProfile
 {
 public Guid Id { get; set; }
  public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string Email { get; set; } = string.Empty;
      public string Username { get; set; } = string.Empty;
     public List<string> Roles { get; set; } = new();
      }
    }
}