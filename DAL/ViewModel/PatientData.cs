using System.ComponentModel.DataAnnotations;
using DAL.Models;
using Microsoft.AspNetCore.Http;
namespace DAL.ViewModel
{
    public class PatientData
    {
        [Required(ErrorMessage ="Please select one region")]
        public int RegionId { get; set; }
        public List<Request> Requests { get; set; }
        public string? Symptoms { get; set; }

        [Required(ErrorMessage = "Please enter First Name")]
        public string? FirstName { get; set; }

        public int? Id { get; set; }

        [Required(ErrorMessage = "Please enter Last Name")]
        public string? LastName { get; set; }

		[Required(ErrorMessage = "Enter your Phone Number")]
		[RegularExpression(@"^[0-9]{8,12}$", ErrorMessage = "Please enter a valid phone number between 8 and 12 digits.")]
		public string? Phone { get; set; }

        [Required(ErrorMessage = "Enter your Birthdate")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage ="Password filed is Required")]
        public string? PasswordHash { get; set; }

        [Required(ErrorMessage ="Please enter your email")]
        public string? Email { get; set; }

        [Required(ErrorMessage ="street is required")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string? State { get; set; }

        [Required(ErrorMessage = "ZipCode is required")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "invalid zipcode")]
        public string? ZipCode { get; set; }

        public string? Room { get; set; }

        public string? Documents { get; set; }
        public string? Status { get; set; }
        public string? CreatedDate { get; set; }

        public IFormFile? Filepath { get; set; } = null;

        public int? Count { get; set; }
        public List<Region> regions { get; set; }
    }
}
