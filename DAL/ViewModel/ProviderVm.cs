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

        public List<Physician> physicians { get; set; }

        public string Roles { get; set; }

    }
}
