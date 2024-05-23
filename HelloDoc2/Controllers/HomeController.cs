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
using DAL.ViewModelProvider;
using HelloDoc2.hub;
using Microsoft.AspNetCore.SignalR;
namespace DAL.Controllers
{
    public class HomeController : Controller
    {
        private IPatientRequest _patientRequest;
        private readonly IPatientDashboard _patientDashboard;
        private readonly IAdminDashboard _adminDashboard;
        private readonly HellodocContext _context;
        private readonly ILogin _login;
        private readonly IJWT _jwt;
        private readonly IHubContext<MessageHub> _notificationHubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(HellodocContext context, ILogin login, IPatientRequest _PatientRequest, IPatientDashboard patientDashboard, IJWT jwt, IAdminDashboard adminDashboard, IHttpContextAccessor httpContextAccessor, IHubContext<MessageHub> notificationHubContext)
        {
            _context = context;
            _login = login;
            this._patientRequest = _PatientRequest;
            _patientDashboard = patientDashboard;
            _adminDashboard = adminDashboard;
            _httpContextAccessor = httpContextAccessor;
            _notificationHubContext = notificationHubContext;
            _jwt = jwt;
        }

        #region Index
        public IActionResult Index()
        {
            var model = new List<PatientData>();
            return View();
        }
        #endregion

        #region Privacy
        public IActionResult Privacy()
        {
            return View();
        }
        #endregion

        #region Forgot_Password
        public IActionResult Forgot_Password()
        {
            return View();
        }
        #endregion

        #region Patient_Login
        public IActionResult Patient_Login()
        {
            return View();
        }
        #endregion

        #region Patient_Login : post
        [HttpPost]
        public IActionResult Patient_Login(LoginVm loginVm)
        {
            AspNetUser user = _login.patientLogin(loginVm);
            if(user != null)
            {
                int Id = user.Id;
                HttpContext.Session.SetInt32("AspId", Id);
            }
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    AspNetUserRole aspNetUserRole = _login.findAspNetRole(user);
                    //var user1 = _context.Users.FirstOrDefault(x => x.Email == loginVm.Email);
                    int roleId = aspNetUserRole.RoleId;
                    HttpContext.Session.SetInt32("RoleId", roleId);
                    if (aspNetUserRole.RoleId == 1 || aspNetUserRole.RoleId == 3)
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
                else
                {
                    TempData["error"] = "Invalid Username or Password!";
                    return View();
                }
            }
            return View();
        }
        #endregion

