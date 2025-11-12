using AutoMapper;
using HotelBooking.Domain.Abstractions.Repositories;
using HotelBooking.Domain.Models;
using HotelBooking.Infrastructure.Extensions;
using HotelBooking.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Infrastructure.Repositories
{
    /// <inheritdoc cref="IBookingRepository"/>
    internal class BookingRepository : IBookingRepository
    {
        private readonly HotelsBookingDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(
            HotelsBookingDbContext dbContext, IMapper mapper, ILogger<BookingRepository> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddAsync(BookingDTO newBooking)
        {
            var booking = _mapper.Map<BookingTable>(newBooking);
            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Created booking with Id: {id}", booking.Id);
        }

        public bool RoomIsBookedBetween(Guid roomId, DateTime startingDate, DateTime endingDate)
        {
            return _dbContext.Bookings
                .AsEnumerable()
                .Any(booking =>
                    booking.RoomId == roomId &&
                    booking.IntersectsWith(startingDate, endingDate));
        }

        public async Task<IEnumerable<BookingWithDetailsDTO>> GetBookingsForUserAsync(Guid userId, int itemsToSkip, int itemsToTake)
        {
            var bookings = await _dbContext.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .OrderByDescending(b => b.CreationDate)
                .Skip(itemsToSkip)
                .Take(itemsToTake)
                .Select(b => new BookingWithDetailsDTO
                {
                    Id = b.Id,
                    CreationDate = b.CreationDate,
                    StartingDate = b.StartingDate,
                    EndingDate = b.EndingDate,
                    Price = b.Price,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    RoomNumber = b.Room.Number,
                    RoomType = b.Room.Type,
                    RoomDescription = b.Room.BriefDescription,
                    PricePerNight = b.Room.PricePerNight,
                    AdultsCapacity = b.Room.AdultsCapacity,
                    ChildrenCapacity = b.Room.ChildrenCapacity,
                    HotelId = b.Room.HotelId,
                    HotelName = b.Room.Hotel.Name,
                    HotelDescription = b.Room.Hotel.BriefDescription,
                    HotelStarRating = b.Room.Hotel.StarRating,
                    OwnerName = b.Room.Hotel.OwnerName
                })
                .ToListAsync();

            return bookings;
        }

        public async Task<int> GetBookingsCountForUserAsync(Guid userId)
        {
            return await _dbContext.Bookings
                .Where(b => b.UserId == userId)
                .CountAsync();
        }

        public async Task<BookingWithDetailsDTO?> GetBookingByIdForUserAsync(Guid bookingId, Guid userId)
        {
            var booking = await _dbContext.Bookings
                .Where(b => b.Id == bookingId && b.UserId == userId)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Select(b => new BookingWithDetailsDTO
                {
                    Id = b.Id,
                    CreationDate = b.CreationDate,
                    StartingDate = b.StartingDate,
                    EndingDate = b.EndingDate,
                    Price = b.Price,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    RoomNumber = b.Room.Number,
                    RoomType = b.Room.Type,
                    RoomDescription = b.Room.BriefDescription,
                    PricePerNight = b.Room.PricePerNight,
                    AdultsCapacity = b.Room.AdultsCapacity,
                    ChildrenCapacity = b.Room.ChildrenCapacity,
                    HotelId = b.Room.HotelId,
                    HotelName = b.Room.Hotel.Name,
                    HotelDescription = b.Room.Hotel.BriefDescription,
                    HotelStarRating = b.Room.Hotel.StarRating,
                    OwnerName = b.Room.Hotel.OwnerName
                })
                .FirstOrDefaultAsync();

            return booking;
        }

        public async Task<bool> CancelBookingForUserAsync(Guid bookingId, Guid userId)
        {
            var booking = await _dbContext.Bookings
                .Where(b => b.Id == bookingId && b.UserId == userId)
                .FirstOrDefaultAsync();

            if (booking == null)
            {
                _logger.LogWarning("Attempted to cancel booking {BookingId} for user {UserId}, but booking not found or doesn't belong to user", bookingId, userId);
                return false;
            }

            _dbContext.Bookings.Remove(booking);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation("Cancelled booking {BookingId} for user {UserId}", bookingId, userId);
            return true;
        }

        public async Task<bool> BookingBelongsToUserAsync(Guid bookingId, Guid userId)
        {
            return await _dbContext.Bookings
                .AnyAsync(b => b.Id == bookingId && b.UserId == userId);
        }

        public async Task<IEnumerable<BookingWithDetailsDTO>> GetAvailableRoomsAsync(Guid hotelId, DateTime startingDate, DateTime endingDate, int itemsToSkip, int itemsToTake)
        {
            var availableRooms = await _dbContext.Rooms
                .Where(r => r.HotelId == hotelId)
                .Include(r => r.Hotel)
                .Include(r => r.Bookings)
                .Where(r => !r.Bookings.Any(b => b.IntersectsWith(startingDate, endingDate)))
                .OrderBy(r => r.Number)
                .Skip(itemsToSkip)
                .Take(itemsToTake)
                .Select(r => new BookingWithDetailsDTO
                {
                    RoomId = r.Id,
                    RoomNumber = r.Number,
                    RoomType = r.Type,
                    RoomDescription = r.BriefDescription,
                    PricePerNight = r.PricePerNight,
                    AdultsCapacity = r.AdultsCapacity,
                    ChildrenCapacity = r.ChildrenCapacity,
                    HotelId = r.HotelId,
                    HotelName = r.Hotel.Name,
                    HotelDescription = r.Hotel.BriefDescription,
                    HotelStarRating = r.Hotel.StarRating,
                    OwnerName = r.Hotel.OwnerName
                })
                .ToListAsync();

            return availableRooms;
        }

        public async Task<RoomAvailabilityInfo> CheckRoomAvailabilityAsync(Guid roomId, DateTime startingDate, DateTime endingDate)
        {
            var conflictingBookings = await _dbContext.Bookings
                .Where(b => b.RoomId == roomId && b.IntersectsWith(startingDate, endingDate))
                .OrderBy(b => b.StartingDate)
                .ToListAsync();

            if (!conflictingBookings.Any())
            {
                return RoomAvailabilityInfo.Available(roomId);
            }

            var conflicts = conflictingBookings.Select(b => new BookingConflict
            {
                BookingId = b.Id,
                StartingDate = b.StartingDate,
                EndingDate = b.EndingDate
            }).ToList();

            // Trouver la prochaine date disponible
            var lastConflictEnd = conflictingBookings.Max(b => b.EndingDate);
            DateTime? nextAvailable = null;
            
            // Vérifier s'il y a une date libre après le dernier conflit
            var futureBookings = await _dbContext.Bookings
                .Where(b => b.RoomId == roomId && b.StartingDate >= lastConflictEnd)
                .OrderBy(b => b.StartingDate)
                .FirstOrDefaultAsync();

            if (futureBookings == null)
            {
                nextAvailable = lastConflictEnd;
            }

            string message;
            if (conflicts.Count == 1)
            {
                message = $"Chambre déjà réservée du {conflicts[0].Period}";
            }
            else
            {
                message = $"Chambre réservée pour {conflicts.Count} périodes sur cette période";
            }

            if (nextAvailable.HasValue)
            {
                message += $". Prochaine disponibilité : {nextAvailable.Value:dd/MM/yyyy}";
            }

            return RoomAvailabilityInfo.Unavailable(roomId, message, conflicts, nextAvailable);
        }
    }
}
