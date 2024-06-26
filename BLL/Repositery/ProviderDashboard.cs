﻿using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using DAL.ViewModelProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repositery
{
    public class ProviderDashboard : IProviderDashboard
    {
        private readonly HellodocContext _context;
        public ProviderDashboard(HellodocContext context)
        {
            _context = context;
        }

        #region checkphysician
        public bool checkphysician(int phyId,int requestId)
        {
            var req = _context.Requests.Any(x => x.RequestId == requestId && x.PhysicianId == phyId);
            if (req)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region getRequestDataForProvider
        public List<RequestDataProvider> getRequestDataForProvider(string statusarray, int reqTypeId, string searchdata, int phyId)
        {
            var list = _context.Requests.Where(o => statusarray.Contains(o.Status.ToString()) && o.PhysicianId == phyId);

            if (searchdata != null)
            {
                searchdata = searchdata.ToLower();
                list = list.Where(x => x.RequestClients.Select(j => j.FirstName).First().ToLower().Contains(searchdata));
            }
            if (reqTypeId != 0 )
            {
                list = list.Where(o => o.RequestTypeId == reqTypeId);
            }

            var data = list.Select(x => new RequestDataProvider()
            {
                Email = x.RequestClients.Select(x => x.Email).First(),
                DateOfBirth = x.RequestClients.Select(x => x.IntDate).First() == null ? null : x.RequestClients.Select(x => x.IntDate).First() + "/" + x.RequestClients.Select(x => x.StrMonth).First() + "/" + x.RequestClients.Select(x => x.IntYear).First(),
                RequestId = x.RequestId,
                RequestTypeId = x.RequestTypeId,
                Name = x.RequestClients.Select(j => j.FirstName).First() + " " + x.RequestClients.Select(h => h.LastName).First(),
                Phone = x.PhoneNumber,
                Address = x.RequestClients.Select(x => x.Street).First() + "," + x.RequestClients.Select(x => x.City).First() + "," + x.RequestClients.Select(x => x.State).First(),
                Status = "1",
                StatusForDash = x.Status,
                IsFinalize = x.Encounters.Select(x => x.IsFinalize).First() ?? false,
            }).ToList();

            return data;
        }
        #endregion

        #region forCountRequest
        public IQueryable<Request> forCountRequest(int phyId)
        {
            var data = _context.Requests.Where(x => x.PhysicianId == phyId);
            return data;
        }
        #endregion

        #region getProviderDetails : for my profile
        public ProviderProfileVm getProviderDetails(int id, int physicianId)
        {
            List<Region> region = _context.Regions.ToList();    
            List<PhysicianRegion> physicianRegion = _context.PhysicianRegions.Where(x => x.PhysicianId == physicianId).ToList();

            var phy = _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId);
            var aspdata = _context.AspNetUsers.FirstOrDefault(x => x.Id == id);
            ProviderProfileVm providerProfileVm = new ProviderProfileVm
            {
                PhysicianId = physicianId,
                UserName = aspdata.UserName,
                FirstName = phy.FirstName,
                LastName = phy.LastName,
                Email = phy.Email,
                Mobile = phy.Mobile,
                MedicalLicense = phy.MedicalLicense,
                NPINumber = phy.Npinumber,
                Address1 = phy.Address1,
                Address2 = phy.Address2,
                City = phy.City,
                State = _context.Regions.FirstOrDefault(x => x.RegionId == phy.RegionId).Name,
                Zip = phy.Zip,
                AlternateMobile = phy.AltPhone,
                BusinessName = phy.BusinessName,    
                BusinessWebSite = phy.BusinessWebsite,
                Photo = phy.Photo,
                Signature = phy.Signature,
                regions = region,
                PhysicianRegions = physicianRegion,
            };
            return providerProfileVm;
        }
        #endregion

        #region getParticularScheduleData
        public List<ShiftDetail> getParticularScheduleData(int PhysicianId)
        {
            try
            {
                return _context.ShiftDetails.Include(m => m.Shift).ThenInclude(j => j.Physician).Where(m => m.IsDeleted == false && m.Shift.PhysicianId == PhysicianId).ToList();
            }
            catch { return new List<ShiftDetail> { }; }
        }
        #endregion

        #region getRegions 
        public List<Region> getRegions()
        {
            var data = _context.Regions.ToList();
            return data;
        }
        #endregion

        #region providerResetPassword
        public bool providerResetPassword(string password, int physicianId)
        {
            var aspId = _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId).AspNetUserId;
            var passdata = _context.AspNetUsers.FirstOrDefault(x => x.Id == aspId);

            passdata.PasswordHash = password;
            _context.SaveChanges();
            return true;
        }
        #endregion

        #region getPhysician
        public Physician getPhysician(ProviderProfileVm model)
        {
            var data = _context.Physicians.FirstOrDefault(x => x.PhysicianId == model.PhysicianId);
            return data;
        }
        #endregion

        #region getEmailByCreatedBy
        public AspNetUser getEmailByCreatedBy(int createdBy)
        {
            var data = _context.AspNetUsers.FirstOrDefault(x => x.Id == createdBy);
            return data;
        }
        #endregion

        #region createShiftPost
        public void createShiftPost(ViewShiftVm model, int aspId, List<int> WeekDaysList)
        {
            Shift shift = new Shift()
            {
                PhysicianId = model.PhysicianId,
                StartDate = model.ShiftDate,
                RepeatUpto = model.RepeatUpto,
                CreatedDate = DateTime.Now,
                CreatedBy = aspId,
                IsRepeat = model.IsRepeat,

            };
            _context.Shifts.Add(shift);
            _context.SaveChanges();

            ShiftDetail shiftDetail = new ShiftDetail()
            {
                ShiftId = shift.ShiftId,
                ShiftDate = model.ShiftDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Status = 0,
                IsDeleted = false,
                RegionId = model.RegionId,

            };
            _context.ShiftDetails.Add(shiftDetail);
            _context.SaveChanges();


            for (int i = 0; i < model.RepeatUpto; i++)
            {
                DateOnly shiftDate = model.ShiftDate.AddDays(7 * i);
                foreach (DayOfWeek dow in WeekDaysList)
                {
                    ShiftDetail shiftDetail1 = new ShiftDetail();
                    shiftDetail1.ShiftId = shift.ShiftId;
                    shiftDetail1.ShiftDate = dow - shiftDate.DayOfWeek > 0 ? shiftDate.AddDays(dow - model.ShiftDate.DayOfWeek) : shiftDate.AddDays(7 + dow - model.ShiftDate.DayOfWeek);
                    shiftDetail1.RegionId = model.RegionId;
                    shiftDetail1.StartTime = model.StartTime;
                    shiftDetail1.EndTime = model.EndTime;
                    shiftDetail1.Status = 0;
                    shiftDetail1.IsDeleted = false;
                    _context.ShiftDetails.Add(shiftDetail1);
                    _context.SaveChanges();
                }
            }
        }
        #endregion

        #region checkshiftExistsForPhysician
        public bool checkshiftExistsForPhysician(int physicianId, DateOnly shiftdate, TimeOnly starttime, TimeOnly endtime)
        {
            var data = _context.ShiftDetails.Any(x => x.Shift.PhysicianId == physicianId && (x.ShiftDate == shiftdate) && (x.StartTime <= starttime && x.EndTime >= starttime));

            if (data)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region editViewNotes
        public void editViewNotes(ViewNotesVm model, int requestId)
        {
            var reqnotes = _context.RequestNotes.FirstOrDefault(x => x.RequestId == requestId);
            var req = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            if (reqnotes != null)
            {
                reqnotes.PhysicianNotes = model.PhysicianNotes;
                reqnotes.ModifiedBy = (int)_context.Users.FirstOrDefault(x => x.UserId == req.UserId).AspNetUserId;
                reqnotes.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
            }

        }
        #endregion

        #region acceptRequest
        public void acceptRequest(int requestId)
        {
            var data = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            data.Status = 2;
            data.AcceptedDate = DateTime.Now;
            _context.SaveChanges();
        }
        #endregion

        #region getRequestData
        public Request getRequestData(int requestId)
        {
            var data = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            return data;
        }
        #endregion

        #region transferCasePost
        public void transferCasePost(AdminAsignVm model, int newStatus)
        {
            var request = _context.Requests.FirstOrDefault(x => x.RequestId == model.RequestId);
            request.ModifiedDate = DateTime.Now;
            request.PhysicianId = null;
            request.Status = 1;
            _context.SaveChanges();

            var reqstatus = new RequestStatusLog
            {
                RequestId = model.RequestId,
                Status = 1,
                Notes = model.Description,
                CreatedDate = DateTime.Now,
                PhysicianId = model.PhysicianId,
                TransToAdmin = new BitArray(1, true),
            };
            _context.RequestStatusLogs.Add(reqstatus);
            _context.SaveChanges();

        }
        #endregion

        #region orderDataStore
        public void orderDataStore(AdminOrderVm model, int requestId)
        {

            OrderDetail orderDetail = new OrderDetail
            {
                VendorId = model.VendorId,
                RequestId = requestId,
                FaxNumber = model.FaxNumber,
                Email = model.Email,
                BusinessContact = model.BusinessContact,
                Prescription = model.Prescription,
                NoOfRefill = model.Refill,
                CreatedDate = DateTime.Now,
                CreatedBy = "Physician",
            };
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }
        #endregion

        #region typeOfCare
        public bool typeOfCare(EncounterVm model, string care)
        {
            if(care == "Housecall")
            {
                var data = _context.Requests.FirstOrDefault(x => x.RequestId == model.RequestId);
                data.Status = 5;
                _context.SaveChanges();
                return true;
            }
            else if(care == "Consult")
            {
                var data = _context.Requests.FirstOrDefault(x => x.RequestId == model.RequestId);
                data.Status = 6;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region houseCall 
        public bool houseCall(int requestId)
        {
            if(requestId != 0)
            {
                var data = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
                data.Status = 6;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region postEncounterData
        public void postEncounterData(EncounterVm model,int encounterbtn)
        {
            if(encounterbtn == 1)
            {
                var data = _context.Encounters.FirstOrDefault(x => x.RequestId == model.RequestId);
                if (data != null)
                {
                    data.MedicalHistory = model.MedicalHistory;
                    data.Medications = model.Medications;
                    data.Allergies = model.Allergies;
                    data.Temp = model.Temp;
                    data.Hr = model.Hr;
                    data.Rr = model.Rr;
                    data.BloodPressureS = model.BloodPressureS;
                    data.BloodPressureD = model.BloodPressureD;
                    data.O2 = model.O2;
                    data.Pain = model.Pain;
                    data.Heent = model.Heent;
                    data.Cv = model.Cv;
                    data.Chest = model.Chest;
                    data.Abd = model.Abd;
                    data.Extr = model.Extr;
                    data.Skin = model.Skin;
                    data.Neuro = model.Neuro;
                    data.Other = model.Other;
                    data.Diagnosis = model.Diagnosis;
                    data.TreatmentPlan = model.TreatmentPlan;
                    data.MedicationsDispensed = model.MedicationsDispensed;
                    data.Procedures = model.Procedures;
                    data.Followup = model.FollowUp;
                }
                _context.SaveChanges();
            }
            else
            {
                var data = _context.Encounters.FirstOrDefault(x => x.RequestId == model.RequestId);
                data.IsFinalize = true;
                _context.SaveChanges();
            }
        }
        #endregion

        #region CreateNewReq
        public bool CreateNewReq(AdminCreateRequestVm model, int PhysicianId)
        {
            User user1 = _context.Users.FirstOrDefault(x => x.Email == model.Email);
            var physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == PhysicianId);


            string abbreviation;
            if (model.RegionId == 1)
            {
                abbreviation = "GJ";
            }
            else if (model.RegionId == 2)
            {
                abbreviation = "RJ";
            }
            else if (model.RegionId == 3)
            {
                abbreviation = "PN";
            }
            else
            {
                abbreviation = "GO";
            }

            Request request = new Request();
            RequestClient requestClient = new RequestClient();
            var st = _context.Regions.FirstOrDefault(u => u.RegionId == model.RegionId);

            request.FirstName = model.FirstName;
            request.LastName = model.LastName;
            request.PhoneNumber = model.Phone;
            request.Email = model.Email;
            request.Status = 1;
            request.CreatedDate = DateTime.Now;
            request.RequestTypeId = 2;
            request.PhysicianId = PhysicianId;
            _context.Requests.Add(request);
            _context.SaveChanges();


            requestClient.RequestId = request.RequestId;
            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.Email = model.Email;
            requestClient.Notes = model.Symptoms;
            requestClient.Street = model.Street;
            requestClient.City = model.City;
            requestClient.State = st.Name;
            requestClient.RegionId = st.RegionId;
            requestClient.ZipCode = model.ZipCode;
            requestClient.Address = model.Street + " " + model.City + " " + st.Name + " " + model.ZipCode;
            requestClient.PhoneNumber = request.PhoneNumber;
            requestClient.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(model.DateOfBirth.Month);
            requestClient.IntYear = model.DateOfBirth.Year;
            requestClient.IntDate = model.DateOfBirth.Day;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            if (user1 != null)
            {
                request.UserId = user1.UserId;
                _context.SaveChanges();
                return false;
            }
            else
            {
                AspNetUser aspNetUser = new AspNetUser();
                User user = new User();
                AspNetUserRole aspNetUserRole = new AspNetUserRole();



                aspNetUser.UserName = model.FirstName + model.LastName;
                aspNetUser.Email = model.Email;
                aspNetUser.Phonenumber = model.Phone;
                aspNetUser.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(aspNetUser);
                _context.SaveChanges();


                user.AspNetUserId = aspNetUser.Id;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Mobile = model.Phone;
                user.Street = model.Street;
                user.City = model.City;
                user.State = st.Name;
                user.RegionId = st.RegionId;
                user.ZipCode = model.ZipCode;
                user.CreatedBy = physician.PhysicianId.ToString();
                user.CreatedDate = DateTime.Now;
                user.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(model.DateOfBirth.Month);
                user.IntDate = model.DateOfBirth.Day;
                user.IntYear = model.DateOfBirth.Year;

                _context.Users.Add(user);
                _context.SaveChanges();

                aspNetUserRole.UserId = aspNetUser.Id;
                aspNetUserRole.RoleId = 2;

                _context.AspNetUserRoles.Add(aspNetUserRole);
                _context.SaveChanges();


                request.UserId = user.UserId;
                request.CreatedUserId = user.UserId;
                request.PatientAccountId = user.AspNetUserId.ToString();
                var numberString = request.RequestId.ToString();
                var paddedNumberString = numberString.PadLeft(4, '0');
                var ConfirmationNumber = abbreviation + DateTime.Now.ToString("dd") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("yy") + model.FirstName.ToUpper().Substring(0, 2) + model.LastName.ToUpper().Substring(0, 2) + paddedNumberString;
                request.ConfirmationNumber = ConfirmationNumber;
                request.UserId = user.UserId;
                _context.SaveChanges();

                return true;
            }
        }
        #endregion

        #region aspNetUserCheck
        public bool aspNetUserCheck(string userData)
        {
            var data = _context.AspNetUsers.FirstOrDefault(x => x.Email == userData);
            if(data != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        //for conclude 
        #region getFilesByRequestId
        public List<RequestWiseFile> getFilesByRequestId(int requestId)
        {
            var data = _context.RequestWiseFiles.Where(x => x.RequestId == requestId).ToList().Where( x=> x.IsDeleted == null || !x.IsDeleted[0]).ToList(); return data;
        }
        #endregion

        #region concludeCareDelete
        public bool concludeCareDelete(int documentId)
        {
            //BitArray bit = new BitArray(1,true);
            RequestWiseFile requestWiseFile = _context.RequestWiseFiles.First(x => x.RequestWiseFileId == documentId);
            if(requestWiseFile != null)
            {
                requestWiseFile.IsDeleted = new BitArray(1, true);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region concludeCarePost 
        public void concludeCarePost(AdminViewUploadVm model)
        {
            var data = _context.Requests.FirstOrDefault(x => x.RequestId == model.RequestId);
            data.Status = 8;
            _context.SaveChanges();
        }
        #endregion

        #region getEncounterDataByRequestId
        public Encounter getEncounterDataByRequestId(int requestId)
        {
            var data = _context.Encounters.FirstOrDefault(x => x.RequestId == requestId);
            return data;
        }
        #endregion

        #region getPatientNameForConclude
        public string getPatientNameForConclude(int requestId)
        {
            var data = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);
            var name = "";
            if(data != null)
            {
                name = data.FirstName + " " + data.LastName;
            }
            return name;
        }
        #endregion

        #region concludeCareUpload
        public bool concludeCareUpload(IFormFile file, int requestId, int physicianId)
        {
            var data = _context.Requests.Any(x => x.RequestId == requestId);
            if (data)
            {
                RequestWiseFile requestWiseFile = new RequestWiseFile();
                requestWiseFile.RequestId = requestId;
                requestWiseFile.FileName = file.FileName;
                requestWiseFile.CreatedDate = DateTime.Now;
                requestWiseFile.IsDeleted = new BitArray(1,false);
                requestWiseFile.PhysicianId = physicianId;
                _context.RequestWiseFiles.Add(requestWiseFile);
                _context.SaveChanges();     
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        //#region getFinalizeTimeSheetData 
        //public async Task<TimeSheetModel> getFinalizeTimeSheetDataAsync(int PhysicianId, string SelectedValue)
        //{
        //    TimeSheetModel finalizeVm = new TimeSheetModel();
        //    string[] dateRange = SelectedValue.Split('*');
        //    finalizeVm.SelectedStartDate = DateOnly.Parse(dateRange[0]);
        //    finalizeVm.SelectedEndDate = DateOnly.Parse(dateRange[1]);

        //    WeeklyTimeSheet weeklyTimesheet = await _context.WeeklyTimeSheets.FirstOrDefaultAsync(x => x.StartDate == finalizeVm.SelectedStartDate && x.ProviderId == PhysicianId);


        //    if(weeklyTimesheet != null)
        //    {
        //        var sheet = await _context.WeeklyTimeSheetDetails.Where(u => u.TimeSheetId == weeklyTimesheet.TimeSheetId).OrderBy(x => x.Date).Select(x => new BiWeeklyTimeSheetModel
        //        {
        //            Date = x.Date,
        //            OnCallHours = (int)x.OnCallHours,
        //            TotalHours = (int)x.TotalHours,
        //            IsWeekend = (bool)x.IsWeekendHoliday,
        //            NoOfHouseCalls = x.HouseCall,
        //            NoOfPhoneConsults = x.PhoneConsult,
        //            NightWeekendHouseCall = x.HouseCallNightWeekend,
        //            NightWeekendPhoneConsult = x.PhoneConsultNightWeekend,
        //            Item = x.Item,
        //            Amount = (int)x.Amount,
        //            BillName = x.Bill,
        //            WeeklyTimeSheetId = x.TimeSheetDetailId,

        //        }).ToListAsync();
        //        List<BiWeeklyTimeSheetModel> list = new List<BiWeeklyTimeSheetModel>();
        //        foreach(BiWeeklyTimeSheetModel i in sheet)
        //        {
        //            var shift = await _context.Shifts.FirstOrDefaultAsync(x => x.PhysicianId == PhysicianId);
        //            var shiftdetail = await _context.ShiftDetails.FirstOrDefaultAsync(x => x.ShiftId == shift.ShiftId && x.ShiftDate == i.Date);

        //            if (shiftdetail != null)
        //            {
        //                i.OnCallHours = (int)(shiftdetail.EndTime - shiftdetail.StartTime).TotalMinutes / 60;
        //            }
        //            else
        //            {
        //                i.OnCallHours = 0;
        //            }
        //            list.Add(i);
        //        }
        //        finalizeVm.TimeSheetList = list;
        //        return finalizeVm;
        //    }

        //    else
        //    {
        //        List<BiWeeklyTimeSheetModel> list = new List<BiWeeklyTimeSheetModel>();
        //        for(var i = finalizeVm.SelectedStartDate; i<= finalizeVm.SelectedEndDate; i = i.AddDays(1))
        //        {
        //            var shift = _context.Shifts.FirstOrDefault(x => x.PhysicianId == PhysicianId);
        //            var shiftdetail = _context.ShiftDetails.FirstOrDefault(x => x.ShiftId == shift.ShiftId && x.ShiftDate == i);

        //            BiWeeklyTimeSheetModel ListOfDates = new BiWeeklyTimeSheetModel();
        //            ListOfDates.Date = i;
        //            if(shiftdetail != null)
        //            {
        //                ListOfDates.OnCallHours = (int)(shiftdetail.EndTime - shiftdetail.StartTime).TotalMinutes / 60;
        //                ListOfDates.OnCallHours = ListOfDates.OnCallHours;
        //            }
        //            else
        //            {
        //                ListOfDates.OnCallHours = 0;
        //                ListOfDates.TotalHours = 0;
        //            }
        //            list.Add(ListOfDates);
        //        }
        //        finalizeVm.TimeSheetList = list;
        //        return finalizeVm;
        //    }
        //}
        //#endregion

        //#region SubmitBiWeeklyTimesheet
        //public void SubmitBiWeeklyTimesheet(TimeSheetModel model, bool isFinalize, int? physicianId)
        //{
        //    WeeklyTimeSheet wts = _context.WeeklyTimeSheets.FirstOrDefault(u => u.StartDate == model.TimeSheetList[0].Date && u.ProviderId == physicianId);

        //    int count = model.TimeSheetList.Count;

        //    if (wts == null)
        //    {
        //        WeeklyTimeSheet newSheet = new WeeklyTimeSheet();
        //        newSheet.StartDate = model.TimeSheetList[0].Date;
        //        newSheet.EndDate = model.TimeSheetList[0].Date.AddDays(count - 1);
        //        newSheet.Status = 1;
        //        newSheet.CreatedDate = DateTime.Now;
        //        newSheet.ProviderId = (int)physicianId;
        //        if (isFinalize)
        //            newSheet.IsFinalized = true;

        //        _context.WeeklyTimeSheets.Add(newSheet);
        //        _context.SaveChanges();

        //        for (int i = 0; i < count; i++)
        //        {
        //            WeeklyTimeSheetDetail detail = new WeeklyTimeSheetDetail();
        //            detail.TimeSheetId = newSheet.TimeSheetId;
        //            detail.Date = model.TimeSheetList[i].Date;
        //            detail.TotalHours = model.TimeSheetList[i].TotalHours;
        //            detail.IsWeekendHoliday = model.TimeSheetList[i].IsWeekend;
        //            if (model.TimeSheetList[i].IsWeekend)
        //            {
        //                detail.HouseCallNightWeekend = model.TimeSheetList[i].NoOfHouseCalls;
        //                detail.PhoneConsultNightWeekend = model.TimeSheetList[i].NoOfPhoneConsults;
        //            }
        //            else
        //            {
        //                detail.HouseCall = model.TimeSheetList[i].NoOfHouseCalls;
        //                detail.PhoneConsult = model.TimeSheetList[i].NoOfPhoneConsults;
        //            }
        //            detail.Item = model.TimeSheetList[i].Item;
        //            detail.Amount = model.TimeSheetList[i].Amount;

        //            if (model.TimeSheetList[i].Bill != null)
        //            {
        //                detail.Bill = model.TimeSheetList[i].Bill.FileName;

        //                var filePath = Path.Combine("wwwroot", "InvoicingDocs", physicianId.ToString() + Path.GetFileName(model.TimeSheetList[i].Bill.FileName));
        //                using (FileStream stream = System.IO.File.Create(filePath))
        //                {
        //                    model.TimeSheetList[i].Bill.CopyToAsync(stream);
        //                }
        //            }
        //            _context.WeeklyTimeSheetDetails.Add(detail);
        //            _context.SaveChanges();
        //        }
        //    }
        //    else
        //    {
        //        for(int i = 0; i < count; i++)
        //        {

        //            //WeeklyTimeSheetDetail detail = new WeeklyTimeSheetDetail();
        //            var detail = _context.WeeklyTimeSheetDetails.FirstOrDefault(x => x.TimeSheetDetailId == model.TimeSheetList[i].WeeklyTimeSheetId);
        //            if (model.TimeSheetList[i].Bill != null)
        //            {
        //                detail.Bill = model.TimeSheetList[i].Bill.FileName;

        //                var filePath = Path.Combine("wwwroot", "InvoicingDocs", physicianId.ToString() + Path.GetFileName(model.TimeSheetList[i].Bill.FileName));
        //                using (FileStream stream = System.IO.File.Create(filePath))
        //                {
        //                    model.TimeSheetList[i].Bill.CopyToAsync(stream);
        //                }
        //                //_context.WeeklyTimeSheetDetails.Add(detail);
        //                _context.SaveChanges();
        //            }
        //        }
        //    }
        //}
        //#endregion

        //#region getInvoicingTableDataAsync
        //public async Task<TimeSheetModel> getInvoicingTableDataAsync(int PhysicianId, string SelectedValue)
        //{
        //    TimeSheetModel timeSheetModel = new TimeSheetModel();
        //    string[] dateRange = SelectedValue.Split('*');
        //    timeSheetModel.SelectedStartDate = DateOnly.Parse(dateRange[0]);
        //    timeSheetModel.SelectedEndDate = DateOnly.Parse(dateRange[1]);

        //    WeeklyTimeSheet weeklyTimesheet = await _context.WeeklyTimeSheets.FirstOrDefaultAsync(x => x.StartDate == timeSheetModel.SelectedStartDate && x.ProviderId == PhysicianId);

        //    if (weeklyTimesheet == null)
        //    {
        //        timeSheetModel.CheckData = 1;
        //        return timeSheetModel;
        //    }
        //    else
        //    {
        //        var sheet = await _context.WeeklyTimeSheetDetails.Where(u => u.TimeSheetId == weeklyTimesheet.TimeSheetId).OrderBy(x => x.Date).Select(x => new BiWeeklyTimeSheetModel
        //        {
        //            Date = x.Date,
        //            OnCallHours = (int)x.OnCallHours,
        //            TotalHours = (int)x.TotalHours,
        //            IsWeekend = (bool)x.IsWeekendHoliday,
        //            NoOfHouseCalls = x.HouseCall,
        //            NoOfPhoneConsults = x.PhoneConsult,
        //            NightWeekendHouseCall = x.HouseCallNightWeekend,
        //            NightWeekendPhoneConsult = x.PhoneConsultNightWeekend,
        //            Item = x.Item,
        //            Amount = (int)x.Amount,
        //            BillName = x.Bill,
        //            NightShiftWeekend = x.NightShiftWeekend,
        //            NumberOfShift = x.NumberOfShifts,
        //        }).ToListAsync();

        //        List<BiWeeklyTimeSheetModel> list = new List<BiWeeklyTimeSheetModel>();
        //        foreach (BiWeeklyTimeSheetModel i in sheet)
        //        {
        //            var shift = await _context.Shifts.FirstOrDefaultAsync(x => x.PhysicianId == PhysicianId);
        //            var shiftdetail = await _context.ShiftDetails.FirstOrDefaultAsync(x => x.ShiftId == shift.ShiftId && x.ShiftDate == i.Date);

        //            if (shiftdetail != null)
        //            {
        //                i.OnCallHours = (int)(shiftdetail.EndTime - shiftdetail.StartTime).TotalMinutes / 60;
        //            }
        //            else
        //            {
        //                i.OnCallHours = 0;
        //            }
        //            list.Add(i);
        //        }
        //        timeSheetModel.TimeSheetList = list;
        //        return timeSheetModel;
        //    }
        //}
        //#endregion

        public TimeSheetModel GetTimePeriodList(int? ProviderId, string? selectedDate)
        {
            DateOnly currentDate = DateOnly.ParseExact("01-04-2024", "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            TimeSheetModel model = new TimeSheetModel();
            model.PhysicianList = _context.Physicians.Where(x => x.IsDeleted != true).ToList();
            model.SelectedPeriod = selectedDate;
            List<TimePeriodModel> periodlist = new List<TimePeriodModel>();
            for (int a = 1; a <= 6; a++)
            {
                int daysInMonth = currentDate.AddMonths(1).AddDays(-1).Day;

                DateOnly firstFourteenDatesStart = currentDate;
                DateOnly firstFourteenDatesEnd = currentDate.AddDays(13);

                DateOnly remainingDatesStart = firstFourteenDatesEnd.AddDays(1);

                // Add these dates to the Data list
                periodlist.Add(new TimePeriodModel
                {
                    StartDate = firstFourteenDatesStart,
                    EndDate = firstFourteenDatesEnd
                });

                periodlist.Add(new TimePeriodModel
                {
                    StartDate = remainingDatesStart,
                    EndDate = currentDate.AddDays(daysInMonth - 1)
                });

                currentDate = currentDate.AddMonths(1);
            }
            model.PeriodList = periodlist;

            if (selectedDate != null)
            {
                string[] dateRange = selectedDate.Split('*');
                model.SelectedStartDate = DateOnly.Parse(dateRange[0]);
                model.SelectedEndDate = DateOnly.Parse(dateRange[1]);
                WeeklyTimeSheet weeklyTimeSheet = _context.WeeklyTimeSheets.FirstOrDefault(u => u.StartDate == model.SelectedStartDate && u.ProviderId == ProviderId);
                if (weeklyTimeSheet != null)
                {
                    TimeSheetModel getTimesheet = GetBiweeklyDetails((int)ProviderId, selectedDate);
                    model.IsFinalized = weeklyTimeSheet.IsFinalized;
                    model.Status = weeklyTimeSheet.Status;
                    model.TimeSheetList = getTimesheet.TimeSheetList;
                    model.IsAnyBillPresent = getTimesheet.IsAnyBillPresent;
                }
            }
            return model;
        }

        public TimeSheetModel GetBiweeklyDetails(int ProviderId, string selectedDate)
        {
            TimeSheetModel model = new TimeSheetModel();
            model.SelectedPeriod = selectedDate;
            string[] dateRange = selectedDate.Split('*');
            model.SelectedStartDate = DateOnly.Parse(dateRange[0]);
            model.SelectedEndDate = DateOnly.Parse(dateRange[1]);

            var payrate = _context.PayRates.FirstOrDefault(x => x.PhysicianId == ProviderId);
            if (payrate != null)
            {
                model.Shift = payrate.Shift;
                model.NightShiftWeekend = payrate.NightShiftWeekend;
                model.HouseCalls = payrate.HouseCall;
                model.PhoneConsults = payrate.PhoneConsult;
                model.PhoneConsultsNightsWeekend = payrate.PhoneConsultNightWeekend;
                model.HouseCallsNightsWeekend = payrate.HouseCallNightWeekend;
            }
            else
            {
                model.NightShiftWeekend = 0;
                model.HouseCalls = 0;
                model.PhoneConsults = 0;
                model.PhoneConsultsNightsWeekend = 0;
                model.HouseCallsNightsWeekend = 0;
            }

            WeeklyTimeSheet weeklyTimeSheet = _context.WeeklyTimeSheets.FirstOrDefault(u => u.StartDate == model.SelectedStartDate && u.ProviderId == ProviderId);

            if (weeklyTimeSheet != null)
            {
                var listWeeklyTimeSheetDetail = _context.WeeklyTimeSheetDetails.Where(x => x.TimeSheetId == weeklyTimeSheet.TimeSheetId).ToList();
                model.IsAnyBillPresent = listWeeklyTimeSheetDetail.Any(x => x.Bill != null);

                var sheet = _context.WeeklyTimeSheetDetails.Where(u => u.TimeSheetId == weeklyTimeSheet.TimeSheetId).Select(p => new BiWeeklyTimeSheetModel
                {
                    Date = p.Date,
                    OnCallHours = p.OnCallHours,
                    TotalHours = p.TotalHours,
                    IsWeekend = (bool)p.IsWeekendHoliday,
                    NoOfHouseCalls = p.HouseCall,
                    NoOfPhoneConsults = p.PhoneConsult,
                    NightWeekendHouseCall = p.HouseCallNightWeekend,
                    NightWeekendPhoneConsult = p.PhoneConsultNightWeekend,
                    Item = p.Item,
                    Amount = (int)p.Amount,
                    BillName = p.Bill,

                }).ToList();

                int SumOfTotalHours = 0;
                int SumofWeekendHoliday = 0;
                int SumofHouseCalls = 0;
                int SumofPhoneConsult = 0;

                List<BiWeeklyTimeSheetModel> list = new List<BiWeeklyTimeSheetModel>();
                foreach (BiWeeklyTimeSheetModel p in sheet)
                {
                    var shift = _context.Shifts.FirstOrDefault(x => x.PhysicianId == ProviderId);
                    var shiftdetail = _context.ShiftDetails.FirstOrDefault(x => x.ShiftId == shift.ShiftId && x.ShiftDate == p.Date);

                    SumOfTotalHours += p.TotalHours ?? 0;
                    SumofHouseCalls += p.NoOfHouseCalls ?? 0;
                    SumofPhoneConsult += p.NoOfPhoneConsults ?? 0;
                    if (p.IsWeekend == true)
                    {
                        SumofWeekendHoliday += 1;
                    }

                    if (shiftdetail != null)
                    {
                        p.OnCallHours = (int)(shiftdetail.EndTime - shiftdetail.StartTime).TotalMinutes / 60;
                    }
                    else
                    {
                        p.OnCallHours = 0;
                    }
                    list.Add(p);

                }
                if (payrate != null)
                {
                    model.InvoiceTotalHours = SumOfTotalHours * payrate.Shift;
                    model.InvoiceWeekendHoliday = SumofWeekendHoliday * payrate.NightShiftWeekend;
                    model.InvoiceHouseCalls = SumofHouseCalls * payrate.HouseCall;
                    model.InvoicePhoneConsults = SumofPhoneConsult * payrate.PhoneConsult;
                    model.InvoiceTotal = model.InvoiceTotalHours + model.InvoiceWeekendHoliday +
                                        model.InvoiceHouseCalls + model.InvoicePhoneConsults;
                }
                model.TimeSheetList = list.OrderBy(u => u.Date).ToList();
                return model;
            }
            else
            {
                List<BiWeeklyTimeSheetModel> list = new List<BiWeeklyTimeSheetModel>();
                for (var i = model.SelectedStartDate; i <= model.SelectedEndDate; i = i.AddDays(1))
                {
                    var shift = _context.Shifts.FirstOrDefault(x => x.PhysicianId == ProviderId);
                    var shiftdetail = _context.ShiftDetails.FirstOrDefault(x => x.ShiftId == shift.ShiftId && x.ShiftDate == i);

                    BiWeeklyTimeSheetModel listofdates = new BiWeeklyTimeSheetModel();
                    listofdates.Date = i;
                    if (shiftdetail != null)
                    {
                        listofdates.OnCallHours = (int)(shiftdetail.EndTime - shiftdetail.StartTime).TotalMinutes / 60;
                        listofdates.TotalHours = listofdates.OnCallHours;
                    }
                    else
                    {
                        listofdates.OnCallHours = 0;
                        listofdates.TotalHours = 0;
                    }
                    list.Add(listofdates);
                }
                model.TimeSheetList = list.OrderBy(u => u.Date).ToList();
                return model;
            }
        }

        public void SubmitBiWeeklyTimesheet(TimeSheetModel model, bool isFinalize, int? physicianId)
        {
            WeeklyTimeSheet wts = _context.WeeklyTimeSheets.FirstOrDefault(u => u.StartDate == model.TimeSheetList[0].Date && u.ProviderId == physicianId);

            int count = model.TimeSheetList.Count;
            if (wts == null)
            {
                WeeklyTimeSheet newSheet = new WeeklyTimeSheet();
                newSheet.StartDate = model.TimeSheetList[0].Date;
                newSheet.EndDate = model.TimeSheetList[0].Date.AddDays(count - 1);
                newSheet.Status = 1;
                newSheet.CreatedDate = DateTime.Now;
                newSheet.ProviderId = (int)physicianId;
                if (isFinalize)
                    newSheet.IsFinalized = true;

                _context.WeeklyTimeSheets.Add(newSheet);
                _context.SaveChanges();

                for (int i = 0; i < count; i++)
                {
                    WeeklyTimeSheetDetail detail = new WeeklyTimeSheetDetail();
                    detail.TimeSheetId = newSheet.TimeSheetId;
                    detail.Date = model.TimeSheetList[i].Date;
                    detail.TotalHours = model.TimeSheetList[i].TotalHours;
                    detail.IsWeekendHoliday = model.TimeSheetList[i].IsWeekend;
                    if (model.TimeSheetList[i].IsWeekend)
                    {
                        detail.HouseCallNightWeekend = model.TimeSheetList[i].NoOfHouseCalls;
                        detail.PhoneConsultNightWeekend = model.TimeSheetList[i].NoOfPhoneConsults;
                    }
                    else
                    {
                        detail.HouseCall = model.TimeSheetList[i].NoOfHouseCalls;
                        detail.PhoneConsult = model.TimeSheetList[i].NoOfPhoneConsults;
                    }
                    detail.Item = model.TimeSheetList[i].Item;
                    detail.Amount = model.TimeSheetList[i].Amount;

                    if (model.TimeSheetList[i].Bill != null)
                    {
                        detail.Bill = model.TimeSheetList[i].Bill.FileName;

                        var filePath = Path.Combine("wwwroot", "InvoicingDocs", physicianId.ToString() + Path.GetFileName(model.TimeSheetList[i].Bill.FileName));
                        using (FileStream stream = System.IO.File.Create(filePath))
                        {
                            model.TimeSheetList[i].Bill.CopyToAsync(stream);
                        }
                    }
                    _context.WeeklyTimeSheetDetails.Add(detail);
                    _context.SaveChanges();
                }
            }
            else
            {
                if (isFinalize)
                {
                    wts.IsFinalized = true;
                    _context.WeeklyTimeSheets.Update(wts);
                    _context.SaveChanges();
                }
                if (model.IsAdmin == true)
                {
                    wts.BonusAmount = model.BonusAmount;
                    wts.AdminId = model.AdminId;
                    wts.AdminNote = model.AdminNote;

                    _context.WeeklyTimeSheets.Update(wts);
                    _context.SaveChanges();
                }
                for (int i = 0; i < count; i++)
                {
                    WeeklyTimeSheetDetail detail = _context.WeeklyTimeSheetDetails.FirstOrDefault(u => u.Date == model.TimeSheetList[i].Date && u.TimeSheetId == wts.TimeSheetId);
                    detail.TotalHours = model.TimeSheetList[i].TotalHours;
                    detail.IsWeekendHoliday = model.TimeSheetList[i].IsWeekend;
                    if (model.TimeSheetList[i].IsWeekend)
                    {
                        detail.HouseCallNightWeekend = model.TimeSheetList[i].NightWeekendHouseCall;
                        detail.PhoneConsultNightWeekend = model.TimeSheetList[i].NightWeekendPhoneConsult;
                    }
                    else
                    {
                        detail.HouseCall = model.TimeSheetList[i].NoOfHouseCalls;
                        detail.PhoneConsult = model.TimeSheetList[i].NoOfPhoneConsults;
                    }
                    detail.Item = model.TimeSheetList[i].Item;
                    detail.Amount = model.TimeSheetList[i].Amount;

                    if (model.TimeSheetList[i].Bill != null)
                    {
                        detail.Bill = model.TimeSheetList[i].Bill!.FileName;

                        var filePath = Path.Combine("wwwroot", "InvoicingDocs", physicianId.ToString() + Path.GetFileName(model.TimeSheetList[i].Bill!.FileName));
                        using (FileStream stream = System.IO.File.Create(filePath))
                        {
                            model.TimeSheetList[i].Bill!.CopyToAsync(stream);
                        }
                    }
                    _context.WeeklyTimeSheetDetails.Update(detail);
                    _context.SaveChanges();
                }
            }
        }

        public void ApproveBiWeeklyTimesheet(TimeSheetModel model)
        {
            var weeklyTimesheet = _context.WeeklyTimeSheets.FirstOrDefault(x => x.ProviderId == model.PhysicianId && x.StartDate == model.TimeSheetList[0].Date);
            if (weeklyTimesheet != null)
            {
                weeklyTimesheet.AdminId = model.AdminId;
                weeklyTimesheet.Status = 2;
                weeklyTimesheet.AdminNote = model.AdminNote;
                weeklyTimesheet.BonusAmount = model.BonusAmount;

                _context.WeeklyTimeSheets.Update(weeklyTimesheet);
                _context.SaveChanges();
            }
        }
    }
}
