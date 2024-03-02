using DAL.Models;
using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IAdminDashboard
    {
        List<RequestListAdminDash> requestDataAdmin(int Status, string reqTypeId, int RegionId );

        List<RequestListAdminDash> ViewCase(int requestId);

        ViewNotesVm ViewNotes(int requestId);

        void editViewNotes(ViewNotesVm model, int requestId);

        void cancelCase(AdminDashboardViewModel model, int requestId);

        List<DAL.ViewModel.CaseTag> cancelCaseMain();

        List<Region> asignCase();

        List<Physician> asignPhysician(int regionId);
    }
}
