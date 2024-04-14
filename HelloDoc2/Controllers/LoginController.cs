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
	

		public IActionResult Login()
		{
			return View();
		}

        #region AdminLogin : Post
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
                else
                {
                    var jwtToken = _jwt.GenerateJwtToken(aspNetUserRole);
                    Response.Cookies.Append("Jwt", jwtToken);
                    TempData["success"] = "Login SuccessFull";
                    return RedirectToAction("ProviderDashboard","Provider");
                }
            }
            return View();
        }
        #endregion
    }
}
