using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminOrderVm
    {
        public int RequestId { get; set; }
        public int VendorId { get; set; }

        public List<HealthProfessionalType> healthProfessionType {  get; set; }

        public List<HealthProfessional> healthProfessionals { get; set; }

        [Required(ErrorMessage ="Please select one profession")]
        public int ProfessionId { get; set; }

        public string ProfessionName { get; set; }
        public string Profession {  get; set; }

        [Required(ErrorMessage ="Please Enter Business Name")]
        public string Business {  get; set; }

        [Required(ErrorMessage ="Please Enter Business Contact")]
        public string BusinessContact { get; set; }

        [Required(ErrorMessage ="Please Enter Email Address")]
        public string Email { get; set; }

        public string FaxNumber { get; set; }

        [Required]
        public string Prescription { get; set; }

        public int Refill { get; set; }
    }
}
