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

        [Required(ErrorMessage = "Please Enter Phone Number")]

        public string F_Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Email")]
        public string F_Email { get; set; }

        [Required(ErrorMessage = "Please Enter Your Relation with patient")]

        public string Relation { get; set; }
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
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please Enter Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please Enter State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please Enter Zip Code")]
        public string ZipCode { get; set; }

        public string? Room { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Please Enter Phone Number")]
        public string PhoneNumber { get; set; }
        public string File { get; set; }

    }
}
