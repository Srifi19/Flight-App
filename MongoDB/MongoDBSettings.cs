using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flight2.MongoDB
{
    public class MongoDBSettings
    {
        [Required]
        [JsonPropertyName("ConnectionURI")]
        public string? ConnectionURI { get; init; }

        [Required]
        [JsonPropertyName("DatabaseName")]
        public string? DatabaseName { get; init; }

        [Required]
        [JsonPropertyName("CollectionName")]
        public CollectionNames Collections { get; init; } = new CollectionNames();
    }

    public class CollectionNames
    {
        [Required]
        [JsonPropertyName("Flights")]
        public string? Flights { get; init; }

        [Required]
        [JsonPropertyName("Bookings")]
        public string? Bookings { get; init; }

        [Required]
        [JsonPropertyName("Passengers")]
        public string? Passengers { get; init; }
    }
}
