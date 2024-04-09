using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage ="Please enter reason to block")]
        public string Reason { get; set; }

    }
}
