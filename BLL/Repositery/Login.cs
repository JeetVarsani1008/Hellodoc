using BLL.Interface;
using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using MimeKit;
using System.Security.Cryptography;

namespace BLL.Repositery
{
    public class Login : ILogin
    {
        private readonly HellodocContext _context;

        public Login(HellodocContext context)
        {
            _context = context;
        }

        //login for patient
        #region patientLogin
        public AspNetUser patientLogin(LoginVm loginVm)
        {
            var data = _context.AspNetUsers.FirstOrDefault(u => u.Email == loginVm.Email && u.PasswordHash == loginVm.PasswordHash);
            return data;
        }
        #endregion


        //login for admin
        #region adminLogin
        public AspNetUser adminLogin(LoginVm model)
        {
            var data = _context.AspNetUsers.FirstOrDefault(x => x.Email == model.Email && x.PasswordHash == model.PasswordHash);
                return data;
        }
        #endregion

        #region findAspNetRole
        public AspNetUserRole findAspNetRole(AspNetUser user)
        {
             var admindata = _context.AspNetUserRoles.FirstOrDefault(x => x.UserId == user.Id);
            return admindata;
        }
        #endregion

        //this is send mail and reset password and forgot password part
        #region GetUserByEmail
        public AspNetUser GetUserByEmail(string email) {
            return _context.AspNetUsers.FirstOrDefault(o => o.Email == email);
        }
        #endregion

        #region UpdateUserPassword
        public void UpdateUserPassword(AspNetUser user, string newPassword) {
            user.PasswordHash = newPassword;
            _context.AspNetUsers.Update(user);
        }
        #endregion

        #region SaveChanges
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        #endregion

        #region SendEmailAsync
        public async Task SendEmailAsync(string toEmail, string subject, string body) 
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HelloDoc2", "testinghere1008@outlook.com"));
            message.To.Add(new MailboxAddress("HelloDoc2 Member", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;


            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.office365.com", 587, false);
                await client.AuthenticateAsync("testinghere1008@outlook.com", "Simple@12345");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        #endregion

        private const int TokenExpirationHours = 24;

        #region GenerateToken
        public string GenerateToken()
        {
            byte[] tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            string token = Convert.ToBase64String(tokenBytes);

            return token;
        }
        #endregion

        #region GetTokenExpiration
        public DateTime GetTokenExpiration()
        {
            return DateTime.UtcNow.AddHours(TokenExpirationHours);
        }
        #endregion

        //create account for family/friend part
        #region getAspUser
        public AspNetUser getAspUser(LoginVm loginVm) {
            return _context.AspNetUsers.FirstOrDefault(o => o.Email == loginVm.Email);
        }
        #endregion

        #region addNewUserData
        public void addNewUserData(LoginVm loginVm, RequestClient requestClient)
        {
            AspNetUser newAspNetUser = new AspNetUser();
            User newuser = new User();
            newAspNetUser.UserName = loginVm.Email;
            newAspNetUser.Email = loginVm.Email;
            newAspNetUser.Phonenumber = requestClient.PhoneNumber;
            newAspNetUser.PasswordHash = loginVm.PasswordHash;
            _context.Add(newAspNetUser);
            _context.SaveChanges();

            newuser.FirstName = requestClient.FirstName;
            newuser.LastName = requestClient.LastName;
            newuser.City = requestClient.City;
            newuser.Street = requestClient.Street;
            newuser.Mobile = requestClient.PhoneNumber;
            newuser.Email = requestClient.Email;
            newuser.State = requestClient.State;
            newuser.AspNetUserId = newAspNetUser.Id;
            newuser.ZipCode = requestClient.ZipCode;
            _context.Add(newuser);
            _context.SaveChanges();

            Request request = _context.Requests.FirstOrDefault(x => x.RequestId == requestClient.RequestId);
            if (request != null)
            {
                request.UserId = newuser.UserId;
                _context.Update(request);
            }
        }
        #endregion

        #region getUser
        public User getUser(LoginVm loginVm) {
            return _context.Users.FirstOrDefault(o => o.Email == loginVm.Email);
        }
        #endregion

        #region getRequestClient
        public RequestClient getRequestClient(LoginVm loginVm) {
            return _context.RequestClients.FirstOrDefault(o => o.Email == loginVm.Email);
        }
        #endregion
    }
}
