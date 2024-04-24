using LibraryManagement.BLL.Interface;
using LibraryManagement.DAL.ViewModels;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibraryManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILibraryDashboard _libraryDashboard;

        public HomeController(ILogger<HomeController> logger,ILibraryDashboard libraryDashboard)
        {
            _logger = logger;
            _libraryDashboard = libraryDashboard;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region LibraryDashboard
        public IActionResult LibraryDashboard()
        {
            return View();
        }
        #endregion

        #region LibraryDashboardTabel
        public IActionResult LibraryDashboardTabel(string searchdata, int PageNumber)
        {
            if(PageNumber == 0)
            {
                PageNumber = 1;
            }
            int PageSize = 5;
            var libdashboard = _libraryDashboard.getLibraryDashboardData(searchdata);
            var aginationData = libdashboard.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            LibraryDashboardVm libraryDashboardVm = new LibraryDashboardVm()
            {
                libraryDashboardData = aginationData,
                Page = new PageVm
                {
                    totalitem = libdashboard.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = libdashboard.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
            };
            libraryDashboardVm.TotalPages = (int)Math.Ceiling((decimal)libraryDashboardVm.Page.totalitem / PageSize);
            return PartialView("_LibraryDashboardTable", libraryDashboardVm);
        }
        #endregion

        #region SearchLibraryData
        public IActionResult SearchLibraryData(string searchdata, int PageNumber)
        {
            if(PageNumber == 0)
            {
                PageNumber = 1;
            }
            int PageSize = 5;
            var libdashboard = _libraryDashboard.getLibraryDashboardData(searchdata);
            var aginationData = libdashboard.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            LibraryDashboardVm libraryDashboardVm = new LibraryDashboardVm()
            {
                libraryDashboardData = aginationData,
                Page = new PageVm
                {
                    totalitem = libdashboard.Count(),
                    currentpage = PageNumber,
                    itemperpage = PageSize,
                },
                skipCount = libdashboard.Take((PageNumber - 1) * PageSize).ToList().Count(),
                CurrentPage = PageNumber,
            };
            libraryDashboardVm.TotalPages = (int)Math.Ceiling((decimal)libraryDashboardVm.Page.totalitem / PageSize);
            return PartialView("_LibraryDashboardTable", libraryDashboardVm);
        }
        #endregion

        #region LibraryPopUp
        public IActionResult LibraryPopUp()
        {
            return PartialView("_LibraryPopUp");
        }
        #endregion

        #region LibraryAddBookData
        public IActionResult LibraryAddBookData(LibraryAddVm model)
        {
            var data = _libraryDashboard.addLibraryBookData(model);
            if(data)
            {
                TempData["success"] = "Book Added Successfully";
                return RedirectToAction("LibraryDashboard");
            }
            else
            {
                TempData["error"] = "Something Went Wrong while saving data, Please Try again";
                return RedirectToAction("LibraryDashboard");
            }
        }
        #endregion

        #region EditLibraryData
        public IActionResult EditLibraryData(int bookId)
        {
            var data = _libraryDashboard.getEditBookData(bookId);
            if(data != null)
            {
                return PartialView("_LibraryEditPopUp",data);
            }
            else
            {
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("LibraryDashboard");
            }
        }
        #endregion

        #region EditLibraryDataPost
        public IActionResult EditLibraryDataPost(LibraryAddVm model)
        {
            _libraryDashboard.editBookDataPost(model);
            TempData["success"] = "Data Edited Successfully";
            return RedirectToAction("LibraryDashboard");
        }
        #endregion

        #region BookDeleteData
        public IActionResult BookDeleteData(int bookId)
        {
            _libraryDashboard.bookDeleteData(bookId);
            TempData["success"] = "Data Deleted Successfully";
            return RedirectToAction("LibraryDashboard");
        }
        #endregion
    }
}
