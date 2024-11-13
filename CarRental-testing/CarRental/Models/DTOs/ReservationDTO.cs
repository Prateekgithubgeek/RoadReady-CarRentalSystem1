using CarRental.Validations;
using System.ComponentModel.DataAnnotations;

namespace CarRental.DTOs
{
    public class ReservationDTO
    {
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "CarId is required.")]
        public int CarId { get; set; }

        //[Required(ErrorMessage = "PickupDate is required.")]
        //[DataType(DataType.Date, ErrorMessage = "PickupDate must be a valid date.")]
        //public DateOnly PickupDate { get; set; }

        //[Required(ErrorMessage = "DropoffDate is required.")]
        //[DataType(DataType.Date, ErrorMessage = "DropoffDate must be a valid date.")]
        //[CompareDate(ErrorMessage = "DropoffDate must be after PickupDate.")]
        //public DateOnly DropoffDate { get; set; }
        [Required]
        public DateOnly PickupDate { get; set; }

        [Required]
        [CompareDate(ErrorMessage = "Drop-off date must be after the pickup date.")]
        public DateOnly DropOffDate { get; set; }

        [Required(ErrorMessage = "TotalPrice is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "TotalPrice must be greater than 0.")]
        public decimal TotalPrice { get; set; }

        [StringLength(50, ErrorMessage = "ReservationStatus cannot exceed 50 characters.")]
        public string? ReservationStatus { get; set; }
    }
}
