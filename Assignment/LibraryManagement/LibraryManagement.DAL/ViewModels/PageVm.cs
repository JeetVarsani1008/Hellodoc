using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.DAL.ViewModels
{
    public class PageVm
    {
        public int totalitem { get; set; }
        public int currentpage { get; set; }
        public int itemperpage { get; set; }
    }
}
