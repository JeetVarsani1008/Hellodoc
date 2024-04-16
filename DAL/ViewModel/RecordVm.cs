using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
	public class RecordVm
	{
		public List<PatientHistory> patientHistory {  get; set; }

		public List<EmailLogs> emailLog { get; set; }
		public List<Role> roles { get; set; }
		public List<BlockHistory> blockHistory { get; set; }
		public List<SearchRecord> searchRecord { get; set; }

		public List<SmsLogs> smsLog { get; set; }

		public int? TotalPages { get; set; }
		public int? skipCount;
		public int? CurrentPage;
		public int? pageSize {  get; set; }
		public PageVm Page { get; set; }

		public List<PatientRecordExplore> patientRecordExplores { get; set; }
	}

	#region PatientHistory
	public class PatientHistory
	{

		
		public int PatientId { get; set; }
		public string FirstName { get; set;}

		public string LastName { get; set;}
		public string Email { get; set;}	
		public string Phone { get; set;}
		public string Address { get; set;}

	}
	#endregion

	#region EmailLogs
	public class EmailLogs
	{
		public int EmailLogId { get; set; }
		public string Recipient { get; set; }
		public int? Action { get; set; }

		public string RoleName { get; set; }
		public string Email { get; set; }

		public DateOnly? CreatedDate { get; set; }
		public DateOnly? SentDate { get; set; }

		public int? SentTries { get; set; }

		public string? ConfirmationNumber { get; set; }
		public bool Sent { get; set; }
	}
	#endregion

	#region BlockHistory
	public class BlockHistory
	{
		public int BlockRequestId { get; set; }
		public int RequestId { get; set; }
		public string PatientName { get; set; }

		public string Email { get; set; }	

		public string PhoneNumber { get; set; }
		public DateOnly? CreatedDate { get; set; }

		public string Notes { get; set; }

		public bool? IsActive { get; set; }

	}
	#endregion

	#region SmsLogs 
	public class SmsLogs
	{
		public int SmsLogId { get; set; }
		public string Recipient { get; set; }
		public int? Action { get; set; }

		public string RoleName { get; set; }
		public string MobileNumber { get; set; }

		public DateOnly CreatedDate { get; set; }
		public DateOnly? SentDate { get; set; }

		public int? SentTries { get; set; }

		public string? ConfirmationNumber { get; set; }
		public bool Sent { get; set; }
	}
	#endregion

	#region SearchRecord
	public class SearchRecord
	{
		public int RequestId { get; set; }
		public string PatientName { get; set; }

		public string Requestor { get; set; }
		public DateOnly DateOfService { get; set; }
		public DateOnly? CloseCaseDate { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Address { get; set; }

		public string Zip { get; set; }

		public string PatientNotes { get; set; }
		public string AdminNotes { get; set; }
		public string RequestStatus { get; set; }
		public string Physician { get; set; }
		public string PhysicianNotes { get; set; }
	}
	#endregion

	#region PatientRecordExplore
	public class PatientRecordExplore
	{
		public string Client {  get; set; }

		public string CreatedDate { get; set; }

		public string Confirmation { get; set; }

		public string ProviderName { get; set; }

		public string ConcludedDate { get; set; }

		public string Status { get; set; }

	}
	#endregion
}
