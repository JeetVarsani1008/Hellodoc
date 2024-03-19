using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ViewCaseVm
    {
        public int RequestId { get; set; }

        public string FirstName {  get; set; }

        public string LastName { get; set; }

        public string Notes { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int RequestTypeId { get; set; }

        public string Region {  get; set; }
    }
}
