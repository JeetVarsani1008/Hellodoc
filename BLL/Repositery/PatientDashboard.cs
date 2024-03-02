using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repositery
{
    public class PatientDashboard : IPatientDashboard
    {
        private readonly HellodocContext _context;
        public PatientDashboard(HellodocContext context)
        {
            _context = context;
        }
        public List<PatientData> patientDashboardMain(int? uid)
        {
            var applicationDbContext = (from r in _context.Requests
                                        where r.UserId == uid
                                        select new
                                        {
                                            r.RequestId,
                                            r.CreatedDate,
                                            r.Status,
                                        }   

                                        ).ToList();

            List<PatientData> list = new List<PatientData>();
            foreach (var item in applicationDbContext)
            {
                //ViewBag.reqId = item.RequestId;
                var count = _context.RequestWiseFiles.Where(o => o.RequestId == item.RequestId).Count();
                var reqwisefile = _context.RequestWiseFiles.FirstOrDefault(x => x.RequestId == item.RequestId);
                string createddate = item.CreatedDate.ToString();

                PatientData user = new PatientData();
                user.CreatedDate = createddate;
                if (item.Status == 1)
                {
                    user.Status = "Unassigned";
                }
                else if (item.Status == 2)
                {
                    user.Status = "Accepted";
                }
                else if (item.Status == 3)
                {
                    user.Status = "Cancelled";
                }
                else if (item.Status == 4)
                {
                    user.Status = "Reserving";
                }
                user.Count = count;

                user.Id = item.RequestId;
                list.Add(user);
            }
            return list;

        }

        public PatientProfile patientDashboardProfile(int? userID)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userID);

            int intYear = (int)user.IntYear;
            int intDate = (int)user.IntDate;
            string month = (string)user.StrMonth;
            DateTime date = new DateTime(intYear, DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month, intDate);
            PatientProfile patientProfile = new PatientProfile()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Street = user.Street,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Phone = user.Mobile,
                BirthDate = date,
                Address = user.Street + user.City + user.State,

            };
            return patientProfile;
        }


        public void editPatientProfile(PatientProfile model, int? userid)
        {
            var existUser = _context.Users.FirstOrDefault(u => u.UserId == userid);
            var aspnetuser = _context.AspNetUsers.FirstOrDefault(u => u.Id == existUser.AspNetUserId);

            if (userid != null)
            {
                if (existUser != null)
                {
                    existUser.FirstName = model.FirstName;
                    existUser.LastName = model.LastName;
                    existUser.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(model.BirthDate.Month);
                    existUser.IntYear = model.BirthDate.Year;
                    existUser.IntDate = model.BirthDate.Day;
                    existUser.Email = model.Email;
                    existUser.Mobile = model.Phone;
                    existUser.State = model.State;
                    existUser.ZipCode = model.ZipCode;
                    existUser.Street = model.Street;
                    existUser.City = model.City;
                    _context.SaveChanges();

                }
                if (aspnetuser != null)
                {
                    aspnetuser.Email = model.Email;
                    _context.SaveChanges();
                }
            }

        }

        public Request GetRequestById(int? requestId)
        {
            return _context.Requests.FirstOrDefault(u => u.RequestId == requestId);
        }

        public List<RequestWiseFile> GetFilesByRequestId(int requestId)
        {
            return _context.RequestWiseFiles.Where(u => u.RequestId == requestId).ToList();
        }

        public RequestWiseFile GetFileById(int fileId)
        {
            return _context.RequestWiseFiles.FirstOrDefault(u => u.RequestWiseFileId == fileId);
        }

        public List<RequestWiseFile> GetAllFilesByRequestId(int reqId)
        {
            return _context.RequestWiseFiles.Where(x => x.RequestId == reqId).ToList();
        }

        public void UploadFile(int requestId, string fileName)
        {
            RequestWiseFile requestWiseFile = new RequestWiseFile()
            {
                RequestId = requestId,
                FileName = fileName,
            };
            _context.Add(requestWiseFile);
            _context.SaveChanges();
        }

    }
}
