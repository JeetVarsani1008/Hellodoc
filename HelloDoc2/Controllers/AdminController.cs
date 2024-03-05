using BLL.Interface;
using BLL.Repositery;
using DAL.Models;
using DAL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections;
using System.IO.Compression;
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
            HttpContext.Session.SetInt32("asignReq", req);
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

        [HttpPost]
        public IActionResult AsignCasePost(AdminAsignVm model)
        {
            var req = HttpContext.Session.GetInt32("asignReq");
            _adminDashboard.asignCasePost(model, req ?? 0,2);
            return RedirectToAction("AdminDashboard", "Admin");
        }

        public IActionResult ViewUploads(AdminViewUploadVm model,int requestId)
        {
            model.RequestId = requestId;
            HttpContext.Session.SetInt32("reqIdUpload",requestId);
            ViewBag.RequestIdForDownloadAll = requestId;
            ViewBag.RequestIdForDeleteAll = requestId;
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

        public IActionResult ViewUploadDownloadAll(int requestId)
        {
            var filesRow = _adminDashboard.GetAllFilesByRequestId(requestId);
            MemoryStream ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                filesRow.ForEach(file =>
                {
                    var path = "D:\\main project\\HelloDoc\\Hellodoc2\\wwwroot\\upload\\" + file.FileName;
                    ZipArchiveEntry zipEntry = zip.CreateEntry(file.FileName);
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (Stream zipEntryStream = zipEntry.Open())
                    {
                        fs.CopyTo(zipEntryStream);
                    }
                });
            return File(ms.ToArray(), "application/zip", "download.zip");
        }

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

        public IActionResult ViewUploadDeleteAll(int requestId)
        {
            var filesToDelete = _adminDashboard.GetAllFilesByRequestId(requestId);
            if(!filesToDelete.Any())
            {
                return NotFound();
            }
            foreach (var fileToDelete in filesToDelete)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/upload", fileToDelete.FileName);
                try
                {
                    System.IO.File.Delete(filePath);

                    //delete all files records that store in database
                    // inplemetation remaining 
                    //_adminDashboard.DeleteFile(fileToDelete.RequestWiseFileId);

                    _context.RequestWiseFiles.Remove(fileToDelete);
                    _context.SaveChanges();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error while deleting File{ex.Message}");
                    return StatusCode(500);// 500 is for internal server
                }
            }
            TempData["success"] = "All Files are deleted Sucessfully.";
            return RedirectToAction("ViewUploads","Admin");
        }
    }
}
