using LibraryManagement.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.BLL.Interface
{
    public interface ILibraryDashboard
    {
        List<LibraryDashboardData> getLibraryDashboardData(string searchdata);

        bool addLibraryBookData(LibraryAddVm model);

        LibraryAddVm getEditBookData(int bookId);

        bool editBookDataPost(LibraryAddVm model);

        void bookDeleteData(int bookId);
    }
}
