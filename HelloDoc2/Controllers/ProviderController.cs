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

        public ProviderController(IProviderDashboard providerDashboard)
        {
            _providerDashboard = providerDashboard;
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
            if(searchdata != null)
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




        [CustomAuthorize("3")]
        #region ProviderMyProfile
        public IActionResult ProviderMyProfile()
        {
            var id = HttpContext.Session.GetInt32("AspId");
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
			ViewBag.ActiveDashboardNav = "ProviderMyProfile";
            var data = _providerDashboard.getProviderDetails(id??0, physicianId?? 0);
			return View(data);
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

        #region CreateShift
        public IActionResult CreateShift()
        {
            ViewShiftVm viewShiftVm = new ViewShiftVm();
            viewShiftVm.Regions = _providerDashboard.getRegions();
            return PartialView("Admin/CreateShift", viewShiftVm);
        }
        #endregion
    }
}

//date :- 14/04/2024
