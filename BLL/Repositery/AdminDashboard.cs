using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public List<RequestListAdminDash> requestDataAdmin(int Status, string reqTypeId, int RegionId)
        {
            //var requestTypeId = _context.Requests.Where(o => o.RequestTypeId == reqTypeId);
            var requestList = _context.Requests.Where(o => o.Status == Status);
            List<DAL.ViewModel.CaseTag> caseTag = new List<DAL.ViewModel.CaseTag>();

            if (reqTypeId != null)
            {
                requestList = requestList.Where(o => o.Status == Status && o.RequestTypeId.ToString() == reqTypeId);
            }
            if(RegionId != 0)
            {
            var requestdata = _context.RequestClients.Where(i => i.RegionId == RegionId);
            requestList = requestList.Where(i => i.RequestId == requestdata.Select(u => u.RequestId).First());

            }
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
                blockreq.RequestId = obj.RequestId.ToString();
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
            var user = _context.Requests.FirstOrDefault(x => x.RequestId == requestId);
            if(user == null)
            {
                return new AdminViewUploadVm();
            }
            var mymodel = new AdminViewUploadVm
            {
                RequestId=requestId,
                Name = user.FirstName + " " + user.LastName,
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
    }
}
