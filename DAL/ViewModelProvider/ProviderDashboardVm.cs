using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModelProvider
{
    public class ProviderDashboardVm
    {
        public List<RequestDataProvider> requestDataProvider {  get; set; }

        public PageVm Page { get; set; }
        public int? TotalPages { get; set; }
        public int? skipCount;
        public int? CurrentPage;

        public string statusArray {  get; set; }

        public int requestTypeId { get; set; }

        //for count
        public int NewCount { get; set; }
        public int PendingCount { get; set; }
        public int ActiveCount { get; set; }
        public int ConcludeCount { get; set; }
    }

    public class RequestDataProvider
    {
        public string Email { get; set; }
        public string DateOfBirth { get; set; }
        public bool IsFinalize { get; set; }
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }

        public int StatusForDash { get; set; }

    }
}
