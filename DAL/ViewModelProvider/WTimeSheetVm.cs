//using DAL.ViewModel;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DAL.ViewModelProvider
//{
//    public class WTimeSheetVm
//    {
//            public int? WeeklyTimeSheetId { get; set; } = 0;
//            public int ProviderId { get; set; } = 0;

//            public string? ProviderName { get; set; } = null;
//            public int? AdminId { get; set; } = 0;
//            public DateTime StartDate { get; set; }
//            public DateTime EndDate { get; set; }

//            public string? StartsDate { get; set; } = null;

//            public string? EndsDate { get; set; } = null;

//            public DateTime SelectedWeek { get; set; }
//            public List<Weeks> Weeks { get; set; }

//            public List<TimeSheetData> Data { get; set; }

//            public string? IsFinalize { get; set; } = null;

//            public string? Status { get; set; } = null;

//            public bool? IsAdmin { get; set; } = false;

//            public int? ShiftPayrate { get; set; } = 0;

//            public int? NightWeekendShiftPayrate { get; set; } = 0;

//            public int? PhoneConsultPayrate { get; set; } = 0;

//            public int? HouseCallPayrate { get; set; } = 0;

//            public int? TotalHoursInvoice { get; set; } = 0;

//            public int? WeekendHolidayInvoice { get; set; } = 0;

//            public int? PhoneConsultInvoice { get; set; } = 0;

//            public int? HouseCallInvoice { get; set; } = 0;

//            public int? TotalInvoice { get; set; } = 0;

//            public int? BonusAmount { get; set; } = 0;

//            public string? AdminNote { get; set; } = null;

//            public List<ProviderVm> Providers { get; set; }

//    }
//        public class Weeks
//        {
//            public DateTime StartDate { get; set; }
//            public DateTime EndDate { get; set; }
//        }

//        public class TimeSheetData
//        {
//            public int? weeklySheetDetailsId { get; set; } = 0;
//            public DateTime Date { get; set; }

//            public string? ShiftDate { get; set; } = null;

//            [RegularExpression(@"^[0-9]\d*$")]
//            [Required]
//            public int? OnCallHours { get; set; } = 0;

//            [RegularExpression(@"^[0-9]\d*$")]
//            [Required]
//            public int? totalHours { get; set; } = 0;


//            public bool HolidayOrWeekend { get; set; } = false;

//            [RegularExpression(@"^[0-9]\d*$")]
//            [Required]
//            public int? NumberOfHouseCalls { get; set; } = 0;

//            public int? NumberOfHouseCallsWeekend { get; set; } = 0;

//            [Required]
//            public int? NightShiftWeekend { get; set; } = 0;

//            [RegularExpression(@"^[0-9]\d*$")]
//            [Required]
//            public int? NumberOfPhoneConsult { get; set; } = 0;


//            public int? NumberOfPhoneConsultWeekend { get; set; } = 0;

//            [Required]
//            public int? BatchTesting { get; set; } = 0;

//            public string? Items { get; set; } = null;

//            [RegularExpression(@"^[0-9]\d*$")]
//            [Required]
//            public int? Amount { get; set; } = 0;

//            public string? BillName { get; set; } = null;


//            public IFormFile? Bill { get; set; }
//            public int? PayRateId { get; set; } = 0;


//            public int? NumberOfShifts { get; set; }

//        }

//        public class FilterWeeklyTimeSheet : WTimeSheetVm
//        {
//            public DateTime StartDate { get; set; }

//            public DateTime EndDate { get; set; }

//            public int? AdminId { get; set; } = 0;
//        }
//}
