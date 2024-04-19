using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//use this model for 3 pop up clear and send agreement and send mail
namespace DAL.ViewModel
{
    public class AdminClearVm
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage ="Enter Your Phone Number")]
        [RegularExpression(@"^[0-9]{8,12}$", ErrorMessage = "Please enter a valid phone number between 8 and 12 digits.")]
        public string Number { get; set; }

        [Required(ErrorMessage ="Please Enter Email")]
        public string Email { get; set; }

        public int RequestTypeId { get; set; }

        //send mail field
        [Required(ErrorMessage ="Please Enter Your First Name")]
        public string FirstName { get; set; } 

        [Required(ErrorMessage ="Please Enter Your Last Name")]
        public string LastName { get; set; }


    }
}
