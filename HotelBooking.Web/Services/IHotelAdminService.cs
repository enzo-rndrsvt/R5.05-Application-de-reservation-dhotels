using HotelBooking.Web.Models;

namespace HotelBooking.Web.Services
{
    public interface IHotelAdminService
    {
        Task<bool> CreateHotelAsync(HotelCreation hotelData);
        Task<bool> DeleteAllHotelsAsync();
        Task<bool> CreateRoomAsync(RoomCreation roomData);
        Task<IEnumerable<HotelForUser>> GetHotelsForRoomCreationAsync();
    }
}