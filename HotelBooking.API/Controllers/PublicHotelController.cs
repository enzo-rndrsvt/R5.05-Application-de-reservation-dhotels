using FluentValidation;
using HotelBooking.Api.Extensions;
using HotelBooking.Domain.Abstractions.Services.Hotel;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Models.Hotel;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers
{
    [ApiController]
    [Route("api/public/hotels")]
    public class PublicHotelController : Controller
    {
        private readonly IHotelAdminService _hotelAdminService;
        private readonly IHotelService _hotelService;

        public PublicHotelController(IHotelAdminService hotelAdminService, IHotelService hotelService)
        {
            _hotelAdminService = hotelAdminService;
            _hotelService = hotelService;
        }

        /// <summary>
        /// Get a paginated list of hotels (public endpoint - no authentication required).
        /// </summary>
        /// <response code="200">The list of hotels is retrieved successfully.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<HotelForAdminDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHotelsAsync([FromQuery] PaginationDTO pagination)
        {
            IEnumerable<HotelForAdminDTO> hotels;

            try
            {
                hotels = await _hotelAdminService.GetByPageAsync(pagination);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }

            var hotelsCount = await _hotelService.GetCountAsync();
            Response.Headers.AddPaginationMetadata(hotelsCount, pagination);

            return Ok(hotels);
        }
    }
}
