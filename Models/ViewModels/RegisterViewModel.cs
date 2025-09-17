using System.ComponentModel.DataAnnotations;

namespace CMCS.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Employee Number")]
        public string EmployeeNumber { get; set; }

        [Display(Name = "Hourly Rate")]
        [Range(0, 10000)]
        public decimal? HourlyRate { get; set; }
    }
}