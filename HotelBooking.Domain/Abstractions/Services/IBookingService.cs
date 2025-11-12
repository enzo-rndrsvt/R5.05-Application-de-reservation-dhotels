using HotelBooking.Domain.Models;

namespace HotelBooking.Domain.Abstractions.Services
{
    /// <summary>
    /// Responsible for processing main operations for booking.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Add new booking to the system and send an email for the user who created it 
        /// containing the booking's details.
        /// </summary>
        Task AddAsync(BookingDTO bookingDTO);

        /// <summary>
        /// Get all bookings for a specific user.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns>User's bookings with room and hotel details</returns>
        Task<IEnumerable<BookingWithDetailsDTO>> GetBookingsForUserAsync(Guid userId, PaginationDTO pagination);

        /// <summary>
        /// Get the count of bookings for a specific user.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>Number of bookings for the user</returns>
        Task<int> GetBookingsCountForUserAsync(Guid userId);

        /// <summary>
        /// Get a specific booking by ID if it belongs to the user.
        /// </summary>
        /// <param name="bookingId">Id of the booking</param>
        /// <param name="userId">Id of the user</param>
        /// <returns>Booking details if found and belongs to user</returns>
        Task<BookingWithDetailsDTO?> GetBookingByIdForUserAsync(Guid bookingId, Guid userId);

        /// <summary>
        /// Cancel a booking for a user.
        /// </summary>
        /// <param name="bookingId">Id of the booking to cancel</param>
        /// <param name="userId">Id of the user</param>
        /// <returns>True if booking was cancelled successfully</returns>
        Task<bool> CancelBookingForUserAsync(Guid bookingId, Guid userId);

        /// <summary>
        /// Check if a room is available for booking in the given period.
        /// </summary>
        /// <param name="roomId">Id of the room to check</param>
        /// <param name="startingDate">Check-in date</param>
        /// <param name="endingDate">Check-out date</param>
        /// <returns>Room availability information with detailed feedback</returns>
        Task<RoomAvailabilityInfo> CheckRoomAvailabilityAsync(Guid roomId, DateTime startingDate, DateTime endingDate);
    }
}
