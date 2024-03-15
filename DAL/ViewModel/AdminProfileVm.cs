using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminProfileVm
    {
        public int AspNetUserId { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
        public string Status { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }

        public string Zip {  get; set; }

        public string Mobile { get; set; }

        public string AlternateMobile { get; set; }

    }
}
