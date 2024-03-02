using BLL.Repositery;
using DAL.Models;
using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface ILogin
    {

        //this is login part 
        bool ValidateLogin(LoginVm loginVm);

        //this is email sender and forgot password and reset password part
        AspNetUser GetUserByEmail(string email);
        void UpdateUserPassword(AspNetUser user, string newPassword);
        void SaveChanges();

        Task SendEmailAsync(string toEmail, string subject, string body);

        string GenerateToken();
        DateTime GetTokenExpiration();

        //create account for family friends part

        AspNetUser getAspUser(LoginVm loginVm);

        User getUser(LoginVm loginVm);

        RequestClient getRequestClient(LoginVm loginVm);

        void addNewUserData(LoginVm loginVm, RequestClient requestClient);
    }
}
