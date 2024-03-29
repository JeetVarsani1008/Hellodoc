﻿using DAL.Models;
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

        public int RegionId {  get; set; }
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
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Phone Number should be between 8 and 12 characters")]
        public string Phone { get; set; }
        public string MedicalLicence { get; set; }
        public string AltPhone { get; set; }
        public string NPINumber { get; set;}
        public string SynchronizationEmail { get; set;}


        [Required(ErrorMessage = "Enter your Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; } 
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
        public IFormFile AgreementDoc { get; set; }
        public IFormFile BackgroundDoc { get; set; }
        public IFormFile NonDisclosureDoc { get; set; }

        public IFormFile HIPAACompliance {  get; set; }
        public IFormFile LicenseDoc { get; set; }
    }
    public class Provider
    {
        public int PhysicianId { get; set; }
        public bool StopNotification {  get; set; }
        public string Name { get; set; }

        public string Role { get; set; }

        public int Status {  get; set; }
        public string OnCallStatus { get; set; }

    }
}
