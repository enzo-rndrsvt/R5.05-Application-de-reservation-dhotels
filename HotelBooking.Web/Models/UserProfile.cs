using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
      public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
 [EmailAddress(ErrorMessage = "Format d'email invalide")]
 public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
      [StringLength(50, ErrorMessage = "Le nom d'utilisateur ne peut pas dépasser 50 caractères")]
        public string Username { get; set; } = string.Empty;
        
        public string? NewPassword { get; set; }
        
   [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas")]
  public string? ConfirmPassword { get; set; }
        
public List<string> Roles { get; set; } = new();
    }
}