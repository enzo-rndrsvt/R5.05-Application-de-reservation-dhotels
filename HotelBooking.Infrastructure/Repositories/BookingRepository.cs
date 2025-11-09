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
    }
}
