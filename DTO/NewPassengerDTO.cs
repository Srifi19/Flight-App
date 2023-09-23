using System.ComponentModel.DataAnnotations;
namespace Flight2.DTO
{
    public record NewPassengerDTO(
        [Required][EmailAddress][StringLength(100,MinimumLength = 3)] String email,
        [Required][MinLength(2)][MaxLength(35)] String firstname,
        [Required][MinLength(2)][MaxLength(35)] String lastname,
        [Required] bool gender);
    
}
