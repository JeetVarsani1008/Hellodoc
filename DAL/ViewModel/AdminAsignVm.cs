using DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminAsignVm
    {
        [Required(ErrorMessage="Please enter description")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Please select One Physician")]
        public int PhysicianId { get; set; }

        public int RequestId { get; set; }

        public List<Region> regions {  get; set; }

        public List<Physician> physicianList { get; set; }
    }
    public class regions
    {
        public int RegionId { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }
    }
}
