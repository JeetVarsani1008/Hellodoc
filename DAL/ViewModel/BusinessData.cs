using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel
{
    public class BusinessData
    {
        [Required, StringLength(50), Display(Name = "First Name")]
        public string B_FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Last Name")]
        public string B_LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Phone Number")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Phone Number should be between 8 and 12 characters")]

        public string B_Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Email")]
        public string B_Email { get; set; }

        [Required(ErrorMessage = "Please Enter Relation ")]
        public string Relation { get; set; }

        public int CaseNumber { get; set; }

        public string Symptoms { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Birth Date")]
        public DateTime Birthdate { get; set; }

        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Phone")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Phone Number should be between 8 and 12 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please Enter State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please Enter Zip Code")]
        public string ZipCode { get; set; }

        public string Room { get; set; }
    }
}
