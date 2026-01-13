using AutoFixture;
using FluentAssertions;
using FluentValidation;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Abstractions.Repositories;
using HotelBooking.Domain.Abstractions.Repositories.Hotel;
using HotelBooking.Domain.Abstractions.Repositories.Room;
using HotelBooking.Domain.Abstractions.Utilities;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Models.Hotel;
using HotelBooking.Domain.Models.Room;
using HotelBooking.Domain.Models.User;
using Moq;

namespace HotelBooking.Application.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
        private readonly Mock<IValidator<BookingDTO>> _bookingValidatorMock = new();
        private readonly Mock<IValidator<PaginationDTO>> _paginationValidatorMock = new();
        private readonly Mock<IHotelDiscountRepository> _hotelDiscountRepositoryMock = new();
        private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly Mock<IHotelRepository> _hotelRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly BookingService _bookingService;
        private readonly Fixture _fixture = new();

        public BookingServiceTests()
        {
            _bookingService = new(
                _bookingRepositoryMock.Object,
                _bookingValidatorMock.Object,
                _paginationValidatorMock.Object,
                _hotelDiscountRepositoryMock.Object,
                _roomRepositoryMock.Object,
                _emailServiceMock.Object,
                _userRepositoryMock.Object,
                _hotelRepositoryMock.Object);
        }

        [Theory]
        [InlineData(100, 1, 0, 100)]
        [InlineData(100, 1, 10, 90)]
        [InlineData(100, 2, 10, 180)]
        public async Task AddAsync_AddsValidBookingPrice(
            decimal roomPrice, int days, float discountPercentage, decimal finalPrice)
        {
            // Arrange
            var booking = GetBooking(days);
            var discount = _fixture.Build<DiscountDTO>()
                .With(x => x.AmountPercent, discountPercentage)
                .Create();
            _hotelDiscountRepositoryMock.Setup(x =>
                x.GetHighestActiveDiscount(It.IsAny<Guid>())).Returns(discount);
            SetupMocks(roomPrice);

            // Act
            await _bookingService.AddAsync(booking);

            // Assert
            booking.Price.Should().Be(finalPrice);
        }

        [Theory]
        [InlineData(100, 1, 100)]
        public async Task AddAsync_HandlesFinalPriceFor_NullDiscount(
            decimal roomPrice, int days, decimal finalPrice)
        {
            // Arrange
            var booking = GetBooking(days);
            _hotelDiscountRepositoryMock.Setup(x =>
                x.GetHighestActiveDiscount(It.IsAny<Guid>())).Returns((DiscountDTO)null);
            SetupMocks(roomPrice);

            // Act
            await _bookingService.AddAsync(booking);

            // Assert
            booking.Price.Should().Be(finalPrice);
        }

        private void SetupMocks(decimal roomPrice)
        {
            var room = _fixture.Build<RoomDTO>()
                .With(x => x.PricePerNight, roomPrice)
                .Create();
            _roomRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(room);
            _hotelRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_fixture.Create<HotelDTO>());
            _userRepositoryMock.Setup(x =>
                x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_fixture.Create<UserDTO>);
        }

        private BookingDTO GetBooking(int days)
        {
            var booking = _fixture.Build<BookingDTO>()
                .With(x => x.StartingDate, DateTime.Now)
                .With(x => x.EndingDate, DateTime.Now.AddDays(days - 1))
                .Create();

            return booking;
        }

        [Fact]
        public async Task CancelBookingForUserAsync_ShouldReturnTrue_WhenBookingExistsAndNotStarted()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var futureBooking = _fixture.Build<BookingWithDetailsDTO>()
                .With(x => x.Id, bookingId)
                .With(x => x.UserId, userId)
                .With(x => x.StartingDate, DateTime.UtcNow.AddDays(1)) // Réservation future
                .Create();

            _bookingRepositoryMock
                .Setup(x => x.GetBookingByIdForUserAsync(bookingId, userId))
                .ReturnsAsync(futureBooking);
            
            _bookingRepositoryMock
                .Setup(x => x.CancelBookingForUserAsync(bookingId, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _bookingService.CancelBookingForUserAsync(bookingId, userId);

            // Assert
            result.Should().BeTrue();
            _bookingRepositoryMock.Verify(x => x.CancelBookingForUserAsync(bookingId, userId), Times.Once);
        }

        [Fact]
        public async Task CancelBookingForUserAsync_ShouldReturnFalse_WhenBookingNotFound()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _bookingRepositoryMock
                .Setup(x => x.GetBookingByIdForUserAsync(bookingId, userId))
                .ReturnsAsync((BookingWithDetailsDTO?)null);

            // Act
            var result = await _bookingService.CancelBookingForUserAsync(bookingId, userId);

            // Assert
            result.Should().BeFalse();
            _bookingRepositoryMock.Verify(x => x.CancelBookingForUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task CancelBookingForUserAsync_ShouldThrowInvalidOperationException_WhenBookingAlreadyStarted()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var startedBooking = _fixture.Build<BookingWithDetailsDTO>()
                .With(x => x.Id, bookingId)
                .With(x => x.UserId, userId)
                .With(x => x.StartingDate, DateTime.UtcNow.AddDays(-1)) // Réservation déjà commencée
                .Create();

            _bookingRepositoryMock
                .Setup(x => x.GetBookingByIdForUserAsync(bookingId, userId))
                .ReturnsAsync(startedBooking);

            // Act & Assert
            await _bookingService.Invoking(x => x.CancelBookingForUserAsync(bookingId, userId))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot cancel a booking that has already started or is in the past.");

            _bookingRepositoryMock.Verify(x => x.CancelBookingForUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}
