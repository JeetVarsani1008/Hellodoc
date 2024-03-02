using BLL.Interface;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace DAL.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _adminDashboard;
        private readonly HellodocContext _context;

        public AdminController(HellodocContext context, IAdminDashboard adminDashboard) {
            _context = context;
            _adminDashboard = adminDashboard;

        }
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
        public IActionResult Orders()
        {
            return View();
        }
        public IActionResult Transfer()
        {
            return View();
        }
        public IActionResult ViewCase(int requestId)
        {
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel();
            adminDashboardViewModel.requestListAdminDash = _adminDashboard.ViewCase(requestId);
            return View(adminDashboardViewModel);
        }
        public IActionResult ViewNotes(int requestId)
        {
            var akshay = _adminDashboard.ViewNotes(requestId);
            return View(akshay);
        }


        [HttpPost]
        public IActionResult ViewNotes(ViewNotesVm model, int requestId) {
             _adminDashboard.editViewNotes(model, requestId);
            model = _adminDashboard.ViewNotes(requestId);
            return View(model) ;
        }


        public IActionResult ViewUploads() {
            return View();
        }

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


        public IActionResult AsignCase(int req)
        {
            AdminAsignVm adminAsignVm = new AdminAsignVm();
            adminAsignVm.regions = _adminDashboard.asignCase();
            return PartialView("_adminAsignCase",adminAsignVm);
        }

        public IActionResult GetPhysiciansByRegionId(int regionId)
        {
            AdminAsignVm adminAsignVm=new AdminAsignVm();
            adminAsignVm.physicianList =  _adminDashboard.asignPhysician(regionId);
            return Json(new { adminAsignVm });
        }
    }
}
