using HotelBooking.Web.Models;

namespace HotelBooking.Web.Services
{
    public interface IUserProfileService
{
      Task<UserProfile?> GetCurrentUserProfileAsync();
        Task<bool> UpdateUserProfileAsync(UserProfile userProfile);
 Task<bool> ChangePasswordAsync(string newPassword);
    }
}