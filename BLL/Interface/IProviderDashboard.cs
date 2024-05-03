using DAL.Models;
using DAL.ViewModel;
using DAL.ViewModelProvider;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IProviderDashboard
    {
        //this is for user cannot open another request that nor exist or that not asign to him/her
        bool checkphysician(int phyId, int requestId);
        ProviderProfileVm getProviderDetails(int id, int physicianId);
        List<RequestDataProvider> getRequestDataForProvider(string statusarray, int reqTypeId, string searchdata, int phyId);

        IQueryable<Request> forCountRequest(int phyId);

        List<ShiftDetail> getParticularScheduleData(int PhysicianId);

        List<Region> getRegions();

        //for my profile 
        bool providerResetPassword(string password, int physicianId);

        //send request via mail to edit profile
        Physician getPhysician(ProviderProfileVm model);
        AspNetUser getEmailByCreatedBy(int createdBy);
        void createShiftPost(ViewShiftVm model, int aspId, List<int> WeekDaysList);
        bool checkshiftExistsForPhysician(int physicianId, DateOnly shiftdate, TimeOnly starttime, TimeOnly endtime);

        void editViewNotes(ViewNotesVm model, int requestId);
        void acceptRequest(int requestId);
        Request getRequestData(int requestId);

        void transferCasePost(AdminAsignVm model, int newStatus);
        void orderDataStore(AdminOrderVm model, int requestID);

        bool typeOfCare(EncounterVm model, string care);

        bool houseCall(int requestId);  
        void postEncounterData(EncounterVm model, int encounterbtn);
        bool CreateNewReq(AdminCreateRequestVm model, int PhysicianId);

        bool aspNetUserCheck(string userData);
        List<RequestWiseFile> getFilesByRequestId(int requestId);
        bool concludeCareDelete(int documentId);
        void concludeCarePost(AdminViewUploadVm model);

        Encounter getEncounterDataByRequestId(int requestId);
        string getPatientNameForConclude(int requestId);

        bool concludeCareUpload(IFormFile file, int requestId, int physicianId);

        Task<TimeSheetModel> getFinalizeTimeSheetDataAsync(int PhysicianId, string SelectedValue);

        void SubmitBiWeeklyTimesheet(TimeSheetModel model, bool isFinalize, int? physicianId);

        Task<TimeSheetModel> getInvoicingTableDataAsync(int PhysicianId, string SelectedValue);
    }
}
