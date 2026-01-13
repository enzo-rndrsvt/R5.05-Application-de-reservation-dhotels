using AutoMapper;
using FluentValidation;
using HotelBooking.Api.Extensions;
using HotelBooking.Api.Models.User;
using HotelBooking.Domain.Abstractions.Repositories;
using HotelBooking.Domain.Abstractions.Services;
using HotelBooking.Domain.Constants;
using HotelBooking.Domain.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.Identity;

namespace HotelBooking.Api.Controllers
{
    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.RegularUser}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
 [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ApiController]
    [Route("api/users")]
    public class UserProfileController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            IUserRepository userRepository,
     IPasswordHasher passwordHasher,
          IMapper mapper,
     ILogger<UserProfileController> logger)
     {
            _userRepository = userRepository;
 _passwordHasher = passwordHasher;
  _mapper = mapper;
      _logger = logger;
        }

        /// <summary>
        /// Get current user profile information.
        /// </summary>
        /// <response code="200">Returns the user profile information.</response>
   /// <response code="404">User not found.</response>
        [HttpGet("current-user/profile")]
        [ProducesResponseType(typeof(UserProfileDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<IActionResult> GetCurrentUserProfile()
   {
    var userId = new Guid(HttpContext.User.Identity.Name);

        try
  {
         var user = await _userRepository.GetByIdIncludingRolesAsync(userId);
    if (user == null)
 {
     return NotFound("User not found");
     }

           var userProfile = new UserProfileDTO
              {
       Id = user.Id,
       FirstName = user.FirstName,
     LastName = user.LastName,
         Email = user.Email,
         Username = user.Username,
         Roles = user.Roles.Select(r => r.Name).ToList()
      };

      return Ok(userProfile);
         }
catch (Exception ex)
     {
    _logger.LogError(ex, "Error retrieving user profile for user {UserId}", userId);
   return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update current user profile information.
  /// </summary>
     /// <param name="profileUpdate">Updated profile information.</param>
        /// <response code="200">Profile updated successfully.</response>
        /// <response code="400">Invalid data provided.</response>
     /// <response code="404">User not found.</response>
[HttpPut("current-user/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCurrentUserProfile(UserProfileUpdateDTO profileUpdate)
        {
    var userId = new Guid(HttpContext.User.Identity.Name);

       try
        {
            var user = await _userRepository.GetByIdAsync(userId);
         if (user == null)
       {
    return NotFound("User not found");
    }

       // Mettre à jour les informations
    user.FirstName = profileUpdate.FirstName;
                user.LastName = profileUpdate.LastName;
         user.Email = profileUpdate.Email;

  await _userRepository.UpdateAsync(user);

  _logger.LogInformation("User profile updated for user {UserId}", userId);
          return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating user profile for user {UserId}", userId);
      return StatusCode(500, "Internal server error");
         }
   }

        /// <summary>
        /// Change current user password.
        /// </summary>
     /// <param name="passwordChange">New password information.</param>
    /// <response code="200">Password changed successfully.</response>
        /// <response code="400">Invalid data provided.</response>
        /// <response code="404">User not found.</response>
        [HttpPut("current-user/password")]
 [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
 public async Task<IActionResult> ChangeCurrentUserPassword(PasswordChangeDTO passwordChange)
      {
          var userId = new Guid(HttpContext.User.Identity.Name);

            try
            {
           if (string.IsNullOrEmpty(passwordChange.NewPassword))
    {
           return BadRequest("New password is required");
       }

     var user = await _userRepository.GetByIdAsync(userId);
 if (user == null)
      {
           return NotFound("User not found");
    }

      // Hacher le nouveau mot de passe
                user.Password = _passwordHasher.HashPassword(passwordChange.NewPassword);
       await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Password changed for user {UserId}", userId);
    return Ok(new { message = "Password changed successfully" });
  }
       catch (Exception ex)
       {
      _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
      }
        }
    }
}