using System.ComponentModel.DataAnnotations;
namespace Flight2.DTO
{ 
    public record BookDto(
        [Required]Guid FlightId,
        [Required][EmailAddress][StringLength(100,MinimumLength = 3)]string PassengerEmail,
        [Required][Range(1,254)]byte numberOfSeats);//Minimize the size Max 256 seats
   
}
