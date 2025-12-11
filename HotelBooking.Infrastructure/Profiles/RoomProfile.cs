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
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); // On gère ça manuellement dans le repository
            
            CreateMap<RoomTable, RoomForUserDTO>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    ParseImageUrls(src.ImageUrl)))
                .ReverseMap()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); // Aussi géré manuellement
                
            CreateMap<RoomTable, RoomForAdminDTO>()
                .ReverseMap()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); // Aussi géré manuellement
        }

        /// <summary>
        /// Parse ImageUrl field to extract multiple image URLs
        /// Supports both single URL and comma-separated URLs
        /// </summary>
        private static List<string> ParseImageUrls(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return new List<string>();

            // Si l'URL contient des virgules, on assume que c'est une liste d'URLs
            if (imageUrl.Contains(','))
            {
                return imageUrl.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(url => url.Trim())
                             .Where(url => !string.IsNullOrWhiteSpace(url) && url.Length > 3) // Filtrer les URLs trop courtes
                             .Distinct() // Éviter les doublons
                             .ToList();
            }

            // Sinon, c'est une URL unique
            if (imageUrl.Trim().Length > 3)
            {
                return new List<string> { imageUrl.Trim() };
            }

            return new List<string>();
        }
    }
}
