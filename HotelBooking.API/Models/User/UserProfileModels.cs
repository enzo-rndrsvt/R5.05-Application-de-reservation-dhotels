using HotelBooking.Domain.Models.User;

namespace HotelBooking.Api.Models.User
{
    /// <summary>
    /// Model for updating user profile information.
    /// </summary>
    public class UserProfileUpdateDTO
    {
        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
      /// Email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for changing user password.
  /// </summary>
    public class PasswordChangeDTO
    {
        /// <summary>
        /// New password for the user.
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for user profile information.
    /// </summary>
    public class UserProfileDTO
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the user.
      /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
      /// Email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

   /// <summary>
  /// Username of the user.
    /// </summary>
        public string Username { get; set; } = string.Empty;

   /// <summary>
        /// User roles.
        /// </summary>
        public List<string> Roles { get; set; } = new();
    }
}