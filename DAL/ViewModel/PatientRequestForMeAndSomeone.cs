using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace DAL.ViewModel
{
    public class PatientRequestForMeAndSomeone
    {
        public string Symptoms { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }
        public string Email { get; set; }

        [StringLength(12, MinimumLength = 8, ErrorMessage = "Phone Number should be between 8 and 12 characters")]
        public string Phone { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }


        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "invalid zipcode")]
        public string ZipCode { get; set; }

        public string Room { get; set; }
        public  IFormFile? Filepath { get; set; } = null;

        public string Relation { get; set; }

        public string Comments { get; set; }
    }
}
