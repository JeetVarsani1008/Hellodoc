using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminBlockVm
    {
        public int RequestId { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Description { get; set; }
        public string Reason { get; set; }

    }
}
