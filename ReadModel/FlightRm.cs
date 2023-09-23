namespace Flight2.ReadModel
{
    public record FlightRm(
        Guid Id,
        String airline,
        String price,
        TimePlaceRm Departure,
        TimePlaceRm Arrival,
        int RemainingNumberOfSeats
        );
    
}
