using Flight2.Domain.Error;
using Flight2.ReadModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Flight2.MongoDB;
using MongoDB.Bson.Serialization.Serializers;

namespace Flight2.Domain.Entities
{
    public class Flight
    {
        MongoDBService? mongoDBService;
        [BsonId]
        
        public Guid Id { get; set; }
        [BsonElement("airline")]
        public  String airline { get; set; }
        [BsonElement("price")]
        public String price { get; set; }
        [BsonElement("departure")]
        public TimePlace departure { get; set; }
        [BsonElement("arrival")]
        public TimePlace Arrival { get; set; }
        [BsonElement("remainingnumberofseats")]
        public int RemainingNumberOfSeats{ get; set; }

        public Flight(
        Guid id,
        String airline,
        String price,
        TimePlace departure,
        TimePlace arrival,
        int remainingNumberOfSeats,
        MongoDBService mongoDBService
            ) { 
            this.Id = id;
            this.airline = airline;
            this.price = price;
            this.departure = departure;
            this.Arrival = arrival; 
            this.RemainingNumberOfSeats = remainingNumberOfSeats;
            this.mongoDBService = mongoDBService;
        }
        public Flight(
        Guid id,
        String airline,
        String price,
        TimePlace departure,
        TimePlace arrival,
        int remainingNumberOfSeats
        
            )
        {
            this.Id = id;
            this.airline = airline;
            this.price = price;
            this.departure = departure;
            this.Arrival = arrival;
            this.RemainingNumberOfSeats = remainingNumberOfSeats;
            
        }





        public IList<Booking> Bookings = new List<Booking>() ;
        //public Flight() { }
        internal async Task<object?> MakeBooking(string passengerEmail , byte numberOfSeats , MongoDBService mongoDB)
        {
            var flight = this;
            mongoDBService = mongoDB;
            if (flight.RemainingNumberOfSeats < numberOfSeats)
            {
                return new OverBookError();
            }

            Booking booking = new Booking(
                    passengerEmail,
                    numberOfSeats
            );
            flight.Bookings.Add(booking);
        
            

            flight.RemainingNumberOfSeats -= numberOfSeats;
            if(mongoDBService != null)
            {
               
                
                await mongoDBService.CreateBooking(this, passengerEmail, booking);
               
            }
            return null;
        }



        public async Task<object?> CancelBooking(string passengerEmail , byte numberOfSeats , MongoDBService mongoDB)
        {
            var booking = Bookings.FirstOrDefault(b => numberOfSeats == b.numberOfSeats
            && b.PassengerEmail == passengerEmail);
            if(booking == null)
            {
                return new NotFoundError();
            }
            mongoDBService = mongoDB;
            RemainingNumberOfSeats += (int)booking.numberOfSeats;
            
            if (mongoDBService != null)
            { 
                await mongoDBService.DeleteBooking(booking, this);
            }

            return null;
        }
    }
    
}
