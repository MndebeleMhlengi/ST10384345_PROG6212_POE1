using System.ComponentModel.DataAnnotations;

namespace CMCS.Models.ViewModels
{
    public class ClaimSubmissionViewModel
    {
        [Required]
        [Range(1, 12)]
        [Display(Name = "Month Worked")]
        public int MonthWorked { get; set; }

        [Required]
        [Range(2020, 2030)]
        [Display(Name = "Year Worked")]
        public int YearWorked { get; set; }

        [Required]
        [Range(0.1, 500)]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(0, 10000)]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Module Taught")]
        public string ModuleTaught { get; set; }

        [Display(Name = "Additional Notes")]
        public string AdditionalNotes { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount => HoursWorked * HourlyRate;
    }
}