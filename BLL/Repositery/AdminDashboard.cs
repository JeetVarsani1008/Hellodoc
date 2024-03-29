﻿using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Region = DAL.Models.Region;

namespace BLL.Repositery
{
    public class AdminDashboard : IAdminDashboard
    {
        private BitArray _isDeleted = new BitArray(1);

        private readonly HellodocContext _context;
        public AdminDashboard(HellodocContext context)
        {
            _context = context;

        }
        public List<RequestListAdminDash> requestDataAdmin(string statusarray, int[] Status, string reqTypeId, int regionId)
        {
            //var requestTypeId = _context.Requests.Where(o => o.RequestTypeId == reqTypeId);
            var requestList = _context.Requests.Where(o => statusarray.Contains(o.Status.ToString()));
            //List<DAL.ViewModel.CaseTag> caseTag = new List<DAL.ViewModel.CaseTag>();

            if (reqTypeId != null)
            {
                requestList = requestList.Where(o => o.RequestTypeId.ToString() == reqTypeId);
            }
            if (regionId != 0)
            {
                //var requestdata = _context.RequestClients.Where(i => i.RegionId == RegionId);
                //requestList = requestList.Where(i => i.RequestId == requestdata.Select(u => u.RequestId).First());

                requestList = requestList.Where(i => i.RequestClients.Select(r => r.RegionId.ToString()).Contains(regionId.ToString()));

            }

            var GetRequestData = requestList.Select(r => new RequestListAdminDash()
            {

                FirstName = r.RequestClients.Select(x => x.FirstName).First(),
                LastName = r.RequestClients.Select(x => x.LastName).First(),
                RequestId = r.RequestId,
                Name = r.RequestClients.Select(x => x.FirstName).First() + " " + r.RequestClients.Select(x => x.LastName).First(),
                Requestor = r.FirstName + " " + r.LastName,
                RequestDate = r.CreatedDate,
                Address = r.RequestClients.Select(x => x.Street).First() + "," + r.RequestClients.Select(x => x.City).First() + "," + r.RequestClients.Select(x => x.State).First(),
                Notes = r.RequestClients.Select(x => x.Notes).First(),
                ChatWith = r.PhysicianId.ToString(),
                Physician = r.Physician.FirstName,
                Status = r.Status,
                Phone = r.PhoneNumber,
                //year = (int)_context.RequestClients.Select(x => x.IntYear).First(),
                //date = (int)_context.RequestClients.Select(x => x.IntDate).First(),
                //month = _context.RequestClients.Select(x => x.StrMonth).First(),

                //DateOfService = r.CreatedDate,
                DateOfBirth = r.RequestClients.Select(x => x.IntDate).First() == null ? null : r.RequestClients.Select(x => x.IntDate).First() + "/" + r.RequestClients.Select(x => x.StrMonth).First()+ "/" + r.RequestClients.Select(x => x.IntYear).First(),
                rPhonenumber = r.PhoneNumber,
                RequestTypeId = r.RequestTypeId,


            }).ToList();
            return GetRequestData;
        }

        //this method is for download excel foe all request id
        public List<RequestListAdminDash> requestDataDownloadExcelAll()
        {
            var requestList = _context.Requests;

            var GetRequestData = requestList.Select(r => new RequestListAdminDash()
            {

                FirstName = r.RequestClients.Select(x => x.FirstName).First(),
                LastName = r.RequestClients.Select(x => x.LastName).First(),
                RequestId = r.RequestId,
                Name = r.RequestClients.Select(x => x.FirstName).First() + " " + r.RequestClients.Select(x => x.LastName).First(),
                Requestor = r.FirstName + " " + r.LastName,
                RequestDate = r.CreatedDate,
                Address = r.RequestClients.Select(x => x.Street).First() + "," + r.RequestClients.Select(x => x.City).First() + "," + r.RequestClients.Select(x => x.State).First(),
                Notes = r.RequestClients.Select(x => x.Notes).First(),
                ChatWith = r.PhysicianId.ToString(),
                Physician = r.Physician.FirstName,
                Status = r.Status,
                year = (int)_context.RequestClients.Select(x => x.IntYear).First(),
                date = (int)_context.RequestClients.Select(x => x.IntDate).First(),
                month = _context.RequestClients.Select(x => x.StrMonth).First(),
                //DateOfBirth = new DateTime(year, DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month, date),

                rPhonenumber = r.PhoneNumber,
                RequestTypeId = r.RequestTypeId,


            }).ToList();
            return GetRequestData;
        }

        public List<Region> getRegions()
        {
            var data = _context.Regions.ToList();
            return data;
        }

