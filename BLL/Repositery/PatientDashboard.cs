using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using DAL.ViewModelProvider;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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

        public List<TEntity> GetAllData<TEntity>(Expression<Func<Chat, TEntity>> select, Expression<Func<Chat, bool>> where, Expression<Func<Chat, dynamic>> orderby) where TEntity : class
        {

            return _context.Chats.Where(where).OrderBy(orderby).Select(select).ToList();
        }

        #region patientDashboardMain
        public List<PatientData> patientDashboardMain(int? uid)
        {
            var applicationDbContext = (from r in _context.Requests
                                        where r.UserId == uid
                                        select new
                                        {
                                            r.RequestId,
                                            r.CreatedDate,
                                            r.Status,
                                            r.PhysicianId,
                                        }).ToList();

            List<PatientData> list = new List<PatientData>();
            foreach (var item in applicationDbContext)
            {
                //ViewBag.reqId = item.RequestId;
                var count = _context.RequestWiseFiles.Where(o => o.RequestId == item.RequestId).Count();
                var reqwisefile = _context.RequestWiseFiles.FirstOrDefault(x => x.RequestId == item.RequestId);
                string createddate = item.CreatedDate.ToString();

                PatientData user = new PatientData();
                user.CreatedDate = createddate;
                user.PhysicianId = item.PhysicianId;
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
        #endregion

        #region patientDashboardProfile
        public PatientProfile patientDashboardProfile(int? userID)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userID);

            int intYear = user?.IntYear ?? 2000;
            int intDate = user?.IntDate ?? 10;
            string month = (string)user?.StrMonth ?? "Jan";
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
        #endregion

        #region editPatientProfile
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
        #endregion

        #region GetRequestById
        public Request GetRequestById(int? requestId)
        {
            return _context.Requests.FirstOrDefault(u => u.RequestId == requestId);
        }
        #endregion

        #region GetFilesByRequestId
        public List<RequestWiseFile> GetFilesByRequestId(int requestId)
        {
            return _context.RequestWiseFiles.Where(u => u.RequestId == requestId).ToList();
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

        #region UploadFile
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
        #endregion



        public ChatViewModel GetProviderChatDetails(int ProviderId, int RequestId, int? roleId)
        {
            if (ProviderId != 0)
            {
                Physician physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == ProviderId);
                ChatViewModel chatViewModel = new()
                {
                    ProviderId = ProviderId,
                    RequestId = RequestId,
                    ProviderName = physician.FirstName + " " + physician.LastName,
                    ProviderPhoto = physician.Photo,
                    ProviderAspNetUserId = physician.AspNetUserId,
                };

                Expression<Func<Chat, bool>> whereClauseSyntext = PredicateBuilder.New<Chat>();
                whereClauseSyntext = x => x.ProviderId == ProviderId && x.RequestId == RequestId;
                var datatable = GetAllData(x => new ChatViewModel
                {
                    Message = x.Message,
                    sentBy = x.SentBy ?? 0,
                    sentDate = x.CreatedDate,
                }, whereClauseSyntext, x => x.CreatedDate);
                chatViewModel.ListOfChats = new List<ChatViewModel>();
                foreach (ChatViewModel chat in datatable)
                {
                    if (chat.sentBy == roleId)
                    {
                        chat.IsSender = true;
                    }
                    chatViewModel.ListOfChats.Add(chat);
                }

                return chatViewModel;
            }
            return new ChatViewModel();
        }

        public ChatViewModel GetAdminChatDetails(int AdminId, int RequestId, int? roleId)
        {
            if (AdminId != 0)
            {
                Admin admin = _context.Admins.FirstOrDefault(x => x.AdminId == AdminId);
                ChatViewModel chatViewModel = new()
                {
                    AdminId = AdminId,
                    AdminAspNetUserId = admin.AspNetUserId,
                    RequestId = RequestId,
                    AdminName = admin.FirstName + " " + admin.LastName,
                };
                Expression<Func<Chat, bool>> whereClauseSyntext = PredicateBuilder.New<Chat>();
                whereClauseSyntext = x => x.AdminId == AdminId && x.RequestId == RequestId;
                var datatable = GetAllData(x => new ChatViewModel
                {
                    Message = x.Message,
                    sentBy = x.SentBy ?? 0,
                    sentDate = x.CreatedDate,
                }, whereClauseSyntext, x => x.CreatedDate);
                chatViewModel.ListOfChats = new List<ChatViewModel>();
                foreach (ChatViewModel chat in datatable)
                {
                    if (chat.sentBy == roleId)
                    {
                        chat.IsSender = true;
                    }
                    chatViewModel.ListOfChats.Add(chat);
                }

                return chatViewModel;
            }
            return new ChatViewModel();
        }
    }
}
