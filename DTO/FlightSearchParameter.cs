using System.ComponentModel;

namespace Flight2.DTO
{
    public record FlightSearchParameter (

        [DefaultValue("12/25/2022 10:30:00 AM")]
        DateTime? FromDate,
        [DefaultValue("12/25/2022 10:30:00 AM")]
        DateTime? ToDate,
        [DefaultValue("Los Angeles")]
        string? From,
        [DefaultValue("Berlin")]
        string? Destination,
        [DefaultValue("1")]
        int? numberOfPassenger)
        ;
    
}
