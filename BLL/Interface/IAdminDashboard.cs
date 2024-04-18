using DAL.Models;
using DAL.ViewModel;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Region = DAL.Models.Region;

namespace BLL.Interface
{
    public interface IAdminDashboard
    {
        List<RequestListAdminDash> requestDataAdmin(string statusarray, int[] Status, string reqTypeId, int regionId,string searchdata);

        //this is for download excel for all request
        List<RequestListAdminDash> requestDataDownloadExcelAll();


        ViewCaseVm ViewCase(int requestId);

        ViewNotesVm ViewNotes(int requestId);

        void editViewNotes(ViewNotesVm model, int requestId);

        void cancelCase(AdminDashboardViewModel model, int requestId);

        List<DAL.ViewModel.CaseTag> cancelCaseMain();

        List<Region> asignCase();

        List<Physician> asignPhysician(int regionId);

        void blockCase(AdminBlockVm model, int regionId);
        AdminBlockVm adminBlockVm(int RequestId);

        void asignCasePost(AdminAsignVm model, int requestId);

        //view upload section 
        void UploadFile(int requestId, string fileName);

        AdminViewUploadVm GetAdminViewUploadData(AdminViewUploadVm model,int requestId);

        List<RequestWiseFile> GetFilesByRequestId(int requestId);
        List<RequestWiseFile> GetAllFilesByRequestId(int reqId);

        RequestWiseFile GetFileById(int fileId);

        //void DeleteFile(int RequestWiseFileId);

        //order case dropdown and all
        List<HealthProfessionalType> healthProfessionalTypes();
        List<HealthProfessional> asignBusiness(int healthProfessionId);
        List<HealthProfessional> getVendorDetails(int vendorId);
        void orderDataStore(AdminOrderVm model, int requestID);

        //transfer case all methods
        List<Region> transferRegion();
        List<Physician> transferPhysician(int regionId);
        void transferCasePost(AdminAsignVm model, int newStatus);

        //export data 
        byte[] ExportToExcel(List<RequestListAdminDash> data);

        //clear case 
        void clearCasePost(AdminClearVm model);

        //review agreement and send agreement
        bool checkStatus(ReviewAgreementVm model);
        void reviewAgreeementSubmit(ReviewAgreementVm model);
        void reviewAgreementCancel(ReviewAgreementVm model);

        //close case
        CloseCaseVm closeCaseGet(int requestId);
        void closeCaseEdit(int command, CloseCaseVm model, int requestId);

        // this part is for adminprofile
        AdminProfileVm getAdminDetails(int aspId, int adminId);

        void adminResetPassword(AdminProfileVm model);

        void adminEditDetails1(AdminProfileVm model, List<int>? checkboxForAll, int adminId);

        void adminEditDetails2(AdminProfileVm model);


        //this part is for encounter form
        EncounterVm encounterFormGetData(int reqId);
        void postEncounterData(EncounterVm model);

        List<Region> getRegions();


        //this part is for provider that accessible by admin and it is in nav bar 
        List<Provider> getPhysicianDetails(string regionId);

        //this part is for edit provider details
        ProviderVm getPhysicianDetails(int physicianId);

        void editPhysicianPassword(ProviderVm model);
        void physicianEditDetails1(ProviderVm model, List<int>? checkboxForAll);
        void physicianEditDetails2(ProviderVm model);
        void physicianEditDetails3(ProviderVm model);
        //void changeCheckBox(int physicianId, int checkbox);
        void updateProviderDoc(ProviderVm model);

        //this is for create request by admin
        bool CreateNewReq(AdminCreateRequestVm model, int PhysicianId);

        //this is for send mail to provider
        ProviderVm getPhysicianDetailsByPhysicianId(int phyId);

		void adminSendMailToProvider(ProviderVm model, string subject, string body,string ContactType);

        //create account for physician by admin
        List<Role> getRoles();
        void createProviderAccount(ProviderVm model, List<int>? checkboxForAll);

        //this part is for access role
        List<Menu> getMenu();
        List<AspNetRole> getAspNetRoles();
        List<Access> getAccessRoles();

        List<Menu> getAccess(int id);

        void createRole(AccessVm model, List<int> checkboxForAllRole);

        AccessVm editAccessRole(int RoleId, int AccountType);

        void editAccessRolePost(AccessVm model, List<int>? checkboxForAllRole);
        void deleteAccessRole(int RoleId);

        List<UserAccess> getUserAccessData(int RoleId);

        void adminCreateAccount(AdminProfileVm model, List<int>? checkboxForAllRegion);

        //this part is for vendor 
        List<Vendor> getVendorList(int Id, string searchvalue);
        List<HealthProfessionalType> getProfessionList();
        Vendor updateBusiness(int vendorId);

        void partnersDeleteVendors(int vendorId);
        void updateBusinessPost(Vendor model);

        //add business in partners
        void addBusiness(PartnersVm model);

        //this part is for records
        List<PatientHistory> patientHistoryData(string firstname,string lastname, string email, string phonenumber);

        List<PatientRecordExplore> patientRecordExploreData(int patientId);


        List<EmailLogs> emailLogData(int roleId, string receiverName, string email,DateOnly createdDate, DateOnly sentDate);
        List<PhysicianLocation> getPhysicianLocation();

        List<BlockHistory> blockHistoryData(string patientname, DateOnly date, string email, string phonenumber, int PageNumber);

        List<SmsLogs> smsLogData(int roleId, string receiverName, string phoneNumber, DateOnly createdDate, DateOnly sendData);
        void unblockRequest(int requestId, int blockReqId);

		//search record 
		List<SearchRecord> searchRecordData(string patietnname,int stauts, int requesttype, string email, DateOnly fromdate, DateOnly todate,string providername, string phoneNumber, int pageNumber);


        //this part is for scheduling
        List<Physician> fetchRegionWiseProviders(int region);
        List<ShiftDetail> GetScheduleData();

        ViewShiftVm viewShiftGetData(int ShiftDetailId);

        void viewShiftPost(ViewShiftVm model, int command);
        List<Physician> getPhysicianByRegion(int regionId);

        void createShiftPost(ViewShiftVm model, int aspId, List<int> WeekDaysList);
        bool checkshiftExistsForPhysician(int physicianId, DateOnly shiftdate, TimeOnly starttime, TimeOnly endtime);

        List<Provider> getPhysicianList(int regionId);

        List<RequestedShift> requestedShiftData(int regionId);

        bool deleteSelectedShift(List<int> shiftDetailId);

        bool approveSelectedShift(List<int> shiftDetailId);

    }
}
