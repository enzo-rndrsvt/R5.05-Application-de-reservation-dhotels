using AutoMapper;
using FluentValidation;
using HotelBooking.Domain.Abstractions.Services.Room;
using HotelBooking.Domain.Constants;
using HotelBooking.Domain.Models.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers
{
    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.RegularUser}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// Get room details by ID.
        /// </summary>
        /// <param name="roomId">ID of the room to retrieve</param>
        /// <response code="200">Room retrieved successfully</response>
        /// <response code="404">Room not found</response>
        [HttpGet("{roomId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RoomDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoomByIdAsync(Guid roomId)
        {
            try
            {
                var room = await _roomService.GetByIdAsync(roomId);
                return Ok(room);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}