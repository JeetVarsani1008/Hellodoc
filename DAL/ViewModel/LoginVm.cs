using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class LoginVm
    {
        public string PasswordHash { get; set; }
        //public string PasswordConfirm {  get; set; }

        public string Email { get; set; }
    }
}
