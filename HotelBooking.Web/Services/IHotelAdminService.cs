using HotelBooking.Web.Models;

namespace HotelBooking.Web.Services
{
    public interface IHotelAdminService
    {
        Task<bool> CreateHotelAsync(HotelCreation hotelData);
        Task<bool> DeleteAllHotelsAsync();
    }
}