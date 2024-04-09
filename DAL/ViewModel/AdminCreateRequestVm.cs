using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminCreateRequestVm
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone {  get; set; }
        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Street {  get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string Zipcode { get; set; }
        public string Room { get; set; }
        public string Comments { get; set; }

    }
}
