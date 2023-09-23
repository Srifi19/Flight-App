using MongoDB.Bson.Serialization.Attributes;

namespace Flight2.Domain.Entities
{
    public record TimePlace {

        [BsonElement("place")]
        public String Place;
        [BsonElement("time")]
        public DateTime Time;

        public TimePlace(string place, DateTime time)
        {
            Place = place;
            Time = time;
        }
    }

    
}
