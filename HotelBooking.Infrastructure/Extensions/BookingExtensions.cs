using HotelBooking.Infrastructure.Tables;

namespace HotelBooking.Infrastructure.Extensions
{
    internal static class BookingExtensions
    {
        /// <summary>
        /// Determine whether a booking intersects with a given interval.
        /// </summary>
        /// <param name="booking">Booking to check for intersecting.</param>
        /// <param name="startingDate">Starting of the interval.</param>
        /// <param name="endingDate">Ending of the interval.</param>
        public static bool IntersectsWith(
            this BookingTable booking, DateTime startingDate, DateTime endingDate)
        {
            // Un chevauchement existe si :
            // 1. La nouvelle période commence avant la fin de la réservation existante ET
            // 2. La nouvelle période se termine après le début de la réservation existante
            // MAIS : Si la nouvelle période commence exactement quand l'ancienne se termine, il n'y a pas de conflit
            return startingDate < booking.EndingDate && endingDate > booking.StartingDate;
        }

        /// <summary>
        /// Determine whether a booking intersects with a given date.
        /// </summary>
        /// <param name="booking">Booking to check for intersecting.</param>
        /// <param name="date">Date to check for its intersecting with the booking.</param>
        public static bool IntersectsWith(this BookingTable booking, DateTime date) =>
            date >= booking.StartingDate && date < booking.EndingDate;

        /// <summary>
        /// Determine whether a date is within a given interval or not (inclusive start, exclusive end).
        /// </summary>
        /// <param name="dateToTest">Date to check.</param>
        /// <param name="startingDate">Starting of the interval.</param>
        /// <param name="endingDate">Ending of the interval.</param>
        /// <returns></returns>
        private static bool IsBetween(
            this DateTime dateToTest, DateTime startingDate, DateTime endingDate)
        {
            return dateToTest >= startingDate && dateToTest < endingDate;
        }
    }
}
