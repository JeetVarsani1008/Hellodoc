using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<RequestListAdminDash> requestDataAdmin(int[] Status, string reqTypeId)
        {
            //var requestTypeId = _context.Requests.Where(o => o.RequestTypeId == reqTypeId);
            var requestList = _context.Requests.Where(o => Status.Contains(o.Status));
            List<DAL.ViewModel.CaseTag> caseTag = new List<DAL.ViewModel.CaseTag>();

            if (reqTypeId != null)
            {
                requestList = requestList.Where(o => o.RequestTypeId.ToString() == reqTypeId);
            }
            //if(RegionId != 0)
            //{
            //    //var requestdata = _context.RequestClients.Where(i => i.RegionId == RegionId);
            //    //requestList = requestList.Where(i => i.RequestId == requestdata.Select(u => u.RequestId).First());

            //    requestList = requestList.Where(i => i.RequestClients.Select(r => r.RegionId.ToString()).Contains(RegionId.ToString()));

            //}
            var GetRequestData = requestList.Select(r => new RequestListAdminDash() {

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

        public List<RequestListAdminDash> ViewCase(int requestId)
        {

            //var requestTypeId = _context.Requests.Where(o => o.RequestTypeId == reqTypeId);
            var requestList = _context.Requests.Where(o => o.RequestId == requestId);
            var user = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);

            int intYear = (int)user.IntYear;
            int intDate = (int)user.IntDate;
            string month = (string)user.StrMonth;
            DateTime birthdate = new DateTime(intYear, DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month, intDate);
            var GetRequestData = requestList.Select(r => new RequestListAdminDash()
            {
                
                RequestId = r.RequestId,
                FirstName = r.RequestClients.Select(x => x.FirstName).First(),
                LastName = r.RequestClients.Select(x => x.LastName).First(),
                Email = r.Email,
                Name = r.RequestClients.Select(x => x.FirstName).First() + " " + r.RequestClients.Select(x => x.LastName).First(),
                Requestor = r.FirstName + " " + r.LastName,
                RequestDate = r.CreatedDate,
                Phone = r.PhoneNumber,
                Address = r.RequestClients.Select(x => x.Street).First() + "," + r.RequestClients.Select(x => x.City).First() + "," + r.RequestClients.Select(x => x.State).First(),
                Notes = r.RequestClients.Select(x => x.Notes).First(),
                ChatWith = r.PhysicianId.ToString(),
                Physician = r.Physician.FirstName,
                Status = r.Status,
               
                rPhonenumber = r.PhoneNumber,
                RequestTypeId = r.RequestTypeId,

            }).ToList();
            return GetRequestData;
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
            if(reqnotes != null)
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

        public void cancelCase(AdminDashboardViewModel model,int requestId)
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
            request.ModifiedDate = DateTime.UtcNow;
            //_context.SaveChanges();
            var reqstatus = new RequestStatusLog
            {
                RequestId = requestId,
                Status = (short)newStatus,
                Notes = model.Description,
                CreatedDate = DateTime.Now,
                PhysicianId = model.PhysicianId,
            };
            if(newStatus == 2)
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


        public AdminViewUploadVm GetAdminViewUploadData(AdminViewUploadVm model,int requestId)
        {
            var client = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId);
            if(client == null)
            {
                return new AdminViewUploadVm();
            }
            var mymodel = new AdminViewUploadVm
            {
                RequestId=requestId,
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
            if(data)
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
            if(data != null)
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
    }
}
