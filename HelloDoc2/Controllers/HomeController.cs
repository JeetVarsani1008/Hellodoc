using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting.Internal;
using MimeKit;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using DAL.ViewModel;
using BLL.Interface;
using BLL.Repositery;
using DAL.Models;
using Org.BouncyCastle.Ocsp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using HelloDoc2.Auth;
using Newtonsoft.Json.Linq;
namespace DAL.Controllers
{
    public class HomeController : Controller
    {
        private IPatientRequest _patientRequest;
        private readonly IPatientDashboard _patientDashboard;
        private readonly HellodocContext _context;
        private readonly ILogin _login;
        private readonly IJWT _jwt;
        public HomeController(HellodocContext context, ILogin login, IPatientRequest _PatientRequest, IPatientDashboard patientDashboard, IJWT jwt)
        {
            _context = context;
            _login = login;
            this._patientRequest = _PatientRequest;
            _patientDashboard = patientDashboard;
            _jwt = jwt;
        }

        public IActionResult Index()
        {
            var model = new List<PatientData>();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Forgot_Password()
        {
            return View();
        }

        public IActionResult Patient_Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Patient_Login(LoginVm loginVm)
        {
            AspNetUser user = _login.patientLogin(loginVm);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    AspNetUserRole aspNetUserRole = _login.findAspNetRole(user);
                    //var user1 = _context.Users.FirstOrDefault(x => x.Email == loginVm.Email);

                    if (aspNetUserRole == null)
                    {
                        //ModelState.AddModelError(String.Empty, "Cant Have access to this site");
                        TempData["error"] = "this email has no access to this site";
                        return View("Patient_Login");
                    }
                    else
                    {
                        var jwtToken = _jwt.GenerateJwtToken(aspNetUserRole);
                        Response.Cookies.Append("Jwt", jwtToken);
                        User user1 = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                        HttpContext.Session.SetInt32("userId", user1.UserId);
                        HttpContext.Session.SetString("email", user1.Email);
                        HttpContext.Session.SetString("username", user1.FirstName + " " + user1.LastName);
                        TempData["success"] = "Login Successfull";
                        return RedirectToAction("PatientDashboard", "Home");
                    }

                }
            }
            return View();
        }


