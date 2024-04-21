using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminViewUploadVm
    {
        public int PhysicianId { get; set; }
        public int RequestId { get; set; }
        
        public string Name { get; set; }

        public string ProviderNotes { get; set; }
        public List<RequestWiseFile> requestWiseFiles { get; set; }

        public bool IsFinalized { get; set; }

        public string PatientName { get; set; }
    }
}
