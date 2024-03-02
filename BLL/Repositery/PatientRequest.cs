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
using System.Net;
using DAL.Models;
using HellodocContext = DAL.Models.HellodocContext;
namespace BLL.Repositery
{
    public class PatientRequest : IPatientRequest
    {
        private readonly HellodocContext _context;

        public PatientRequest(HellodocContext context)
        {
            _context = context;
        }

        public void patientRequestForm(PatientData model) {

            AspNetUser aspNetUser = new AspNetUser();

            User user = new User();

            Request request = new Request();

            RequestClient requestClient = new RequestClient();

            RequestStatusLog requestStatusLog = new RequestStatusLog();


            RequestWiseFile requestWiseFile = new RequestWiseFile();

            var existUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);


            if (existUser == null)
            {
                aspNetUser.UserName = model.FirstName;
                aspNetUser.Email = model.Email;
                aspNetUser.Phonenumber = model.Phone;
                aspNetUser.CreatedDate = DateTime.Now;
                aspNetUser.PasswordHash = model.PasswordHash;
                _context.AspNetUsers.Add(aspNetUser);
                _context.SaveChanges();

                user.AspNetUserId = aspNetUser.Id;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Street = model.Street;
                user.City = model.City;
                user.State = model.State;
                user.ZipCode = model.ZipCode;
                user.CreatedBy = "1";
                user.Mobile = model.Phone;
                user.CreatedDate = DateTime.Now;
                //user.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(model.BirthDate.Month);
                //user.IntYear = model.BirthDate.Year;
                //user.IntDate = model.BirthDate.Day;
                _context.Users.Add(user);
                _context.SaveChanges();
                request.UserId = user.UserId;
            }

            request.RequestTypeId = 1;
            request.FirstName = model.FirstName;
            request.LastName = model.LastName;
            request.PhoneNumber = model.Phone;
            request.Email = model.Email;
            request.Status = 1;
            request.UserId = user.UserId;
            request.CreatedDate = DateTime.Now;
            _context.Requests.Add(request);
            _context.SaveChanges();

            requestClient.RequestId = request.RequestId;
            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.PhoneNumber = model.Phone;
            requestClient.Email = model.Email;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Status = 3;
            requestStatusLog.Notes = model.Symptoms;
            requestStatusLog.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();

            if (model.Filepath != null)
            {

                IFormFile SingleFile = model.Filepath;
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.CreatedDate = DateTime.Now;
                requestWiseFile.FileName = SingleFile.FileName;
                _context.Add(requestWiseFile);
                _context.SaveChanges();
                var filePath = Path.Combine("wwwroot", "upload", Path.GetFileName(SingleFile.FileName));
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    // The file is saved in a buffer before being processed
                    SingleFile.CopyTo(stream);
                }
            }
            User user1 = _context.Users.Where(s => s.Email == model.Email).FirstOrDefault();

