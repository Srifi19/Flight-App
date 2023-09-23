using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Flight2.DTO;
using Flight2.ReadModel;
using Flight2.Domain.Entities;

using Flight2.MongoDB;

namespace Flight2.Controllers
{


    [Route("[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private MongoDBService mongoDBService;
        

        public PassengerController(MongoDBService mongoDBService)
        {
            
            this.mongoDBService = mongoDBService;
        }

        [HttpPost]
        //Creating
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register(NewPassengerDTO dto)
        {


            await mongoDBService.CreatePassenger(new Passenger(
                           dto.firstname,
                           dto.lastname,
                           dto.email,
                           dto.gender
                    ));
            
            return CreatedAtAction(nameof(Find) ,new {email = dto.email});
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<PassengerRm>> Find(String email)
        {

            var passengers = await mongoDBService.GetPassengers();
            var passenger = passengers.FirstOrDefault(p => p.email == email);
            if(passenger == null)
            {
                return NotFound();
            }
            

            var pass = new PassengerRm(
                passenger.firstName,
                passenger.lastName,
                passenger.email,
                passenger.gender
                );
            return Ok(pass);
        }

        


    }
}
