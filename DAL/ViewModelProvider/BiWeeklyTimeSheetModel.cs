using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModelProvider
{
    public class BiWeeklyTimeSheetModel
    {
        public int WeeklyTimeSheetId { get; set; }
        public DateOnly Date { get; set; }

        public int? OnCallHours { get; set; }

        public int? TotalHours { get; set; }

        public bool IsWeekend { get; set; }

        public int? NoOfHouseCalls { get; set; }

        public int? NoOfPhoneConsults { get; set; }

        public string Item { get; set; }

        public int Amount { get; set; }

        public IFormFile? Bill { get; set; }

        public string BillName { get; set; }

        public int? NightWeekendHouseCall { get; set; }

        public int? NightWeekendPhoneConsult { get; set; }

        public int? NightShiftWeekend {  get; set; }

        public int? NumberOfShift { get; set; }

        public bool IsFinalize { get; set; }
    }
}
