using DAL.Models;
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
        public int AdminId { get; set; }
        public int AspNetUserId { get; set; }

        [Required(ErrorMessage ="Please Enter User Name")]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
        public string Status { get; set; }

        public string Role { get; set; }

        [Required(ErrorMessage ="Please Enter Email")]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Compare("Email")]
        public string ConfirmEmail { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }

        public string Zip {  get; set; }

        public string Mobile { get; set; }

        public string AlternateMobile { get; set; }

        public List<Region> regions { get; set; }

        public List<AdminRegion> AdminRegions { get; set; }

        public int RegionId { get; set; }

        public int RoleId { get; set; }
    }
}