        public IActionResult patient_logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("session1");
            HttpContext.Session.Clear();
            return View("Patient_Login");
        }


        [CustomAuthorize("2")]
        public IActionResult PatientDashboard()
        {
            int? uid = HttpContext.Session.GetInt32("userId");
            ViewBag.UserName = HttpContext.Session.GetString("username");
            ViewBag.ActivePage = "PatientDashboard";
            _patientDashboard.patientDashboardMain(uid);
            return View(_patientDashboard.patientDashboardMain(uid));
        }

        public IActionResult Submit_Request()
        {
            return View();
        }

        public IActionResult PatientRequestForm()
        {
            PatientData patientData = new PatientData();
            patientData.regions = _patientRequest.getRegion();
            return View(patientData);
        }

        [HttpPost]
        public IActionResult PatientRequestForm(PatientData model)
        {
                _patientRequest.patientRequestForm(model);
                return RedirectToAction("Index","Home");       
   
        }

        public IActionResult FamilyFriendsForm()
        {
            FamilyData familyData = new FamilyData();
            familyData.regions = _patientRequest.getRegion();
            return View(familyData);
        }

        [HttpPost]
        public async Task<IActionResult> FamilyFriendsForm(FamilyData familyData)
        {
            _patientRequest.familyRequestForm(familyData);
            var email = _context.Users.FirstOrDefault(o => o.Email == familyData.Email);
            if (email == null)
            {
                var resetToken = GenerateToken();
                var resetLink = "<a href=" + Url.Action("CreateAccount", "Home", new { email = familyData.Email, code = resetToken }, "https") + ">Reset Password</a>";

                var subject = "Password Reset Request";
                var body = "<b>Please find the Password Reset Link.</b><br/>" + resetLink;


                await SendEmailAsync(familyData.Email, subject, body);

            }
            return RedirectToAction("Index","Home");
        }


        public IActionResult CreateAccount() {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(LoginVm loginVm) {
                if (loginVm.Email == null)
                {
                    TempData["errorEmail"] = "Enter UserName";
                    return View("CreateAccount", loginVm);
                }
                AspNetUser aspnetUser = _login.getAspUser(loginVm);
                User user = _login.getUser(loginVm);
                RequestClient requestClient = _login.getRequestClient(loginVm);
                if (loginVm.PasswordHash == loginVm.PasswordHash)
                {
                    if (aspnetUser == null)
                    {
                        _login.addNewUserData(loginVm, requestClient);
                        TempData["successrequest"] = "Your Account is Created Succesfully";
                        return RedirectToAction("Patient_Login");
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "Entered Email is wrong or Existing Email";
                        return View("CreateAccount");
                    }
                }
                else
                {
                    return View("CreateAccount", loginVm);
                }
            }

        public IActionResult ConciergeForm()
        {
            ConciergeData conciergeData = new ConciergeData();
            conciergeData.regions = _patientRequest.getRegion();
            return View(conciergeData);
        }

        [HttpPost]
        public IActionResult ConciergeForm(ConciergeData model)
        {

            _patientRequest.conciergeRequestForm(model);
            return RedirectToAction("Index", "Home");

        }

        public IActionResult BusinessForm()
        {
            BusinessData businessData = new BusinessData();
            businessData.regions = _patientRequest.getRegion(); 
            return View(businessData);
        }

        [HttpPost]
        public IActionResult BusinessForm(BusinessData model)
        {
            _patientRequest.businessRequestForm(model);
            return RedirectToAction("Index","Home");
        }


        [CustomAuthorize("2")]
        public IActionResult PatientDashboardProfile()
        {
            ViewBag.ActivePage = "PatientDashboardProfile";
            int? userID = HttpContext.Session.GetInt32("userId");
            _patientDashboard.patientDashboardProfile(userID);
            return View(_patientDashboard.patientDashboardProfile(userID));
        }

        public IActionResult EditProfile(PatientProfile model) 
        {
            int? userid = HttpContext.Session.GetInt32("userId");
            _patientDashboard.editPatientProfile(model, userid);
            TempData["success"] = "Profile Edited Successfully";
            return RedirectToAction("PatientDashboardProfile", "Home");
        }

        [CustomAuthorize("2")]
        public IActionResult ViewDocument(int reqId)
        {
            ViewBag.userName = HttpContext.Session.GetString("session1");
            HttpContext.Session.SetInt32("RequestId", reqId);
            var requestData = _patientDashboard.GetRequestById(reqId);
            ViewBag.Uploader = requestData.FirstName;
            ViewBag.reqid = reqId;
            var documents = _patientDashboard.GetFilesByRequestId(reqId);
            ViewBag.document = documents;
            return View();
        }



        public IActionResult Download(int documentid)
        {
            var filename = _patientDashboard.GetFileById(documentid);
            if (filename == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload", filename.FileName);
            return File(System.IO.File.ReadAllBytes(filePath), "multipart/form-data", System.IO.Path.GetFileName(filePath));
        }



        public IActionResult DownloadAll(int reqId)
        {
            var filesRow = _patientDashboard.GetAllFilesByRequestId(reqId);
            MemoryStream ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                filesRow.ForEach(file =>
                {
                    var path = "D:\\my copy\\HelloDoc2\\HelloDoc2\\wwwroot\\upload\\" + file.FileName;
                    ZipArchiveEntry zipEntry = zip.CreateEntry(file.FileName);
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (Stream zipEntryStream = zipEntry.Open())
                    {
                        fs.CopyTo(zipEntryStream);
                    }
                });
            return File(ms.ToArray(), "application/zip", "download.zip");
        }


        [HttpPost]
        public IActionResult Upload([FromForm] IFormFile Filepath)
        {
            int? reqid = HttpContext.Session.GetInt32("RequestId");
            if (!reqid.HasValue)
            {
                return BadRequest();
            }
            string fileName = Path.GetFileName(Filepath.FileName);
            _patientDashboard.UploadFile(reqid.Value, fileName);

            var filePath = Path.Combine("wwwroot", "upload", fileName);
            using (FileStream stream = System.IO.File.Create(filePath))
            {
                // The file is saved in a buffer before being processed
                Filepath.CopyTo(stream);
            }
            TempData["Uploadscs"] = "File Uploaded Successfully.Please Refresh Page";
            return View();
        }

        [CustomAuthorize("2")]
        public IActionResult PatientRequestForMe()
        {
            int? userID = HttpContext.Session.GetInt32("userId");
            _patientRequest.PatientRequestForMe(userID);
            return View(_patientRequest.PatientRequestForMe(userID));
        }

        [HttpPost]
        public IActionResult PatientRequestForMe(PatientRequestForMeAndSomeone model)
        {
            _patientRequest.createRequestForMe(model);
            return RedirectToAction("PatientDashboard", "Home");
        }

        [CustomAuthorize("2")]
        public IActionResult PatientRequestForSomeone()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PatientRequestForSomeone(PatientRequestForMeAndSomeone model)
        {
            _patientRequest.createRequestForSomeone(model);
            return RedirectToAction("PatientDashboard", "Home");
        }


        public IActionResult ResetPasswordPatient(String code, String email)
        {
            ViewBag.Code = code;
            ViewBag.Email = email;
            if (code == null)
            {
                return NotFound();
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPasswordPatient(CreateAccount req)
        {
            if (ModelState.IsValid)
            {
                var aspnetuser = _login.GetUserByEmail(req.UserName);
                if(aspnetuser != null)
                {
                    if (req.PasswordHash != req.ConfirmPassword) { 
                        TempData["passerror"] = "Password and Confirmpassword doesn't match";
                        return View();
                    }
                _login.UpdateUserPassword(aspnetuser, req.PasswordHash);
                _login.SaveChanges();
                TempData["pwdupdate"] = "Password is updated successfully";
                return RedirectToAction("Patient_Login", "Home");
                }
            TempData["notvalidemail"] = "You are entered wrong email";
            return RedirectToAction("ResetPasswordPatient", "Home", new { code = req.Token, email = req.UserName });
            }
            TempData["passerror"] = "Password and Confirmpassword doesn't match";
            return RedirectToAction("ResetPasswordPatient", "Home", new { code = req.Token, email = req.UserName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailAsync(string username, string subject, string body)
        {
                await _login.SendEmailAsync(username, subject, body);
                TempData["emailsuccess"] = "Email is sent successfully";
                return RedirectToAction("Index", "Home");
        }

        public IActionResult GetTokenExpiration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateToken()
        {
                string token = _login.GenerateToken();
                // Use the token for your purpose
                TempData["tokengenerated"] = "Token is generated successfully";
                return RedirectToAction("Index", "Home");       
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordRequest(CreateAccount req)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Email == req.UserName);
            if (user.UserName == null)
            {
                TempData["emailnotenter"] = "Please Enter Valid Email";
                return RedirectToAction("Forgot_Password", "Home");
            }

            var resetToken = GenerateToken();
            var resetLink = "<a href=" + Url.Action("ResetPasswordPatient", "Home", new { email = req.UserName, code = resetToken }, "https") + ">Reset Password</a>";

            var subject = "Password Reset Request";
            var body = "<b>Please find the Password Reset Link.</b><br/>" + resetLink;


            await SendEmailAsync(req.UserName, subject, body);
            TempData["emailsend"] = "Email is sent successfully to your email account";
            return RedirectToAction("Patient_Login", "Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/Home/PatientRequestForm/checkemailexists/{email}")]

        [HttpGet]
        public IActionResult checkemailexists(string email)
        {
            var emailExists = _context.AspNetUsers.Any(u => u.Email == email);
            return Json(new { exists = emailExists });
        }
    }
}