        public ViewCaseVm ViewCase(int requestId)
        {
            var request = _context.Requests.Where(o => o.RequestId == requestId);
            var user = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);
            var reqtype = _context.Requests.FirstOrDefault(x => x.RequestId == requestId).RequestTypeId;

            var region = _context.Regions.FirstOrDefault(x => x.RegionId == user.RegionId)?.Name ?? "Unknown";
            int intYear = user?.IntYear?? 1;
            int intDate = user?.IntDate?? 1;
            string month = user.StrMonth;
            DateTime birthdate = new DateTime(intYear, DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month, intDate);
            ViewCaseVm viewCaseVm = new ViewCaseVm()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Notes = user.Notes,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                DateOfBirth = birthdate,
                RequestTypeId = reqtype,
                Region = region,
                RequestId = requestId,

            };
            return viewCaseVm;
        }


        public ViewNotesVm ViewNotes(int requestId)
        {

            var user = _context.RequestNotes.FirstOrDefault(u => u.RequestId == requestId);
            var usertwo = _context.RequestStatusLogs.FirstOrDefault(x => x.RequestId == requestId);

            if (user != null && usertwo != null)
            {
                ViewNotesVm viewNotesVm = new ViewNotesVm()
                {
                    AdminNotes = user.AdminNotes,
                    PhysicianNotes = user.PhysicianNotes,
                    TransferNotes = usertwo.Notes,
                    RequestId = user.RequestId,
                };
                return viewNotesVm;
            }
            else
            {
                var usermain = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
                _context.RequestNotes.Add(new RequestNote()
                {
                    RequestId = requestId,
                    CreatedBy = (int)_context.Users.FirstOrDefault(x => x.UserId == usermain.UserId).AspNetUserId,
                    CreatedDate = DateTime.Now,
                });
                _context.SaveChanges();


                _context.RequestStatusLogs.Add(new RequestStatusLog()
                {
                    RequestId = requestId,
                    CreatedDate = DateTime.Now,
                });
                _context.SaveChanges();


                var edituser = _context.RequestNotes.FirstOrDefault(u => u.RequestId == requestId);
                var editusertwo = _context.RequestStatusLogs.FirstOrDefault(x => x.RequestId == requestId);

                ViewNotesVm viewNotesVm = new ViewNotesVm()
                {
                    AdminNotes = edituser.AdminNotes,
                    PhysicianNotes = edituser.PhysicianNotes,
                    TransferNotes = editusertwo.Notes,
                    RequestId = edituser.RequestId,
                };
                return viewNotesVm;
            }
        }

        public void editViewNotes(ViewNotesVm model, int requestId)
        {
            var reqnotes = _context.RequestNotes.FirstOrDefault(x => x.RequestId == requestId);
            var req = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            if (reqnotes != null)
            {
                reqnotes.AdminNotes = model.AdminNotes;
                reqnotes.PhysicianNotes = model.PhysicianNotes;
                reqnotes.ModifiedBy = (int)_context.Users.FirstOrDefault(x => x.UserId == req.UserId).AspNetUserId;
                reqnotes.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
            }

        }


        public List<DAL.ViewModel.CaseTag> cancelCaseMain()
        {
            var data = _context.CaseTags.ToList();
            var mappedData = data.Select(item => new DAL.ViewModel.CaseTag
            {
                CaseTagId = item.CaseTagId,
                Name = item.Name,
            }).ToList();
            return mappedData;
        }

        public void cancelCase(AdminDashboardViewModel model, int requestId)
        {
            RequestStatusLog requestStatuslog = new RequestStatusLog();
            requestStatuslog.RequestId = requestId;
            requestStatuslog.Status = 5;
            requestStatuslog.Notes = model.Notes;
            _context.Add(requestStatuslog);
            _context.SaveChanges();

            var cancel = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            cancel.Status = 5;
            cancel.CaseTag = model.CaseTagss;

            _context.SaveChanges();
        }

        public List<Region> asignCase()
        {
            var data = _context.Regions.ToList();
            var mappedData = data.Select(item => new Region
            {
                RegionId = item.RegionId,
                Name = item.Name,
            }).ToList();
            return mappedData;
        }

        public List<Physician> asignPhysician(int regionId)
        {
            var data = _context.Physicians.Where(p => p.RegionId == regionId).ToList();
            return data;
        }


        public AdminBlockVm adminBlockVm(int requestId)
        {
            AdminBlockVm adminBlockVm = new AdminBlockVm()
            {
                RequestId = requestId,
            };
            return adminBlockVm;
        }

        public void blockCase(AdminBlockVm model, int requestId)
        {
            var request1 = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            request1.Status = 11;

            var email = request1.Email;
            var phoneNumber = request1.PhoneNumber;

            var request2 = _context.Requests.Where(x => (x.Email == email || x.PhoneNumber == phoneNumber) && x.Status == 1).ToList();

            var blockreq = new BlockRequest();
            foreach (var obj in request2)
            {
                obj.Status = 11;
                blockreq.RequestId = obj.RequestId;
                blockreq.PhoneNumber = phoneNumber;
                blockreq.Email = email;
                blockreq.Reason = model.Reason;
                blockreq.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

                _context.BlockRequests.Add(blockreq);
                _context.Requests.Update(request1);
            }
            _context.SaveChanges();
        }


        public void asignCasePost(AdminAsignVm model, int requestId, int newStatus)
        {
            var request = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            request.Status = (short)newStatus;
            request.ModifiedDate = DateTime.Now;
            _context.SaveChanges();
            var reqstatus = new RequestStatusLog
            {
                RequestId = requestId,
                Status = (short)newStatus,
                Notes = model.Description,
                CreatedDate = DateTime.Now,
                PhysicianId = model.PhysicianId,
            };
            if (newStatus == 2)
            {
                reqstatus.TransToPhysicianId = model.PhysicianId;
                request.PhysicianId = model.PhysicianId;
            }
            _context.Requests.Update(request);
            _context.RequestStatusLogs.Add(reqstatus);
            _context.SaveChanges();
        }


        public void UploadFile(int requestId, string fileName)
        {
            RequestWiseFile requestWiseFile = new RequestWiseFile()
            {
                RequestId = requestId,
                FileName = fileName,
                IsDeleted = _isDeleted,
                CreatedDate = DateTime.Now,
            };
            _context.Add(requestWiseFile);
            _context.SaveChanges();
        }


        public AdminViewUploadVm GetAdminViewUploadData(AdminViewUploadVm model, int requestId)
        {
            var client = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);
            if (client == null)
            {
                return new AdminViewUploadVm();
            }
            var mymodel = new AdminViewUploadVm
            {
                RequestId = requestId,
                Name = client.FirstName + " " + client.LastName,
            };
            return mymodel;
        }

        public List<RequestWiseFile> GetFilesByRequestId(int requestId)
        {
            List<RequestWiseFile> files = _context.RequestWiseFiles.Where(u => u.RequestId == requestId).ToList();
            return files.Where(u => !u.IsDeleted[0]).ToList();
        }

        public RequestWiseFile GetFileById(int fileId)
        {
            return _context.RequestWiseFiles.FirstOrDefault(u => u.RequestWiseFileId == fileId);
        }

        public List<RequestWiseFile> GetAllFilesByRequestId(int reqId)
        {
            return _context.RequestWiseFiles.Where(x => x.RequestId == reqId).ToList();
        }

        //public void DeleteFile(int RequestWiseFileId)
        //{

        //}


        //from here 4 methods are for order details
        public List<HealthProfessionalType> healthProfessionalTypes()
        {
            var data = _context.HealthProfessionalTypes.ToList();
            var mappedData = data.Select(item => new HealthProfessionalType
            {
                HealthProfessionalId = item.HealthProfessionalId,
                ProfessionName = item.ProfessionName,
            }).ToList();
            return mappedData;
        }

        public List<HealthProfessional> asignBusiness(int healthProfessionId)
        {
            var data = _context.HealthProfessionals.Where(p => p.Profession == healthProfessionId).ToList();
            return data;
        }

        public List<HealthProfessional> getVendorDetails(int vendorId)
        {
            var data = _context.HealthProfessionals.Where(x => x.VendorId == vendorId).ToList();
            return data;
        }

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
            };
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }

        //order details are completed

        //from here this is for transfer note
        public List<Region> transferRegion()
        {
            var data = _context.Regions.ToList();
            return data;
        }

        public List<Physician> transferPhysician(int regionId)
        {
            var data = _context.Physicians.Where(p => p.RegionId == regionId).ToList();
            return data;
        }
        public void transferCasePost(AdminAsignVm model, int newStatus)
        {
            var request = _context.Requests.FirstOrDefault(x => x.RequestId == model.RequestId);
            request.ModifiedDate = DateTime.Now;
            _context.SaveChanges();

            var reqstatus = new RequestStatusLog
            {
                RequestId = model.RequestId,
                Status = (short)newStatus,
                Notes = model.Description,
                CreatedDate = DateTime.Now,
                PhysicianId = model.PhysicianId,
            };
            if (newStatus == 2)
            {
                reqstatus.TransToPhysicianId = model.PhysicianId;
                request.PhysicianId = model.PhysicianId;
            }
            _context.Requests.Update(request);
            _context.RequestStatusLogs.Add(reqstatus);
            _context.SaveChanges();

        }

        //export logic
        public byte[] ExportToExcel(List<RequestListAdminDash> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())

            {
                var worksheet = package.Workbook.Worksheets.Add("Admin Data");

                // Add header row
                int col = 1;
                foreach (var prop in typeof(RequestListAdminDash).GetProperties())
                {
                    worksheet.Cells[1, col].Value = prop.Name;
                    col++;
                }

                // Add data rows
                int row = 2;
                foreach (var item in data)
                {
                    col = 1;
                    foreach (var prop in typeof(RequestListAdminDash).GetProperties())
                    {
                        worksheet.Cells[row, col].Value = prop.GetValue(item);
                        col++;
                    }
                    row++;
                }

                // Style the header
                using (var range = worksheet.Cells[1, 1, 1, col - 1])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                }

                return package.GetAsByteArray();
            }
        }

        //Logic for clear case
        public void clearCasePost(AdminClearVm model)
        {
            var data = _context.Requests.FirstOrDefault(x => x.RequestId == model.RequestId);
            if (data != null)
            {
                data.Status = 10;
                _context.SaveChanges();

                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = model.RequestId,
                    Status = data.Status,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();

            }
        }
        //clear case completed


        //review agreement and send agreement
        public bool checkStatus(ReviewAgreementVm model)
        {
            var data = _context.Requests.Any(x => x.RequestId == model.RequestId);
            if (data)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void reviewAgreeementSubmit(ReviewAgreementVm model)
        {
            var data = _context.Requests.FirstOrDefault(x => x.RequestId == model.RequestId && x.Status == 2);
            if (data != null)
            {
                data.Status = 4;
                data.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
            }

            RequestStatusLog requestStatusLog = new RequestStatusLog();
            requestStatusLog.RequestId = model.RequestId;
            requestStatusLog.Status = 4;
            requestStatusLog.CreatedDate = DateTime.Now;
            requestStatusLog.Notes = "Review Agreement";

            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();
        }

        public void reviewAgreementCancel(ReviewAgreementVm model)
        {
            var data = _context.Requests.FirstOrDefault(x => x.RequestId == model.RequestId);
            if (data != null)
            {
                data.Status = 7;
                data.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
            }

            RequestStatusLog requestStatusLog = new RequestStatusLog();
            requestStatusLog.RequestId = model.RequestId;
            requestStatusLog.Status = 7;
            requestStatusLog.CreatedDate = DateTime.Now;
            requestStatusLog.Notes = model.Notes;

            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();
        }

        //close case 
        public CloseCaseVm closeCaseGet(int requestId)
        {
            var reqFiles = _context.RequestWiseFiles.Where(x => x.RequestId == requestId).ToList();
            var data = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);
            if (data != null)
            {
                int intYear = (int)data.IntYear;
                int intDate = (int)data.IntDate;
                string month = (string)data.StrMonth;
                DateTime birthdate = new DateTime(intYear, DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month, intDate);
                CloseCaseVm closeCaseVm = new CloseCaseVm
                {
                    getFilesForCloseCase = reqFiles,
                    RequestId = requestId,
                    FirstName = data?.FirstName ?? "Unknown",
                    LastName = data?.LastName ?? "Unknow",
                    PhoneNumber = data.PhoneNumber,
                    DateOfBirth = birthdate,
                    Email = data.Email,
                };

                return closeCaseVm;
            }
            return null;
        }

        public void closeCaseEdit(int command, CloseCaseVm model, int requestId)
        {
            if (command == 1)
            {
                var data = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);
                data.PhoneNumber = model.PhoneNumber;
                data.Email = model.Email;
                _context.SaveChanges();

                var request = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
                request.PhoneNumber = model.PhoneNumber;
                request.Email = model.Email;
                request.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
            }
            else
            {
                var req = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
                req.Status = 9;
                req.ModifiedDate = DateTime.Now;
                _context.SaveChanges();


                RequestStatusLog requestStatusLog = new RequestStatusLog
                {
                    RequestId = requestId,
                    Status = 10,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
            }

        }

        //this part is for get admin profile details
        public AdminProfileVm getAdminDetails(int aspId, int adminId)
        {
            var region = _context.Regions.ToList();
            List<AdminRegion> adminRegion = _context.AdminRegions.Where(x => x.AdminId == adminId).ToList();
            var data = _context.Admins.FirstOrDefault(x => x.AspNetUserId == aspId);
            AdminProfileVm adminProfileVm = new AdminProfileVm
            {
                AspNetUserId = aspId,
                UserName = data.FirstName + " " + data.LastName,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                Mobile = data.Mobile,
                Address1 = data.Address1,
                Address2 = data.Address2,
                City = data.City,
                Zip = data.Zip,
                State = data.RegionId.ToString(),
                regions = region,
                AdminRegions = adminRegion,
            };
            return adminProfileVm;
        }

        public void adminResetPassword(AdminProfileVm model)
        {
            var data = _context.AspNetUsers.FirstOrDefault(x => x.Id == model.AspNetUserId);
            data.PasswordHash = model.Password;
            _context.SaveChanges();
        }

        public void adminEditDetails1(AdminProfileVm model, List<int>? checkboxForAll, int adminId)
        {
            Admin admin = _context.Admins.FirstOrDefault(u => u.AdminId == adminId);
            List<AdminRegion> adminRegion = _context.AdminRegions.Where(x => x.AdminId == adminId).ToList();
            var data = _context.Admins.FirstOrDefault(x => x.AspNetUserId == model.AspNetUserId);
            data.FirstName = model.FirstName;
            data.LastName = model.LastName;
            data.Email = model.Email;
            data.Mobile = model.Mobile;
            _context.SaveChanges();

            if (checkboxForAll != null)
            {
                foreach (var item in adminRegion)
                {
                    _context.AdminRegions.Remove(item);
                    _context.SaveChanges();
                }
                foreach (var regionId in checkboxForAll)
                {
                    AdminRegion region = new AdminRegion();
                    region.AdminId = admin.AdminId;
                    region.RegionId = regionId;
                    _context.AdminRegions.Add(region);
                    _context.SaveChanges();
                }
            }
            else
            {
                foreach (var item in adminRegion)
                {
                    _context.AdminRegions.Remove(item);
                    _context.SaveChanges();
                }
            }

        }

        public void adminEditDetails2(AdminProfileVm model)
        {
            var data = _context.Admins.FirstOrDefault(x => x.AspNetUserId == model.AspNetUserId);
            data.Address1 = model.Address1;
            data.Address2 = model.Address2;
            data.City = model.City;
            data.Zip = model.Zip;
            data.AltPhone = model.AlternateMobile;
            _context.SaveChanges();
        }

        //this part is for encounter form 
        public EncounterVm encounterFormGetData(int reqId)
        {
            var enc = _context.Encounters.FirstOrDefault(x => x.RequestId == reqId);
            var reqClient = _context.RequestClients.FirstOrDefault(x => x.RequestId == reqId);
            int intYear = (int)reqClient.IntYear;
            int intDate = (int)reqClient.IntDate;
            string month = (string)reqClient.StrMonth;
            DateTime birthdate = new DateTime(intYear, DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month, intDate);

            if (enc != null)
            {

                EncounterVm encounterVm = new EncounterVm
                {
                    RequestId = reqId,
                    FirstName = reqClient.FirstName,
                    LastName = reqClient.LastName,
                    Address = reqClient.Address,
                    DateOfBirth = birthdate,
                    PhoneNumber = reqClient.PhoneNumber,
                    Email = reqClient.Email,
                    MedicalHistory = enc.MedicalHistory,
                    Medications = enc.Medications,
                    Allergies = enc.Allergies,
                    Temp = enc.Temp,
                    Hr = enc.Hr,
                    Rr = enc.Rr,
                    BloodPressureS = enc.BloodPressureS,
                    BloodPressureD = enc.BloodPressureD,
                    O2 = enc.O2,
                    Pain = enc.Pain,
                    Heent = enc.Heent,
                    Cv = enc.Cv,
                    Chest = enc.Chest,
                    Abd = enc.Abd,
                    Extr = enc.Extr,
                    Skin = enc.Skin,
                    Neuro = enc.Neuro,
                    Other = enc.Other,
                    Diagnosis = enc.Diagnosis,
                    TreatmentPlan = enc.TreatmentPlan,
                    MedicationsDispensed = enc.MedicationsDispensed,
                    Procedures = enc.Procedures,
                    FollowUp = enc.Followup
                };
                return encounterVm;
            }
            else
            {
                _context.Encounters.Add(new Encounter()
                {
                    RequestId = reqId,
                });
                _context.SaveChanges();

                EncounterVm encounterVm = new EncounterVm
                {
                    FirstName = reqClient.FirstName,
                    LastName = reqClient.LastName,
                    Address = reqClient.Address,
                    DateOfBirth = birthdate,
                    PhoneNumber = reqClient.PhoneNumber,
                    Email = reqClient.Email,
                };
                return encounterVm;
            }
        }

        public void postEncounterData(EncounterVm model)
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
        //encounter part is completed


        //this is for get provider/physician main details
        public List<Provider> getPhysicianDetails(string regionId)
        {
            var list = _context.Physicians.Include(x => x.PhysicianNotifications).ToList();

            if (regionId != null)
            {
                list = list.Where(x => x.RegionId.ToString() == regionId).ToList();
            }
            
            var data = list.Select(x => new Provider()
            {
                PhysicianId = x.PhysicianId,
                StopNotification = x.PhysicianNotifications.Any(pn => pn.IsNotificationStopped),
                Name = x.FirstName,
                Role = _context.Roles.FirstOrDefault(i => i.RoleId == x.RoleId)?.Name,
                Status = (int)x.Status,
                OnCallStatus = "Available",
            }).ToList();
            return data;
        }
        //provider main details completed


        //this is for edit provider/physician details : 6 methods
        public ProviderVm getPhysicianDetails(int physicianId)
        {
            var phy = _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId);
            var aspid = _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId).AspNetUserId;
            var aspuser = _context.AspNetUsers.FirstOrDefault(x => x.Id == aspid);
            var region = _context.Regions.ToList();
            var physicianRegion = _context.PhysicianRegions.Where(x => x.PhysicianId == physicianId).ToList();
            ProviderVm providerVm = new ProviderVm
            {
                PhysicianId = phy.PhysicianId,
                UserName = aspuser.UserName,
                regions = region,
                physicianRegions = physicianRegion,
                FirstName = phy.FirstName,
                LastName = phy.LastName,
                Address1 = phy.Address1,
                Address2 = phy.Address2,
                City = phy.City,
                RegionId = phy.RegionId ?? 1,
                Zip = phy.Zip,
                Email = aspuser.Email,
                Phone = phy.Mobile,
                MedicalLicence = phy.MedicalLicense,
                NPINumber = phy.Npinumber,
                BusinessName = phy.BusinessName,
                BusinessWebsite = phy.BusinessWebsite,
                SynchronizationEmail = phy.SyncEmailAddress,
                Photo = phy.Photo,
                Signature = phy.Signature,
                AdminNotes = phy.AdminNotes,
                IsAgreementDoc = phy.IsAgreementDoc ?? false,
                IsBackgroundDoc = phy.IsBackgroundDoc ?? false,
                IsNonDisclosureDoc = phy.IsNonDisclosureDoc ?? false,
                IsLicenseDoc = phy.IsLicenseDoc ?? false,
            };
            return providerVm;
        }

        public void editPhysicianPassword(ProviderVm model)
        {
            var aspId = _context.Physicians.FirstOrDefault(x => x.PhysicianId == model.PhysicianId).AspNetUserId;
            var passdata = _context.AspNetUsers.FirstOrDefault(x => x.Id == aspId);

            passdata.PasswordHash = model.Password;
            _context.SaveChanges();
        }

        public void physicianEditDetails1(ProviderVm model, List<int>? checkboxForAll)
        {
            Physician physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == model.PhysicianId);
            var physicianRegion = _context.PhysicianRegions.Where(x => x.PhysicianId == model.PhysicianId).ToList();

            physician.FirstName = model.FirstName;
            physician.LastName = model.LastName;
            physician.Email = model.Email;
            physician.MedicalLicense = model.MedicalLicence;
            physician.Npinumber = model.NPINumber;
            physician.SyncEmailAddress = model.SynchronizationEmail;
            _context.SaveChanges();


            if (checkboxForAll != null)
            {
                foreach (var item in physicianRegion)
                {
                    _context.PhysicianRegions.Remove(item);
                    _context.SaveChanges();
                }
                foreach (var regionId in checkboxForAll)
                {
                    PhysicianRegion region = new PhysicianRegion();
                    region.PhysicianId = physician.PhysicianId;
                    region.RegionId = regionId;
                    _context.PhysicianRegions.Add(region);
                    _context.SaveChanges();
                }
            }
            else
            {
                foreach (var item in physicianRegion)
                {
                    _context.PhysicianRegions.Remove(item);
                    _context.SaveChanges();
                }
            }
        }

        public void physicianEditDetails2(ProviderVm model)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == model.PhysicianId);
            physician.Address1 = model.Address1;
            physician.Address2 = model.Address2;
            physician.City = model.City;
            //physician.RegionId = model.RegionId;
            physician.Zip = model.Zip;
            physician.AltPhone = model.AltPhone;
            _context.SaveChanges();
        }

        public void physicianEditDetails3(ProviderVm model)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == model.PhysicianId);
            physician.BusinessName = model.BusinessName;
            physician.BusinessWebsite = model.BusinessWebsite;
            physician.Signature = model.Signature;
            physician.Photo = model.Photo;
            _context.SaveChanges();

        }

        // this is for upload document when admin edit provider account
        public void updateProviderDoc(ProviderVm model)
        {
            var phy = _context.Physicians.FirstOrDefault(x => x.PhysicianId == model.PhysicianId);
            string uniquefilename1 = null;
            if (model.AgreementDoc != null && model.AgreementDoc.Length > 0)
            {
                string uploadfolder = Path.Combine("wwwroot", "upload");
                uniquefilename1 = $"{model.PhysicianId}" + "_AgreementDoc.pdf";
                string filePath = Path.Combine(uploadfolder, uniquefilename1);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.AgreementDoc.CopyTo(stream);
                }
                phy!.IsAgreementDoc = true;
            }

            if(model.BackgroundDoc != null && model.BackgroundDoc.Length > 0)
            {
                string uploadfolder = Path.Combine("wwwroot","upload");
                uniquefilename1 = $"{model.PhysicianId}" + "_BackgroundDoc.pdf";
                string filePath = Path.Combine(uploadfolder, uniquefilename1);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.BackgroundDoc.CopyTo(stream);
                }
                phy!.IsBackgroundDoc = true;    
            }
            
            if(model.NonDisclosureDoc != null && model.NonDisclosureDoc.Length > 0)
            {
                string uploadfolder = Path.Combine("wwwroot","upload");
                uniquefilename1 = $"{model.PhysicianId}" + "_NonDisclosureDoc.pdf";
                string filePath = Path.Combine(uploadfolder, uniquefilename1);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.NonDisclosureDoc.CopyTo(stream);
                }
                phy!.IsNonDisclosureDoc = true;    
            }
            
            if(model.LicenseDoc != null && model.LicenseDoc.Length > 0)
            {
                string uploadfolder = Path.Combine("wwwroot","upload");
                uniquefilename1 = $"{model.PhysicianId}" + "_LicenseDoc.pdf";
                string filePath = Path.Combine(uploadfolder, uniquefilename1);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.LicenseDoc.CopyTo(stream);
                }
                phy!.IsLicenseDoc = true;    
            }

            _context.SaveChanges();
        }
        //edit provider details completed

        public List<Role> getRoles()
        {
            var data = _context.Roles.ToList();
            return data;
        }

        public void createProviderAccount(ProviderVm model, List<int>? checkboxForAll)
        {

            AspNetUser aspNetUser = new AspNetUser
            {
                UserName = model.UserName,
                PasswordHash = model.Password,
                Email = model.Email,
                Phonenumber = model.Phone,
                CreatedDate = DateTime.Now,
            };
            _context.AspNetUsers.Add(aspNetUser);
            _context.SaveChanges();


            AspNetUserRole aspNetUserRole = new AspNetUserRole
            {
                UserId = aspNetUser.Id,
                RoleId = 3,
            };
            _context.AspNetUserRoles.Add(aspNetUserRole);
            _context.SaveChanges();

            Physician physician = new Physician
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Mobile = model.Phone,
                MedicalLicense = model.MedicalLicence,
                AdminNotes = model.AdminNotes,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Zip = model.Zip,
                AltPhone = model.AltPhone,
                BusinessName = model.BusinessName,
                BusinessWebsite = model.BusinessWebsite,
                CreatedDate = DateTime.Now,
                CreatedBy = 1,
                Status = 1,
                RegionId = model.RegionId,
                RoleId = model.RoleId,
                Photo = model.Photo,
            };
            _context.Physicians.Add(physician);
            _context.SaveChanges();


            if (checkboxForAll != null)
            {
                foreach (var item in checkboxForAll)
                {
                    PhysicianRegion region = new PhysicianRegion();
                    region.RegionId = model.RegionId;
                    region.PhysicianId = physician.PhysicianId;
                    _context.PhysicianRegions.Add(region);
                    _context.SaveChanges();
                }

            }

            PhysicianNotification physicianNotification = new PhysicianNotification
            {
                PhysicianId = physician.PhysicianId,
                IsNotificationStopped = false,
            };
            _context.PhysicianNotifications.Add(physicianNotification);
            _context.SaveChanges();
        }

        //this part is for access role
        public List<Menu> getMenu()
        {
            var data = _context.Menus.ToList();
            return data;
        }
        public List<AspNetRole> getAspNetRoles()
        {
            var data = _context.AspNetRoles.ToList();
            return data;
        }
        public List<Access> getAccessRoles()
        {
            var list = _context.Roles.Where(x => x.IsDeleted == false).ToList();
            var data = list.Select(x => new Access()
            {
                Name = x.Name,
                AccountType = _context.AspNetRoles.FirstOrDefault(i => i.Id == x.AccountType).Name,
                RoleId = x.RoleId,
                AccountTypeId = x.AccountType,
            }).ToList();
            return data;
        }

        public List<Menu> getAccess(int id)
        {
            if (id == 0)
            {

                List<Menu> role = _context.Menus.ToList();
                return role;
            }
            else
            {
                List<Menu> roles = _context.Menus.Where(x => x.AccountType == id).ToList();
                return roles;
            }
        }

        public void createRole(AccessVm model, List<int> checkboxForAllRole)
        {
            Role role = new Role
            {
                Name = model.RoleName,
                AccountType = (short)model.AccountType,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now,
                IsDeleted = false,
            };
            _context.Roles.Add(role);
            _context.SaveChanges();

            if(checkboxForAllRole != null)
            {
               foreach(var item in checkboxForAllRole)
                {
                    RoleMenu roleMenu = new RoleMenu
                    {
                        RoleId = role.RoleId,
                        MenuId = item,
                    };
                    _context.RoleMenus.Add(roleMenu);
                    _context.SaveChanges();
                }
            }
        }

        //this is for edit access role
        public AccessVm editAccessRole(int RoleId, int AccountType)
        {
            List<RoleMenu> roleMenu = _context.RoleMenus.Where(x => x.RoleId == RoleId).ToList();
            var menus = _context.Menus.Where(x => x.AccountType == AccountType).ToList();
            
            var role = _context.Roles.FirstOrDefault(x => x.RoleId == RoleId);
            AccessVm accessVm = new AccessVm
            {
                RoleName = role.Name,
                AccountType = role.AccountType,
                menu = menus,
                roleMenu = roleMenu,
                RoleId = RoleId,
            };
            return accessVm;
        }

        public void editAccessRolePost(AccessVm model, List<int>? checkboxForAllRole) 
        {
            var role = _context.Roles.FirstOrDefault(x => x.RoleId == model.RoleId);

            var rolemenu = _context.RoleMenus.Where(x => x.RoleId == model.RoleId).ToList();

            role.Name = model.RoleName;
            role.ModifiedBy = "Admin";
            role.ModifiedDate = DateTime.Now;
            _context.SaveChanges();

            if(checkboxForAllRole != null)
            {
                foreach(var item in rolemenu)
                {
                    _context.RoleMenus.Remove(item);
                    _context.SaveChanges();
                }
                foreach(var item in checkboxForAllRole)
                {
                    RoleMenu roleMenu = new RoleMenu
                    {
                        RoleId = role.RoleId,
                        MenuId = item,
                    };
                    _context.RoleMenus.Add(roleMenu);
                    _context.SaveChanges();
                }
            }
            else
            {
                foreach(var item in rolemenu)
                {
                    _context.RoleMenus.Remove(item);
                    _context.SaveChanges(); 
                }
            }
        }

        public void deleteAccessRole(int RoleId)
        {
            var role = _context.Roles.FirstOrDefault(x => x.RoleId == RoleId);
            role.IsDeleted = true;
            _context.SaveChanges();
        }

        public List<UserAccess> getUserAccessData(int RoleId)
        {
            var list1 = _context.Admins.ToList();
            var list2 = _context.Physicians.ToList();

            var data1 = list1.Select(x => new UserAccess()
            {
                RoleId = RoleId,
                AccountType = "Admin",
                AccountPOC = x.FirstName + " " + x.LastName,
                Phone = x.Mobile,
                Status = (int)x?.Status,
                OpenRequest = "Yes"
            }).ToList();

            var data2 = list2.Select(x => new UserAccess()
            {
                AccountType = "Physician",
                AccountPOC = x.FirstName + " " + x.LastName,
                Phone = x.Mobile,
                Status = (int)x?.Status,
                OpenRequest = "Yes"
            }).ToList();

            var data = data1.Concat(data2).ToList();

            if(RoleId == 1)
            {
                return data1;
            }
            else if(RoleId == 2)
            {
                return data2;
            }
            else
            {
                return data;
            }
        }
    }
}
