using BLL.Interface;
using BLL.Repositery;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DAL.Controllers
{
	public class LoginController : Controller
	{
		private readonly HellodocContext _context;
        private readonly ILogin _login;
        private readonly IJWT _jwt;
		
		public LoginController(HellodocContext context,ILogin login,IJWT jwt)
		{
			_context = context;
            _login = login;
            _jwt = jwt;
		}

        #region Login
        public IActionResult Login()
		{
            Response.Cookies.Delete("Jwt");
            Response.Cookies.Delete("RoleMenu");
            Response.Cookies.Delete(".AspNetCore.Session");
            Response.Cookies.Delete(".AspNetCore.Antiforgery.N0iw8MAOgzI");
            return View();
		}
        #endregion

        #region Login : Post
        [HttpPost]
        public IActionResult Login(LoginVm loginVm)
        {
            var netuser = _context.AspNetUsers.FirstOrDefault(x => x.Email == loginVm.Email);
            if (netuser != null)
            {
                var Id = netuser.Id;
                HttpContext.Session.SetInt32("AspId", Id);
            }
            else
            {
                TempData["error"] = "Email Does Not Exist";
                return View();
            }


            var admin = _context.Admins.FirstOrDefault(x => x.Email == loginVm.Email);
            var physician = _context.Physicians.FirstOrDefault(x => x.Email == loginVm.Email);

            if (admin == null && physician == null)
            {
                TempData["error"] = "This Email Has No Access To This Site!";
                return View();
            }

            if (admin != null )
            {
                var adminId = admin.AdminId;
                HttpContext.Session.SetInt32("AdminId", adminId);
                var name = _context.Admins.FirstOrDefault(x => x.Email == loginVm.Email).FirstName;
                HttpContext.Session.SetString("adminName", name);
            }
            else
            {
                var physicianId = physician.PhysicianId;
                HttpContext.Session.SetInt32("PhysicianId", physicianId);
                var physicianName = physician.FirstName;
                HttpContext.Session.SetString("Name", physicianName);
            }


            AspNetUser user = _login.adminLogin(loginVm);
            if (user != null)
            {
                AspNetUserRole aspNetUserRole = _login.findAspNetRole(user);
                int roleId = aspNetUserRole.RoleId;
                HttpContext.Session.SetInt32("RoleId",roleId);
                if (aspNetUserRole == null)
                {
                    TempData["error"] = "Can't have Access to this site";
                    return View();
                }
                else if(aspNetUserRole.RoleId == 1)
                {
                    Admin? admin1 = _context.Admins.FirstOrDefault(x => x.AdminId == admin.AdminId);
                    var roleCookie = new Cookie()
                    {
                        Name = "RoleMenu",
                        Value = admin1!.RoleId.ToString(),
                    };
                    Response.Cookies.Append(roleCookie.Name, roleCookie.Value!);


                    var jwtToken = _jwt.GenerateJwtToken(aspNetUserRole);
                    Response.Cookies.Append("Jwt", jwtToken);
                    User user1 = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                    HttpContext.Session.SetString("session1", user.UserName);
                    HttpContext.Session.SetString("email", user.Email);

                    HttpContext.Session.SetInt32("UserId", user1.UserId);

                    TempData["loginsuccess"] = "Login Successfull";

                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else if(aspNetUserRole.RoleId == 3)
                {
                    var jwtToken = _jwt.GenerateJwtToken(aspNetUserRole);
                    Response.Cookies.Append("Jwt", jwtToken);
                    TempData["success"] = "Login SuccessFull";
                    return RedirectToAction("ProviderDashboard","Provider");
                }
                else
                {
                    TempData["error"] = "Can't have access to this site";
                    return RedirectToAction("Login","Login");
                }
            }
            else
            {
                TempData["error"] = "Invalid Username or Password!";
                return View();
            }
        }
        #endregion

        #region ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        #endregion

        #region ResetPasswordRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordRequest(CreateAccount req)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Email == req.Email);
            if (user.Email == null)
            {
                TempData["emailnotenter"] = "Please Enter Valid Email";
                return RedirectToAction("ForgotPassword", "Login");
            }

            var resetToken = GenerateToken();
            var resetLink = "<a href=" + Url.Action("ResetPasswordPatient", "Home", new { email = req.Email, code = resetToken }, "https") + ">Reset Password</a>";

            var subject = "Password Reset Request";
            var body = "<b>Please find the Password Reset Link.</b><br/>" + resetLink;


            await SendEmailAsync(req.Email, subject, body);
            TempData["emailsend"] = "Email is sent successfully to your email account";
            return RedirectToAction("Login", "Login");
        }
        #endregion

        #region GetTokenExpiration
        public IActionResult GetTokenExpiration()
        {
            return View();
        }
        #endregion

        #region GenerateToken : post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateToken()
        {
            string token = _login.GenerateToken();
            // Use the token for your purpose
            TempData["success"] = "Token is generated successfully";
            return RedirectToAction("Login", "Login");
        }
        #endregion

        #region SendEmailAsync
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailAsync(string username, string subject, string body)
        {
            await _login.SendEmailAsync(username, subject, body);
            TempData["success"] = "Email is sent successfully";
            return RedirectToAction("Login", "Login");
        }
        #endregion
    }
}
