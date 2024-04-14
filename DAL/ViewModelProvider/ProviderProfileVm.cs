using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModelProvider
{
    public class ProviderProfileVm
    {
        public List<Region> regions {  get; set; }
        
        public List<PhysicianRegion> PhysicianRegions { get; set; }
        public int AspNetUserId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }

        [Required(ErrorMessage = "Please Enter Zip Code")]
        [RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "invalid zipcode")]
        public string Zip { get; set; }
        public string AlternateMobile { get; set; }


        public string BusinessName { get; set; }
        public string BusinessWebSite { get; set; }

        public IFormFile PhotoFile { get; set; }

        public IFormFile SignatureFile { get; set; }

        public int PhysicianId { get; set; }
        public string MedicalLicense { get; set; }
        public string NPINumber { get; set; }
        public string Photo {  get; set; }
        public string Signature {  get; set; }
    }
}
