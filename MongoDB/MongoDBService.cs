
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using Flight2.Domain.Entities;
using Flight2.ReadModel;
using Microsoft.IdentityModel.Tokens;

namespace Flight2.MongoDB
{
   

   

    public class MongoDBService
    {
        

        private IMongoCollection<Booking>? _bookingCollection;
        private IMongoCollection<Flight>? _flightsCollection;
        private IMongoCollection<Passenger>? _passengerCollection;

        public MongoDBService()
        {
           
            

        }

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            

            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);

            Console.WriteLine("ConnectionURI: " + mongoDBSettings.Value.ConnectionURI);
            Console.WriteLine("DatabaseName: " + mongoDBSettings.Value.DatabaseName);
            Console.WriteLine("Collections.Flights: " + mongoDBSettings.Value.Collections.Flights);
            Console.WriteLine("Collections.Bookings: " + mongoDBSettings.Value.Collections.Bookings);
            Console.WriteLine("Collections.Passengers: " + mongoDBSettings.Value.Collections.Passengers);
            Console.WriteLine(database.DatabaseNamespace.ToString());
            //Console.WriteLine(database.GetCollection<Flight>(mongoDBSettings.Value.Collections.Flights).CollectionNamespace.ToString());
           
            try {
                
                _flightsCollection = database.GetCollection<Flight>(mongoDBSettings.Value.Collections.Flights);
                _bookingCollection = database.GetCollection<Booking>(mongoDBSettings.Value.Collections.Bookings);
                _passengerCollection = database.GetCollection<Passenger>(mongoDBSettings.Value.Collections.Passengers);
            }catch(Exception ex)
            {
                Console.WriteLine("Hayda Maam yzbat");
                Console.WriteLine(ex.StackTrace);
            }
            
        }

        
        public async Task<List<Flight>> GetFlights() {
           
                return await _flightsCollection.AsQueryable().ToListAsync();
            
           
        }
        public async Task<List<Booking>> GetBookings(String email) {

            var filter = Builders<Booking>.Filter.Eq("passengeremail", email);
            var bookings = await _bookingCollection.Find(filter).ToListAsync();
            return bookings;
        }
        public async Task<List<Passenger>> GetPassengers() {

            return await _passengerCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task CreateFlight(Flight flight)
        {
            try
            {
                if(_flightsCollection != null)
                {
                    await _flightsCollection.InsertOneAsync(flight);
                }
            }catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }


        }
        public async Task CreatePassenger(Passenger passenger)
        {
            try {
                if(_passengerCollection != null)
                {
                   

                    var filter = Builders<Passenger>.Filter.Eq("email", passenger.email);
                    var pass = await _passengerCollection.Find(filter).FirstOrDefaultAsync();
                    if (pass != null)
                    {
                        // passenger exists, do not create it again
                    }
                    else
                    {
                        // passenger does not exist, create it
                        await _passengerCollection.InsertOneAsync(passenger);
                    }
                }
            }catch(Exception ex) { Console.WriteLine(ex.Message); }

        }
        public async Task CreateBooking(Flight flight, String email , Booking booking)
        {
           
            // Retrieve the flight and passenger from the database based on their IDs
            var flightVar = await _flightsCollection.Find(f => f.Id == flight.Id).FirstOrDefaultAsync();
            var passengerVar = await _passengerCollection.Find(p => p.email == email).FirstOrDefaultAsync();

            if (flightVar != null && passengerVar != null)
            {
                // Update the flight and passenger information based on the booking details

                flightVar.Bookings.Add(booking);
                flightVar.RemainingNumberOfSeats -= booking.numberOfSeats;
                // Save the updated flight and passenger back to the database
                await _flightsCollection.ReplaceOneAsync(f => f.Id == flight.Id, flightVar);
                await _passengerCollection.ReplaceOneAsync(p => p.email == email, passengerVar);
                if(_bookingCollection!=null)
                {
                
                    await _bookingCollection.InsertOneAsync(booking);
                }
            }

        }
        public async Task DeleteBooking(Booking booking , Flight flight)
        {
           
             
         
                
                var flightVar = flight;
                flightVar.Bookings.Remove(booking);
            Console.WriteLine(booking.PassengerEmail);
            Console.WriteLine(booking.numberOfSeats);
            await _bookingCollection.DeleteOneAsync(b => b.PassengerEmail == booking.PassengerEmail);
                await _flightsCollection.ReplaceOneAsync(f => f.Id == flight.Id, flightVar);
            
            

        }

        public async Task SeedFlights(IEnumerable<Flight> flights)
        {
             if(_flightsCollection == null)
            {
                return;
            }
            else
            {
                try
                {
                  await _flightsCollection.InsertManyAsync(flights);
                }catch(Exception ex) { Console.WriteLine(ex.StackTrace); }
            }
            
        }


    }
}
