using AutoMapper;
using FluentValidation;
using HotelBooking.Api.Extensions;
using HotelBooking.Api.Models;
using HotelBooking.Domain.Abstractions.Services;
using HotelBooking.Domain.Constants;
using HotelBooking.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers
{
    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.RegularUser}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICartItemService _cartItemService;
        private readonly IBookingService _bookingService;
        private readonly IHotelReviewService _hotelReviewService;

        public UserController(
            IMapper mapper,
            ICartItemService cartItemService,
            IBookingService bookingService,
            IHotelReviewService hotelReviewService)
        {
            _mapper = mapper;
            _cartItemService = cartItemService;
            _bookingService = bookingService;
            _hotelReviewService = hotelReviewService;
        }

        /// <summary>
        /// Create and store a new cart item for a user.
        /// </summary>
        /// <param name="newCartItem">Properties of the new cart item.</param>
        [HttpPost("current-user/cart-items")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostCartItemAsync(CartItemCreationDTO newCartItem)
        {
            var userId = new Guid(HttpContext.User.Identity.Name);
            newCartItem.UserId = userId;

            try
            {
                await _cartItemService.AddAsync(_mapper.Map<CartItemDTO>(newCartItem));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }

            return Created();
        }

        /// <summary>
        /// Get a paginated list of cart items for a user.
        /// </summary>
        /// <response code="200">The list of cart items is retrieved successfully.</response>
        [HttpGet("current-user/cart-items")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<CartItemDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartItemsAsync([FromQuery] PaginationDTO pagination)
        {
            var userId = new Guid(HttpContext.User.Identity.Name);
            IEnumerable<CartItemDTO> cartItems;
            int citiesCount = 0;

            try
            {
                cartItems = await _cartItemService.GetAllForUserByPageAsync(userId, pagination);
                citiesCount = await _cartItemService.GetCountForUserAsync(userId);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Invalid user id.");
            }

            Response.Headers.AddPaginationMetadata(citiesCount, pagination);

            return Ok(cartItems);
        }

        /// <summary>
        /// Create, store a new booking for a user and send an email of its details to the user.
        /// </summary>
        /// <param name="newBooking">Properties of the new booking.</param>
        [HttpPost("current-user/bookings")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostBookingAsync(BookingCreationDTO newBooking)
        {
            newBooking.UserId = new Guid(HttpContext.User.Identity.Name);

            try
            {
                await _bookingService.AddAsync(_mapper.Map<BookingDTO>(newBooking));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }

            return Created();
        }

        /// <summary>
        /// Get paginated list of bookings for the current user.
        /// </summary>
        /// <response code="200">The list of user bookings is retrieved successfully.</response>
        [HttpGet("current-user/bookings")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<BookingWithDetailsDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserBookingsAsync([FromQuery] PaginationDTO pagination)
        {
            var userId = new Guid(HttpContext.User.Identity.Name);

            try
            {
                var bookings = await _bookingService.GetBookingsForUserAsync(userId, pagination);
                var bookingsCount = await _bookingService.GetBookingsCountForUserAsync(userId);

                Response.Headers.AddPaginationMetadata(bookingsCount, pagination);

                return Ok(bookings);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }
        }

        /// <summary>
        /// Get a specific booking by ID for the current user.
        /// </summary>
        /// <param name="bookingId">ID of the booking to retrieve</param>
        /// <response code="200">Booking retrieved successfully</response>
        /// <response code="404">Booking not found or doesn't belong to user</response>
        [HttpGet("current-user/bookings/{bookingId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BookingWithDetailsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserBookingByIdAsync(Guid bookingId)
        {
            var userId = new Guid(HttpContext.User.Identity.Name);

            try
            {
                var booking = await _bookingService.GetBookingByIdForUserAsync(bookingId, userId);

                if (booking == null)
                {
                    return NotFound("Booking not found or you don't have permission to access it.");
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving booking: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancel a specific booking for the current user.
        /// </summary>
        /// <param name="bookingId">ID of the booking to cancel</param>
        /// <response code="200">Booking cancelled successfully</response>
        /// <response code="404">Booking not found or doesn't belong to user</response>
        /// <response code="400">Booking cannot be cancelled (already started or in the past)</response>
        [HttpDelete("current-user/bookings/{bookingId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelUserBookingAsync(Guid bookingId)
        {
            var userId = new Guid(HttpContext.User.Identity.Name);

            try
            {
                var result = await _bookingService.CancelBookingForUserAsync(bookingId, userId);

                if (!result)
                {
                    return NotFound("Booking not found or you don't have permission to cancel it.");
                }

                return Ok(new { message = "Booking cancelled successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error cancelling booking: {ex.Message}");
            }
        }

        /// <summary>
        /// Create and store a new hotel review for a user.
        /// </summary>
        /// <param name="newReview">Properties of the new review.</param>
        /// <response code="201">The review is created successfully.</response>
        [HttpPost("current-user/hotel-reviews")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostHotelReviewAsync(HotelReviewCreationDTO newReview)
        {
            newReview.UserId = new Guid(HttpContext.User.Identity.Name);

            try
            {
                await _hotelReviewService.AddAsync(_mapper.Map<HotelReviewDTO>(newReview));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }

            return Created();
        }

        /// <summary>
        /// Check room availability for a specific period.
        /// </summary>
        /// <param name="roomId">ID of the room to check</param>
        /// <param name="startDate">Check-in date (YYYY-MM-DD)</param>
        /// <param name="endDate">Check-out date (YYYY-MM-DD)</param>
        /// <response code="200">Room availability information retrieved successfully</response>
        /// <response code="400">Invalid parameters</response>
        [HttpGet("room-availability/{roomId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckRoomAvailabilityAsync(Guid roomId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                // Log des paramètres reçus
                Console.WriteLine($"=== DEBUG CheckRoomAvailability ===");
                Console.WriteLine($"Room ID: {roomId}");
                Console.WriteLine($"Start Date: {startDate}");
                Console.WriteLine($"End Date: {endDate}");

                if (!DateTime.TryParse(startDate, out var startDateTime) || !DateTime.TryParse(endDate, out var endDateTime))
                {
                    Console.WriteLine($"ERREUR: Format de date invalide");
                    return BadRequest("Invalid date format. Please use YYYY-MM-DD format.");
                }

                if (startDateTime >= endDateTime)
                {
                    Console.WriteLine($"ERREUR: Date de début après date de fin");
                    return BadRequest("Check-in date must be before check-out date.");
                }

                if (startDateTime < DateTime.Today)
                {
                    Console.WriteLine($"ERREUR: Date de début dans le passé");
                    return BadRequest("Check-in date cannot be in the past.");
                }

                Console.WriteLine($"Appel de CheckRoomAvailabilityAsync...");
                var availability = await _bookingService.CheckRoomAvailabilityAsync(roomId, startDateTime, endDateTime);
                Console.WriteLine($"Résultat: Available={availability?.IsAvailable}, Message={availability?.Message}");
                
                return Ok(availability);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION dans CheckRoomAvailability: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest($"Error checking availability: {ex.Message}");
            }
        }
    }
}
