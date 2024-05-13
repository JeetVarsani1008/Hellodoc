using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModelProvider
{
    public class TimeSheetModel
    {
        public int CheckData { get; set; }
        public List<BiWeeklyTimeSheetModel> TimeSheetList { get; set; } = null!;

        public List<TimePeriodModel> PeriodList { get; set; } = null!;
        public List<Physician> PhysicianList { get; set; }

        public DateOnly SelectedStartDate { get; set; }

        public DateOnly SelectedEndDate { get; set; }

        public string? SelectedPeriod { get; set; }

        public int PhysicianId { get; set; }

        public bool IsFinalize { get; set; }


        public bool? IsAnyBillPresent { get; set; }

        public string? PhysicianName { get; set; }

        public bool? IsAdmin { get; set; }

        public int? AdminId { get; set; }

        public int? Status { get; set; }

        public string? AdminNote { get; set; }

        public int? BonusAmount { get; set; }

        public int? NightShiftWeekend { get; set; }

        public int? Shift { get; set; }

        public int? HouseCallsNightsWeekend { get; set; }

        public int? PhoneConsults { get; set; }

        public int? PhoneConsultsNightsWeekend { get; set; }

        public int? BatchTesting { get; set; }

        public int? HouseCalls { get; set; }

        public int? InvoiceTotalHours { get; set; }

        public int? InvoiceWeekendHoliday { get; set; }

        public int? InvoiceHouseCalls { get; set; }

        public int? InvoicePhoneConsults { get; set; }

        public int? InvoiceTotal { get; set; }
        public bool? IsFinalized { get; set; }
    }
}
