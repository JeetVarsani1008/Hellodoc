using BLL.Interface;
using BLL.Repositery;
using DAL.Models;
using DAL.ViewModel;
using DAL.ViewModelProvider;
using HelloDoc2.Auth;
using Microsoft.AspNetCore.Mvc;

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
            var request = _providerDashboard.forCountRequest;

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

            var reqProvider = _providerDashboard.getRequestDataForProvider(str, 0, searchdata);
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
            HttpContext.Session.SetString("StatusFetch", statusarray);
            var reqProvider = _providerDashboard.getRequestDataForProvider(statusarray, requestTypeId, searchdata);
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
            statusarray = HttpContext.Session.GetString("StatusFetch");
            var reqProvider = _providerDashboard.getRequestDataForProvider(statusarray, requestTypeId, searchdata);
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
        public IActionResult RequestToAdmin()
        {
            return PartialView("Provider/_RequestToAdminToChangeProfile");
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
        public IActionResult Accept(int requestId)
        {
            
            return RedirectToAction("ProviderDashboard", "Provider");
        }
        #region ViewCase
        public IActionResult ViewCase(int requestId)
        {
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
            var data = _adminDashboard.ViewNotes(requestId);
            return View(data);
        }
        #endregion

        #region ViewNotes : post
        [HttpPost]
        public IActionResult ViewNotes(ViewNotesVm model, int requestId)
        {
            _providerDashboard.editViewNotes(model, requestId);
            model = _adminDashboard.ViewNotes(requestId);
            return View(model);
        }
        #endregion

    }
}

