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
            Response.Cookies.Delete("jwt");
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(LoginVm loginVm)
        {
            AspNetUser user = _login.adminLogin(loginVm);
            AspNetUserRole aspNetUserRole = _login.findAspNetRole(user);
            if(user != null)
            {
                if(aspNetUserRole == null)
                {
                    ModelState.AddModelError(String.Empty, "Cant Have access to this site");
                    return View("Patientlogin");
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
                    return RedirectToAction("AdminDashboard","Admin");
                }
            }
            return View();
        }


        [CustomAuthorize("1")]
        public IActionResult AdminDashboard(int Status, string reqtypeid, int RegionId)
        {

            var request = _context.Requests;

            var reqCount = request.GroupBy(o => o.Status).Select(h => new { Status = h.Key, Count = h.Count() }).ToList();

            ViewBag.newRequest = reqCount.Find(o => o.Status == 1) ?.Count ?? 0;
            ViewBag.pendingRequest = reqCount.Find(i => i.Status ==2)?.Count ?? 0; 
            ViewBag.activeRequest = reqCount.Find(i => i.Status == 3)?.Count ?? 0;
            ViewBag.concludeRequest = reqCount.Find(i => i.Status == 4)?.Count ?? 0;
            ViewBag.toCloseRequest = reqCount.Find(i => i.Status == 5)?.Count ?? 0;
            ViewBag.unpaidRequest = reqCount.Find(i => i.Status == 6)?.Count ?? 0;

            var requestAdmin=_adminDashboard.requestDataAdmin(1,null, 0);
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel()
            {
                requestListAdminDash = requestAdmin,
                StatusForName = Status,
                reqTypId = reqtypeid,
                Regin_Short = RegionId
            };
            return View(adminDashboardViewModel);
        }
        public IActionResult fetchRequests(int Status, string reqtypeid, int RegionId) 
        {

            var requestAdmin = _adminDashboard.requestDataAdmin(Status,reqtypeid, RegionId);
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel()
            {
                requestListAdminDash = requestAdmin,
                StatusForName= Status,
                Regin_Short = RegionId,
                reqTypId = reqtypeid,


            };
            return PartialView("_RequestsAccToStatus",adminDashboardViewModel);
        }

        public IActionResult AdminMyProfile() { 
            return View(); 
        }

        public IActionResult CloseCase() {
            return View();
        }


        public IActionResult ViewCase(int requestId)
        {
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel();
            adminDashboardViewModel.requestListAdminDash = _adminDashboard.ViewCase(requestId);
            return View(adminDashboardViewModel);
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
            return View(model) ;
        }
        //view notes completed


        //cancel case start : 2 methods
        public IActionResult CancelCase(int req)
        {
            HttpContext.Session.SetInt32("reqId", req);
            AdminDashboardViewModel adminDashboardViewModel=new AdminDashboardViewModel();
            adminDashboardViewModel.CaseTags = _adminDashboard.cancelCaseMain();
            return PartialView("_adminCancelCase", adminDashboardViewModel);
        }

        [HttpPost]
        public IActionResult CancelCase(AdminDashboardViewModel model)
        {
            int? req = HttpContext.Session.GetInt32("reqId");
            _adminDashboard.cancelCase(model, req??0);
            return RedirectToAction("AdminDashboard","Admin");
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
            _adminDashboard.asignCasePost(model, req ?? 0,2);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        //asign case completed

        public IActionResult ViewUploads(AdminViewUploadVm model,int requestId)
        {
            model.RequestId = requestId;
            HttpContext.Session.SetInt32("reqIdUpload",requestId);
            ViewBag.RequestIdForDownloadAll = requestId;
            ViewBag.RequestIdForDeleteAll = requestId;
            ViewBag.RequestIdForSendMail = requestId;
            var returnViewData= _adminDashboard.GetAdminViewUploadData(model, requestId);
            var documents = _adminDashboard.GetFilesByRequestId(requestId);
            ViewBag.document = documents;
            return View(returnViewData);
        }


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
            bitarray.Set(0,true);

            RequestWiseFile requestWiseFile = _context.RequestWiseFiles.First(x => x.RequestWiseFileId == documentId);
            requestWiseFile.IsDeleted = bitarray;
            _context.RequestWiseFiles.Update(requestWiseFile);
            _context.SaveChanges();

            TempData["success"] = "File deleted successfully.";
            return RedirectToAction("ViewUploads","Admin", new { requestId });
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


        //public IActionResult ViewUploadDeleteAll(int requestId)
        //{
        //    var filesToDelete = _adminDashboard.GetAllFilesByRequestId(requestId);
        //    if(!filesToDelete.Any())
        //    {
        //        return NotFound();
        //    }
        //    foreach (var fileToDelete in filesToDelete)
        //    {
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/upload", fileToDelete.FileName);
        //        try
        //        {
        //            System.IO.File.Delete(filePath);
                    
        //        }
        //        catch(Exception ex)
        //        {
        //            Console.WriteLine($"Error while deleting File{ex.Message}");
        //            return StatusCode(500);// 500 is for internal server
        //        }
        //    }
        //    TempData["success"] = "All Files are deleted Sucessfully.";
        //    return RedirectToAction("ViewUploads","Admin");
        //}



        //this is for sending mail 
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


        //this part is for order details
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
            return Json(  vendordata );
        }
        [HttpPost]
        public IActionResult Orders(AdminOrderVm model,int requestID)
        {
            _adminDashboard.orderDataStore(model, requestID);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        //order part completed


        //this part is for transfer case
        public IActionResult TransferCase() 
        {
            return PartialView("_adminTransferCase"); 
        }


        //clear case
        public IActionResult ClearCase(int req)
        {
            return PartialView("_adminClearCase");
        }
    }
}
