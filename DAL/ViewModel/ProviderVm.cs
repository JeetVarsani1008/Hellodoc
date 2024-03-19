using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ProviderVm
    {
        public List<Region> regions {  get; set; }

        //public List<Physician> physicians { get; set; }

        public List<Provider> providers { get; set; }

        //public string Roles { get; set; }

    }
    public class Provider
    {
        public string Name { get; set; }

        public string Role { get; set; }

        public int Status {  get; set; }
        public string OnCallStatus { get; set; }

    }
}
