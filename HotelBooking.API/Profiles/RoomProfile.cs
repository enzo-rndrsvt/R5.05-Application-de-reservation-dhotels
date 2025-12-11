using AutoMapper;
using HotelBooking.Api.Models.Room;
using HotelBooking.Domain.Models.Room;

namespace HotelBooking.Api.Profiles
{
    /// <inheritdoc cref="Profile"/>
    public class RoomProfile : Profile
    {
        /// <inheritdoc cref="Profile"/>
        public RoomProfile()
        {
            CreateMap<RoomCreationDTO, RoomDTO>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => 
                    src.ImageUrls != null && src.ImageUrls.Any() ? src.ImageUrls.First() : src.ImageUrl))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls ?? new List<string>()));
            CreateMap<RoomUpdateDTO, RoomDTO>();
        }
    }
}
