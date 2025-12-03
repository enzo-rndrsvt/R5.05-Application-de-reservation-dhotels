using AutoMapper;
using FluentValidation;
using HotelBooking.Api.Extensions;
using HotelBooking.Api.Models.Room;
using HotelBooking.Domain.Abstractions.Services.Room;
using HotelBooking.Domain.Constants;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Models.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers
{
    [Authorize(Roles = $"{UserRoles.Admin}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ApiController]
    [Route("api/admin/rooms")]
    public class RoomAdminController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly IRoomAdminService _roomAdminService;

        public RoomAdminController(
            IRoomService roomService, IMapper mapper, IRoomAdminService roomAdminService)
        {
            _roomService = roomService;
            _mapper = mapper;
            _roomAdminService = roomAdminService;
        }

        /// <summary>
        /// Get a paginated list of rooms for an admin.
        /// </summary>
        /// <response code="200">The list of rooms is retrieved successfully.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<RoomForAdminDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoomsForAdminAsync(
            [FromQuery] PaginationDTO pagination)
        {
            IEnumerable<RoomForAdminDTO> rooms;

            try
            {
                rooms = await _roomAdminService.GetByPageAsync(pagination);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }

            var roomsCount = await _roomService.GetCountAsync();
            Response.Headers.AddPaginationMetadata(roomsCount, pagination);

            return Ok(rooms);
        }

        /// <summary>
        /// Get Paginated list of rooms for an admin based on search query.
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="query">The search query</param>
        /// <response code="200">The list of rooms is retrieved successfully.</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<RoomForAdminDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchRoomsForAdminAsync(
            [FromQuery] PaginationDTO pagination, string query)
        {
            IEnumerable<RoomForAdminDTO> rooms;

            try
            {
                rooms = await _roomAdminService.SearchByPageAsync(pagination, query);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }

            var roomsCount = await _roomAdminService.GetSearchCountAsync(query);
            Response.Headers.AddPaginationMetadata(roomsCount, pagination);

            return Ok(rooms);
        }

        /// <summary>
        /// Create and store a new room.
        /// </summary>
        /// <param name="newRoom">Properties of the new room.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync(RoomCreationDTO newRoom)
        {
            Console.WriteLine("=== DEBUG API PostAsync (Room) ===");
            Console.WriteLine($"Reçu RoomCreationDTO: Number={newRoom.Number}, Type={newRoom.Type}");
            Console.WriteLine($"HotelId={newRoom.HotelId}, Price={newRoom.PricePerNight}");
            Console.WriteLine($"ImageUrl={newRoom.ImageUrl ?? "NULL"}");
            Console.WriteLine($"ImageUrls Count={newRoom.ImageUrls?.Count ?? 0}");
            
            if (newRoom.ImageUrls?.Any() == true)
            {
                for (int i = 0; i < newRoom.ImageUrls.Count; i++)
                {
                    Console.WriteLine($"  ImageUrls[{i}]: {newRoom.ImageUrls[i]}");
                }
            }
            
            try
            {
                var roomDTO = _mapper.Map<RoomDTO>(newRoom);
                Console.WriteLine($"Mapped to RoomDTO: {System.Text.Json.JsonSerializer.Serialize(roomDTO)}");
                
                await _roomService.AddAsync(roomDTO);
                Console.WriteLine("Room ajoutée avec succès !");
                
                return Created();
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"ValidationException: {ex.Message}");
                return BadRequest(ex.GetErrorsForClient());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception générale: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // Si c'est une erreur de base de données liée à ImageUrl, retourner un message spécifique
                if (ex.Message.Contains("Invalid column name 'ImageUrl'") || 
                    ex.Message.Contains("ImageUrl") ||
                    ex.InnerException?.Message.Contains("ImageUrl") == true)
                {
                    return BadRequest($"ERREUR BASE DE DONNÉES: La colonne ImageUrl n'existe pas. Veuillez appliquer la migration. Détails: {ex.Message}");
                }
                
                // Retourner l'erreur pour debugging
                return BadRequest($"Erreur interne: {ex.Message}");
            }
        }

        /// <summary>
        /// EMERGENCY: Fix database schema by adding ImageUrl column
        /// </summary>
        [HttpPost("emergency-fix-imageurl")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EmergencyFixImageUrlAsync()
        {
            try
            {
                Console.WriteLine("=== EMERGENCY FIX IMAGEURL ===");
                
                // Utiliser le RoomService pour tester si on peut créer une chambre test avec ImageUrl
                var testRoom = new RoomDTO 
                {
                    Id = Guid.NewGuid(),
                    Number = 99999, // Numéro test unique
                    Type = "EMERGENCY_TEST",
                    AdultsCapacity = 1,
                    ChildrenCapacity = 0,
                    BriefDescription = "Test urgence ImageUrl",
                    PricePerNight = 1.00m,
                    CreationDate = DateTime.Now,
                    ModificationDate = DateTime.Now,
                    HotelId = Guid.Parse("415d2c20-3590-4111-e70b-08de1d4a02ab"), // SkullKing
                    ImageUrl = null // Test avec NULL d'abord
                };

                // Essayer d'ajouter puis supprimer immédiatement
                var roomId = await _roomService.AddAsync(testRoom);
                await _roomService.DeleteAsync(roomId);
                
                Console.WriteLine("Test réussi - la colonne ImageUrl existe !");
                
                return Ok(new { 
                    success = true, 
                    message = "La base de données est correctement configurée. La colonne ImageUrl existe.",
                    action = "NO_FIX_NEEDED"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test échoué: {ex.Message}");
                
                if (ex.Message.Contains("Invalid column name 'ImageUrl'") || 
                    ex.InnerException?.Message.Contains("Invalid column name 'ImageUrl'") == true)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "CONFIRMÉ: La colonne ImageUrl n'existe pas dans la base de données.",
                        error = ex.Message,
                        solution = "Vous devez exécuter le script SQL manuellement ou appliquer les migrations EF.",
                        action = "MANUAL_FIX_REQUIRED"
                    });
                }
                
                return BadRequest(new { 
                    success = false, 
                    message = "Erreur lors du test",
                    error = ex.Message,
                    action = "UNKNOWN_ERROR"
                });
            }
        }

        /// <summary>
        /// Delete a room with a specific Id.
        /// </summary>
        /// <param name="roomId">The Id of the room to delete.</param>
        /// <response code="404">The room with the given Id doesn't exist.</response>
        /// <response code="204">The room is deleted successfully.</response>
        [HttpDelete("{roomId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync(Guid roomId)
        {
            try
            {
                await _roomService.DeleteAsync(roomId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Partially update a specific room.
        /// </summary>
        /// <param name="roomId">The Id of the room to update.</param>
        /// <param name="roomPatchDocument">Patch operations for (Number, AdultsCapacity, ChildrenCapacity).</param>
        /// <response code="404">The room with the given Id doesn't exist.</response>
        /// <response code="204">The room is updated successfully.</response>
        [HttpPatch("{roomId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PatchRoomAsync(
            Guid roomId, JsonPatchDocument<RoomUpdateDTO> roomPatchDocument)
        {
            RoomDTO roomToUpdate;

            try
            {
                roomToUpdate = await _roomService.GetByIdAsync(roomId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            var roomToPartiallyUpdate = GetRoomToPartiallyUpdate(
                roomPatchDocument, roomToUpdate);

            if (!ModelState.IsValid || !TryValidateModel(roomToPartiallyUpdate))
                return BadRequest(ModelState);

            _mapper.Map(roomToPartiallyUpdate, roomToUpdate);

            try
            {
                await _roomService.UpdateAsync(roomToUpdate);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.GetErrorsForClient());
            }

            return NoContent();
        }

        private RoomUpdateDTO GetRoomToPartiallyUpdate(
            JsonPatchDocument<RoomUpdateDTO> roomPatchDocument, RoomDTO roomToUpdate)
        {
            var roomToPartiallyUpdate = _mapper.Map<RoomUpdateDTO>(roomToUpdate);
            roomPatchDocument.ApplyTo(roomToPartiallyUpdate, ModelState);

            return roomToPartiallyUpdate;
        }

        /// <summary>
        /// TESTING: Add multiple test images to an existing room
        /// </summary>
        [HttpPost("{roomId}/add-test-images")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddTestImagesToRoomAsync(Guid roomId)
        {
            try
            {
                Console.WriteLine($"=== ADDING TEST IMAGES TO ROOM {roomId} ===");
                
                // Vérifier que la chambre existe
                var existingRoom = await _roomService.GetByIdAsync(roomId);
                
                // Ajouter des URLs d'images de test
                var testImageUrls = new List<string>
                {
                    "/images/pirate-cabin1.jpg",
                    "/images/pirate-cabin2.jpg", 
                    "/images/pirate-cabin3.jpg",
                    "/images/ocean-view.jpg"
                };

                // Mettre à jour avec les images multiples
                existingRoom.ImageUrls = testImageUrls;
                existingRoom.ImageUrl = testImageUrls.First(); // Image principale
                
                Console.WriteLine($"Adding {testImageUrls.Count} test images");
                foreach (var img in testImageUrls.Select((url, i) => new { url, i }))
                {
                    Console.WriteLine($"  Image {img.i + 1}: {img.url}");
                }
                
                await _roomService.UpdateAsync(existingRoom);
                
                Console.WriteLine("Test images added successfully!");
                
                return Ok(new { 
                    success = true, 
                    message = $"{testImageUrls.Count} images de test ajoutées à la chambre {existingRoom.Number}",
                    images = testImageUrls,
                    roomNumber = existingRoom.Number,
                    roomType = existingRoom.Type
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Chambre non trouvée");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur ajout images test: {ex.Message}");
                return BadRequest($"Erreur: {ex.Message}");
            }
        }
    }
}