            if (user1 != null)
            {
                List<Request> requests = _context.Requests.Where(x => x.Email == model.Email).ToList();
                requests.ForEach(x => x.UserId = user1.UserId);
                _context.SaveChanges();
            }
        }

        public void familyRequestForm(FamilyData familyData) 
        {

            Request request = new Request();

            RequestClient requestClient = new RequestClient();

            RequestStatusLog requestStatusLog = new RequestStatusLog();

            RequestWiseFile requestWiseFile = new RequestWiseFile();

            request.RequestTypeId = 2;
            request.FirstName = familyData.F_FirstName;
            request.LastName = familyData.F_LastName;
            request.Email = familyData.F_Email;
            request.CreatedDate = DateTime.Now;
            _context.Requests.Add(request);
            _context.SaveChanges();



            requestClient.RequestId = request.RequestId;
            requestClient.FirstName = familyData.FirstName;
            requestClient.LastName = familyData.LastName;
            requestClient.PhoneNumber = familyData.PhoneNumber;
            requestClient.Email = familyData.Email;

            requestClient.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(familyData.Birthdate.Month);
            requestClient.IntYear = familyData.Birthdate.Year;
            requestClient.IntDate = familyData.Birthdate.Day;

            requestClient.Street = familyData.Street;
            requestClient.City = familyData.City;
            requestClient.State = familyData.State;
            requestClient.ZipCode = familyData.ZipCode;

            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            //requestStatusLog.RequestStatusLogId = 1;
            requestStatusLog.RequestId = request.RequestId;
            requestStatusLog.Notes = familyData.Symptoms;
            requestStatusLog.Status = 2;
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();

            var email = _context.Users.FirstOrDefault(o => o.Email == familyData.Email);
            if (email == null) { 
                
            }
        }

        public void conciergeRequestForm(ConciergeData model) 
        {
            Request request = new Request();

            RequestClient requestClient = new RequestClient();

            RequestWiseFile requestWiseFile = new RequestWiseFile();

            Concierge concierge = new Concierge();

            request.FirstName = model.C_FirstName;
            request.LastName = model.C_LastName;


            concierge.ConciergeName = model.C_FirstName + " " + model.C_LastName;
            concierge.Address = model.Street + ", " + model.City + " " + model.State + " " + model.ZipCode;
            concierge.Street = model.Street;
            concierge.City = model.City;
            concierge.State = model.State;
            concierge.ZipCode = model.ZipCode;
            concierge.CreatedDate = DateTime.Now;
            _context.Concierges.Add(concierge);
            _context.SaveChanges();


            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.Email = model.Email;
            requestClient.PhoneNumber = model.Phone;
            requestClient.StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(model.Birthdate.Month);
            requestClient.IntYear = model.Birthdate.Year;
            requestClient.IntDate = model.Birthdate.Day;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

        }


        public void businessRequestForm(BusinessData model) 
        {
            Request request = new Request();

            RequestClient requestClient = new RequestClient();

            RequestWiseFile requestWiseFile = new RequestWiseFile();

            Business business = new Business();


            business.Name = model.B_FirstName + " " + model.B_LastName;
            business.CreatedDate = DateTime.Now;
            _context.Businesses.Add(business);
             _context.SaveChanges();


            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.Email = model.Email;
            requestClient.PhoneNumber = model.Phone;
            requestClient.Street = model.Street;
            requestClient.City = model.City;
            requestClient.State = model.State;
            requestClient.ZipCode = model.ZipCode;
            _context.RequestClients.Add(requestClient);
             _context.SaveChanges();

            request.FirstName = model.B_FirstName;
            request.LastName = model.B_LastName;
            request.Email = model.B_Email;
            request.PhoneNumber = model.B_Phone;
            _context.Requests.Add(request);
             _context.SaveChanges();
        }


        public void createRequestForMe(PatientRequestForMeAndSomeone model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            Request request = new Request
            {
                UserId = user.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedDate = DateTime.Now,
                Status = 4,
                PhoneNumber = model.Phone,
                Email = model.Email,
            };
            _context.Requests.Add(request);
            _context.SaveChanges();

            RequestClient requestClient = new RequestClient
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                Email = model.Email,
                IntDate = model.BirthDate.Day,
                IntYear = model.BirthDate.Year,
                StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(model.BirthDate.Month),
                Street = model.Street,
                State = model.State,
                City = model.City,
                ZipCode = model.ZipCode,
                RequestId = request.RequestId,

            };
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            RequestStatusLog requestStatusLog = new RequestStatusLog
            {
                RequestId = request.RequestId,
                Status = request.Status,
                CreatedDate = DateTime.Now,
                Notes = model.Comments,

            };
            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();

            RequestWiseFile requestWiseFile = new RequestWiseFile();

            if (model.Filepath != null)
            {

                IFormFile SingleFile = model.Filepath;
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.CreatedDate = DateTime.Now;
                requestWiseFile.FileName = SingleFile.FileName;
                _context.Add(requestWiseFile);
                _context.SaveChanges();
                var filePath = Path.Combine("wwwroot", "upload", Path.GetFileName(SingleFile.FileName));
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    // The file is saved in a buffer before being processed
                    SingleFile.CopyTo(stream);
                }
            }
        }

        public PatientRequestForMeAndSomeone PatientRequestForMe(int? userID)
        {

            var user = _context.Users.FirstOrDefault(u => u.UserId == userID);

            int intYear = (int)user.IntYear;
            int intDate = (int)user.IntDate;
            string month = (string)user.StrMonth;
            DateTime date = new DateTime(intYear, DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month, intDate);
            PatientRequestForMeAndSomeone patientRequestForMe = new PatientRequestForMeAndSomeone()
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

            };
            return patientRequestForMe;
        }


        public void createRequestForSomeone(PatientRequestForMeAndSomeone model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            Request request = new Request
            {

                UserId = user.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedDate = DateTime.Now,
                Status = 4,
                PhoneNumber = model.Phone,
                Email = model.Email,
            };
            _context.Requests.Add(request);
            _context.SaveChanges();



            RequestClient requestClient = new RequestClient
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.Phone,
                Email = model.Email,
                IntDate = model.BirthDate.Day,
                IntYear = model.BirthDate.Year,
                StrMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(model.BirthDate.Month),
                Street = model.Street,
                State = model.State,
                City = model.City,
                ZipCode = model.ZipCode,
                RequestId = request.RequestId,

            };
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            RequestWiseFile requestWiseFile = new RequestWiseFile();

            if (model.Filepath != null)
            {

                IFormFile SingleFile = model.Filepath;
                requestWiseFile.RequestId = request.RequestId;
                requestWiseFile.CreatedDate = DateTime.Now;
                requestWiseFile.FileName = SingleFile.FileName;
                _context.Add(requestWiseFile);
                _context.SaveChanges();
                var filePath = Path.Combine("wwwroot", "upload", Path.GetFileName(SingleFile.FileName));
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    // The file is saved in a buffer before being processed
                    SingleFile.CopyTo(stream);
                }
            }
        }
    }
}
