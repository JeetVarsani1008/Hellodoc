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
namespace DAL.Controllers
{
    public class HomeController : Controller
    {
        private  IPatientRequest _patientRequest;
        private readonly IPatientDashboard _patientDashboard;
        private readonly HellodocContext _context;
        private readonly ILogin _login;
        public HomeController(HellodocContext context, ILogin login, IPatientRequest _PatientRequest, IPatientDashboard patientDashboard)
        {
            _context = context;
            _login = login;
            this._patientRequest = _PatientRequest;
            _patientDashboard = patientDashboard;
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


        public IActionResult PatientDashboard()
        {
            int? uid = HttpContext.Session.GetInt32("userId");
            ViewBag.UserName = HttpContext.Session.GetString("username");
            ViewBag.ActivePage = "PatientDashboard";
            _patientDashboard.patientDashboardMain(uid);
            return View(_patientDashboard.patientDashboardMain(uid));
        }


        public IActionResult Patient_Login(LoginVm loginVm)
        {

            if (ModelState.IsValid) {
                if (_login.ValidateLogin(loginVm))
                {
                    var user = _context.Users.FirstOrDefault(x => x.Email == loginVm.Email);
                    HttpContext.Session.SetInt32("userId", user.UserId);
                    HttpContext.Session.SetString("email", user.Email);
                    //HttpContext.Session.SetString("session1", user.UserName);
                    HttpContext.Session.SetString("username", user.FirstName + " " + user.LastName);
                    TempData["success"] = "Login Success";
                    return RedirectToAction("PatientDashboard");
                }
                else {
                    TempData["error"] = "Login Failed";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }



        public IActionResult patient_logout()
        {
            HttpContext.Session.Remove("session1");
            return View("Patient_Login");
        }

        public IActionResult Submit_Request()
        {
            return View();
        }

        public IActionResult PatientRequestForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PatientRequestForm(PatientData model)
        {
                _patientRequest.patientRequestForm(model);
                return RedirectToAction("Index","Home");       
   
        }

        public IActionResult FamilyFriendsForm()
        {
            return View();
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
            return View();
        }

        [HttpPost]
        public IActionResult ConciergeForm(ConciergeData model)
        {

            _patientRequest.conciergeRequestForm(model);
            return RedirectToAction("Index", "Home");

        }

        public IActionResult BusinessForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BusinessForm(BusinessData model)
        {
            _patientRequest.businessRequestForm(model);
            return RedirectToAction("Index","Home");
        }



        public IActionResult PatientDashboardProfile()
        {
            ViewBag.ActivePage = "PatientDashboardProfile";
            int? userID = HttpContext.Session.GetInt32("userId");
            _patientDashboard.patientDashboardProfile(userID);
            TempData["success"] = "Login Successful";
            return View(_patientDashboard.patientDashboardProfile(userID));
        }

        public IActionResult EditProfile(PatientProfile model) 
        {
            int? userid = HttpContext.Session.GetInt32("userId");
            _patientDashboard.editPatientProfile(model, userid);
            return RedirectToAction("PatientDashboardProfile", "Home");
        }



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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult ResetPasswordPatient(CreateAccount req)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var aspnetuser = _context.AspNetUsers.FirstOrDefault(a => a.Email == req.UserName);
        //        if (aspnetuser != null)
        //        {
        //            if (req.PasswordHash != req.ConfirmPassword)
        //            {
        //                TempData["passerror"] = "Password and Confirmpassword doesn't match";
        //                return View();
        //            }

        //            aspnetuser.PasswordHash = req.PasswordHash;
        //            _context.AspNetUsers.Update(aspnetuser);
        //            _context.SaveChanges();
        //            TempData["pwdupdate"] = "Password is updated successfully";
        //            return RedirectToAction("Patient_Login", "Home");
        //        }
        //        TempData["notvalidemail"] = "You are entered wrong email";
        //        return RedirectToAction("ResetPasswordPatient", "Home", new { code = req.Token, email = req.UserName });

        //    }
        //    TempData["passerror"] = "Password and Confirmpassword doesn't match";
        //    return RedirectToAction("ResetPasswordPatient", "Home", new { code = req.Token, email = req.UserName });

        //}

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



        //public async Task SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    var message = new MimeMessage();
        //    message.From.Add(new MailboxAddress("HelloDoc2", "testinghere1008@outlook.com"));
        //    message.To.Add(new MailboxAddress("HelloDoc2 Member", toEmail));
        //    message.Subject = subject;

        //    var bodyBuilder = new BodyBuilder();
        //    bodyBuilder.HtmlBody = body;

        //    message.Body = bodyBuilder.ToMessageBody();

        //    using (var client = new MailKit.Net.Smtp.SmtpClient())
        //    {
        //        await client.ConnectAsync("smtp.office365.com", 587, false);
        //        //await client.AuthenticateAsync("fakeidofjd00@gmail.com", "gzskmjedfwsnulle");
        //        await client.AuthenticateAsync("testinghere1008@outlook.com", "Simple@12345");
        //        await client.SendAsync(message);
        //        await client.DisconnectAsync(true);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailAsync(string username, string subject, string body)
        {
                await _login.SendEmailAsync(username, subject, body);
                TempData["emailsuccess"] = "Email is sent successfully";
                return RedirectToAction("Index", "Home");
        }

        //private const int TokenExpirationHours = 24;

        //public string GenerateToken()
        //{
        //    byte[] tokenBytes = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(tokenBytes);
        //    }

        //    string token = Convert.ToBase64String(tokenBytes);

        //    return token;
        //}

        //public DateTime GetTokenExpiration()
        //{
        //    return DateTime.UtcNow.AddHours(TokenExpirationHours);
        //}

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