        #region patient_logout
        public IActionResult patient_logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("session1");
            HttpContext.Session.Clear();
            return View("Patient_Login");
        }
        #endregion

        [CustomAuthorize("2")]
        #region PatientDashboard
        public IActionResult PatientDashboard()
        {
            int? uid = HttpContext.Session.GetInt32("userId");
            ViewBag.ActivePage = "PatientDashboard";
            _patientDashboard.patientDashboardMain(uid);
            return View(_patientDashboard.patientDashboardMain(uid));
        }
        #endregion

        #region Submit_Request
        public IActionResult Submit_Request()
        {
            return View();
        }
        #endregion

        #region PatientRequestForm
        public IActionResult PatientRequestForm()
        {
            PatientData patientData = new PatientData();
            patientData.regions = _patientRequest.getRegion();
            return View(patientData);
        }
        #endregion

        #region PatientRequestForm : post
        [HttpPost]
        public IActionResult PatientRequestForm(PatientData model)
        {
            _patientRequest.patientRequestForm(model);
            return RedirectToAction("Index","Home");       
        }
        #endregion

        #region FamilyFriendsForm
        public IActionResult FamilyFriendsForm()
        {
            FamilyData familyData = new FamilyData();
            familyData.regions = _patientRequest.getRegion();
            return View(familyData);
        }
        #endregion

        #region FamilyFriendsForm : post
        [HttpPost]
        public async Task<IActionResult> FamilyFriendsForm(FamilyData familyData)
        {
            _patientRequest.familyRequestForm(familyData);
            var email = _context.Users.FirstOrDefault(o => o.Email == familyData.Email);
            if (email == null)
            {
                var resetToken = GenerateToken();
                var createLink = "<a href=" + Url.Action("CreateAccount", "Home", new { email = familyData.Email, code = resetToken }, "https") + ">Create Account</a>";

                var subject = "Create account Request";
                var body = "<b>Please find the Password Reset Link.</b><br/>" + createLink;


                await SendEmailAsync(familyData.Email, subject, body);

            }
            return RedirectToAction("Index","Home");
        }
        #endregion

        #region CreateAccount
        public IActionResult CreateAccount() {
            return View();
        }
        #endregion

        #region CreateAccount : post
        [HttpPost]
        public IActionResult CreateAccount(LoginVm loginVm) 
        {
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
                    TempData["success"] = "Your Account is Created Succesfully";
                    return RedirectToAction("Patient_Login");
                }
                else
                {
                    aspnetUser.PasswordHash = loginVm.PasswordHash;
                    _context.SaveChanges();
                    TempData["success"] = "Entered Email is Exist And Password Saved";
                    return RedirectToAction("Login","Login");
                }
            }
            else
            {
                return View("CreateAccount", loginVm);
            }
        }
        #endregion

        #region ConciergeForm
        public IActionResult ConciergeForm()
        {
            ConciergeData conciergeData = new ConciergeData();
            conciergeData.regions = _patientRequest.getRegion();
            return View(conciergeData);
        }
        #endregion

        #region ConciergeForm : post
        [HttpPost]
        public async Task<IActionResult> ConciergeForm(ConciergeData model)
        {
            _patientRequest.conciergeRequestForm(model);

            var email = _context.Users.FirstOrDefault(o => o.Email == model.Email);
            if (email == null)
            {
                var resetToken = GenerateToken();
                var createLink = "<a href=" + Url.Action("CreateAccount", "Home", new { email = model.Email, code = resetToken }, "https") + ">Create Account</a>";

                var subject = "Create account Request";
                var body = "<b>Please find the Create Account Link.</b><br/>" + createLink;


                await SendEmailAsync(model.Email, subject, body);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region BusinessForm
        public IActionResult BusinessForm()
        {
            BusinessData businessData = new BusinessData();
            businessData.regions = _patientRequest.getRegion(); 
            return View(businessData);
        }
        #endregion

        #region BusinessForm : post
        [HttpPost]
        public async Task<IActionResult> BusinessForm(BusinessData model)
        {
            _patientRequest.businessRequestForm(model);

            var email = _context.Users.FirstOrDefault(o => o.Email == model.Email);
            if (email == null)
            {
                var resetToken = GenerateToken();
                var createLink = "<a href=" + Url.Action("CreateAccount", "Home", new { email = model.Email, code = resetToken }, "https") + ">Create Account</a>";

                var subject = "Create account Request";
                var body = "<b>Please find the Create Account Link.</b><br/>" + createLink;
                await SendEmailAsync(model.Email, subject, body);
            }
            return RedirectToAction("Index","Home");
        }
        #endregion

        [CustomAuthorize("2")]
        #region PatientDashboardProfile
        public IActionResult PatientDashboardProfile()
        {
            ViewBag.ActivePage = "PatientDashboardProfile";
            int? userID = HttpContext.Session.GetInt32("userId");
            _patientDashboard.patientDashboardProfile(userID);
            return View(_patientDashboard.patientDashboardProfile(userID));
        }
        #endregion

        #region EditProfile
        public IActionResult EditProfile(PatientProfile model) 
        {
            int? userid = HttpContext.Session.GetInt32("userId");
            _patientDashboard.editPatientProfile(model, userid);
            TempData["success"] = "Profile Edited Successfully";
            return RedirectToAction("PatientDashboardProfile", "Home");
        }
        #endregion

        [CustomAuthorize("2")]
        #region ViewDocument
        public IActionResult ViewDocument(int reqId)
        {
            HttpContext.Session.SetInt32("RequestId", reqId);
            ViewBag.ActivePage = "PatientDashboard";
            var requestData = _patientDashboard.GetRequestById(reqId);
            ViewBag.Uploader = requestData.FirstName;
            ViewBag.reqid = reqId;
            var documents = _patientDashboard.GetFilesByRequestId(reqId);
            ViewBag.document = documents;
            return View();
        }
        #endregion

        #region Download
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
        #endregion

        #region DownloadAll
        public IActionResult DownloadAll(int reqId)
        {
            var filesRow = _patientDashboard.GetAllFilesByRequestId(reqId);
            MemoryStream ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                filesRow.ForEach(file =>
                {
                    var path = "D:\\main project\\Hellodoc\\HelloDoc2\\wwwroot\\upload\\" + file.FileName;
                    ZipArchiveEntry zipEntry = zip.CreateEntry(file.FileName);
                    try
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        using (Stream zipEntryStream = zipEntry.Open())
                        {
                            fs.CopyTo(zipEntryStream);
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        TempData["ErrorMessage"] = $"File '{file.FileName}' not found.";
                    }
                });
            if (TempData.ContainsKey("ErrorMessage"))
            {
                TempData["error"] = "something went wrong while downloading file";
                return RedirectToAction("ViewDocument","Home",new {reqId = reqId});
            }
            else
            {
                return File(ms.ToArray(), "application/zip", "download.zip");
            }
        }
        #endregion

        #region Upload
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
        #endregion

        [CustomAuthorize("2")]
        #region PatientRequestForMe
        public IActionResult PatientRequestForMe()
        {
            ViewBag.ActivePage = "PatientDashboard";
            int? userID = HttpContext.Session.GetInt32("userId");
            _patientRequest.PatientRequestForMe(userID);
            return View(_patientRequest.PatientRequestForMe(userID));
        }
        #endregion

        #region PatientRequestForMe : post
        [HttpPost]
        public IActionResult PatientRequestForMe(PatientRequestForMeAndSomeone model)
        {
            _patientRequest.createRequestForMe(model);
            return RedirectToAction("PatientDashboard", "Home");
        }
        #endregion


        [CustomAuthorize("2")]
        #region PatientRequestForSomeone
        public IActionResult PatientRequestForSomeone()
        {
            ViewBag.ActivePage = "PatientDashboard";
            return View();
        }
        #endregion

        #region PatientRequestForSomeone : post
        [HttpPost]
        public IActionResult PatientRequestForSomeone(PatientRequestForMeAndSomeone model)
        {
            _patientRequest.createRequestForSomeone(model);
            return RedirectToAction("PatientDashboard", "Home");
        }
        #endregion

        #region ResetPasswordPatient
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
        #endregion

        #region ResetPasswordPatient
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
        #endregion

        #region SendEmailAsync
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailAsync(string username, string subject, string body)
        {
                await _login.SendEmailAsync(username, subject, body);
                TempData["emailsuccess"] = "Email is sent successfully";
                return RedirectToAction("Index", "Home");
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
                TempData["tokengenerated"] = "Token is generated successfully";
                return RedirectToAction("Index", "Home");       
        }
        #endregion

        #region ResetPasswordRequest
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
        #endregion


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        #region Error
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

        //this is for checkmail 
        [Route("/Home/PatientRequestForm/checkemailexists/{email}")]
        #region checkemailexists : get
        [HttpGet]
        public IActionResult checkemailexists(string email)
        {
            var emailExists = _context.AspNetUsers.Any(u => u.Email == email);
            return Json(new { exists = emailExists });
        }
        #endregion

        //for chat 
        //#region Chat 
        //public IActionResult Chat()
        //{
        //    ChatViewModel chatViewModel = new ChatViewModel();
        //    return PartialView("_Chat",chatViewModel);
        //}
        //#endregion

        #region Chat
        public IActionResult Chat(int req, int adminId, int ProviderId)
        {
            int? RoleId = HttpContext.Session.GetInt32("RoleId");
            int? aspNetUserId = HttpContext.Session.GetInt32("AspId");
            ChatViewModel chatViewModel = new();
            if (ProviderId != 0)
            {
                chatViewModel = _patientDashboard.GetProviderChatDetails(ProviderId, req, RoleId);
            }
            else
            {
                chatViewModel = _patientDashboard.GetAdminChatDetails(adminId, req, RoleId);
            }
            chatViewModel.ProviderId = ProviderId;
            return PartialView("_chat", chatViewModel);
        }

        #endregion


        #region SendToSpecificUser
        public async Task<ActionResult> SendToSpecificUser(ChatViewModel chatViewModel)
        {
            int aspnetuserId = 0;
            int currentAspNetUserId = HttpContext.Session.GetInt32("AspId") ?? 0;
            if (chatViewModel.AdminAspNetUserId != null && chatViewModel.AdminId != 0 && chatViewModel.ProviderId == 0)
            {
                aspnetuserId = (int)chatViewModel.AdminAspNetUserId;
                chatViewModel.sentBy = 2;
                chatViewModel.ProviderId = null;
                _adminDashboard.AddChats(chatViewModel);
            }
            else if (chatViewModel.ProviderAspNetUserId != null && chatViewModel.AdminId == 0 && chatViewModel.ProviderId != 0)
            {
                aspnetuserId = (int)chatViewModel.ProviderAspNetUserId;
                chatViewModel.sentBy = 2;
                chatViewModel.AdminId = null;
                _adminDashboard.AddChats(chatViewModel);
            }

            else if (chatViewModel.PatientAspNetUserId != null && chatViewModel.AdminId != 0 && chatViewModel.ProviderId == 0)
            {
                aspnetuserId = (int)chatViewModel.PatientAspNetUserId;
                chatViewModel.sentBy = 1;
                chatViewModel.ProviderId = null;
                _adminDashboard.AddChats(chatViewModel);
            }
            else if (chatViewModel.ProviderAspNetUserId != null && chatViewModel.AdminId != 0 && chatViewModel.ProviderId != 0)
            {
                aspnetuserId = (int)chatViewModel.ProviderAspNetUserId;
                chatViewModel.sentBy = 1;
                _adminDashboard.AddChats(chatViewModel);
            }
            else if (chatViewModel.AdminAspNetUserId != null && chatViewModel.AdminId != 0 && chatViewModel.ProviderId != 0)
            {
                aspnetuserId = (int)chatViewModel.AdminAspNetUserId;
                chatViewModel.sentBy = 3;
                _adminDashboard.AddChats(chatViewModel);

            }
            else if (chatViewModel.PatientAspNetUserId != null && chatViewModel.AdminId == 0 && chatViewModel.ProviderId != 0)
            {
                aspnetuserId = (int)chatViewModel.PatientAspNetUserId;
                chatViewModel.sentBy = 3;
                chatViewModel.AdminId = null;
                _adminDashboard.AddChats(chatViewModel);
            }

            MessageHub hub = new MessageHub(_httpContextAccessor);
            List<string> connections = MessageHub.GetUserConnections(aspnetuserId);
            if (connections.Count > 0)
            {
                foreach (string connectionId in connections)
                {
                    await _notificationHubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", chatViewModel.Message, DateTime.Now.ToString("hh:mm tt"), false, chatViewModel.RequestId);
                }
            }
            List<string> currentconnections = MessageHub.GetUserConnections(currentAspNetUserId);
            await _notificationHubContext.Clients.Client(currentconnections[0]).SendAsync("ReceiveMessage", chatViewModel.Message, DateTime.Now.ToString("hh:mm tt"), true, chatViewModel.RequestId);

            return Json(true);
        }

        #endregion

    }
}
