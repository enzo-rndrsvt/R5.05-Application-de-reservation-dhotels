using Microsoft.AspNetCore.Components.Forms;

namespace HotelBooking.Web.Services
{
    public interface IImageUploadService
    {
        /// <summary>
        /// Upload an image file and return the URL
        /// </summary>
        Task<string?> UploadImageAsync(IBrowserFile imageFile, string folder = "rooms");
        
        /// <summary>
        /// Delete an image file
        /// </summary>
        Task<bool> DeleteImageAsync(string imageUrl);
        
        /// <summary>
        /// Check if an image file is valid
        /// </summary>
        bool IsValidImage(IBrowserFile imageFile);
    }
}