using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; } = string.Empty;
    }
}