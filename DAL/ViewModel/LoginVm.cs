using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class LoginVm
    {
        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please Enter Password")]
        public string PasswordHash { get; set; }


    }
}
