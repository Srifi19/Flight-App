using Flight2.ReadModel;
using Microsoft.AspNetCore.Mvc;
using System;
using Flight2.Domain.Entities;
using Flight2.DTO;
using Flight2.Domain.Error;

using Microsoft.EntityFrameworkCore;
using Flight2.MongoDB;

namespace Flight2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlightController : ControllerBase
    {

       
        private readonly ILogger<FlightController> _logger;
      
        private readonly MongoDBService mongoDbService;
        private List<Flight>? _flights;


        public FlightController(ILogger<FlightController> logger , 
            MongoDBService mongoDb)
        {
            _logger = logger;
            this.mongoDbService = mongoDb;
        }




        private async Task<IQueryable<Flight>> getThePlanesAsync()
        {
         _flights = await mongoDbService.GetFlights();
            return _flights.AsQueryable();
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(IEnumerable<FlightRm>), 200)]
        [HttpGet]

       
    public async Task<IEnumerable<FlightRm>> Search([FromQuery] FlightSearchParameter @params)
        {


            IQueryable<Flight> flights = await getThePlanesAsync();//all the flights


            if (!string.IsNullOrWhiteSpace(@params.Destination))
                flights = flights.Where(f => f.Arrival.Place.ToLower().Contains(@params.Destination.ToLower()));

            if (!string.IsNullOrWhiteSpace(@params.From))
                flights = flights.Where(f => f.departure.Place.ToLower().Contains(@params.From.ToLower()));

            if (@params.FromDate != null)
                flights = flights.Where(f => f.departure.Time >= @params.FromDate.Value.Date);

            if (@params.ToDate != null)
                flights = flights.Where(f => f.departure.Time < @params.ToDate.Value.Date);

            if (@params.numberOfPassenger != null && @params.numberOfPassenger != 0)
                flights = flights.Where(f => f.RemainingNumberOfSeats >= @params.numberOfPassenger);
            else
                flights = flights.Where(f => f.RemainingNumberOfSeats >= 1);



            _logger.LogInformation("Searching For a flight for: {Destination}" , @params);

            var flightRmList = flights
                .Select(flight => new FlightRm( 
                flight.Id,
                flight.airline,
                flight.price,
                new TimePlaceRm(flight.departure.Place.ToString(), flight.departure.Time),
                new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time),
                flight.RemainingNumberOfSeats)).ToArray();

                
            return flightRmList;
        }




        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FlightRm), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightRm>> Find(Guid id)
        {


            var flights = await getThePlanesAsync();
            var flight  = flights.SingleOrDefault(f => f.Id == id);
            if (flight == null)
                return NotFound();

            var readModel = new FlightRm(
                flight.Id,
                flight.airline,
                flight.price,
                new TimePlaceRm(flight.departure.Place.ToString(), flight.departure.Time),
                new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time),
                flight.RemainingNumberOfSeats
                );



            return Ok(readModel);
            
           
        }



        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Book(BookDto dto)
        {
            System.Diagnostics.Debug.WriteLine($"Booking a new flight {dto.FlightId}");
            var flights = await getThePlanesAsync();
            var flight = flights.SingleOrDefault(f => f.Id == dto.FlightId);
            
            if (flight == null)
            {
                return NotFound();
            }
          
            var error = flight.MakeBooking(dto.PassengerEmail, dto.numberOfSeats , mongoDbService);

            if (await error is OverBookError)
            {
                return Conflict(new { message = "Not Enough Seats" });
            }

            
            return CreatedAtAction(nameof(Find), new { id = dto.FlightId });
        }


    }

    
}