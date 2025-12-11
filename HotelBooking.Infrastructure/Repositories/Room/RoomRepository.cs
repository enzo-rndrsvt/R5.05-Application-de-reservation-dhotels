using AutoMapper;
using HotelBooking.Domain.Abstractions.Repositories.Room;
using HotelBooking.Domain.Models.Room;
using HotelBooking.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Infrastructure.Repositories.Room
{
    /// <inheritdoc cref="IRoomRepository"/>
    internal class RoomRepository : IRoomRepository
    {
        private readonly HotelsBookingDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(
            HotelsBookingDbContext dbContext, IMapper mapper, ILogger<RoomRepository> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> AddAsync(RoomDTO newRoom)
        {
            try
            {
                // Convert ImageUrls list to comma-separated string for storage
                var roomTable = _mapper.Map<RoomTable>(newRoom);
                
                // Si on a plusieurs images, les stocker comme une liste séparée par virgules
                if (newRoom.ImageUrls?.Any() == true)
                {
                    roomTable.ImageUrl = string.Join(",", newRoom.ImageUrls);
                    Console.WriteLine($"Storing multiple images as: {roomTable.ImageUrl}");
                }
                else if (!string.IsNullOrEmpty(newRoom.ImageUrl))
                {
                    roomTable.ImageUrl = newRoom.ImageUrl;
                    Console.WriteLine($"Storing single image as: {roomTable.ImageUrl}");
                }

                var entityEntry = await _dbContext.Rooms.AddAsync(roomTable);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Created room with Id: {id}", entityEntry.Entity.Id);
                return entityEntry.Entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            _dbContext.Rooms.Remove(new RoomTable { Id = id });
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Deleted room with Id: {id}", id);
        }

        public Task<bool> ExistsAsync(Guid id) =>
            _dbContext.Rooms.AnyAsync(room => room.Id == id);

        public async Task<RoomDTO> GetByIdAsync(Guid id)
        {
            return _mapper.Map<RoomDTO>(
                await _dbContext.Rooms
                    .FindAsync(id));
        }

        public async Task<int> GetCountAsync()
        {
            if (_dbContext.Rooms.TryGetNonEnumeratedCount(out var count))
                return count;

            return await _dbContext.Rooms.CountAsync();
        }

        public async Task UpdateAsync(RoomDTO room)
        {
            try
            {
                var roomTable = _mapper.Map<RoomTable>(room);
                
                // Appliquer la même logique que pour AddAsync
                if (room.ImageUrls?.Any() == true)
                {
                    roomTable.ImageUrl = string.Join(",", room.ImageUrls);
                    Console.WriteLine($"Updating with multiple images as: {roomTable.ImageUrl}");
                }
                else if (!string.IsNullOrEmpty(room.ImageUrl))
                {
                    roomTable.ImageUrl = room.ImageUrl;
                    Console.WriteLine($"Updating with single image as: {roomTable.ImageUrl}");
                }
                
                _dbContext.Rooms.Update(roomTable);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Updated room with Id: {id}", room.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room with Id: {id}", room.Id);
                throw;
            }
        }
    }
}
