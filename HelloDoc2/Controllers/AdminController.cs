using BLL.Interface;
using BLL.Repositery;
using DAL.Models;
using DAL.ViewModel;
using HelloDoc2.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections;
using System.IO.Compression;
using System.Net.Mail;
using System.Net;
using System.Net.NetworkInformation;
using System.Drawing;
using HellodocContext = DAL.Models.HellodocContext;
using AspNetUser = DAL.Models.AspNetUser;
using System.Data;
using ClosedXML.Excel;
using HtmlAgilityPack;
using MimeKit;
using Org.BouncyCastle.Ocsp;
using DocumentFormat.OpenXml.Office2010.Excel;
namespace DAL.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _adminDashboard;
        private readonly HellodocContext _context;
        private readonly ILogin _login;
        private readonly IJWT _jwt;

        public AdminController(HellodocContext context, IAdminDashboard adminDashboard, ILogin login, IJWT jwt)
        {
            _context = context;
            _adminDashboard = adminDashboard;
            _login = login;
            _jwt = jwt;
        }

        public IActionResult AdminLogin()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Jwt");
            Response.Cookies.Delete(".AspNetCore.Session");
            Response.Cookies.Delete(".AspNetCore.Antiforgery.N0iw8MAOgzI");

            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(LoginVm loginVm)
        {
            var Id = _context.AspNetUsers.FirstOrDefault(x => x.Email == loginVm.Email).Id;
            HttpContext.Session.SetInt32("AspId",Id);
            AspNetUser user = _login.adminLogin(loginVm);
            if (user != null)
            {
            AspNetUserRole aspNetUserRole = _login.findAspNetRole(user);
                if (aspNetUserRole == null)
                {
                    ModelState.AddModelError(String.Empty, "Cant Have access to this site");
                    return View("Patient_Login");
                }
                else
                {
                    var jwtToken = _jwt.GenerateJwtToken(aspNetUserRole);
                    Response.Cookies.Append("Jwt", jwtToken);
                    User user1 = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                    HttpContext.Session.SetString("session1", user.UserName);
                    HttpContext.Session.SetString("email", user.Email);

                    HttpContext.Session.SetInt32("UserId", user1.UserId);

                    TempData["success"] = "Login Successfull";
                    return RedirectToAction("AdminDashboard", "Admin");
                }
            }
            return View();
        }

        public IActionResult AdminLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Jwt");
            Response.Cookies.Delete(".AspNetCore.Session");
            Response.Cookies.Delete(".AspNetCore.Antiforgery.N0iw8MAOgzI");
            return RedirectToAction("AdminLogin", "Admin");
        }


        [CustomAuthorize("1")]
        public IActionResult AdminDashboard(string reqtypeid)
        {
            ViewBag.ActiveDashboardNav = "AdminDashboard";
            string str = "1";
            int[] arr = { 1 };
            var request = _context.Requests;

            var reqCount = request.GroupBy(o => o.Status).Select(h => new { Status = h.Key, Count = h.Count() }).ToList();

            var newCount = reqCount.Find(o => o.Status == 1)?.Count ?? 0;
            var pendingCount = reqCount.Find(i => i.Status == 2)?.Count ?? 0;
            var active1 = reqCount.Find(i => i.Status == 4)?.Count ?? 0;
            var active2 = reqCount.Find(i => i.Status == 5)?.Count ?? 0;
            var activeCount = active1 + active2;
            var concludeCount = reqCount.Find(i => i.Status == 6)?.Count ?? 0;
            var toclose1 = reqCount.Find(i => i.Status == 3)?.Count ?? 0;
            var toclose2 = reqCount.Find(i => i.Status == 7)?.Count ?? 0;
            var toclose3 = reqCount.Find(i => i.Status == 8)?.Count ?? 0;
            var toCloseCount = toclose1 + toclose2 + toclose3;
            var unpaidCount = reqCount.Find(i => i.Status == 9)?.Count ?? 0;
            var getRegion = _adminDashboard.getRegions();
            var requestAdmin = _adminDashboard.requestDataAdmin(str,arr, null);
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel()
            {
                NewCount = newCount,
                PendingCount = pendingCount,
                ActiveCount = activeCount,
                ConcludeCount = concludeCount,
                ToCloseCount = toCloseCount,
                UnpaidCount = unpaidCount,
                requestListAdminDash = requestAdmin,
                statusArray = str,
                StatusForName = 1,
                reqTypId = reqtypeid,
                regions = getRegion,
            };
            return View(adminDashboardViewModel);
        }

        public IActionResult FetchRequests(string statusarray,string status, string reqtypeid)
        {

            int[] Status = status.Split(',').Select(s => int.Parse(s)).ToArray();
            var getRegion = _adminDashboard.getRegions();

            var requestAdmin = _adminDashboard.requestDataAdmin(statusarray, Status, reqtypeid);
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                requestListAdminDash = requestAdmin,
                StatusForName = Status[0],
                statusArray = statusarray,
                reqTypId = reqtypeid,
                regions = getRegion,
            };
            return PartialView("_RequestsAccToStatus", adminDashboardViewModel);
        }

        public IActionResult FilterRequests(string statusarray, int[] status, string reqtypeid)
        {
            //int[] Status = status.Split(',').Select(s => int.Parse(s)).ToArray();
            var getRegion = _adminDashboard.getRegions();

            var requestListAdmin = _adminDashboard.requestDataAdmin(statusarray, status, reqtypeid);
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                requestListAdminDash = requestListAdmin,
                StatusForName = status[0],
                statusArray = statusarray,
                regions = getRegion,
            };

            return PartialView("_RequestsAccToStatus", adminDashboardViewModel);
        }



        // this is for accordian to fetch names
        public string GetNameByRequestTypeId(int requestTypeId)
        {
            Dictionary<int, string> reqTypesName = new Dictionary<int, string>()
            {
                {1, "Patient" },
            };
            return requestTypeId.ToString() != null ? reqTypesName.GetValueOrDefault(requestTypeId, "Unknown Request Type"): "Unknown Request Type";
        }

        public IActionResult ViewCase(int requestId)
        {
            var data = _adminDashboard.ViewCase(requestId);
            return View(data);
        }

        //view notes start : 2 methods
        public IActionResult ViewNotes(int requestId)
        {
            var data = _adminDashboard.ViewNotes(requestId);
            return View(data);
        }


        [HttpPost]
        public IActionResult ViewNotes(ViewNotesVm model, int requestId) {
            _adminDashboard.editViewNotes(model, requestId);
            model = _adminDashboard.ViewNotes(requestId);
            return View(model);
        }
        //view notes completed


        //cancel case start : 2 methods
        public IActionResult CancelCase(int req)
        {
            HttpContext.Session.SetInt32("reqId", req);
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel();
            adminDashboardViewModel.CaseTags = _adminDashboard.cancelCaseMain();
            return PartialView("_adminCancelCase", adminDashboardViewModel);
        }

        [HttpPost]
        public IActionResult CancelCase(AdminDashboardViewModel model)
        {
            int? req = HttpContext.Session.GetInt32("reqId");
            _adminDashboard.cancelCase(model, req ?? 0);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        //cancel case completed


        //block case start : 2 methods
        public IActionResult BlockCase(int req)
        {
            HttpContext.Session.SetInt32("requestid", req);
            AdminBlockVm adminBlockVm = new AdminBlockVm();
            adminBlockVm = _adminDashboard.adminBlockVm(req);
            return PartialView("_adminBlockCase", adminBlockVm);
        }

        [HttpPost]
        public IActionResult BlockCasePost(AdminBlockVm model)
        {
            var req = HttpContext.Session.GetInt32("requestid");
            _adminDashboard.blockCase(model, req ?? 0);
            TempData["success"] = "User Blocked Successfully";
            return RedirectToAction("AdminDashboard", "Admin");
        }
        //block case completed


        //this three methods are for asign case
        public IActionResult AsignCase(int req)
        {
            HttpContext.Session.SetInt32("asignReq", req);
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.regions = _adminDashboard.asignCase();
            return PartialView("_adminAsignCase", adminAsignVm);
        }

        public IActionResult GetPhysiciansByRegionId(int regionId)
        {
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.physicianList = _adminDashboard.asignPhysician(regionId);
            return Json(new { adminAsignVm });
        }

        [HttpPost]
        public IActionResult AsignCasePost(AdminAsignVm model)
        {
            var req = HttpContext.Session.GetInt32("asignReq");
            _adminDashboard.asignCasePost(model, req ?? 0, 2);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        //asign case completed


        //this part is for viewupload : total 6 methods for all view upload
        public IActionResult ViewUploads(AdminViewUploadVm model, int requestId)
        {
            model.RequestId = requestId;
            HttpContext.Session.SetInt32("reqIdUpload", requestId);
            ViewBag.RequestIdForDownloadAll = requestId;
            ViewBag.RequestIdForDeleteAll = requestId;
            ViewBag.RequestIdForSendMail = requestId;
            var returnViewData = _adminDashboard.GetAdminViewUploadData(model, requestId);
            var documents = _adminDashboard.GetFilesByRequestId(requestId);
            ViewBag.document = documents;
            return View(returnViewData);
        }

        //upload file in view uploads
        [HttpPost]
        public IActionResult Upload([FromForm] IFormFile Filepath)
        {
            int? reqid = HttpContext.Session.GetInt32("reqIdUpload");
            if (!reqid.HasValue)
            {
                return BadRequest();
            }
            string fileName = Path.GetFileName(Filepath.FileName);
            _adminDashboard.UploadFile(reqid.Value, fileName);

            var filePath = Path.Combine("wwwroot", "upload", fileName);
            using (FileStream stream = System.IO.File.Create(filePath))
            {
                // The file is saved in a buffer before being processed
                Filepath.CopyTo(stream);
            }
            TempData["Uploadscs"] = "File Uploaded Successfully.Please Refresh Page";
            return View();
        }
        //upload file in view upload completed


        //this part is for download single file for view upload
        public IActionResult ViewUploadDownload(int documentId)
        {
            var filename = _adminDashboard.GetFileById(documentId);
            if (filename == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload", filename.FileName);
            return File(System.IO.File.ReadAllBytes(filePath), "multipart/form-data", System.IO.Path.GetFileName(filePath));
        }
        //single file download complete


        //this part is for delete single file
        public IActionResult ViewUploadDelete(int documentId, int requestId)
        {
            var fileToDelete = _adminDashboard.GetFileById(documentId);
            if (fileToDelete == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload", fileToDelete.FileName);

            try
            {
                System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file : {ex.Message}");
                return StatusCode(500); // this is for internal server error
            }


            //now delete this file only from upload but isdeleted is change in requestwisefile 

            BitArray bitarray = new BitArray(1);
            bitarray.Set(0, true);

            RequestWiseFile requestWiseFile = _context.RequestWiseFiles.First(x => x.RequestWiseFileId == documentId);
            requestWiseFile.IsDeleted = bitarray;
            _context.RequestWiseFiles.Update(requestWiseFile);
            _context.SaveChanges();

            TempData["success"] = "File deleted successfully.";
            return RedirectToAction("ViewUploads", "Admin", new { requestId });
        }
        //delete single file completed


        //delete selected files
        public IActionResult DeleteSelectedDocuments(List<int> requestFilesId, int requestId)
        {
            if (requestFilesId == null || requestFilesId.Count == 0)
            {
                return BadRequest("No files selected for deletion.");
            }

            var deletedFilesCount = 0; // Track successfully deleted files

            foreach (var fileId in requestFilesId)
            {
                try
                {
                    var requestWiseFile = _context.RequestWiseFiles.FirstOrDefault(x => x.RequestWiseFileId == fileId && x.RequestId == requestId);
                    if (requestWiseFile == null)
                    {
                        continue; // Skip non-existent files
                    }

                    BitArray bitarray = new BitArray(1);
                    bitarray.Set(0, true);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload", requestWiseFile.FileName);
                    System.IO.File.Delete(filePath); // Delete physical file

                    requestWiseFile.IsDeleted = bitarray; // Mark file as deleted in database
                    _context.RequestWiseFiles.Update(requestWiseFile);
                    deletedFilesCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {fileId}: {ex.Message}");
                    // Log the error for further investigation
                }
            }

            _context.SaveChanges();

            if (deletedFilesCount > 0)
            {
                return Json(new { success = true, message = $"Successfully deleted {deletedFilesCount} file(s)." });
            }
            else
            {
                return Json(new { success = false, errorMessage = "No files were deleted." });
            }
        }
        //delete selected document completed


        //this is for sending mail for selected files
        public async Task<IActionResult> SendDocumentsByMail(List<int> requestFilesId, int requestId)
        {
            bool isMailSent = false;
            string? email = _context.RequestClients.FirstOrDefault(x => x.RequestId == requestId)?.Email;

            if (email != null)
            {
                string senderEmail = "testinghere1008@outlook.com";
                string senderPassword = "Simple@12345";

                SmtpClient client = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                string message = $@"<html>
                                    <body>  
                                    <h1>All Documents</h1>
                                    </body>
                                    </html>";

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Documents",
                    Body = message,
                    IsBodyHtml = true
                };

                var requestWiseFile = from requestFiles in _context.RequestWiseFiles
                                      where requestFilesId.Contains(requestFiles.RequestWiseFileId)
                                      select new RequestWiseFile
                                      {
                                          RequestWiseFileId = requestFiles.RequestWiseFileId,
                                          FileName = requestFiles.FileName,
                                          RequestId = requestFiles.RequestId,
                                      };

                foreach (var item in requestWiseFile)
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/" + item.FileName);
                    Attachment attachment = new Attachment(filePath);
                    mailMessage.Attachments.Add(attachment);
                }

                mailMessage.To.Add(email);
                //await client.Send(mailMessage);
                client.Send(mailMessage);

                foreach (var attachment in mailMessage.Attachments)
                {
                    attachment.Dispose();
                }

                isMailSent = true;
            }

            if (isMailSent)
            {
                return Ok(new { message = "Email sent successfully" });
            }
            else
            {
                return BadRequest(new { message = "Error sending email" });
            }
        }
        //send mail completed
        //view upload completed


        //this part is for order details : 4 methods
        public IActionResult Orders(int requestID)
        {
            AdminOrderVm adminOrderVm = new AdminOrderVm();
            adminOrderVm.healthProfessionType = _adminDashboard.healthProfessionalTypes();
            adminOrderVm.RequestId = requestID;
            return View(adminOrderVm);
        }

        public IActionResult BusinessSelect(int healthProfessionId)
        {
            AdminOrderVm adminOrderVm = new AdminOrderVm();
            adminOrderVm.healthProfessionals = _adminDashboard.asignBusiness(healthProfessionId);
            return Json(new { adminOrderVm });
        }

        public IActionResult GetVendorDetails(int vendorId)
        {
            var vendordata = _adminDashboard.getVendorDetails(vendorId);
            return Json(vendordata);
        }
        [HttpPost]
        public IActionResult Orders(AdminOrderVm model, int requestID)
        {
            _adminDashboard.orderDataStore(model, requestID);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        //order part completed


        //this part is for transfer case -- transfer and asign similar that's why use same model for that
        public IActionResult TransferCase(int reqId)
        {
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.regions = _adminDashboard.transferRegion();
            adminAsignVm.RequestId = reqId;
            return PartialView("_adminTransferCase", adminAsignVm);
        }
        public IActionResult GetPhysiciansByRegionIdTransfer(int regionId)
        {
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.physicianList = _adminDashboard.transferPhysician(regionId);
            return Json(new { adminAsignVm });
        }

        public IActionResult TransferCasePost(AdminAsignVm model)
        {
            _adminDashboard.transferCasePost(model, 2);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        //transfer case completed

        //this is for download excel
        //this is download excel file for indevidual status (new,pending, active,conclude, toclose and unpaid)
        public IActionResult DownloadExcel(string statusarray)
        {
            string str = statusarray;
            var data = _adminDashboard.requestDataAdmin(str,null, null);
            var excelData = _adminDashboard.ExportToExcel(data);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AdminData.xlsx");
        }

        //this method id used for download excel file for all request
        public IActionResult DownloadExcelAll()
        {
            var data = _adminDashboard.requestDataDownloadExcelAll();
            var excelData = _adminDashboard.ExportToExcel(data);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AdminData.xlsx");
        }

        //clear case : 2 methods
        public IActionResult ClearCase(int req)
        {
            AdminClearVm adminClearVm = new AdminClearVm();
            adminClearVm.RequestId = req;
            return PartialView("_adminClearCase", adminClearVm);
        }

        [HttpPost]
        public IActionResult ClearCase(AdminClearVm model)
        {
            _adminDashboard.clearCasePost(model);
            return RedirectToAction("AdminDashboard");
        }
        //clear case completed

        //create request for admin part
        public IActionResult CreateRequestForAdmin()
        {
            return View();
        }

        //send agreement-used adminclear case model for send agreement
        public IActionResult SendAgreement(int req, int reqTypeId)
        {
            var data = _context.RequestClients.FirstOrDefault(x => x.RequestId == req);
            AdminClearVm adminClearVm = new AdminClearVm();
            adminClearVm.RequestTypeId = reqTypeId;
            adminClearVm.RequestId = req;
            adminClearVm.Number = data.PhoneNumber;
            adminClearVm.Email = data.Email;
            return PartialView("_adminSendAgreement", adminClearVm);
        }

        [HttpPost]
        public async Task<IActionResult> SendAgreement(AdminClearVm model)
        {
            
            var subject = "Review Agreement Request";
            var agreementLink = "<a href=" + Url.Action("ReviewAgreement", "Admin", new { email = model.Email, RequestId = model.RequestId }, "https") + ">Confirm Agreement</a>";

            
            var body = "<b>Please find the Password Reset Link.</b><br/>" + agreementLink;

            await SendEmailAsync(model.Email, subject, body);
            return RedirectToAction("AdminDashboard");
        }
        //this sendemail function is work for all send mail that are present in admin page
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

         public IActionResult ReviewAgreement(string email, int requestId)
         {
            ReviewAgreementVm reviewAgreementVm = new ReviewAgreementVm();
            reviewAgreementVm.RequestId = requestId;
            return View(reviewAgreementVm);
         }


        public IActionResult ReviewAgreementSubmit(ReviewAgreementVm model)
        {
            if (_adminDashboard.checkStatus(model))
            {
                _adminDashboard.reviewAgreeementSubmit(model);
                TempData["success"] = "Status Changed Successfully";
            }
            else
            {
                TempData["error"] = "Status Already Changed";
            }
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult ReviewAgreementCancel(ReviewAgreementVm model)
        {
            if (_adminDashboard.checkStatus(model))
            {
                _adminDashboard.reviewAgreementCancel(model);
                TempData["success"] = "Agreement Cancelled Successfully";
            }
            else
            {
                TempData["error"] = "Your Agreement Already Cancelled.";
            }
            return RedirectToAction("AdminDashboard");
        }


        //send mail-used adminclear case model for send mail to patient 
        //this are one of the five buttons
        public IActionResult SendMail()
        {
            return PartialView("_adminSendMail");
        }
        public IActionResult AdminCreateRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMail(AdminClearVm model)
        {
            var subject = "Submit Request";
            var submitLink = "<a href=" + Url.Action("Submit_Request", "Home", new { email = model.Email, RequestId = model.RequestId }, "https") + ">Submit Your Request</a>";

            var body = "<b>Please find the Password Reset Link.</b><br/>" + submitLink;

            await SendEmailAsync(model.Email, subject, body);
            return RedirectToAction("AdminDashboard");
        }


        //this part is for close case
        public IActionResult CloseCase(int requestId)
        {
            var data = _adminDashboard.closeCaseGet(requestId);
            return View(data);
        }

        public IActionResult CloseCaseEdit(int command, CloseCaseVm model, int requestId)
        {
            _adminDashboard.closeCaseEdit(command, model, requestId);
            var data = _adminDashboard.closeCaseGet(requestId);
            return View("CloseCase", data);
        }
        //close case completed


        //this is for admin profile 
        public IActionResult AdminMyProfile()
        {
            ViewBag.ActiveDashboardNav = "AdminMyProfile";
            int? id = HttpContext.Session.GetInt32("AspId");
            var data = _adminDashboard.getAdminDetails(id??0);
            return View(data);
        }

        public IActionResult AdminResetPassword(AdminProfileVm model)
        {
            _adminDashboard.adminResetPassword(model);
            int? id = HttpContext.Session.GetInt32("AspId");
            var data = _adminDashboard.getAdminDetails(id ?? 0);
            return View("AdminMyProfile",data);
        }
        
        public IActionResult AdminEditDetails1(AdminProfileVm model)
        {
            _adminDashboard.adminEditDetails1(model);
            int? id = HttpContext.Session.GetInt32("AspId");
            var data = _adminDashboard.getAdminDetails(id ?? 0);
            return View("AdminMyProfile",data);
        }

        public IActionResult AdminEditDetails2(AdminProfileVm model)
        {
            _adminDashboard.adminEditDetails2(model);
            int? id = HttpContext.Session.GetInt32("AspId");
            var data = _adminDashboard.getAdminDetails(id ?? 0);
            return View("AdminMyProfile",data);
        }

        //admin profile part is completed

        //this part is for encounter form 
        public IActionResult EncounterForm(int reqId)
        {
            var data = _adminDashboard.encounterFormGetData(reqId);
            return View(data);
        }

        public IActionResult PostEncounterData(EncounterVm model)
        {
            _adminDashboard.postEncounterData(model);
            return View("EncounterForm");
        }

        //provider part 
        public IActionResult Provider(string regionId)
        {
            ViewBag.ActiveDashboardNav = "Provider";

            ProviderVm providerVm = new ProviderVm();
            providerVm.regions = _adminDashboard.getRegions();
            providerVm.providers = _adminDashboard.getPhysicianDetails(regionId);
            return View(providerVm);
        }

    }
}
