
using Flight2.Domain.Entities;
using Flight2.ReadModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Flight2.DTO;
using Flight2.Domain.Error;
using Flight2.MongoDB;
using System.Collections.ObjectModel;

namespace Flight2.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        
        private List<Booking>? bookings;
        private List<Flight>? flights;
        public BookingController(MongoDBService mongoDBService)
        {
                this._mongoDBService = mongoDBService;
                
        }

        [HttpGet("{email}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IEnumerable<BookingRm>) , 200)]
        public async Task<ActionResult<IEnumerable<BookingRm>>> List(string email) {

            
            flights = await _mongoDBService.GetFlights();
            Console.WriteLine();
            var booking = flights.ToArray()
                .SelectMany(f => f.Bookings
                .Where(b => b.PassengerEmail == email)
                .Select(b => new BookingRm(
                    f.Id,
                    f.airline,
                    f.price.ToString(),
                    new TimePlaceRm(f.Arrival.Place, f.Arrival.Time),
                    new TimePlaceRm(f.departure.Place, f.departure.Time),
                    (int)b.numberOfSeats,
                    email
                    )));
           

            return Ok(booking);
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(204)]

        public async Task<IActionResult> Cancel(BookDto book)
        {
            bookings = await _mongoDBService.GetBookings(book.PassengerEmail);
            flights = await _mongoDBService.GetFlights();

            var flight = flights.Find(f => f.Id == book.FlightId);
           
            
            Console.WriteLine(flight.ToString());
            var error = await flight.CancelBooking(book.PassengerEmail, book.numberOfSeats , _mongoDBService);
            
            
                

            if(error == null)
            {
                return NoContent();
            }
            else if (error is NotFoundError)
            {
                return NotFound();
            }
            else
            {
                throw new Exception($"The Error of type: {error.GetType().Name} occured while canceling the booking");
            }
            
        }
    }
}
