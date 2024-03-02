using DAL.Models;
using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IPatientDashboard
    {
        List<PatientData> patientDashboardMain(int? uid);

        PatientProfile patientDashboardProfile(int? userID);

        void editPatientProfile(PatientProfile model, int? userid);

        Request GetRequestById(int? requestId);
        List<RequestWiseFile> GetFilesByRequestId(int requestId);

        RequestWiseFile GetFileById(int fileId);

        List<RequestWiseFile> GetAllFilesByRequestId(int reqId);

        void UploadFile(int requestId, string fileName);

    }
}
