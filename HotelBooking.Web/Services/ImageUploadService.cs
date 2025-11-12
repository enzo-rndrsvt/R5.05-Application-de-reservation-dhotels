using Microsoft.AspNetCore.Components.Forms;

namespace HotelBooking.Web.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ImageUploadService> _logger;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImageUploadService(IWebHostEnvironment webHostEnvironment, ILogger<ImageUploadService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<string?> UploadImageAsync(IBrowserFile imageFile, string folder = "rooms")
        {
            try
            {
                if (!IsValidImage(imageFile))
                {
                    _logger.LogWarning("Invalid image file: {FileName}", imageFile.Name);
                    return null;
                }

                // Créer le dossier s'il n'existe pas
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
                Directory.CreateDirectory(uploadsFolder);

                // Générer un nom de fichier unique
                var fileExtension = Path.GetExtension(imageFile.Name).ToLowerInvariant();
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Sauvegarder le fichier
                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await imageFile.OpenReadStream(_maxFileSize).CopyToAsync(fileStream);

                // Retourner l'URL relative
                var imageUrl = $"/uploads/{folder}/{uniqueFileName}";
                _logger.LogInformation("Image uploaded successfully: {ImageUrl}", imageUrl);
                
                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", imageFile.Name);
                return null;
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return false;

                // Convertir l'URL en chemin de fichier
                var relativePath = imageUrl.TrimStart('/');
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation("Image deleted successfully: {ImageUrl}", imageUrl);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
                return false;
            }
        }

        public bool IsValidImage(IBrowserFile imageFile)
        {
            if (imageFile == null)
                return false;

            // Vérifier la taille
            if (imageFile.Size > _maxFileSize)
                return false;

            // Vérifier l'extension
            var fileExtension = Path.GetExtension(imageFile.Name).ToLowerInvariant();
            if (!_allowedExtensions.Contains(fileExtension))
                return false;

            // Vérifier le type MIME
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedMimeTypes.Contains(imageFile.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }
    }
}