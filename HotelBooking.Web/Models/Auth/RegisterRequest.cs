using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Moussaillon ! Votre prénom est requis pour rejoindre l'équipage")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Votre prénom doit contenir entre 2 et 50 caractères")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capitaine ! Votre nom de famille est nécessaire")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Votre nom doit contenir entre 2 et 50 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ahoy ! Choisissez un nom d'utilisateur pour naviguer")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Votre nom d'utilisateur doit contenir entre 3 et 50 caractères")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Matelot ! Votre adresse email est indispensable")]
        [EmailAddress(ErrorMessage = "Cette adresse email n'est pas valide (ex: capitaine@skullking.com)")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arrgh ! Un mot de passe solide protégera votre trésor")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Votre mot de passe doit contenir au moins 6 caractères pour sécuriser vos richesses")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmez votre mot de passe pour éviter les mutineries")]
        [Compare(nameof(Password), ErrorMessage = "Les deux mots de passe ne correspondent pas, vérifiez votre saisie")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}