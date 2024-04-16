using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using HelloDoc2.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MimeKit;
using System.Collections;
using System.Data;
using System.Net;
using System.Net.Mail;
using HellodocContext = DAL.Models.HellodocContext;
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

        //#region AdminLogin
        //public IActionResult AdminLogin()
        //{
        //    HttpContext.Session.Clear();
        //    Response.Cookies.Delete("Jwt");
        //    Response.Cookies.Delete(".AspNetCore.Session");
        //    Response.Cookies.Delete(".AspNetCore.Antiforgery.N0iw8MAOgzI");
        //    return View();
        //}
        //#endregion

        

        #region AdminLogout
        public IActionResult AdminLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Jwt");
            Response.Cookies.Delete(".AspNetCore.Session");
            Response.Cookies.Delete(".AspNetCore.Antiforgery.N0iw8MAOgzI");
            return RedirectToAction("Login", "Login");
        }
        #endregion

        //this page is for access denied
        #region AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
        //access denied completed

        [CustomAuthorize("1")]
        #region AdminDashboard
        public IActionResult AdminDashboard()
        {
            ViewBag.ActiveDashboardNav = "AdminDashboard";
            var request = _context.Requests;
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");


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


            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel()
            {
                statusArray = "1",
                reqTypId = "0",
                NewCount = newCount,
                PendingCount = pendingCount,
                ActiveCount = activeCount,
                ConcludeCount = concludeCount,
                ToCloseCount = toCloseCount,
                UnpaidCount = unpaidCount,
                StatusForName = 1,
                regions = getRegion,
            };
            return View(adminDashboardViewModel);
        }
        #endregion

        #region AdminDashboardTable
        public IActionResult AdminDashboardTable(string reqtypeid,string searchdata)
        {
            int PageNumber = 1;
            int PageSize = 5;
            ViewBag.ActiveDashboardNav = "AdminDashboard";

            string str = "1";
            int[] arr = { 1 };
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            var getRegion = _adminDashboard.getRegions();


            var requestAdmin = _adminDashboard.requestDataAdmin(str, arr, null, 0,searchdata);
            var requestAdminPaginationData = requestAdmin.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel()
            {
                requestListAdminDash = requestAdminPaginationData,
                statusArray = str,
                StatusForName = 1,
                reqTypId = "0",
                regions = getRegion,

                Page = new PageVm
                {
                    totalitem = requestAdmin.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = requestAdmin.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
                RequestTypeId = reqtypeid,
                RegionId = 0,
            };

            adminDashboardViewModel.TotalPages = (int)Math.Ceiling((decimal)adminDashboardViewModel.Page.totalitem / PageSize);
            return PartialView("_RequestsAccToStatus", adminDashboardViewModel);
        }
        #endregion
 
        #region FetchRequests
        public IActionResult FetchRequests(string statusarray,string status, string reqtypeid,int regionId, int PageNumber,string searchdata)
        {
            HttpContext.Session.SetString("statusfetch", statusarray);
            if(searchdata != null)
            {
                searchdata = null;
            }
            int PageSize = 5;
            int[] Status = status.Split(',').Select(s => int.Parse(s)).ToArray();
            var getRegion = _adminDashboard.getRegions();
            if(reqtypeid == null)
            {
                reqtypeid = "0";
            }
            var requestAdmin = _adminDashboard.requestDataAdmin(statusarray, Status, reqtypeid, regionId,searchdata);
            var requestAdminPaginationData = requestAdmin.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                requestListAdminDash = requestAdminPaginationData,
                StatusForName = Status[0],
                statusArray = statusarray,
                reqTypId = reqtypeid,
                regions = getRegion,
                Page = new PageVm
                {
                    totalitem = requestAdmin.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = requestAdmin.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
                
            };
            adminDashboardViewModel.RegionId = regionId;
            adminDashboardViewModel.TotalPages = (int)Math.Ceiling((decimal)adminDashboardViewModel.Page.totalitem / PageSize);
            return PartialView("_RequestsAccToStatus", adminDashboardViewModel);
        }
        #endregion

        #region FilterRequests
        public IActionResult FilterRequests(string statusarray, int[] status, string reqtypeid,int regionId,int PageNumber,string searchdata)
        {
            statusarray = HttpContext.Session.GetString("statusfetch");
            int PageSize = 5;
            //int[] Status = status.Split(',').Select(s => int.Parse(s)).ToArray();
            var getRegion = _adminDashboard.getRegions();

            var requestListAdmin = _adminDashboard.requestDataAdmin(statusarray, status, reqtypeid, regionId,searchdata);
            var requestAdminPaginationData = requestListAdmin.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();


            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                requestListAdminDash = requestAdminPaginationData,
                StatusForName = status[0],
                statusArray = statusarray,
                regions = getRegion,
                reqTypId = reqtypeid,
                Page = new PageVm
                {
                    totalitem = requestListAdmin.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = requestListAdmin.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
                RequestTypeId = reqtypeid,
            };
            adminDashboardViewModel.RegionId = regionId;
            adminDashboardViewModel.TotalPages = (int)Math.Ceiling((decimal)adminDashboardViewModel.Page.totalitem / PageSize);
            return PartialView("_RequestsAccToStatus", adminDashboardViewModel);
        }
        #endregion

        #region FilterPagination
        public IActionResult FilterPagination(string statusarray, int[] status, string reqtypeid, int regionId, int PageNumber, string RequestTypeId,string searchdata)
        {
            int PageSize = 5;
            //int[] Status = status.Split(',').Select(s => int.Parse(s)).ToArray();
            var getRegion = _adminDashboard.getRegions();

            var requestListAdmin = _adminDashboard.requestDataAdmin(statusarray, status, RequestTypeId, regionId,searchdata);
            var requestAdminPaginationData = requestListAdmin.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();


            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel
            {
                requestListAdminDash = requestAdminPaginationData,
                StatusForName = status[0],
                statusArray = statusarray,
                regions = getRegion,
                reqTypId = reqtypeid,
                Page = new PageVm
                {
                    totalitem = requestListAdmin.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = requestListAdmin.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
                RequestTypeId = RequestTypeId,
            };
            adminDashboardViewModel.TotalPages = (int)Math.Ceiling((decimal)adminDashboardViewModel.Page.totalitem / PageSize);
            return PartialView("_RequestsAccToStatus", adminDashboardViewModel);
        }
        #endregion FilterPagination

        // this is for accordian to fetch names
        #region GetNameByRequestTypeId
        public string GetNameByRequestTypeId(int requestTypeId)
        {
            Dictionary<int, string> reqTypesName = new Dictionary<int, string>()
            {
                {1, "Patient" },
            };
            return requestTypeId.ToString() != null ? reqTypesName.GetValueOrDefault(requestTypeId, "Unknown Request Type"): "Unknown Request Type";
        }
        #endregion


        #region ViewCase
        [CustomAuthorize("1")]
        public IActionResult ViewCase(int requestId)
        {
            var data = _adminDashboard.ViewCase(requestId);
            return View(data);
        }
        #endregion

        #region ViewNotes : get
        [CustomAuthorize("1")]
        //view notes start : 2 methods
        public IActionResult ViewNotes(int requestId)
        {
            var data = _adminDashboard.ViewNotes(requestId);
            return View(data);
        }
        #endregion

        #region ViewNotes : post
        [HttpPost]
        public IActionResult ViewNotes(ViewNotesVm model, int requestId) {
            _adminDashboard.editViewNotes(model, requestId);
            model = _adminDashboard.ViewNotes(requestId);
            return View(model);
        }
        #endregion
        //view notes completed

        [CustomAuthorize("1")]
        //cancel case start : 2 methods
        #region CancelCase : get
        public IActionResult CancelCase(int req)
        {
            HttpContext.Session.SetInt32("reqId", req);
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel();
            adminDashboardViewModel.CaseTags = _adminDashboard.cancelCaseMain();
            return PartialView("_adminCancelCase", adminDashboardViewModel);
        }
        #endregion

        #region CancelCase : post
        [HttpPost]
        public IActionResult CancelCase(AdminDashboardViewModel model)
        {
            int? req = HttpContext.Session.GetInt32("reqId");
            _adminDashboard.cancelCase(model, req ?? 0);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        #endregion
        //cancel case completed


        [CustomAuthorize("1")]
        //block case start : 2 methods
        #region BlockCase
        public IActionResult BlockCase(int req)
        {
            HttpContext.Session.SetInt32("requestid", req);
            AdminBlockVm adminBlockVm = new AdminBlockVm();
            adminBlockVm = _adminDashboard.adminBlockVm(req);
            return PartialView("_adminBlockCase", adminBlockVm);
        }
        #endregion


        #region BlockCasePost
        [HttpPost]
        public IActionResult BlockCasePost(AdminBlockVm model)
        {
            var req = HttpContext.Session.GetInt32("requestid");
            _adminDashboard.blockCase(model, req ?? 0);
            TempData["success"] = "User Blocked Successfully";
            return RedirectToAction("AdminDashboard", "Admin");
        }
        #endregion
        //block case completed

        #region AsignCase
        [CustomAuthorize("1")]
        //this three methods are for asign case
        public IActionResult AsignCase(int req)
        {
            HttpContext.Session.SetInt32("asignReq", req);
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.regions = _adminDashboard.asignCase();
            return PartialView("_adminAsignCase", adminAsignVm);
        }
        #endregion

        #region GetPhysiciansByRegionId
        public IActionResult GetPhysiciansByRegionId(int regionId)
        {
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.physicianList = _adminDashboard.asignPhysician(regionId);
            return Json(new { adminAsignVm });
        }
        #endregion

        #region AsignCasePost
        [HttpPost]
        public IActionResult AsignCasePost(AdminAsignVm model)
        {
            var req = HttpContext.Session.GetInt32("asignReq");
            _adminDashboard.asignCasePost(model, req ?? 0, 2);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        #endregion
        //asign case completed

        //this part is for viewupload : total 6 methods for all view upload
        [CustomAuthorize("1")]
        #region ViewUploads
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
        #endregion

        //upload file in view uploads
        #region Upload : post
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
        #endregion
        //upload file in view upload completed


        //this part is for download single file for view upload
        #region ViewUploadDownload
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
        #endregion
        //single file download complete

        #region ViewUploadDelete : single file
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
        #endregion

        #region DelteSelectedDocuments
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
        #endregion

        #region SendDocumentsByMail
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
        #endregion
        //view upload completed


        //this part is for order details : 4 methods
        [CustomAuthorize("1")]
        #region Orders
        public IActionResult Orders(int requestID)
        {
            AdminOrderVm adminOrderVm = new AdminOrderVm();
            adminOrderVm.healthProfessionType = _adminDashboard.healthProfessionalTypes();
            adminOrderVm.RequestId = requestID;
            return View(adminOrderVm);
        }
        #endregion Orders

        #region BusinessSelect
        public IActionResult BusinessSelect(int healthProfessionId)
        {
            AdminOrderVm adminOrderVm = new AdminOrderVm();
            adminOrderVm.healthProfessionals = _adminDashboard.asignBusiness(healthProfessionId);
            return Json(new { adminOrderVm });
        }
        #endregion

        #region GetVendorDetails
        public IActionResult GetVendorDetails(int vendorId)
        {
            var vendordata = _adminDashboard.getVendorDetails(vendorId);
            return Json(vendordata);
        }
        #endregion

        #region Orders : post
        [HttpPost]
        public IActionResult Orders(AdminOrderVm model, int requestID)
        {
            _adminDashboard.orderDataStore(model, requestID);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        #endregion
        //order part completed


        //this part is for transfer case -- transfer and asign similar that's why use same model for that
        [CustomAuthorize("1")]
        #region TransferCase
        public IActionResult TransferCase(int reqId)
        {
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.regions = _adminDashboard.transferRegion();
            adminAsignVm.RequestId = reqId;
            return PartialView("_adminTransferCase", adminAsignVm);
        }
        #endregion

        #region GetPhysiciansByRegionIdTransfer
        public IActionResult GetPhysiciansByRegionIdTransfer(int regionId)
        {
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.physicianList = _adminDashboard.transferPhysician(regionId);
            return Json(new { adminAsignVm });
        }
        #endregion

        #region TransferCasePost
        public IActionResult TransferCasePost(AdminAsignVm model)
        {
            _adminDashboard.transferCasePost(model, 2);
            return RedirectToAction("AdminDashboard", "Admin");
        }
        #endregion

        //transfer case completed

        //this is for download excel
        //this is download excel file for indevidual status (new,pending, active,conclude, toclose and unpaid)
        #region DownloadExcel
        public IActionResult DownloadExcel(string statusarray, int[] status, int regionId, string searchdata)
        {
            string str = statusarray;
            var data = _adminDashboard.requestDataAdmin(str, status, null, regionId, searchdata);
            var excelData = _adminDashboard.ExportToExcel(data);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AdminData.xlsx");
        }
        #endregion

        //this method id used for download excel file for all request
        #region DownloadExcelAll
        public IActionResult DownloadExcelAll()
        {
            var data = _adminDashboard.requestDataDownloadExcelAll();
            var excelData = _adminDashboard.ExportToExcel(data);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AdminData.xlsx");
        }
        #endregion


        //clear case : 2 methods
        [CustomAuthorize("1")]
        #region ClearCase
        public IActionResult ClearCase(int req)
        {
            AdminClearVm adminClearVm = new AdminClearVm();
            adminClearVm.RequestId = req;
            return PartialView("_adminClearCase", adminClearVm);
        }
        #endregion

        #region ClearCase
        [HttpPost]
        public IActionResult ClearCase(AdminClearVm model)
        {
            _adminDashboard.clearCasePost(model);
            return RedirectToAction("AdminDashboard");
        }
        #endregion
        //clear case completed

        //create request for admin part
        #region CreateRequestForAdmin
        public IActionResult CreateRequestForAdmin()
        {
            return View();
        }
        #endregion

        //send agreement-used adminclear case model for send agreement
        [CustomAuthorize("1")]
        #region SendAgreement
        public IActionResult SendAgreement(int req, int reqTypeId)
        {
            var data = _context.RequestClients.FirstOrDefault(x => x.RequestId == req);
            AdminClearVm adminClearVm = new AdminClearVm();
            adminClearVm.RequestTypeId = reqTypeId;
            adminClearVm.RequestId = req;
            adminClearVm.Number = "879879";
            adminClearVm.Email = "sdfsf";
            return PartialView("_adminSendAgreement", adminClearVm);
        }
        #endregion

        #region SendAgreement : post
        [HttpPost]
        public async Task<IActionResult> SendAgreement(AdminClearVm model)
        {
            
            var subject = "Review Agreement Request";
            var agreementLink = "<a href=" + Url.Action("ReviewAgreement", "Admin", new { email = model.Email, RequestId = model.RequestId }, "https") + ">Confirm Agreement</a>";

            
            var body = "<b>Please find the Password Reset Link.</b><br/>" + agreementLink;

            await SendEmailAsync(model.Email, subject, body);
            return RedirectToAction("AdminDashboard");
        }
        #endregion
        //this sendemail function is work for all send mail that are present in admin page
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

        //this part is for review agreement
        #region ReviewAgreement
        public IActionResult ReviewAgreement(string email, int requestId)
        {
            ReviewAgreementVm reviewAgreementVm = new ReviewAgreementVm();
            reviewAgreementVm.RequestId = requestId;
            return View(reviewAgreementVm);
         }
        #endregion

        #region ReviewAgreementSubmit
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
            return RedirectToAction("Login","Login");
        }
        #endregion

        #region ReviewAgreementCancel
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
            return RedirectToAction("Login","Login");
        }
        #endregion
        //review agreement complete


        //send mail-used adminclear case model for send mail to patient 
        //this are one of the five buttons
        #region SendMail
        public IActionResult SendMail()
        {
            return PartialView("_adminSendMail");
        }
        #endregion

        [HttpPost]
        #region SendMail : post
        public async Task<IActionResult> SendMail(AdminClearVm model)
        {
            var subject = "Submit Request";
            var submitLink = "<a href=" + Url.Action("Submit_Request", "Home", new { email = model.Email, RequestId = model.RequestId }, "https") + ">Submit Your Request</a>";

            var body = "<b>Please find the Password Reset Link.</b><br/>" + submitLink;

            await SendEmailAsync(model.Email, subject, body);
            return RedirectToAction("AdminDashboard");
        }
        #endregion 

        #region AdminCreateRequest
        public IActionResult AdminCreateRequest()
        {
            return View();
        }
        #endregion 


        //this part is for close case
        #region CloseCase
        public IActionResult CloseCase(int requestId)
        {
            var data = _adminDashboard.closeCaseGet(requestId);
            return View(data);
        }
        #endregion

        #region CloseCaseEdit
        public IActionResult CloseCaseEdit(int command, CloseCaseVm model, int requestId)
        {
            _adminDashboard.closeCaseEdit(command, model, requestId);
            var data = _adminDashboard.closeCaseGet(requestId);
            return View("CloseCase", data);
        }
        #endregion
        //close case completed

        //this is for admin profile 
        [CustomAuthorize("1")]
        //[RoleAuthorize(1)]
        #region AdminMyProfile
        public IActionResult AdminMyProfile(int adminId)
        {
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            if(adminId == 0)
            {
            adminId = HttpContext.Session.GetInt32("AdminId")?? 0;

            }
            ViewBag.ActiveDashboardNav = "AdminMyProfile";
            int? id = HttpContext.Session.GetInt32("AspId");
            var data = _adminDashboard.getAdminDetails(id??0, adminId);
            return View(data);
        }
        #endregion

        #region AdminResetPassword
        public IActionResult AdminResetPassword(AdminProfileVm model)
        {
            int? adminId = HttpContext.Session.GetInt32("AdminId");
            _adminDashboard.adminResetPassword(model);
            int? id = HttpContext.Session.GetInt32("AspId");
            var data = _adminDashboard.getAdminDetails(id ?? 0, adminId??0);
            TempData["success"] = "Password Changed Successfully";
            ViewBag.ActiveDashboardNav = "AdminMyProfile";
            return View("AdminMyProfile",data);
        }
        #endregion

        #region AdminEditDetails1
        public IActionResult AdminEditDetails1(AdminProfileVm model, List<int>? checkboxForAll)
        {

            int? adminId = HttpContext.Session.GetInt32("AdminId");
            _adminDashboard.adminEditDetails1(model, checkboxForAll, adminId ?? 0);
            int? id = HttpContext.Session.GetInt32("AspId");
            var data = _adminDashboard.getAdminDetails(id ?? 0, adminId??0);
            TempData["success"] = "Details Changed successfully";
            return View("AdminMyProfile",data);
        }
        #endregion

        #region AdminEditDetails2
        public IActionResult AdminEditDetails2(AdminProfileVm model)
        {
            int? adminId = HttpContext.Session.GetInt32("AdminId");
            _adminDashboard.adminEditDetails2(model);
            int? id = HttpContext.Session.GetInt32("AspId");
            var data = _adminDashboard.getAdminDetails(id ?? 0, adminId??0);
            TempData["success"] = "Details Changed successfully";
            return View("AdminMyProfile",data);
        }
        #endregion

        //admin profile part is completed

        #region EncounterForm
        [CustomAuthorize("1")]
        //this part is for encounter form 
        public IActionResult EncounterForm(int reqId)
        {
            var data = _adminDashboard.encounterFormGetData(reqId);
            return View(data);
        }
        #endregion

        #region PostEncounterData
        public IActionResult PostEncounterData(EncounterVm model)
        {
            _adminDashboard.postEncounterData(model);
            return View("EncounterForm");
        }
        #endregion

        [CustomAuthorize("1")]
        //provider part 
        #region Provider
        public IActionResult Provider(string regionId)
        {
            ViewBag.ActiveDashboardNav = "Provider";
            ViewBag.ActiveDropdown = "Provider";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            ProviderVm providerVm = new ProviderVm();
            providerVm.regions = _adminDashboard.getRegions();
            //providerVm.providers = _adminDashboard.getPhysicianDetails(regionId);
            return View(providerVm);    
        }
        #endregion

        #region ProviderTable
        public IActionResult ProviderTable(string regionId)
        {
            ProviderVm providerVm = new ProviderVm();
            providerVm.regions = _adminDashboard.getRegions();
            providerVm.providers = _adminDashboard.getPhysicianDetails(regionId);
            return PartialView("_adminProviderTable", providerVm);
        }
        #endregion

        #region UpdateIsNotificationStopped
        [HttpPost]
        public IActionResult UpdateIsNotificationStopped(int physicianId)
        {
            var notification = _context.PhysicianNotifications.FirstOrDefault(x => x.PhysicianId == physicianId);
            if (notification != null)
            {
                if(notification.IsNotificationStopped == false)
                {
                    notification.IsNotificationStopped = true;
                    _context.SaveChanges();
                }
                else
                {
                    notification.IsNotificationStopped = false;
                    _context.SaveChanges();
                }
            }
            return Ok();
        }
        #endregion

        //this is edit physician account part
        #region EditPhysicianAccount
        [HttpGet]
        public IActionResult EditPhysicianAccount(int physicianId)
        {
            ViewBag.ActiveDashboardNav = "Provider";
            var data = _adminDashboard.getPhysicianDetails(physicianId);
            return View(data);
        }
        #endregion

        #region EditPhysicianPassword
        public IActionResult EditPhysicianPassword(ProviderVm model)
        {
            _adminDashboard.editPhysicianPassword(model);
            var data = _adminDashboard.getPhysicianDetails(model.PhysicianId);
            return View("EditPhysicianAccount", data);
        }
        #endregion

        #region PhysicianEditDetails1
        public IActionResult PhysicianEditDetails1(ProviderVm model, List<int>? checkboxForAll)
        {
            _adminDashboard.physicianEditDetails1(model, checkboxForAll);
            var data = _adminDashboard.getPhysicianDetails(model.PhysicianId);
            return View("EditPhysicianAccount", data);
        }
        #endregion

        #region PhysicianEditDetails2
        public IActionResult PhysicianEditDetails2(ProviderVm model)
        {
            _adminDashboard.physicianEditDetails2(model);
            var data = _adminDashboard.getPhysicianDetails(model.PhysicianId);
            return View("EditPhysicianAccount", data);
        }
        #endregion

        #region PhysicianEditDetails3
        public IActionResult PhysicianEditDetails3(ProviderVm model)
        {
            _adminDashboard.physicianEditDetails3(model);
            var data = _adminDashboard.getPhysicianDetails(model.PhysicianId);
            return View("EditPhysicianAccount", data);
        }
        #endregion

        #region ContactProvider
        public IActionResult ContactProvider(int phyId)
        {
            var data = _adminDashboard.getPhysicianDetailsByPhysicianId(phyId);
            return PartialView("Admin/_providerContactPopup",data);
        }
        #endregion

        #region AdminSendMailToProvider
        public async Task<IActionResult> AdminSendMailToProvider(ProviderVm model,string ContactType)
        {
            var subject = "Provider Contact";
            var body = "Description:-" + model.Description;

            _adminDashboard.adminSendMailToProvider(model, subject, body,ContactType);
            if (ContactType == "Email")
            {
                TempData["success"] = "Email Send Successfully!";
            }

            return RedirectToAction("Provider");
        }
        #endregion

        //this is for create provider account by admin
        #region CreateProviderAccountByAdmin : get
        [HttpGet]
        public IActionResult CreateProviderAccountByAdmin()
        {
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            ProviderVm providerVm = new ProviderVm();
            providerVm.regions = _adminDashboard.getRegions();
            providerVm.roles = _adminDashboard.getRoles();
            ViewBag.ActiveDashboardNav = "Provider";
            return View(providerVm);
        }
        #endregion

        #region CreateProviderAccount : post
        [HttpPost]
        public IActionResult CreateProviderAccount(ProviderVm model, List<int>? checkboxForAll)
        {
            _adminDashboard.createProviderAccount(model, checkboxForAll);
            return RedirectToAction("Provider", "Admin");
        }
        #endregion

        #region UploadSign
        [HttpPost]
        public IActionResult UploadSign([FromForm] IFormFile Filepath)
        {
            string fileName = Path.GetFileName(Filepath.FileName);
            var filePath = Path.Combine("wwwroot", "upload", fileName);
            using (FileStream stream = System.IO.File.Create(filePath))
            {
                // The file is saved in a buffer before being processed
                Filepath.CopyTo(stream);
            }
            TempData["Uploadscs"] = "File Uploaded Succssfully.Please Refresh Page";
            return Json(new { file1 = filePath });
        }
        #endregion UploadSign

        #region UploadPhoto : post
        [HttpPost]
        public IActionResult UploadPhoto([FromForm] IFormFile Filepath)
        {
            string fileName = Path.GetFileName(Filepath.FileName);
            var filePath = Path.Combine("wwwroot", "upload", fileName);
            using (FileStream stream = System.IO.File.Create(filePath))
            {
                // The file is saved in a buffer before being processed
                Filepath.CopyTo(stream);
            }
            TempData["Uploadscs"] = "File Uploaded Succssfully.Please Refresh Page";
            return Json(new { file1 = filePath });
        }
        #endregion

        #region UploadFileForPhysician
        public IActionResult UploadFileForPhysician(ProviderVm model)
        {
            try
            {
                _adminDashboard.updateProviderDoc(model);
                TempData["UploadStatus"] = "success";
            }
            catch (Exception ex)
            {
                TempData["UploadStatus"] = "error";
            }
            return RedirectToAction("EditPhysicianAccount",new {physicianId = model.PhysicianId});
        }
        #endregion


        // this part is for access role
        #region Access
        public IActionResult Access()
        {
            AccessVm accssVm = new AccessVm();
            accssVm.access =  _adminDashboard.getAccessRoles();
            ViewBag.ActiveDashboardNav = "Access";
            ViewBag.ActiveDropdown = "access";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View(accssVm);
        }
        #endregion

        #region CreateAccess : get
        [HttpGet]
        public IActionResult CreateAccess()
        {
            AccessVm accessVm = new AccessVm();
            accessVm.menu = _adminDashboard.getMenu();
            accessVm.aspNetRole = _adminDashboard.getAspNetRoles();
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View(accessVm);
        }
        #endregion

        #region fetchrole 
        public IActionResult fetchrole(int Id)
        {
            AccessVm accessVm = new AccessVm();
            accessVm.menu = _adminDashboard.getAccess(Id);
            return PartialView("Admin/_CreateAccessTable", accessVm);
        }
        #endregion

        #region CreateRole
        public IActionResult CreateRole(AccessVm model,List<int>? checkboxForAllRole)
        {
            _adminDashboard.createRole(model,checkboxForAllRole);
            AccessVm accssVm = new AccessVm();
            accssVm.access = _adminDashboard.getAccessRoles();
            ViewBag.ActiveDashboardNav = "Access";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View("Access", accssVm);
        }
        #endregion


        //this method is for check role name if it already exists
        #region CheckRoleName : post
        [HttpPost]
        public IActionResult CheckRoleName(string roleName)
        {
            // Check if the role name already exists
            var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                return Json(new { exists = true });
            }
            else
            {
                return Json(new { exists = false });
            }
        }
        #endregion

        #region EditAccessRole : get
        [HttpGet]
        public IActionResult EditAccessRole(int RoleId, int AccountType)
        {
            AccessVm data = _adminDashboard.editAccessRole(RoleId, AccountType);
            data.aspNetRole = _adminDashboard.getAspNetRoles();
            ViewBag.ActiveDashboardNav = "Access";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View(data);
        }
        #endregion

        #region EditAccessRolePost
        public IActionResult EditAccessRolePost(AccessVm model, List<int>? checkboxForAllRole)
        {
            ViewBag.ActiveDashboardNav = "Access";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            _adminDashboard.editAccessRolePost(model,checkboxForAllRole);
            AccessVm accssVm = new AccessVm();
            accssVm.access = _adminDashboard.getAccessRoles();
            return View("Access",accssVm);
        }
        #endregion

        #region DeleteAccessRole
        public IActionResult DeleteAccessRole(int RoleId)
        {
            _adminDashboard.deleteAccessRole(RoleId);
            return RedirectToAction("Access");
        }
        #endregion

        #region UserAccess
        public IActionResult UserAccess(int RoleId)
        {
            ViewBag.ActiveDashboardNav = "Access";
            ViewBag.ActiveDropdown = "UserAccess";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View();
        }
        #endregion

        #region UserAccessTable
        public IActionResult UserAccessTable(int roleId)
        {
            AccessVm accessVm = new AccessVm();
            accessVm.userAccess = _adminDashboard.getUserAccessData(roleId);
            return PartialView("Admin/_UserAccessTable",accessVm);
        }
        #endregion

        //this is for admin create account
        #region AdminCreateAccount : get
        [HttpGet]
        public IActionResult AdminCreateAccount()
        {
            ViewBag.ActiveDashboardNav = "Access";
            AdminProfileVm adminProfileVm = new AdminProfileVm();
            adminProfileVm.regions = _adminDashboard.getRegions();
            return View(adminProfileVm);
        }
        #endregion

        #region AdminCreateAccount :post
        [HttpPost]
        public IActionResult AdminCreateAccount(AdminProfileVm model, List<int>? checkboxForAllRegion)
        {
            _adminDashboard.adminCreateAccount(model, checkboxForAllRegion);
            return RedirectToAction("AdminDashboard","Admin");
        }
        #endregion

        //this part is for recored 
        #region PatientRecord
        public IActionResult PatientRecord()
        {
            RecordVm recordVm = new RecordVm();
            recordVm.pageSize = 0;
            ViewBag.ActiveDashboardNav = "Record";
            ViewBag.ActiveDropdown = "PatientRecord";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View("Record/PatientRecord",recordVm);
        }
        #endregion

        #region PatientHistoryTable
        public IActionResult PatientRecordTable(string firstname, string lastname, string email, string phonenumber, int PageNumber, int PageSize) 
        {
            if(PageNumber == 0)
            {
                PageNumber = 1;
            }
            if(PageSize == 0)
            {
                PageSize = 5;
            }
            //int PageSize = 5;
            RecordVm recordVm = new RecordVm();
            var data = _adminDashboard.patientHistoryData(firstname, lastname, email, phonenumber);
			var pageinatedData = data.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            recordVm.patientHistory = pageinatedData;
			recordVm.Page = new PageVm
            {
                totalitem = data.Count(),
                currentpage = PageNumber,
                itemperpage = PageSize,
            };
            recordVm.pageSize = PageSize;
            recordVm.skipCount = data.Take((PageNumber - 1) * PageSize).ToList().Count();
            recordVm.CurrentPage = PageNumber;
			recordVm.TotalPages = (int)Math.Ceiling((decimal)recordVm.Page.totalitem / PageSize);

			return PartialView("Record/_PatientRecordTable",recordVm);
        }
        #endregion

        #region PatientRecordExplore
        public IActionResult PatientRecordExplore(int patientId)
        {
            ViewBag.ActiveDashboardNav = "Record";
            ViewBag.ActiveDropdown = "PatientRecord";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");

            RecordVm recordVm = new RecordVm();
            recordVm.patientRecordExplores = _adminDashboard.patientRecordExploreData(patientId);
            return View("Record/PatientRecordExplore",recordVm);
        }
        #endregion

        #region EmailLog
        public IActionResult EmailLog()
		{
			RecordVm recordVm = new RecordVm();
			recordVm.roles = _adminDashboard.getRoles();
            ViewBag.ActiveDashboardNav = "Record";
            ViewBag.ActiveDropdown = "EmailLog";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View("Record/EmailLog", recordVm);
		}
		#endregion

		#region EmailLogTable
		public IActionResult EmailLogTable(int roleId,string receiverName, string email,DateOnly createdDate, DateOnly sentDate,int PageNumber)
		{
            if(PageNumber == 0)
            {
                PageNumber = 1;
            }
            int PageSize = 5;
			RecordVm recordVm = new RecordVm();
            var data = _adminDashboard.emailLogData(roleId, receiverName, email, createdDate, sentDate);
            var paginatedData = data.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            recordVm.emailLog = paginatedData;
            recordVm.Page = new PageVm
            {
                currentpage = PageNumber,
                totalitem = data.Count(),
                itemperpage = PageSize,
            };
            recordVm.pageSize = PageSize;
            recordVm.skipCount = data.Take((PageNumber - 1)*PageSize).ToList().Count();
            recordVm.TotalPages = (int)Math.Ceiling((decimal)recordVm.Page.totalitem / PageSize);
			recordVm.roles = _adminDashboard.getRoles();
			return PartialView("Record/_EmailLogTable", recordVm);
		}
		#endregion

		#region SearchRecord
		public IActionResult SearchRecord()
		{
            ViewBag.ActiveDashboardNav = "Record";
            ViewBag.ActiveDropdown = "SearchRecord";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View("Record/SearchRecord");
		}
		#endregion

		#region SearchRecordTable 
		public IActionResult SearchRecordTable(string patientname, int requesttype,int stauts, string email, DateOnly fromdate, DateOnly todate, string providername,string phoneNumber, int PageNumber)
		{

            if(PageNumber == 0)
            {
                PageNumber = 1;
            }
            int PageSize = 10;
			RecordVm recordVm = new RecordVm();
            var data = _adminDashboard.searchRecordData(patientname, stauts, requesttype, email, fromdate, todate, providername, phoneNumber, PageNumber);
            var paginatedData = data.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            recordVm.searchRecord = paginatedData;

            recordVm.Page = new PageVm
            {
                totalitem = data.Count(),
                currentpage = PageNumber,
                itemperpage = PageSize,
            };
            recordVm.pageSize = PageSize;
            recordVm.CurrentPage = PageNumber;
            recordVm.skipCount = data.Take((PageNumber - 1) * PageSize).ToList().Count();
            recordVm.TotalPages = (int)Math.Ceiling((double)recordVm.Page.totalitem / PageSize);
			return PartialView("Record/_SearchRecordTable", recordVm);
		}
		#endregion

		#region SMSLog
		public IActionResult SMSLog()
        {
			RecordVm recordVm = new RecordVm();
			recordVm.roles = _adminDashboard.getRoles();
            ViewBag.ActiveDashboardNav = "Record";
            ViewBag.ActiveDropdown = "SmsLog";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View("Record/SMSLog",recordVm);
        }
		#endregion

		#region SMSLogTable
        public IActionResult SMSLogTable(int roleId, string receiverName, string phoneNumber, DateOnly createdDate, DateOnly sentDate, int PageNumber)
        {
            if(PageNumber == 0)
            {
                PageNumber = 1;
            }
            int PageSize = 5;

            RecordVm recordVm = new RecordVm();
            var data = _adminDashboard.smsLogData(roleId, receiverName, phoneNumber, createdDate, sentDate);
            var paginatedData = data.Skip((PageNumber - 1)*PageSize).Take(PageSize).ToList();   
            recordVm.smsLog = paginatedData;
            recordVm.Page = new PageVm
            {
                totalitem = data.Count(),
                currentpage = PageNumber,
                itemperpage = PageSize,
            };
            recordVm.pageSize = PageSize;
            recordVm.CurrentPage = PageNumber;
            recordVm.TotalPages = (int)Math.Ceiling((decimal)recordVm.Page.totalitem / PageSize);
            recordVm.skipCount = data.Take((PageNumber - 1) * PageSize).ToList().Count();
			return PartialView("Record/_SmsLogTable",recordVm);
        }
		#endregion

		#region BlockHistory
		public IActionResult BlockHistory()
        {
            ViewBag.ActiveDashboardNav = "Record";
            ViewBag.ActiveDropdown = "BlockHistory";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View("Record/BlockHistory");
        }
        #endregion

        #region BlockHistoryTable 
        public IActionResult BlockHistoryTable(string patientname, DateOnly date, string email,string phonenumber, int PageNumber)
        {
            if(PageNumber == 0)
            {
                PageNumber = 1;
            }
            int PageSize = 5;
            RecordVm recordVm = new RecordVm();
            var data = _adminDashboard.blockHistoryData(patientname, date, email, phonenumber, PageNumber);
            var paginatedData = data.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            recordVm.blockHistory = paginatedData;
            recordVm.Page = new PageVm
            {
                totalitem = data.Count(),
                currentpage = PageNumber,
                itemperpage = PageSize,
            };
            recordVm.pageSize = PageSize;
            recordVm.skipCount = data.Take((PageNumber - 1) * PageSize).ToList().Count();
            recordVm.CurrentPage = PageNumber;
            recordVm.TotalPages = (int)Math.Ceiling((decimal)recordVm.Page.totalitem / PageSize);
            return PartialView("Record/_BlockHistoryTable",recordVm);
        }
		#endregion

		#region UnblockRequest
        public IActionResult UnblockRequest(int requestId, int blockReqId)
        {
            _adminDashboard.unblockRequest(requestId, blockReqId);
            TempData["unblockrequest"] = "Request Unblock Successfully";
            return RedirectToAction("BlockHistory");
        }
        #endregion
        //record complete

        //this part is for partners
        #region Partners
        public IActionResult Partners()
        {
            PartnersVm partnersVm = new PartnersVm();
            ViewBag.ActiveDashboardNav = "Partners";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            partnersVm.healthProfessionalType = _adminDashboard.getProfessionList();
            return View(partnersVm);
        }
        #endregion

        #region ProfessionTable
        public IActionResult ProfessionTable(int Id, string searchvalue)
        {
            PartnersVm partnersVm = new PartnersVm();
            partnersVm.vendor = _adminDashboard.getVendorList(Id, searchvalue);
            partnersVm.healthProfessionalType = _adminDashboard.getProfessionList();
            return PartialView("Admin/_PartnersTable", partnersVm);
        }
        #endregion ProfessionTable

        #region AddBusiness
        public IActionResult AddBusiness()
        {
            PartnersVm partnersVm = new PartnersVm();
            ViewBag.ActiveDashboardNav = "Partners";
            partnersVm.healthProfessionalType = _adminDashboard.getProfessionList();
            return View(partnersVm);
        }
        #endregion

        #region AddBusiness : post
        [HttpPost]
        public IActionResult AddBusiness(PartnersVm model)
        {
            _adminDashboard.addBusiness(model);
            PartnersVm partnersVm = new PartnersVm();
            ViewBag.ActiveDashboardNav = "Partners";
            partnersVm.healthProfessionalType = _adminDashboard.getProfessionList();
            return View("Partners", partnersVm);
        }
        #endregion

        #region UpdateBusiness
        [HttpGet]
        public IActionResult UpdateBusiness(int vendorId)
        {
            Vendor vendor = new Vendor();
            vendor = _adminDashboard.updateBusiness(vendorId);
            return View(vendor);
        }
        #endregion UpdateBusiness

        #region UpdateBusiness : post method
        [HttpPost]
        public IActionResult UpdateBusiness(Vendor model)
        {
            _adminDashboard.updateBusinessPost(model);
            PartnersVm partnersVm = new PartnersVm();
            ViewBag.ActiveDashboardNav = "Partners";
            partnersVm.healthProfessionalType = _adminDashboard.getProfessionList();
            return View("Partners", partnersVm);
        }
        #endregion UpdateBusiness : post method

        #region PartnersDeleteVendors
        public IActionResult PartnersDeleteVendors(int vendorId)
        {
            _adminDashboard.partnersDeleteVendors(vendorId);
            PartnersVm partnersVm = new PartnersVm();
            ViewBag.ActiveDashboardNav = "Partners";
            partnersVm.healthProfessionalType = _adminDashboard.getProfessionList();
            return View("Partners",partnersVm);
        }
		#endregion

		#region Scheduling
		public IActionResult Scheduling()
        {
            SchedulingVm schedulingVm = new SchedulingVm();
            schedulingVm.region = _adminDashboard.getRegions();
            ViewBag.ActiveDashboardNav = "Provider";
            ViewBag.ActiveDropdown = "Scheduling";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            return View(schedulingVm);  
        }
		#endregion

		#region GetProviderDetailsForSchedule
		public IActionResult GetProviderDetailsForSchedule(int RegionId)
		{
			List<Physician> model = _adminDashboard.fetchRegionWiseProviders(RegionId);

			List<ShiftViewModel> list = model.Select(p => new ShiftViewModel
			{
				Id = p.PhysicianId,
				title = p.FirstName ?? "",
				imageUrl = "https://api.slingacademy.com/public/sample-photos/" + new Random().Next(0, 100) + ".jpeg"
			}).ToList();

			return Json(list);
		}
		#endregion

		#region GetScheduleData
		public IActionResult GetScheduleData()
		{
			string[] color = { "#edacd2", "#a5cfa6" };
			List<ShiftDetail> shiftDetails = _adminDashboard.GetScheduleData();

			List<SchedulingDetailsViewModel> list = shiftDetails.Select(s => new SchedulingDetailsViewModel
			{
				resourceId = s.Shift.PhysicianId,
				Id = s.ShiftDetailId,
				//title = "Event " + s.ShiftDetailId,
				title = s.Shift.Physician.FirstName +"_Shift-"+ s.ShiftDetailId,
				start = s.ShiftDate.ToString("yyyy-MM-dd") + s.StartTime.ToString("THH:mm:ss"),
				end = s.ShiftDate.ToString("yyyy-MM-dd") + s.EndTime.ToString("THH:mm:ss"),
				color = color[s.Status]
			}).ToList();


			return Json(list);
		}
		#endregion


		//this part is for provider location
		#region ProviderLocation
		public IActionResult ProviderLocation()
        {
            ViewBag.ActiveDashboardNav = "ProviderLocation";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            var data = _adminDashboard.getPhysicianLocation();
            return View(data);
        }
        #endregion

        //provider location complete

        #region ViewShift
        public IActionResult ViewShift(int ShiftDetailId)
        {
            var viewShiftVm = _adminDashboard.viewShiftGetData(ShiftDetailId);
            return PartialView("Admin/ViewShift",viewShiftVm);
        }
        #endregion

        #region ViewShiftPost
        public IActionResult ViewShiftPost(ViewShiftVm model, int command)
        {
            _adminDashboard.viewShiftPost(model,command);
            SchedulingVm schedulingVm = new SchedulingVm();
            schedulingVm.region = _adminDashboard.getRegions();
            if(command == 3)
            {
                TempData["ViewShiftMessage"] = "Shift Deleted Successfully";
            }
            return RedirectToAction("Scheduling","Admin", schedulingVm);
        }
        #endregion

        #region CreateShift
        public IActionResult CreateShift()
        {
            ViewShiftVm viewShiftVm = new ViewShiftVm();
            viewShiftVm.Regions = _adminDashboard.getRegions();
            return PartialView("Admin/CreateShift",viewShiftVm);
        }
        #endregion

        #region GetPhysicianByRegion
        public IActionResult GetPhysiciansByRegion(int regionId)
        {
            ViewShiftVm viewShiftVm = new ViewShiftVm();
            viewShiftVm.physicianList = _adminDashboard.getPhysicianByRegion(regionId);
            return Json(new { viewShiftVm });
        }
        #endregion

        [Route("/Admin/Scheduling/checkshiftexist/{physicianId}/{shiftdate}/{starttime}/{endtime}")]
        #region checkshiftexist
        public IActionResult checkshiftexist(int physicianId, DateOnly shiftdate,TimeOnly starttime,TimeOnly endtime)
        {
            var existshift = _adminDashboard.checkshiftExistsForPhysician(physicianId,shiftdate, starttime, endtime);
            return Json(new { exist = existshift });
        }
        #endregion

        #region CreateShiftPost
        public IActionResult CreateShiftPost(ViewShiftVm model, List<int> WeekDaysList)
        {
            var aspId = HttpContext.Session.GetInt32("AspId");
            _adminDashboard.createShiftPost(model, aspId ?? 1,WeekDaysList);
            SchedulingVm schedulingVm = new SchedulingVm();
            schedulingVm.region = _adminDashboard.getRegions();
            TempData["NewShift"] = "New Shift Created";
            return RedirectToAction("Scheduling","Admin", schedulingVm);
        }
		#endregion

		#region RequestedShift
        public IActionResult RequestedShift()
        {
            ViewBag.ActiveDashboardNav = "Provider";
            ShiftViewModel shiftViewModel = new ShiftViewModel();
            shiftViewModel.regions = _adminDashboard.getRegions();
            return View(shiftViewModel);
        }
		#endregion

		#region RequestedShiftTable
        public IActionResult RequestedShiftTable(int regionId)
        {
            ShiftViewModel shiftViewModel = new ShiftViewModel();
            shiftViewModel.regions = _adminDashboard.getRegions();
            shiftViewModel.requestedShift = _adminDashboard.requestedShiftData(regionId);
            return PartialView("Admin/_RequestedShiftTable", shiftViewModel);
        }
        #endregion

        #region DeleteSelectedShifts
        public IActionResult DeleteSelectedShift(int regionId,List<int> shiftDetailId)
        {
            if (_adminDashboard.deleteSelectedShift(shiftDetailId)) 
            { 
                return Json(new { success = true });   
            }
            else
            {
                return Json(new { success = false });
            }
        }
        #endregion

        #region ApproveSelectedShift
        public IActionResult ApproveSelectedShift(List<int> shiftDetailId)
        {
            if (_adminDashboard.approveSelectedShift(shiftDetailId))
            {
                return Json(new { success = true });
            }
            else { return Json(new { success = false });}
        }
        #endregion

        #region ProviderOncall
        public IActionResult ProviderOnCall()
        {
            ViewBag.ActiveDashboardNav = "Provider";
            ViewBag.ActiveDropdown = "Scheduling";
            ViewBag.AdminName = HttpContext.Session.GetString("adminName");
            ProviderVm providerVm = new ProviderVm();
            providerVm.regions = _adminDashboard.getRegions();
            return View(providerVm);
        }
        #endregion

        #region ProviderOnCallContent
        public IActionResult ProviderOnCallContent(int regionId)
        {
            ProviderVm providerVm = new ProviderVm();
            providerVm.providers = _adminDashboard.getPhysicianList(regionId);
            return PartialView("Admin/_ProviderOnCallContent",providerVm);
        }
        #endregion
    }
}
