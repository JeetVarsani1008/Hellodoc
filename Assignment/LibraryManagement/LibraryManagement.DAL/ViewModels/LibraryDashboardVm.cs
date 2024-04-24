using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.DAL.ViewModels
{
    public class LibraryDashboardVm
    {
        public List<LibraryDashboardData> libraryDashboardData {  get; set; }

        public PageVm Page { get; set; }
        public int? TotalPages { get; set; }
        public int? skipCount;
        public int? CurrentPage;
    }

    public class LibraryDashboardData
    {
        public int BookId { get; set; }

        public string BookName { get; set; }

        public string Author { get; set; }

        public string BorrowerName { get; set; }

        public DateTime DateOfIssue { get; set; }

        public string City { get; set; }
        public string Genere { get; set; }
    }
}
