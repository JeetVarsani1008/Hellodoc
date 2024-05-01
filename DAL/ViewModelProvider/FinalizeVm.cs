using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModelProvider
{
    public class FinalizeVm
    {
        public int PhysicianId { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public List<BiWeeklyTimeSheetModel> TimeSheetList { get; set; } = null!;
        public List<TimePeriodModel> PeriodList { get; set; } = null!;
        public DateOnly SelectedStartDate { get; set; }
        public DateOnly SelectedEndDate { get; set; }

        public string? SelectedPeriod { get; set; }
    }
}
