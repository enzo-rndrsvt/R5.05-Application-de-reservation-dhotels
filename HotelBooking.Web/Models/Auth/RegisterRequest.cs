using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le prénom doit avoir entre 3 et 50 caractères")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom de famille est requis")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom de famille doit avoir entre 3 et 50 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit avoir entre 3 et 50 caractères")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit avoir au moins 8 caractères")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
        [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}