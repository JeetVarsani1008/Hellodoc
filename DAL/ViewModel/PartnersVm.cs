using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class PartnersVm
    {
        public List<Vendor> vendor {  get; set; }
        public List<HealthProfessionalType> healthProfessionalType { get; set; }
        public int VendorId { get; set; }

        public int Profession { get; set; }

        public string BusinessName { get; set; }

        public string Email { get; set; }

        public string FaxNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string BusinessContact { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

    }

    public class Vendor 
    {
        public int VendorId { get; set; }
        public string Profession { get; set; }

        public string BusinessName { get; set; }

        public string Email { get; set; }

        public string FaxNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string BusinessContact { get; set; }

        public string Street {  get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }


    }

}
