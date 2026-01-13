using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Ahoy ! Vous devez saisir votre nom d'utilisateur ou email")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arrgh ! Votre mot de passe est nécessaire pour embarquer")]
        public string Password { get; set; } = string.Empty;
    }
}