using AutoMapper;
using HotelBooking.Domain.Models.Room;
using HotelBooking.Infrastructure.Extensions;
using HotelBooking.Infrastructure.Tables;

namespace HotelBooking.Infrastructure.Profiles
{
    /// <inheritdoc cref="Profile"/>
    public class RoomProfile : Profile
    {
        /// <inheritdoc cref="Profile"/>
        public RoomProfile()
        {
            CreateMap<RoomTable, RoomDTO>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    ParseImageUrls(src.ImageUrl))) // Parse ImageUrl si c'est une liste séparée par virgules
                .ReverseMap()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => 
                    src.ImageUrls != null && src.ImageUrls.Any() ? src.ImageUrls.First() : src.ImageUrl));
            
            CreateMap<RoomTable, RoomForUserDTO>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    ParseImageUrls(src.ImageUrl)))
                .ReverseMap();
                
            CreateMap<RoomTable, RoomForAdminDTO>().ReverseMap();
        }

        /// <summary>
        /// Parse ImageUrl field to extract multiple image URLs
        /// Supports both single URL and comma-separated URLs
        /// </summary>
        private static List<string> ParseImageUrls(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return new List<string>();

            // Si l'URL contient des virgules, on assume que c'est une liste d'URLs
            if (imageUrl.Contains(','))
            {
                return imageUrl.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(url => url.Trim())
                             .Where(url => !string.IsNullOrEmpty(url))
                             .ToList();
            }

            // Sinon, c'est une URL unique
            return new List<string> { imageUrl };
        }
    }
}
