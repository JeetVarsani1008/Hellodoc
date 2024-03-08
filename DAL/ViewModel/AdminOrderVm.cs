using DAL.Models;
using System;
using System.Collections.Generic;
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

        public string Profession {  get; set; }
        
        public string Business {  get; set; }

        public string BusinessContact { get; set; }

        public string Email { get; set; }

        public string FaxNumber { get; set; }

        public string Prescription { get; set; }

        public int Refill { get; set; }
    }
}
