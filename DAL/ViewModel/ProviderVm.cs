using DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
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

        // this part is for edit provider account

        public int RegionId {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email {  get; set; }

        public string Phone { get; set; }
        public string MedicalLicence { get; set; }
        public string AltPhone { get; set; }
        public string NPINumber { get; set;}
        public string SynchronizationEmail { get; set;}

        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; } 
        public string Zip { get; set; }

        public string Description { get; set; }
        public string communication { get; set; }

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
