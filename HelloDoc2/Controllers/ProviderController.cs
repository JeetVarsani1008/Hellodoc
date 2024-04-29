using BLL.Interface;
using BLL.Repositery;
using DAL.Models;
using DAL.ViewModel;
using DAL.ViewModelProvider;
using HelloDoc2.Auth;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Org.BouncyCastle.Ocsp;
using Rotativa.AspNetCore;
using System.Collections;

namespace HelloDoc2.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProviderDashboard _providerDashboard;
        private readonly IAdminDashboard _adminDashboard;

        public ProviderController(IProviderDashboard providerDashboard, IAdminDashboard adminDashboard)
        {
            _providerDashboard = providerDashboard;
            _adminDashboard = adminDashboard;
        }

        [CustomAuthorize("3")]
        #region ProviderDashboard
        public IActionResult ProviderDashboard()
        {
            ViewBag.ActiveDashboardNav = "ProviderDashboard";
            int? phyId = HttpContext.Session.GetInt32("PhysicianId");
            var request = _providerDashboard.forCountRequest(phyId ?? 0);

            var reqCount = request.GroupBy(o => o.Status).Select(h => new { Status = h.Key, Count = h.Count() }).ToList();

            var newCount = reqCount.Find(o => o.Status == 1)?.Count ?? 0;
            var pendingCount = reqCount.Find(i => i.Status == 2)?.Count ?? 0;
            var active1 = reqCount.Find(i => i.Status == 4)?.Count ?? 0;
            var active2 = reqCount.Find(i => i.Status == 5)?.Count ?? 0;
            var activeCount = active1 + active2;
            var concludeCount = reqCount.Find(i => i.Status == 6)?.Count ?? 0;

            ProviderDashboardVm providerDashboardVm = new ProviderDashboardVm
            {
                statusArray = "1",
                requestTypeId = 0,
                NewCount = newCount,
                ActiveCount = activeCount,
                PendingCount = pendingCount,
                ConcludeCount = concludeCount,
            };

            return View(providerDashboardVm);
        }
        #endregion

        #region ProviderDashboardTable
        public IActionResult ProviderDashboardTable(string reqtypeid, string searchdata)
        {
            int PageNumber = 1;
            int PageSize = 5;
            string str = "1";

            var phyId = HttpContext.Session.GetInt32("PhysicianId");
            var reqProvider = _providerDashboard.getRequestDataForProvider(str, 0, searchdata,phyId ?? 0);
            var reqProviderPaginatedData = reqProvider.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            ProviderDashboardVm providerDashboardVm = new ProviderDashboardVm()
            {
                statusArray = str,
                requestTypeId = 0,
                requestDataProvider = reqProviderPaginatedData,
                Page = new PageVm
                {
                    totalitem = reqProvider.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = reqProvider.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
            };
            providerDashboardVm.TotalPages = (int)Math.Ceiling((decimal)providerDashboardVm.Page.totalitem / PageSize);
            return PartialView("Provider/ProviderPartialTable", providerDashboardVm);
        }
        #endregion

        #region FetchRequestsProvider
        public IActionResult FetchRequestsProvider(string statusarray, int requestTypeId, string searchdata)
        {
            int PageNumber = 1;
            int PageSize = 5;
            if (searchdata != null)
            {
                searchdata = null;
            }
            var phyId = HttpContext.Session.GetInt32("PhysicianId");
            HttpContext.Session.SetString("StatusFetch", statusarray);
            var reqProvider = _providerDashboard.getRequestDataForProvider(statusarray, requestTypeId, searchdata,phyId??0);
            var reqProviderPaginatedData = reqProvider.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            ProviderDashboardVm providerDashboardVm = new ProviderDashboardVm()
            {
                statusArray = statusarray,
                requestDataProvider = reqProviderPaginatedData,
                Page = new PageVm
                {
                    totalitem = reqProvider.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = reqProvider.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,

            };
            providerDashboardVm.TotalPages = (int)Math.Ceiling((decimal)providerDashboardVm.Page.totalitem / PageSize);
            return PartialView("Provider/ProviderPartialTable", providerDashboardVm);
        }
        #endregion

        #region FilterRequestsProvider
        public IActionResult FilterRequestsProvider(string statusarray, int requestTypeId, string searchdata)
        {
            int PageNumber = 1;
            int PageSize = 5;

            var phyId = HttpContext.Session.GetInt32("PhysicianId");

            statusarray = HttpContext.Session.GetString("StatusFetch");

            if(statusarray == null)
            {
                statusarray = "1";
            }
            
            var reqProvider = _providerDashboard.getRequestDataForProvider(statusarray, requestTypeId, searchdata, phyId??0);
            var reqProviderPaginatedData = reqProvider.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            ProviderDashboardVm providerDashboardVm = new ProviderDashboardVm()
            {
                
                requestDataProvider = reqProviderPaginatedData,
                Page = new PageVm
                {
                    totalitem = reqProvider.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = reqProvider.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
            };
            providerDashboardVm.TotalPages = (int)Math.Ceiling((decimal)providerDashboardVm.Page.totalitem / PageSize);
            return PartialView("Provider/ProviderPartialTable", providerDashboardVm);
        }
        #endregion

        #region FilterPaginationProvider
        public IActionResult FilterPaginationProvider(string statusarray, int requestTypeId, string searchdata, int PageNumber)
        {
            if(PageNumber == 0)
            {
                PageNumber = 1;
            }
            int PageSize = 5;

            var phyId = HttpContext.Session.GetInt32("PhysicianId");

            statusarray = HttpContext.Session.GetString("StatusFetch");

            if(statusarray == null)
            {
                statusarray = "1";
            }
            
            var reqProvider = _providerDashboard.getRequestDataForProvider(statusarray, requestTypeId, searchdata, phyId??0);
            var reqProviderPaginatedData = reqProvider.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            ProviderDashboardVm providerDashboardVm = new ProviderDashboardVm()
            {
                statusArray = statusarray,
                requestDataProvider = reqProviderPaginatedData,
                Page = new PageVm
                {
                    totalitem = reqProvider.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = reqProvider.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
            };
            providerDashboardVm.TotalPages = (int)Math.Ceiling((decimal)providerDashboardVm.Page.totalitem / PageSize);
            return PartialView("Provider/ProviderPartialTable", providerDashboardVm);
        }
        #endregion


        #region SendLink
        public IActionResult SendLink()
        {
            return PartialView("Provider/_ProviderSendLink");
        }
        #endregion

        [CustomAuthorize("3")]
        #region ProviderMyProfile
        public IActionResult ProviderMyProfile()
        {
            var id = HttpContext.Session.GetInt32("AspId");
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.ActiveDashboardNav = "ProviderMyProfile";
            var data = _providerDashboard.getProviderDetails(id ?? 0, physicianId ?? 0);
            return View(data);
        }
        #endregion

        #region ProviderResetPassword
        public IActionResult ProviderResetPassword(string password, int physicianId)
        {
            var data = _providerDashboard.providerResetPassword(password, physicianId);
            if (data)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        #endregion 

        #region RequestToAdmin
        [HttpGet]
        public IActionResult RequestToAdmin(int physicianId)
        {
            ProviderProfileVm providerProfileVm = new ProviderProfileVm();
            providerProfileVm.PhysicianId = physicianId;
            return PartialView("Provider/_RequestToAdminToChangeProfile",providerProfileVm);
        }
        #endregion

        #region RequestToAdmin : post 
        [HttpPost]
        public IActionResult RequestToAdmin(ProviderProfileVm model)
        {
            var data = _providerDashboard.getPhysician(model);
            var aspuser = _providerDashboard.getEmailByCreatedBy(data.CreatedBy);
            var subject = "To Edit Or Update Physician Profile Data";
            var body = "Request :- " + model.Message;

            SendEmailAsync(aspuser.Email, subject, body);
            TempData["sucess"] = "Request Sent Successfully to Admin";
            return RedirectToAction("ProviderMyProfile", "Provider");
        }
        #endregion

        #region MySchedule : provider
        public IActionResult MySchedule()
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.ActiveDashboardNav = "MySchedule";
            SchedulingVm schedulingVm = new SchedulingVm();
            schedulingVm.PhysicianId = physicianId ?? 0;
            return View(schedulingVm);
        }
        #endregion

        #region GetParticularScheduleData 
        public IActionResult GetParticularScheduleData(int PhysicianId)
        {
            string[] color = { "#edacd2", "#a5cfa6" };
            List<ShiftDetail> shiftDetails = _providerDashboard.getParticularScheduleData(PhysicianId);

            List<SchedulingDetailsViewModel> list = shiftDetails.Select(s => new SchedulingDetailsViewModel
            {
                resourceId = s.Shift.PhysicianId,
                Id = s.ShiftDetailId,
                //title = "Event " + s.ShiftDetailId,
                title = s.Shift.Physician.FirstName + "_Shift-" + s.ShiftDetailId,
                start = s.ShiftDate.ToString("yyyy-MM-dd") + s.StartTime.ToString("THH:mm:ss"),
                end = s.ShiftDate.ToString("yyyy-MM-dd") + s.EndTime.ToString("THH:mm:ss"),
                color = color[s.Status]
            }).ToList();


            return Json(list);
        }
        #endregion

        #region CreateShiftProvider
        public IActionResult CreateShiftProvider(int physicianId)
        {
            ViewShiftVm viewShiftVm = new ViewShiftVm();
            viewShiftVm.Regions = _providerDashboard.getRegions();
            viewShiftVm.PhysicianId = physicianId;
            return PartialView("Provider/_CreateShiftProvider", viewShiftVm);
        }
        #endregion

        [Route("/Provider/MySchedule/checkshiftexist/{physicianId}/{shiftdate}/{starttime}/{endtime}")]
        #region checkshiftexist
        public IActionResult checkshiftexist(int physicianId, DateOnly shiftdate, TimeOnly starttime, TimeOnly endtime)
        {
            var existshift = _providerDashboard.checkshiftExistsForPhysician(physicianId, shiftdate, starttime, endtime);
            return Json(new { exist = existshift });
        }
        #endregion

        #region CreateShiftPost
        public IActionResult CreateShiftPost(ViewShiftVm model, List<int> WeekDaysList)
        {
            var aspId = HttpContext.Session.GetInt32("AspId");
            _providerDashboard.createShiftPost(model, aspId ?? 1, WeekDaysList);
            SchedulingVm schedulingVm = new SchedulingVm();
            schedulingVm.region = _providerDashboard.getRegions();
            TempData["NewShift"] = "New Shift Created";
            return RedirectToAction("MySchedule", "Provider", schedulingVm);
        }
        #endregion

        [CustomAuthorize("3")]
        #region Accept
        public IActionResult Accept(int requestId)
        {
            _providerDashboard.acceptRequest(requestId);
            TempData["acceptrequest"] = "Request Accepted successfully";
            return RedirectToAction("ProviderDashboard", "Provider");
        }
        #endregion

        #region ViewCase
        public IActionResult ViewCase(int requestId)
        {
            var phyId = HttpContext.Session.GetInt32("PhysicianId");
            if(!_providerDashboard.checkphysician(phyId ?? 0, requestId))
            {
                return View("ErrorPage");
            }
            ViewBag.ActiveDashboardNav = "ProviderDashboard";
            var data = _adminDashboard.ViewCase(requestId);
            return View(data);
        }
        #endregion

        [CustomAuthorize("3")]
        #region ViewNotes : get
        //view notes start : 2 methods
        public IActionResult ViewNotes(int requestId)
        {
            ViewBag.ActiveDashboardNav = "ProviderDashboard";
            var phyId = HttpContext.Session.GetInt32("PhysicianId");
            var adminId = 0;
            if (!_providerDashboard.checkphysician(phyId ?? 0, requestId))
            {
                return View("ErrorPage");
            }
            var data = _adminDashboard.ViewNotes(requestId, adminId, phyId??0);
            return View(data);
        }
        #endregion

        #region ViewNotes : post
        [HttpPost]
        public IActionResult ViewNotes(ViewNotesVm model, int requestId)
        {
            var phyId = HttpContext.Session.GetInt32("PhysicianId");
            var adminId = 0;
            _providerDashboard.editViewNotes(model, requestId);
            model = _adminDashboard.ViewNotes(requestId,adminId,phyId ?? 0);
            return View(model);
        }
        #endregion

        #region SendAgreementProvider
        public IActionResult SendAgreementProvider(int req, int reqTypeId)
        {
            var data = _providerDashboard.getRequestData(req);
            AdminClearVm adminClearVm = new AdminClearVm();
            adminClearVm.RequestTypeId = reqTypeId;
            adminClearVm.RequestId = req;
            adminClearVm.Number = data.PhoneNumber;
            adminClearVm.Email = data.Email;
            return PartialView("Provider/_SendAgreementProvider", adminClearVm);
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
            return RedirectToAction("ProviderDashboard");
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

        [CustomAuthorize("3")]
        #region ViewUploads
        public IActionResult ViewUploadsProvider(AdminViewUploadVm model, int requestId)
        {
            var phyId = HttpContext.Session.GetInt32("PhysicianId");
            if (!_providerDashboard.checkphysician(phyId ?? 0, requestId))
            {
                return View("ErrorPage");
            }
            model.RequestId = requestId;
            HttpContext.Session.SetInt32("reqIdUpload", requestId);
            ViewBag.RequestIdForDownloadAll = requestId;
            ViewBag.RequestIdForDeleteAll = requestId;
            ViewBag.RequestIdForSendMail = requestId;
            var returnViewData = _adminDashboard.GetAdminViewUploadData(model, requestId);
            var documents = _adminDashboard.GetFilesByRequestId(requestId);
            ViewBag.document = documents;
            ViewBag.ActiveDashboardNav = "ProviderDashboard";
            return View(returnViewData);
        }
        #endregion

        [CustomAuthorize("3")]
        #region TransferCaseProvider 
        public IActionResult TransferCaseProvider(int reqId)
        {
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.RequestId = reqId;
            var data = _providerDashboard.getRequestData(reqId);
            adminAsignVm.PhysicianId = data.PhysicianId??0;
            return PartialView("Provider/_TransferCaseProvider", adminAsignVm);
        }
        #endregion

        #region TransferCasePostProvider
        public IActionResult TransferCasePostProvider(AdminAsignVm model)
        {
            try
            {
                _providerDashboard.transferCasePost(model, 1);
                TempData["success"] = "Request Transfer Successfully to admin";
                return RedirectToAction("ProviderDashboard", "Provider");
            }
            catch
            {
                TempData["error"] = "Opps... Request Not Transter!";
                return RedirectToAction("ProviderDashboard","Provider");
            }
        }
        #endregion

        [CustomAuthorize("3")]
        #region Orders
        public IActionResult Orders(int requestID)
        {
            AdminOrderVm adminOrderVm = new AdminOrderVm();
            adminOrderVm.healthProfessionType = _adminDashboard.healthProfessionalTypes();
            adminOrderVm.RequestId = requestID;
            ViewBag.ActiveDashboardNav = "ProviderDashboard";
            return View(adminOrderVm);
        }
        #endregion Orders

        #region Orders : post
        [HttpPost]
        public IActionResult Orders(AdminOrderVm model, int requestID)
        {
            _providerDashboard.orderDataStore(model, requestID);
            TempData["success"] = "Order Placed Successfully.";
            return RedirectToAction("ProviderDashboard", "Provider");
        }
        #endregion

        //type of care : encounter

        #region EncounterPopUp 
        public IActionResult EncounterPopUp(int requestId)
        {
            EncounterVm encounterVm = new EncounterVm();
            encounterVm.RequestId = requestId;
            return PartialView("Provider/_EncounterPopUp",encounterVm);
        }
        #endregion

        #region TypeOfCare
        public IActionResult TypeOfCare(EncounterVm model,string care)
        {
            var data = _providerDashboard.typeOfCare(model, care);
            if(care == "Consult")
            {
                TempData["success"] = "Request Consulted";
            }
            else
            {
                TempData["success"] = "Home Call is selected for type of care";
            }
            return RedirectToAction("ProviderDashboard", "Provider");
        }
        #endregion

        #region HouseCall
        public IActionResult HouseCall(int requestId)
        {
            var data = _providerDashboard.houseCall(requestId);
            if (data)
            {
                TempData["success"] = "Requested Concluded Successfully";
            }
            else
            {
                TempData["error"] = "Request Not Concluded";
            }
            return RedirectToAction("ProviderDashboard","Provider");
        }
        #endregion

        #region EncounterForm 
        public IActionResult EncounterForm(int requestId)
        {
            var phyId = HttpContext.Session.GetInt32("PhysicianId");
            if (!_providerDashboard.checkphysician(phyId ?? 0, requestId))
            {
                return View("ErrorPage");
            }
            ViewBag.ActiveDashboardNav = "ProviderDashboard";
            var data = _adminDashboard.encounterFormGetData(requestId);
            return View(data);
        }
        #endregion

        #region PostEncounterData
        public IActionResult PostEncounterData(EncounterVm model, int encounterbtn)
        {
            _providerDashboard.postEncounterData(model, encounterbtn);
            if(encounterbtn == 1)
            {
                TempData["success"] = "Data Saved Successfully";
                return View("EncounterForm");
            }
            else
            {
                TempData["success"] = "Form Finalised.";
                return RedirectToAction("ProviderDashboard","Provider");
            }
        }
        #endregion

        #region GeneratePDF
        public IActionResult GeneratePDF(int requestid)
        {
            //AdminDashboardModel model = new AdminDashboardModel();
            //model.encounterModel = _adminDashboardService.GetEncounterData(requestid);
            EncounterVm model = new EncounterVm();
            model = _adminDashboard.encounterFormGetData(requestid);

            if (model == null)
            {
                TempData["error"] = "Something Went Wrong!";
                return NotFound();
            }

            DateTime currentDateTime = DateTime.Now;
            string fileName = $"encounter_{currentDateTime.ToString("ddMMyyyy_HHmmss")}.pdf";
            return new ViewAsPdf("EncounterPdf", model)
            {
                FileName = fileName
            };

        }
        #endregion

        #region EncounterFinalize 
        public IActionResult EncounterFinalize(int requestId)
        {
            HttpContext.Session.SetInt32("RequestId", requestId);
            return PartialView("Provider/_EncounterFinalize");
        }
        #endregion

        #region CreateRequestProvider
        public IActionResult CreateRequestProvider()
        {
            ViewBag.ActiveDashboardNav = "ProviderDashboard";
            AdminCreateRequestVm adminCreateRequestVm = new AdminCreateRequestVm();
            adminCreateRequestVm.regions = _providerDashboard.getRegions();
            return View(adminCreateRequestVm);
        }
        #endregion

        #region CreateRequestProvider : post
        [HttpPost]
        public async Task<IActionResult> CreateRequestProvider(AdminCreateRequestVm model)
        {
            int physicianId = (int)HttpContext.Session.GetInt32("PhysicianId");
            var user = _providerDashboard.CreateNewReq(model, physicianId);

            if (user == true)
            {
                var resetLink = "<a href=" + Url.Action("CreateAccount", "Home", new { }, "https") + "> CreateAccount</a>";

                var subject = "Create Account Link";
                var body = "<b>Please Create Your Account</b><br/>" + resetLink;


                await SendEmailAsync(model.Email, subject, body);
                TempData["success"] = "Request Created Successfully & Create Account link is sent to Patient's Email";
                return RedirectToAction("ProviderDashboard", "Provider");
            }
            else
            {
                TempData["success"] = "Request Created Successfully";
                return RedirectToAction("ProviderDashboard","Provider");
            }
            
        }
        #endregion

        #region  Checkemail
        public IActionResult Checkemail(string userdata)
        {
            var data = _providerDashboard.aspNetUserCheck(userdata);
            return Json(data);
        }
        #endregion

        //this four method is for conclude care
        #region ConcludeCare : get
        public IActionResult ConcludeCare(int requestId)
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            var encounterdata = _providerDashboard.getEncounterDataByRequestId(requestId);
            AdminViewUploadVm adminViewUploadVm = new AdminViewUploadVm();
            adminViewUploadVm.requestWiseFiles = _providerDashboard.getFilesByRequestId(requestId);
            adminViewUploadVm.RequestId = requestId;
            adminViewUploadVm.PhysicianId = (int)physicianId;
            adminViewUploadVm.IsFinalized = encounterdata.IsFinalize ?? false;
            ViewBag.ActiveDashboardNav = "ProviderDashboard";
            adminViewUploadVm.PatientName = _providerDashboard.getPatientNameForConclude(requestId);
            return View(adminViewUploadVm);
        }
        #endregion

        #region ConcludeCareDownload
        public IActionResult ConcludeCareDownload(int documentId)
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

        #region ConcludeCareDelete
        //this part is for delete single file
        public IActionResult ConcludeCareDelete(int documentId, int requestId)
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
            var filedelete = _providerDashboard.concludeCareDelete(documentId);
            if (filedelete)
            {
                TempData["success"] = "File deleted successfully.";
                return RedirectToAction("ConcludeCare",new {requestId = requestId});
            }
            else
            {
                TempData["error"] = "Something Went Wrong";
                return RedirectToAction("ConcludeCare", "Provider", new { requestId });
            }
        }
        #endregion

        #region UploadForConcludeCare : post
        [HttpPost]
        public IActionResult UploadForConcludeCare(IFormFile file, int requestId, int physicianId)
        {
            if(file != null && file.Length > 0 )
            {
                var filePath = Path.Combine("wwwroot", "upload", file.FileName);
                //var filePath = Path.Combine("", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                };

                // this save file in database 
                var data = _providerDashboard.concludeCareUpload(file,requestId, physicianId);

                if (data)
                {
                    // Return a JSON response
                    return Json(new { success = true, message = "File uploaded successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "File Not uploaded For this id." });
                }
            }
            else
            {
                return Json(new { success = false, message = "No file provided" });
            }
        }
        #endregion

        #region ConcludeCare : post 
        [HttpPost]
        public IActionResult ConcludeCarePost(AdminViewUploadVm model)
        {
            _providerDashboard.concludeCarePost(model);
            TempData["success"] = "Request Concluded Successfully.";
            return RedirectToAction("ProviderDashboard","Provider");
        }
        #endregion
        //conclude care complete

        #region Invoicing
        public IActionResult Invoicing()
        {
            TimeSheetVm timeSheetVm = new TimeSheetVm();
            timeSheetVm.PhysicianId = (int)HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.ActiveDashboardNav = "Invoicing";
            return View(timeSheetVm);
        }
        #endregion

        #region ProviderInvoicingTable
        public IActionResult ProviderInvoicingTable()
        {
            return PartialView("Provider/_ProviderInvoicingTable");
        }
        #endregion

        #region FinalizeTable
        public IActionResult FinalizeTable(int PhysicianId, string SelectedValue)
        {   
            ViewBag.ActiveDashboardNav = "Invoicing";
            return View();
        }
        #endregion
    }
}

