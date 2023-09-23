using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Flight2.Domain.Entities
{
    public class Booking {

        [BsonId] public Guid Id { get; set; }

        [BsonElement("passengeremail")]
        public string PassengerEmail;
        
        [BsonElement("numberofseats")]
        public byte numberOfSeats;

        public Booking(string passengerEmail, byte numberOfSeats)
        {
            PassengerEmail = passengerEmail;
            this.numberOfSeats = numberOfSeats;
        }


        public Guid getId()
        {
            return Id;
        }

        public void setId(Guid id) { this.Id = id; }  


    }
   
}
