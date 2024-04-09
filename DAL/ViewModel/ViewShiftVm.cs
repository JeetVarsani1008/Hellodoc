using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ViewShiftVm
    {
        public List<Region> Regions { get; set; }   

        public List<Physician> physicianList { get; set; }
        public int PhysicianId { get; set; }

        public int RegionId { get; set; }
        public int ShiftDetailId { get; set; }
        public string PhysicianName { get; set; }
        public int ShiftId { get; set; }
        public string Region { get; set; }
        //public DateOnly StartDate { get; set; }
        
        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
        public DateOnly ShiftDate { get; set; }
        public bool IsRepeat { get; set; }
        public int RepeatUpto { get; set; }
    }
}
