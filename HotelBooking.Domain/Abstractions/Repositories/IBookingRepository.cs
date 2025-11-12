using HotelBooking.Domain.Models;

namespace HotelBooking.Domain.Abstractions.Repositories
{
    /// <summary>
    /// Responsible for main operations for booking storage.
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Store new booking.
        /// </summary>
        Task AddAsync(BookingDTO newBooking);

        /// <summary>
        /// Determine whether a room is booked in the given interval or not.
        /// </summary>
        /// <param name="roomId">Id of the room to check for booking.</param>
        /// <param name="startingDate">Starting of the interval.</param>
        /// <param name="endingDate">Ending of the interval.</param>
        bool RoomIsBookedBetween(
            Guid roomId, DateTime startingDate, DateTime endingDate);

        /// <summary>
        /// Get all bookings for a specific user with room and hotel details.
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="itemsToSkip">Number of items to skip for pagination</param>
        /// <param name="itemsToTake">Number of items to take for pagination</param>
        /// <returns>Bookings with associated room and hotel information</returns>
        Task<IEnumerable<BookingWithDetailsDTO>> GetBookingsForUserAsync(Guid userId, int itemsToSkip, int itemsToTake);

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
    }
}
