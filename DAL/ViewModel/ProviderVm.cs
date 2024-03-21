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

        //public string Roles { get; set; }

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
