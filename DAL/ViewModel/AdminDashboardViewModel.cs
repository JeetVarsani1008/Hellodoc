using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminDashboardViewModel
    {
        public string RequestTypeId { get; set; }

        public int RegionId { get; set; }
        public PageVm Page { get; set; }
        public int? TotalPages { get; set; }
        public int? skipCount;
        public int? CurrentPage;
        public List<RequestListAdminDash> requestListAdminDash { get; set; }
        public List<Region> regions { get; set; }

        public RequestListAdminDash? adminDash { get; set; }

        public Request requestData { get; set; }

        public int StatusForName { get; set; }

        public string statusArray { get; set; }

        public int Regin_Short {get; set;}

        public string reqTypId { get; set; }

        public List<CaseTag> CaseTags { get; set; }

        [Required(ErrorMessage ="Please Enter Notes")]
        public string Notes {  get; set; }

        [Required(ErrorMessage ="Please give reason for cancellation")]
        public string CaseTagss { get; set; }

        //for count
        public int NewCount { get; set; }
        public int PendingCount { get; set; }
        public int ActiveCount { get; set; }
        public int ConcludeCount { get; set; }
        public int ToCloseCount { get; set; }
        public int UnpaidCount { get; set; }

    }
    
    public class RequestListAdminDash
    {
        public int RequestId { get; set; }

        public string FirstName { get; set; }   
        public string LastName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string DateOfBirth { get; set; }


        public string Requestor { get; set; }

        public DateTime? RequestDate { get; set; } //Createddate

        public string Phone { get; set; }

        public string? Notes { get; set; }

        public string? Address { get; set; }

        public string ChatWith { get; set; }

        public string Actions { get; set; }

        public string Physician { get; set; }

        public DateTime DateOfService { get; set; }

        public string Region { get; set; }

        public int Status { get; set; }

        public int year { get; set; }

        public int date { get; set; }

        public string month { get; set; }

        public int RequestTypeId { get; set; }
        public string rPhonenumber { get; set; }
    }

    public class CaseTag
    {
        public int CaseTagId { get; set; }
        public string Name { get; set; }
    }

}
