using HotelBooking.Web.Models.Auth;

namespace HotelBooking.Web.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginRequest loginRequest);
        Task<AuthResponse> Register(RegisterRequest registerRequest);
        Task Logout();
        Task<UserInfo?> GetUserInfo();
        bool IsAuthenticated();
    }
}