namespace Flight2.ReadModel
{
    public record BookingRm(
        Guid FlightId,
        string airline,
        string price,
        TimePlaceRm arrival,
        TimePlaceRm departure,
        int numberOfBookedSeats,
        string passengerEmail
        );
    
}
