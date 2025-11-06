using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Rental
{
    /// <summary>
    /// Data Transfer Object for the request to end a rental.
    /// It contains only the information needed from the client.
    /// </summary>
    public class EndRentalRequestDTO 
    {
        [Required(ErrorMessage = "Current latitude is required.")]
        [Range(-90.0, 90.0, ErrorMessage = "Invalid latitude value.")]
        public double CurrentLatitude { get; set; }

        [Required(ErrorMessage = "Current longitude is required.")]
        [Range(-180.0, 180.0, ErrorMessage = "Invalid longitude value.")]
        public double CurrentLongitude { get; set; }
    }
}
