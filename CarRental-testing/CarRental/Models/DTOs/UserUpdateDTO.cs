using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.DTOs
{
    public class UserUpdateDTO
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? Role { get; set; }
    }

}
