using MongoDB.Bson.Serialization.Attributes;

namespace Flight2.Domain.Entities
{
    public class Passenger {

        [BsonId] //this attribute tells MongoDB to use this property as the document id
        public Guid Id { get; set; }
        [BsonElement("firstname")]
        public String firstName;
        [BsonElement("lastname")]
        public String lastName;
        [BsonElement("email")]
        public String email;
        [BsonElement("gender")]
        public bool gender;

        public Passenger(string firstName, string lastName, string email, bool gender)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.gender = gender;
        }
    }
    
}
