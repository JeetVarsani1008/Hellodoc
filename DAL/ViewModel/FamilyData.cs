using DAL.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel
{
    public class FamilyData
    {
        [Required(ErrorMessage = "Please Enter the First Name")]
        public string F_FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter the Last Name")]
        public string F_LastName { get; set; }


		[Required(ErrorMessage = "Please Enter your Phone Number")]
		[RegularExpression(@"^[0-9]{8,12}$", ErrorMessage = "Please enter a valid phone number between 8 and 12 digits.")]
		public string F_Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Your Email")]
        public string F_Email { get; set; }

        [Required(ErrorMessage = "Please Enter Your Relation with patient")]

        public string? Relation { get; set; } = null;
        public string Symptoms { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please Enter First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Birth Date")]
        public DateTime Birthdate { get; set; }

        [Required(ErrorMessage = "Please Enter Patient's Email")]
        public string Email { get; set; }

		[Required(ErrorMessage = "Please Enter your Phone Number")]
		[RegularExpression(@"^[0-9]{8,12}$", ErrorMessage = "Please enter a valid phone number between 8 and 12 digits.")]
		public string Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please Enter State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please Enter Zip Code")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "invalid zipcode")]
        public string ZipCode { get; set; }

        public string? Room { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string PasswordHash { get; set; }

		[Required(ErrorMessage = "Please Enter your Phone Number")]
		[RegularExpression(@"^[0-9]{8,12}$", ErrorMessage = "Please enter a valid phone number between 8 and 12 digits.")]
		public string PhoneNumber { get; set; }
        public string File { get; set; }
        public List<Region> regions { get; set; }

    }
}
