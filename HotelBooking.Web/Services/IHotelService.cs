using HotelBooking.Web.Models;

namespace HotelBooking.Web.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<Hotel>> GetHotelsAsync(int pageNumber, int pageSize);
        Task<Hotel> GetHotelByIdAsync(Guid id);
        // Autres méthodes selon vos besoins
    }
}