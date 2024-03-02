using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ViewNotesVm
    {
            public int RequestId { get; set; }

            public string? TransferNotes { get; set; }

            public string? PhysicianNotes { get; set; }

            public string? AdminNotes { get; set; }

            public string? AdminCancelNotes { get; set; }

            public string? PatientCancelNotes { get; set; }
    }
}
