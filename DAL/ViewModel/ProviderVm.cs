using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ProviderVm
    {
        public int PhysicianId { get; set; }
        public List<Region> regions {  get; set; }

        public List<PhysicianRegion> physicianRegions { get; set; }

        public List<Provider> providers { get; set; }
        public List<Role> roles { get; set; }

        // this part is for edit provider account

        [Required(ErrorMessage ="Enter Your Region From Given Region!")]
        public int RegionId {  get; set; }

        [Required(ErrorMessage ="Select One Role.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Please Enter Username")]
        public string UserName { get; set; }


        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        public string Email {  get; set; }


        [Required(ErrorMessage = "Enter your Phone Number")]
		[RegularExpression(@"^[0-9]{8,12}$", ErrorMessage = "Please enter a valid phone number between 8 and 12 digits.")]
		public string Phone { get; set; }

        public string MedicalLicence { get; set; }

		[RegularExpression(@"^[0-9]{8,12}$", ErrorMessage = "Please enter a valid phone number between 8 and 12 digits.")]
		public string AltPhone { get; set; }
        public string NPINumber { get; set;}
        public string SynchronizationEmail { get; set;}


        [Required(ErrorMessage = "Enter your Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }

		[Required(ErrorMessage = "ZipCode is required")]
		[RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "invalid zipcode")]
		public string Zip { get; set; }

        [Required(ErrorMessage = "Please Enter your Business Name")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Please Enter your Business Website")]
        public string BusinessWebsite { get; set; }
        public string AdminNotes { get; set; }

        public string Description { get; set; }
        public string communication { get; set; }

        public string Filepath { get; set; }

        public string Photo { get; set; }

        public string Signature { get; set; }

        public bool IsAgreementDoc { get; set; }
        public bool IsBackgroundDoc { get; set; }
        public bool IsLicenseDoc { get; set; }
        
        public bool IsNonDisclosureDoc { get; set; }

        //for store name 
        public IFormFile PhotoFile { get; set; }
        public IFormFile SignatureFile { get; set; }
        public IFormFile AgreementDoc { get; set; }
        public IFormFile BackgroundDoc { get; set; }
        public IFormFile NonDisclosureDoc { get; set; }

        public IFormFile HIPAACompliance {  get; set; }
        public IFormFile LicenseDoc { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string SMS { get; set; }
    }

    #region Provider
    public class Provider
    {
        public int PhysicianId { get; set; }
        public bool StopNotification {  get; set; }
        public string Name { get; set; }

        public string Role { get; set; }

        public int Status {  get; set; }
        public string OnCallStatus { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly ShiftDate { get; set;}
        public DateOnly CurrentDate { get; set;}

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set;}
        public TimeOnly CurrentTime { get; set;}
	}
    #endregion
}
