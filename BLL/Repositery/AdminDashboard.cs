using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using MimeKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Region = DAL.Models.Region;
using Request = DAL.Models.Request;

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

        #region ErrorPage : checkreq
        public bool checkreq(int requestid)
        {
            var req = _context.Requests.FirstOrDefault(c => c.RequestId == requestid);

            if (req != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region checkPhysician
        public bool checkPhysician(int physicianId)
        {
            var data = _context.Physicians.Any(x => x.PhysicianId == physicianId);
            return data;
        }
        #endregion

        #region checkRole
        public bool checkRole(int roleId)
        {
            var data = _context.Roles.Any(z => z.RoleId == roleId);
            return data;
        }
        #endregion

        #region checkUser
        public bool checkUser(int userId)
        {
            var data = _context.Users.Any(x => x.UserId == userId);
            return data;
        }
        #endregion

        #region forCountRequest
        public IQueryable<Request> forCountRequestInAdmin => _context.Requests;
        #endregion

        #region reqwuestDataAdmin
        public List<RequestListAdminDash> requestDataAdmin(string statusarray, int[] Status, string reqTypeId, int regionId, string searchdata)
        {

            //var requestTypeId = _context.Requests.Where(o => o.RequestTypeId == reqTypeId);
            var requestList = _context.Requests.Where(o => statusarray.Contains(o.Status.ToString()));
            //List<DAL.ViewModel.CaseTag> caseTag = new List<DAL.ViewModel.CaseTag>();
            if(searchdata != null)
            {
                searchdata = searchdata.ToLower();
                requestList = requestList.Where(x => x.FirstName.ToLower().Contains(searchdata) || x.LastName.ToLower().Contains(searchdata));
            }
            if (reqTypeId != "0" && reqTypeId != null)
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

                //DateOfService = r.RequestStatusLogs.Select(x => x.CreatedDate).First(),
                DateOfBirth = r.RequestClients.Select(x => x.IntDate).First() == null ? null : r.RequestClients.Select(x => x.IntDate).First() + "/" + r.RequestClients.Select(x => x.StrMonth).First() + "/" + r.RequestClients.Select(x => x.IntYear).First(),
                rPhonenumber = r.PhoneNumber,
                RequestTypeId = r.RequestTypeId,

            }).ToList();
            return GetRequestData;
        }
        #endregion

        #region requestDataDownloadExcelAll
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
		#endregion

		#region getRegions
		public List<Region> getRegions()
        {
            var data = _context.Regions.ToList();
            return data;
        }
		#endregion

		#region ViewCase
		public ViewCaseVm ViewCase(int requestId)
        {

                var request = _context.Requests.Where(o => o.RequestId == requestId);
                var user = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);
                var reqtype = _context.Requests.FirstOrDefault(x => x.RequestId == requestId).RequestTypeId;

                var region = _context.Regions.FirstOrDefault(x => x.RegionId == user.RegionId)?.Name ?? "Unknown";
                int intYear = user?.IntYear?? 1;
                int intDate = user?.IntDate?? 1;
                string month = user.StrMonth ?? "Jan";
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
                    Address = user.Address,
                };
                return viewCaseVm;
        }
        #endregion

        #region ViewNotes
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
        #endregion

        #region editViewNotes
        public void editViewNotes(ViewNotesVm model, int requestId)
        {
            var reqnotes = _context.RequestNotes.FirstOrDefault(x => x.RequestId == requestId);
            var req = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            if (reqnotes != null)
            {
                reqnotes.AdminNotes = model.AdminNotes;
                //reqnotes.PhysicianNotes = model.PhysicianNotes;
                reqnotes.ModifiedBy = (int)_context.Users.FirstOrDefault(x => x.UserId == req.UserId).AspNetUserId;
                reqnotes.ModifiedDate = DateTime.Now;
                _context.SaveChanges();
            }

        }
        #endregion

        #region cancelCaseMain
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
        #endregion

        #region cancelCase : post data
        public void cancelCase(AdminDashboardViewModel model, int requestId)
        {
            RequestStatusLog requestStatuslog = new RequestStatusLog();
            requestStatuslog.RequestId = requestId;
            requestStatuslog.Status = 7;
            requestStatuslog.Notes = model.Notes;
            _context.Add(requestStatuslog);
            _context.SaveChanges();

            var cancel = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            cancel.Status = 7;
            cancel.CaseTag = model.CaseTagss;

            _context.SaveChanges();
        }
		#endregion

		#region asignCase
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
		#endregion

		#region asignPhysician
		public List<Physician> asignPhysician(int regionId)
        {
            var data = _context.Physicians.Where(p => p.RegionId == regionId).ToList();
            return data;
        }
		#endregion

		#region adminBlockVm
		public AdminBlockVm adminBlockVm(int requestId)
        {
            AdminBlockVm adminBlockVm = new AdminBlockVm()
            {
                RequestId = requestId,
            };
            return adminBlockVm;
        }
		#endregion

		#region blockCase : post
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
                blockreq.IsActive = true;

                _context.BlockRequests.Add(blockreq);
                _context.Requests.Update(request1);
            }
            _context.SaveChanges();
        }
        #endregion

        #region asignCasePost
        public void asignCasePost(AdminAsignVm model, int requestId)
        {
            var request = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            request.ModifiedDate = DateTime.Now;
            request.PhysicianId = model.PhysicianId;
            _context.SaveChanges();
            var reqstatus = new RequestStatusLog
            {
                RequestId = requestId,
                Status = 1,
                Notes = model.Description,
                CreatedDate = DateTime.Now,
                PhysicianId = model.PhysicianId,
            };
            _context.RequestStatusLogs.Add(reqstatus);
            _context.SaveChanges();

            reqstatus.TransToPhysicianId = model.PhysicianId;
        }
		#endregion

		#region UploadFile
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
		#endregion

		#region GetAdminViewUploadData
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
        #endregion

        #region GetFilesByRequestId
        public List<RequestWiseFile> GetFilesByRequestId(int requestId)
        {
            List<RequestWiseFile> files = _context.RequestWiseFiles.Where(u => u.RequestId == requestId).ToList();
            return files.Where(u => !u.IsDeleted[0]).ToList();
        }
		#endregion

		#region GetFileById
		public RequestWiseFile GetFileById(int fileId)
        {
            return _context.RequestWiseFiles.FirstOrDefault(u => u.RequestWiseFileId == fileId);
        }
		#endregion

		#region GetAllFilesByRequestId
		public List<RequestWiseFile> GetAllFilesByRequestId(int reqId)
        {
            return _context.RequestWiseFiles.Where(x => x.RequestId == reqId).ToList();
        }
		#endregion


		//from here 4 methods are for order details
		#region healthProfessionalTypes
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
        #endregion

        #region asignBusiness
        public List<HealthProfessional> asignBusiness(int healthProfessionId)
        {
            var data = _context.HealthProfessionals.Where(p => p.Profession == healthProfessionId).ToList();
            return data;
        }
        #endregion

        #region getVendorDetails
        public List<HealthProfessional> getVendorDetails(int vendorId)
        {
            var data = _context.HealthProfessionals.Where(x => x.VendorId == vendorId).ToList();
            return data;
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
                CreatedBy = "Admin",
            };
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }
        #endregion
        //order details are completed


        //from here this is for transfer note
        #region transferRegion
        public List<Region> transferRegion()
        {
            var data = _context.Regions.ToList();
            return data;
        }
        #endregion

        #region transferPhysician
        public List<Physician> transferPhysician(int regionId)
        {
            var data = _context.Physicians.Where(p => p.RegionId == regionId).ToList();
            return data;
        }
        #endregion

        #region transferCasePost
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
        #endregion

        //export logic
        #region ExportToExcel
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
		#endregion

		//Logic for clear case
		#region clearCasePost
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
		#endregion
		//clear case completed


		//review agreement and send agreement
		#region checkStatus
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
		#endregion

		#region reviewAgreeementSubmit
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
        #endregion

        #region reviewAgreementCancel
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
        #endregion

        //close case 
        #region closeCaseGet
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
        #endregion

        #region closeCaseEdit
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
                    Status = 9,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
            }

        }
		#endregion

		//this part is for get admin profile details
		#region getAdminDetails
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
		#endregion

		#region adminResetPassword
		public void adminResetPassword(AdminProfileVm model)
        {
            var data = _context.AspNetUsers.FirstOrDefault(x => x.Id == model.AspNetUserId);
            data.PasswordHash = model.Password;
            _context.SaveChanges();
        }
		#endregion

		#region adminEditDetails1
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
		#endregion

		#region adminEditDetails2
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
        #endregion


        //create request by admin
        #region CreateNewReq
        public bool CreateNewReq(AdminCreateRequestVm model, int adminId)
        {
            User user1 = _context.Users.FirstOrDefault(x => x.Email == model.Email);
            var admin = _context.Admins.FirstOrDefault(x => x.AdminId == adminId);

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
                user.CreatedBy = admin.AspNetUserId.ToString();
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

        //this part is for encounter form 
        #region encounterFormGetData
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
		#endregion

		#region postEncounterData
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
		#endregion
		//encounter part is completed


		//this is for get provider/physician main details
		#region getPhysicianDetails
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
		#endregion
		//provider main details completed


		//this is for edit provider/physician details : 6 methods
		#region getPhysicianDetails
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
        #endregion

        #region editPhysicianPassword
        public void editPhysicianPassword(ProviderVm model)
        {
            var aspId = _context.Physicians.FirstOrDefault(x => x.PhysicianId == model.PhysicianId).AspNetUserId;
            var passdata = _context.AspNetUsers.FirstOrDefault(x => x.Id == aspId);

            passdata.PasswordHash = model.Password;
            _context.SaveChanges();
        }
        #endregion

        #region physicianEditDetails1
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
        #endregion

        #region physicianEditDetails2
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
        #endregion

        #region physicianEditDetails3
        public void physicianEditDetails3(ProviderVm model)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == model.PhysicianId);
            physician.BusinessName = model.BusinessName;
            physician.BusinessWebsite = model.BusinessWebsite;
            //if(model.Photo != null)
            //{
            //    physician.Photo = model.Photo;
            //    _context.SaveChanges();
            //}
            //if(model.Signature != null)
            //{
            //    physician.Signature = model.Signature;
            //    _context.SaveChanges();
            //}
            string uniquefilename1 = null;
            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                string uploadfolder = Path.Combine("wwwroot", "upload");
                uniquefilename1 = $"{model.PhysicianId}" + "_Photo.jpg";
                string filePath = Path.Combine(uploadfolder, uniquefilename1);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.PhotoFile.CopyTo(stream);
                }
                physician!.Photo = uniquefilename1;
            }
            
            if (model.SignatureFile != null && model.SignatureFile.Length > 0)
            {
                string uploadfolder = Path.Combine("wwwroot", "upload");
                uniquefilename1 = $"{model.PhysicianId}" + "_Signature.jpg";
                string filePath = Path.Combine(uploadfolder, uniquefilename1);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.SignatureFile.CopyTo(stream);
                }
                physician!.Signature = uniquefilename1;
            }
            _context.SaveChanges();

        }
        #endregion

        // this is for upload document when admin edit provider account
        #region updateProviderDoc
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
		#endregion
		//edit provider details completed


		#region getPhysicianDetailsByPhysicianId
        public ProviderVm getPhysicianDetailsByPhysicianId(int phyId)
        {
            var data = _context.Physicians.FirstOrDefault(x => x.PhysicianId == phyId);
            ProviderVm providerVm = new ProviderVm()
            {
                Email = data.Email,
                SMS = data.Mobile,
            };
            return providerVm;
        }
		#endregion

		#region adminSendMailToProvider
		public void adminSendMailToProvider(ProviderVm model, string subject, string body,string ContactType)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HelloDoc2", "testinghere1008@outlook.com"));
            message.To.Add(new MailboxAddress("HelloDoc2 Member", model.Email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;


            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);
                client.Authenticate("testinghere1008@outlook.com", "Simple@12345");
                client.Send(message);
                client.Disconnect(true);
            }

            if (ContactType == "Email")
            {
                var email = _context.EmailLogs.Any(x => x.EmailId == model.Email && x.SubjectName == subject);
                if (email)
                {
                    var data = _context.EmailLogs.FirstOrDefault(j => j.EmailId == model.Email);
                    data.SentTries += 1;
                    _context.SaveChanges();
                }
                else
                {
                    

                    EmailLog emailLog = new EmailLog();
                    emailLog.EmailId = model.Email;
                    emailLog.SubjectName = subject;
                    emailLog.EmailTemplate = body;
                    _context.EmailLogs.Add(emailLog);   
                    _context.SaveChanges();
                }
            }
            else if(ContactType == "SMS")
            {
                var sms = _context.Smslogs.Any(x => x.MobileNumber == model.SMS);
                if (sms)
                {
                    var data = _context.Smslogs.FirstOrDefault(j => j.MobileNumber == model.SMS);
                    data.SentTries += 1;
                    data.SentDate = DateOnly.FromDateTime(DateTime.Now);
                    _context.SaveChanges();
                }
                else
                {
                    Smslog smsLog = new Smslog();
                    smsLog.MobileNumber = model.SMS;
                    smsLog.Smstemplate = model.Description;
                    smsLog.SentTries = 1;
                    smsLog.CreateDate = DateOnly.FromDateTime(DateTime.Now);
                    smsLog.SentDate = DateOnly.FromDateTime(DateTime.Now);
                    _context.Smslogs.Add(smsLog);
                    _context.SaveChanges();
                }
            }
            else
            {
                var email = _context.EmailLogs.Any(x => x.EmailId == model.Email && x.SubjectName == subject);
                if (email)
                {
                    var data = _context.EmailLogs.FirstOrDefault(j => j.EmailId == model.Email);
                    data.SentTries += 1;
                    data.SentDate = DateOnly.FromDateTime(DateTime.Now);
                    _context.SaveChanges();
                }
                else
                {
                    EmailLog emailLog = new EmailLog();
                    emailLog.EmailId = model.Email;
                    emailLog.SubjectName = subject;
                    emailLog.EmailTemplate = body;
                    emailLog.SentTries = 1;
                    _context.EmailLogs.Add(emailLog);
                    _context.SaveChanges();
                }


                var sms = _context.Smslogs.Any(x => x.MobileNumber == model.SMS);
                if(sms)
                {
                    var data = _context.Smslogs.FirstOrDefault(j => j.MobileNumber == model.SMS);
                    data.SentTries += 1;
                    data.SentDate = DateOnly.FromDateTime(DateTime.Now);
                    _context.SaveChanges();
                }
                else
                {
                    Smslog smsLog = new Smslog();
                    smsLog.MobileNumber = model.SMS;
                    smsLog.Smstemplate = model.Description;
                    smsLog.SentTries = 1;
                    smsLog.CreateDate = DateOnly.FromDateTime(DateTime.Now);
                    smsLog.SentDate = DateOnly.FromDateTime(DateTime.Now);
                    _context.Smslogs.Add(smsLog);
                    _context.SaveChanges();
                }
            }
        }
		#endregion

		#region getRoles
		public List<Role> getRoles()
        {
            var data = _context.Roles.Where(x => x.IsDeleted == false).ToList();
            return data;
        }
        #endregion

        #region createProviderAccount
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
                CreatedDate = DateTime.UtcNow,
                CreatedBy = aspNetUser.Id,
                Status = 1,
                RegionId = model.RegionId,
                RoleId = model.RoleId,
                Photo = model.Photo,
                AspNetUserId = aspNetUser.Id,
            };
            _context.Physicians.Add(physician);
            _context.SaveChanges();


            PhysicianLocation physicianLocation = new PhysicianLocation
            {
                PhysicianId = physician.PhysicianId,
                Latitude = model.Latitude, 
                Longitude = model.Longitude,
                PhysicianName = model.FirstName + model.LastName,
                CreatedDate = DateTime.Now,
                Address = model.Address1,
            };
            _context.PhysicianLocations.Add(physicianLocation);
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

            //in this method this content is for save file
			string uniquefileName = "";

            if(model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                string uploadfolder = Path.Combine("wwwroot", "upload");
                uniquefileName = $"{physician.PhysicianId}" + "_Photo";
                string filePath = Path.Combine(uploadfolder, uniquefileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.PhotoFile.CopyTo(stream);
                }
                physician.Photo = uniquefileName;
            }
			if (model.AgreementDoc != null && model.AgreementDoc.Length > 0)
			{
				string uploadfolder = Path.Combine("wwwroot", "upload");
                uniquefileName = $"{physician.PhysicianId}" + "_AgreementDoc.pdf";
                string filePath = Path.Combine(uploadfolder, uniquefileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.AgreementDoc.CopyTo(stream);
                }
                physician.IsAgreementDoc = true;
			}

			if (model.BackgroundDoc != null && model.BackgroundDoc.Length > 0)
			{
				string uploadfolder = Path.Combine("wwwroot", "upload");
				uniquefileName = $"{model.PhysicianId}" + "_BackgroundDoc.pdf";
				string filePath = Path.Combine(uploadfolder, uniquefileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					model.BackgroundDoc.CopyTo(stream);
				}
				physician!.IsBackgroundDoc = true;
			}

			if (model.NonDisclosureDoc != null && model.NonDisclosureDoc.Length > 0)
			{
				string uploadfolder = Path.Combine("wwwroot", "upload");
				uniquefileName = $"{physician.PhysicianId}" + "_NonDisclosureDoc.pdf";
				string filePath = Path.Combine(uploadfolder, uniquefileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					model.NonDisclosureDoc.CopyTo(stream);
				}
				physician!.IsNonDisclosureDoc = true;
			}

            _context.SaveChanges();

		}
		#endregion

		//this part is for access role
		#region getMenu
		public List<Menu> getMenu()
        {
            var data = _context.Menus.ToList();
            return data;
        }
		#endregion

		#region getAspNetRoles
		public List<AspNetRole> getAspNetRoles()
        {
            var data = _context.AspNetRoles.ToList();
            return data;
        }
		#endregion

		#region getAccessRoles
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
        #endregion

        #region getAccess
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
        #endregion

        #region createRole
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
        #endregion

        //this is for edit access role
        #region editAccessRole
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
        #endregion

        #region editAccessRolePost
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
        #endregion

        #region deleteAccessRole
        public void deleteAccessRole(int RoleId)
        {
            var role = _context.Roles.FirstOrDefault(x => x.RoleId == RoleId);
            role.IsDeleted = true;
            _context.SaveChanges();
        }
        #endregion

        #region getUserAccessData
        public List<UserAccess> getUserAccessData(int RoleId)
        {
            var list1 = _context.Admins.ToList();
            var list2 = _context.Physicians.ToList();

            var data1 = list1.Select(x => new UserAccess()
            {
                AdminId = x.AdminId,
                RoleId = RoleId,
                AccountType = "Admin",
                AccountPOC = x.FirstName + " " + x.LastName,
                Phone = x.Mobile,
                Status = (int)x?.Status,
                OpenRequest = _context.Requests.Count(x => x.Status >= 2 && x.Status <=11).ToString(),
            }).ToList();

            var data2 = list2.Select(x => new UserAccess()
            {
                PhysicianId = x.PhysicianId,
                AccountType = "Physician",
                AccountPOC = x.FirstName + " " + x.LastName,
                Phone = x.Mobile,
                Status = (int)x?.Status,
                OpenRequest = _context.Requests.Count(i => i.PhysicianId == x.PhysicianId).ToString(),
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
        #endregion

        #region adminCreateAccount
        public void adminCreateAccount(AdminProfileVm model, List<int>? checkboxForAllRegion)
        {
            AspNetUser aspNetUser = new AspNetUser
            {
                UserName = model.FirstName + model.LastName,
                PasswordHash = model.Password,
                Email = model.Email,
                Phonenumber = model.Phone,
                CreatedDate = DateTime.Now,

            };
            _context.AspNetUsers.Add(aspNetUser);
            Admin admin = new Admin
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Mobile = model.Mobile,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                Zip = model.Zip,
                AltPhone = model.AlternateMobile,
                CreatedBy = aspNetUser.UserName,
                CreatedDate = DateTime.Now,
                Status = 1,
                RegionId = model.RegionId,
                RoleId = model.RoleId,
                AspNetUserId = aspNetUser.Id,
            };
            _context.Admins.Add(admin);
            _context.SaveChanges();


            if(checkboxForAllRegion != null)
            {
                foreach(var item in checkboxForAllRegion)
                {
					AdminRegion adminRegion = new AdminRegion
					{
                        AdminId = admin.AdminId,
                        RegionId = item,
					};
                    _context.AdminRegions.Add(adminRegion);
                    _context.SaveChanges();
				}
			}

            


            AspNetUserRole aspNetUserRole = new AspNetUserRole
            {
                UserId = aspNetUser.Id,
                RoleId = model.RoleId,
            };

            _context.AspNetUserRoles.Add(aspNetUserRole);
            _context.SaveChanges();
        }
        #endregion

        //this part is for partners
        #region getProfessionList 
        public List<HealthProfessionalType> getProfessionList()
        {
            var data = _context.HealthProfessionalTypes.ToList();
            return data;
        }
        #endregion getProfessionList

        #region getVendorList
        public List<Vendor> getVendorList(int Id, string searchvalue)
        {
            var list = _context.HealthProfessionals.Where(x => x.IsDeleted == false).ToList();

            if(Id != 0)
            {
                list = list.Where(i => i.Profession == Id).ToList();
            }
            if(searchvalue != null)
            {
                searchvalue = searchvalue.ToLower();
                list = list.Where(i => i.VendorName.ToLower().Contains(searchvalue)).ToList();
            }

            var data = list.Select(x => new Vendor()
            {
                VendorId = x.VendorId,
                Profession = _context.HealthProfessionalTypes.FirstOrDefault(i => i.HealthProfessionalId == x.Profession).ProfessionName,
                Email = x.Email,
                FaxNumber = x.FaxNumber,
                PhoneNumber = x.PhoneNumber,
                BusinessContact = x.BusinessContact,
                BusinessName = x.VendorName,

            }).ToList();

            return data;
        }
        #endregion getVendorList

        #region updateBusiness 
        public Vendor updateBusiness(int vendorId)
        {
            
            var data = _context.HealthProfessionals.FirstOrDefault(x => x.VendorId == vendorId);

            Vendor vendor = new Vendor
            {
                BusinessContact = data.BusinessContact,
                BusinessName = data.VendorName,
                Email = data.Email,
                FaxNumber = data.FaxNumber,
                PhoneNumber = data.PhoneNumber,
                VendorId = vendorId,
                State = data.State,
                City = data.City,
                Street = data.Address.Split(',')[0],
                Zip = data.Zip,
            };
            return vendor;
       }
        #endregion updateBusiness

        #region updateBusinessPost 
        public void updateBusinessPost(Vendor model)
        {
            var address = model.Street + ", " + model.City + ", " + model.State;
            var data = _context.HealthProfessionals.FirstOrDefault(x => x.VendorId == model.VendorId);
            data.State = model.State;
            data.City = model.City; 
            data.Email = model.Email;
            data.FaxNumber = model.FaxNumber;
            data.PhoneNumber = model.PhoneNumber;
            data.BusinessContact = model.BusinessContact;
            data.VendorName = model.BusinessName;
            data.Address = address;
            data.Zip = model.Zip;
            _context.SaveChanges();
        }
        #endregion

        #region partnersDeleteVendors
        public void partnersDeleteVendors(int vendorId)
        {
            var data = _context.HealthProfessionals.FirstOrDefault(x => x.VendorId == vendorId);
            data.IsDeleted = true;
            _context.SaveChanges();
        }
        #endregion

        #region addBusiness : post
        public void addBusiness(PartnersVm model)
        {
            var address = model.Street + ", " + model.City + ", " + model.State;
            HealthProfessional healthProfessional = new HealthProfessional()
            {
                VendorName = model.BusinessName,
                Profession = model.Profession,
                FaxNumber = model.FaxNumber,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                BusinessContact = model.BusinessContact,
                State = model.State,
                Address = address,
                City = model.City,
                Zip = model.Zip,
            };
            _context.HealthProfessionals.Add(healthProfessional);
            _context.SaveChanges();
        }
		#endregion

		//this part is for record 
		#region patientHistoryData 
        public List<PatientHistory> patientHistoryData(string firstname,string lastname, string email, string phonenumber)
        {
			var list = _context.Users.ToList();

            if(firstname != null)   
            {
                firstname = firstname.ToLower();
                list = list.Where(x => x.FirstName.ToLower().Contains(firstname)).ToList();
            }
            if(lastname != null)
            {
                lastname = lastname.ToLower();
                list = list.Where(x => x.LastName.ToLower().Contains(lastname)).ToList();
            }
            if(email != null) 
            {
                email = email.ToLower();
                list = list.Where(x => x.Email.ToLower().Contains(email)).ToList();
            }
            if(phonenumber != null)
            {
                list = list.Where(x => x.Mobile.Contains(phonenumber)).ToList();
            }

            //var address = list.Select(x => x.Street).First() + " " + list.Select(x => x.City).First() + " " + list.Select(x => x.State).First();
            var data = list.Select(x => new PatientHistory()
            { 
                PatientId = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Address = x.Street + " " + x.City + " " + x.State,
                Phone = x.Mobile,

            }).ToList();
			return data;
        }
        #endregion

        #region patientRecordExploreData
        public List<PatientRecordExplore> patientRecordExploreData(int patientId)
        {
            var list = _context.RequestClients.Include(x => x.Request).Where(j => j.Request.UserId == patientId).ToList();

            var data = list.Select(x => new PatientRecordExplore()
            {
                Client = x.FirstName,
                //for option
                //CreatedDate = x.StrMonth +" "+ x.IntDate+","+ x.IntYear,
                RequestId = x.RequestId ??0,
                DocCount = _context.RequestStatusLogs.Where(j => j.RequestId == x.RequestId).Count(),
				CreatedDate = x.Request.CreatedDate?.ToString("MMM,dd-yyyy"),
                Confirmation = x.Request.ConfirmationNumber ?? "Random Number",

                ProviderName = x.Request.PhysicianId != null ? _context.Physicians?.FirstOrDefault(d => d.PhysicianId == x.Request.PhysicianId).FirstName : "No Physician",
            }).ToList();
            return data;
        }
        #endregion

        #region emailLogData 
        public List<EmailLogs> emailLogData(int roleId,string receiverName, string email,DateOnly createdDate, DateOnly sentDate)
		{
            var list = _context.EmailLogs.Include(x => x.Role).Include(j => j.Request).ToList();

            

            //this linkq is for joining table 
            //var list = (from e in _context.EmailLogs
            //			join r in _context.Roles on e.RoleId equals r.RoleId
            //			join re in _context.Requests on e.RequestId equals re.RequestId
            //			select new { EmailId = e.EmailId, 
            //                         RoleId = r.RoleId, 
            //                         FirstName = re.FirstName,
            //                         RequestId = re.RequestId, 
            //                         SentDate = e.SentDate,
            //                         SentTries = e.SentTries,
            //                         CreateDate = e.CreateDate,
            //                         Action = e.Action,
            //				ConfirmationNumber = e.ConfirmationNumber,
            //			}).ToList();

            if (roleId != 0)
            {
                list = list.Where(x => x.RoleId == roleId).ToList();
            }
            if(receiverName != null)
            {
                receiverName = receiverName.ToLower();
                list = list.Where(x =>x.Request.FirstName.ToLower().Contains(receiverName)).ToList();
            }

            if (email != null)
            {
                email = email.ToLower();
                list = list.Where(x => x.EmailId.Contains(email)).ToList();
            }
            if(createdDate != DateOnly.MinValue)
            {
                list = list.Where(x => x.CreateDate == createdDate).ToList();
            }

            if(sentDate != DateOnly.MinValue)
            {
                list = list.Where(x => x.SentDate == sentDate).ToList();
            }

            var data = list.Select(x => new EmailLogs()
			{
                EmailLogId = (int)x.EmailLogId,
				Recipient = (_context.Requests.FirstOrDefault(i => i.RequestId == x.RequestId) == null)?"Unknown": _context.Requests.FirstOrDefault(i => i.RequestId == x.RequestId).FirstName,
				Action = x.Action ?? 1,
				RoleName = (_context.Roles.FirstOrDefault(i => i.RoleId == x.RoleId)==null)?"Admin": _context.Roles.FirstOrDefault(i => i.RoleId == x.RoleId).Name,
				Email = x.EmailId,
                CreatedDate = x.CreateDate,
                SentDate = x.SentDate ?? DateOnly.MinValue,
				Sent = x.SentTries > 0 ? true : false,
				SentTries = x.SentTries ?? 1,
				ConfirmationNumber = x.ConfirmationNumber ?? "MD0001JK45",
			}).ToList();
			return data;
		}
        #endregion

        #region smsLogData
        public List<SmsLogs> smsLogData(int roleId, string receiverName, string phoneNumber, DateOnly createdDate, DateOnly sentDate)
        
        {
            var list = _context.Smslogs.Include(x => x.Role).Include(j => j.Request).ToList();

			if (roleId != 0)
			{
				list = list.Where(x => x.RoleId == roleId).ToList();
			}
			if (receiverName != null)
			{
				receiverName = receiverName.ToLower();
				list = list.Where(x => x.Request.FirstName.ToLower().Contains(receiverName)).ToList();
			}

			if (phoneNumber != null)
			{
				list = list.Where(x => x.MobileNumber.Contains(phoneNumber)).ToList();
			}
			if (createdDate != DateOnly.MinValue)
			{
				list = list.Where(x => x.CreateDate == createdDate).ToList();
			}

			if (sentDate != DateOnly.MinValue)
			{
				list = list.Where(x => x.SentDate == sentDate).ToList();
			}

			var data = list.Select(x => new SmsLogs()
            {
                SmsLogId = (int)x.SmslogId,
				Recipient = _context.Requests.FirstOrDefault(i => i.RequestId == x.RequestId).FirstName,
				Action = x.Action,
				RoleName = _context.Roles.FirstOrDefault(i => i.RoleId == x.RoleId).Name,
                MobileNumber = x.MobileNumber,
				CreatedDate = x.CreateDate,
				SentDate = x.SentDate,
				Sent = x.SentTries > 0 ? true : false,
				SentTries = x.SentTries,
				ConfirmationNumber = x.ConfirmationNumber,
			}).ToList();
            return data;
        }
        #endregion

        #region getPhysicianLocation
        public List<PhysicianLocation> getPhysicianLocation()
        {
            var data = _context.PhysicianLocations.ToList();
            return data;
        }
        #endregion

        #region blockHistoryData
        public List<BlockHistory> blockHistoryData(string patientname, DateOnly date, string email, string phonenumber, int PageNumber)
        {
            var list = _context.BlockRequests.Include(i => i.Request).Include(j => j.Request.RequestClients).Where(y => y.IsActive == true).ToList();

            if(patientname != null)
            {
                patientname = patientname.ToLower();
                list = list.Where(x => x.Request.RequestClients.Select(i => i.FirstName).First().Contains(patientname)).ToList();
            }
            if(date != DateOnly.MinValue)
            {
                list = list.Where(x => x.CreatedDate == date).ToList();
            }
            if(email != null)
            {
                list = list.Where(x => x.Email.Contains(email)).ToList();
            }
            if(phonenumber != null)
            {
                list = list.Where(x => x.PhoneNumber.Contains(phonenumber)).ToList();
            }
            var data = list.Select(x => new BlockHistory()
            {
                RequestId = x.RequestId,
                BlockRequestId = x.BlockRequestId,
                PatientName = x.Request.RequestClients.Select(a => a.FirstName).First(),
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                CreatedDate = x.CreatedDate,
                Notes = x.Reason,
                IsActive = x.IsActive,
            }).ToList();
            return data;
        }
        #endregion

        #region unblockRequest
        public void unblockRequest(int requestId, int blockReqId)
        {
            var data = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            data.Status = 1;
            _context.SaveChanges();

            var block = _context.BlockRequests.FirstOrDefault(x => x.BlockRequestId == blockReqId);
            block.IsActive = false;
            _context.SaveChanges();
        }
        #endregion

        #region searchRecordData
        public List<SearchRecord> searchRecordData(string patientname,int status, int requesttype, string email, DateOnly fromdate, DateOnly todate,string providername, string phoneNumber, int pageNumber)
		{
            var list = (from r in _context.Requests
                        join rc in _context.RequestClients on r.RequestId equals rc.RequestId into requestGroup
                        from result in requestGroup.DefaultIfEmpty()
                        join p in _context.Physicians on r.PhysicianId equals p.PhysicianId into physicianGroup
                        from logresult in physicianGroup.DefaultIfEmpty()
                        join rn in _context.RequestNotes on r.RequestId equals rn.RequestId into notesGroup
                        from noteresult in notesGroup.DefaultIfEmpty()
                        where r.IsDeleted == false || r.IsDeleted == null
                        
                        select new
                        {
                            requestId = r.RequestId,
                            requestType = r.RequestTypeId,
                            patientName = result.FirstName == null ? "" : result.FirstName,
                            requestor = r.FirstName,
                            email = r.Email,
                            Phone = r.PhoneNumber,
                            address = result.Address,
                            zip = result.ZipCode,
                            requestStatus = r.Status,
                            physician = logresult.FirstName,
                            physicianNotes = noteresult.PhysicianNotes,
                            adminNotes = noteresult.AdminNotes,
                            patientNotes = result.Notes,
                            dateOfService = r.CreatedDate, // take accepted date instend of created date
                        }).ToList();

            if(patientname != null)
            {
                patientname = patientname.ToLower();
                list = list.Where(x => x.patientName.ToLower().Contains(patientname)).ToList();
            }

            if(requesttype != 0)
            {
                list = list.Where(x => x.requestType == requesttype).ToList();
            }

            if(fromdate != DateOnly.MinValue && todate != DateOnly.MinValue) 
            {
                list = list.Where(x => DateOnly.FromDateTime(x.dateOfService ?? DateTime.MinValue) >= fromdate && DateOnly.FromDateTime(x.dateOfService ?? DateTime.MinValue) <= todate).ToList();   
            }

            if(status != 0)
            {
                list = list.Where(x => x.requestStatus == status).ToList();
            }

            if(email != null)
            {
                list = list.Where(x => x.email.Contains(email)).ToList();
            }
            if(providername!= null)
            {
                if(list.Select(x => x.physician).First() != null)
                {
                    list = list.Where(x => x.physician.Contains(providername)).ToList();
                }
            }
            if(phoneNumber != null)
            {
                list = list.Where(x => x.Phone.Contains(phoneNumber)).ToList(); 
            }

			var data = list.Select(x => new SearchRecord()
			{
                RequestId = x.requestId,
				PatientName = x.patientName,
                Requestor = x.requestor,
                Email = x.email,
                PhoneNumber = x.Phone,
                Address = x.address,
                Zip = x.zip,
                RequestStatus = x.requestStatus.ToString(),
                Physician = x.physician,
                PhysicianNotes = x.physicianNotes,
                AdminNotes = x.adminNotes,
                PatientNotes = x.patientNotes,
                DateOfService = DateOnly.FromDateTime(x.dateOfService ??  DateTime.MinValue),
			}).ToList();
			return data;
		}
        #endregion

        #region deleteSearchRecord
        public void deleteSearchRecord(int requestId)
        {
            var data = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            data.IsDeleted = true;
            _context.SaveChanges();
        }
		#endregion

		//this part is for sechduling
		#region fetchRegionWiseProviders
		public List<Physician> fetchRegionWiseProviders(int region)
		{

			List<Physician> physicians = _context.Physicians.Where(r => region == 0 || r.RegionId == region).ToList();

			return physicians;  
		}
		#endregion

		#region GetScheduleData
		public List<ShiftDetail> GetScheduleData()
		{
			try
			{
				return _context.ShiftDetails.Include(m => m.Shift).ThenInclude(j => j.Physician).Where(m => m.IsDeleted == false).ToList();
			}
			catch { return new List<ShiftDetail> { }; }
		}
        #endregion

        #region viewShiftGetData 
        public ViewShiftVm viewShiftGetData(int ShiftDetailId)
        {
            
            var data = _context.ShiftDetails.FirstOrDefault(x => x.ShiftDetailId == ShiftDetailId);
            var shift = _context.Shifts.FirstOrDefault(h => h.ShiftId == data.ShiftId);
            ViewShiftVm viewShiftVm = new ViewShiftVm()
            {
                PhysicianId = shift.PhysicianId,
                ShiftId = shift.ShiftId,
                ShiftDetailId = data.ShiftDetailId,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                Region = _context.Regions.FirstOrDefault(i => i.RegionId == data.RegionId).Name,
                PhysicianName = _context.Physicians.FirstOrDefault(v => v.PhysicianId == shift.PhysicianId).FirstName,
                StartDate = data.ShiftDate.ToDateTime(TimeOnly.MinValue),
            };
            return viewShiftVm;
        }
        #endregion

        #region viewShiftPost
        public void viewShiftPost(ViewShiftVm model,int command)
        {
            if(command == 1)
            {
                var shiftupdate = _context.ShiftDetails.FirstOrDefault(x => x.ShiftDetailId == model.ShiftDetailId);
                if(shiftupdate.Status == 1)
                {
                    shiftupdate.Status = 0;
                    _context.SaveChanges();
                }
                else
                {
                    shiftupdate.Status = 1;
                    _context.SaveChanges();
                }
            }
            if(command == 2)
            {
                var shiftupdate = _context.ShiftDetails.FirstOrDefault(x => x.ShiftDetailId == model.ShiftDetailId);
                shiftupdate.ShiftDate = DateOnly.FromDateTime( model.StartDate);
                shiftupdate.StartTime = model.StartTime;
                shiftupdate.EndTime = model.EndTime;
                _context.SaveChanges();
            }
            if(command == 3)
            {
                var shiftupdate = _context.ShiftDetails.FirstOrDefault(x => x.ShiftDetailId == model.ShiftDetailId);
                shiftupdate.IsDeleted = true;
                _context.SaveChanges();
            }
        }
        #endregion

        #region getPhysicianByRegion
        public List<Physician> getPhysicianByRegion(int regionId)
        {
            var data = _context.Physicians.Where(x => x.RegionId == regionId).ToList();
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


            for(int i = 0; i < model.RepeatUpto; i++)
            {
                DateOnly shiftDate = model.ShiftDate.AddDays(7*i);
                foreach(DayOfWeek dow in WeekDaysList)
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
            var data = _context.ShiftDetails.Any(x => x.Shift.PhysicianId == physicianId &&  (x.ShiftDate == shiftdate) && (x.StartTime <= starttime  && x.EndTime >= starttime));
            
            if (data)
            {
                return true;
            }   
            return false;
        }
        #endregion

        #region getPhysicianList
        public List<Provider> getPhysicianList(int regionId)
        {

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            TimeOnly currentTime = new TimeOnly(DateTime.Now.Hour,DateTime.Now.Minute, DateTime.Now.Second);
            var list = _context.Physicians.Include(x => x.Shifts).ThenInclude(j => j.ShiftDetails).Where(p => p.Shifts.Any()).ToList();

            if(regionId != 0)
            {
                list = list.Where(x => x.RegionId == regionId).ToList();
            }
            var data = list.Select(x => new Provider()
            {
                Name = x.FirstName,
                StartTime = x.Shifts.Select(x => x.ShiftDetails.Select(j => j.StartTime).First()).First(),
                EndTime = x.Shifts.Select(x => x.ShiftDetails.Select(j => j.EndTime).First()).First(),
                ShiftDate = x.Shifts.Select(x => x.ShiftDetails.Select(j => j.ShiftDate).First()).First(),
                CurrentTime = currentTime,
                CurrentDate = currentDate,
			}).ToList();
            
            return data;
        }
        #endregion

        #region requestedShiftData 
        public List<RequestedShift> requestedShiftData(int regionId)
        {
            var list = _context.ShiftDetails.Where(h => h.IsDeleted == false && h.Status == 0).Include(x => x.Shift).ToList();

            if(regionId != 0)
            {
                list = list.Where(x => x.RegionId == regionId).ToList();
            }
            var data = list.Select(x => new RequestedShift()
            {
                Staff = _context.Physicians.FirstOrDefault(g => g.PhysicianId == x.Shift.PhysicianId).FirstName + _context.Physicians.FirstOrDefault(f => f.PhysicianId == x.Shift.PhysicianId).LastName,
                Region = _context.Regions.FirstOrDefault(j => j.RegionId == x.RegionId).Name,
                Day = x.ShiftDate,
                Time = x.StartTime + "-" + x.EndTime,
                ShiftDetailId = x.ShiftDetailId,
            }).ToList();
            return data;
        }
        #endregion

        #region deleteSelectedShift
        public bool deleteSelectedShift(List<int> shiftDetailId)
        {
            if(shiftDetailId.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var deleteShiftId in shiftDetailId)
                {
                    var shiftdetail = _context.ShiftDetails.FirstOrDefault(x => x.ShiftDetailId == deleteShiftId);
                    try
                    {
                        shiftdetail.IsDeleted = true;
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting file {deleteShiftId}: {ex.Message}");
                        // Log the error for further investigation
                    }
                }
                return true;
            }
        }
        #endregion

        #region approveSelectedShift
        public bool approveSelectedShift(List<int> shiftDetailId)
        {
            if(shiftDetailId.Count == 0)
            {
                return false;
            }
            else
            {
                foreach(var approveShiftId in shiftDetailId)
                {
                    var shiftdetail = _context.ShiftDetails.FirstOrDefault(x => x.ShiftDetailId == approveShiftId);
                    try
                    {
                        shiftdetail.Status = 1;
                        _context.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error approving file {approveShiftId}: {ex.Message}");
                    }
                }
                return true;
            }
        }
        #endregion

    }
}

