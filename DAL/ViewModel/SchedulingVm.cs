using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class SchedulingVm
    {
        public List<Region> region {  get; set; }

        public int PhysicianId { get; set; }
    }
}
