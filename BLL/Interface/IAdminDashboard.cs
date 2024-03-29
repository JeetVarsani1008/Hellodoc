﻿using DAL.Models;
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
        List<RequestListAdminDash> requestDataAdmin(string statusarray, int[] Status, string reqTypeId, int regionId);

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

        void asignCasePost(AdminAsignVm model, int requestId, int newStatus);

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
    }
}
