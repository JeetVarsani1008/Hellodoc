using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminCreateRequestVm
    {
        [Required(ErrorMessage ="Please select one region")]
        public int RegionId { get; set; }
        public List<Region> regions { get; set; }

        [Required(ErrorMessage ="Please Enter First Name.")]
        public string FirstName { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage ="Please Enter Last Name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter your Phone Number")]
        [RegularExpression(@"^[0-9]{8,12}$", ErrorMessage = "Please enter a valid phone number between 8 and 12 digits.")]
        public string Phone {  get; set; }

        [Required(ErrorMessage ="Please Enter Email")]
        public string Email { get; set; }

        
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage ="Please Enter Street.")]
        public string Street {  get; set; }

        [Required(ErrorMessage ="Please Enter City Name.")]
        public string City { get; set; }

        public string State { get; set; }

        [Required(ErrorMessage ="Please Enter Zipcode")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "invalid zipcode")]
        public string ZipCode { get; set; }
        public string Room { get; set; }

        public string Comments { get; set; }
        public string Symptoms { get; set; }
    }
}
