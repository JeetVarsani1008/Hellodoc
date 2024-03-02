using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel
{
    public class ConciergeData
    {
        [Required(ErrorMessage = "Please Enter the First Name")]
        public string C_FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter the Last Name")]
        public string C_LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile Number")]
        public string C_Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Email")]
        public string C_Email { get; set; }

        public string HotelName { get; set; }

        [Required(ErrorMessage = "Please Enter Street Name")]
        public string Street { get; set; }


        [Required(ErrorMessage = "Please Enter City Name")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please Enter State Name")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please Enter Zip Code")]

        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "invalid zipcode")]
        public string ZipCode { get; set; }

        public string Symptoms { get; set; }

        [Required(ErrorMessage = "Please Enter the First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter the Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Birth Date")]
        public DateTime Birthdate { get; set; }

        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile Number")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Phone Number should be between 8 and 12 characters")]
        public string Phone { get; set; }

        public string Room { get; set; }

    }
}
