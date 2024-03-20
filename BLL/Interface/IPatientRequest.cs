using BLL.Repositery;
using DAL.Models;
using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IPatientRequest
    {

        List<Region> getRegion();
        void patientRequestForm(PatientData model);

        void familyRequestForm(FamilyData model);

        void conciergeRequestForm(ConciergeData model);

        void businessRequestForm(BusinessData model);

        void createRequestForMe(PatientRequestForMeAndSomeone model);

        PatientRequestForMeAndSomeone PatientRequestForMe(int? userID);

        void createRequestForSomeone(PatientRequestForMeAndSomeone model);
    }
}
