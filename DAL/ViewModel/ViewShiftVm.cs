using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel
{
   
    public class ViewShiftVm
    {
        public class NotZeroTimeAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    TimeOnly time = (TimeOnly)value;
                    if (time == TimeOnly.MinValue)
                    {
                        return new ValidationResult("Start Time can not be Zero");
                    }
                }
                return ValidationResult.Success;
            }
        }

        public List<Region> Regions { get; set; }   

        public List<Physician> physicianList { get; set; }

        [Required(ErrorMessage ="Please select Physician!")]
        public int PhysicianId { get; set; }

        [Required(ErrorMessage ="Select One Region")]
        public int RegionId { get; set; }
        public int ShiftDetailId { get; set; }
        public string PhysicianName { get; set; }
        public int ShiftId { get; set; }
        public string Region { get; set; }

        public DateTime StartDate { get; set; }

        [NotZeroTime]
        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage ="Please Enter Shift Date!")]
        public DateOnly ShiftDate { get; set; }

        public bool IsRepeat { get; set; }
        public int RepeatUpto { get; set; }
    }
}
