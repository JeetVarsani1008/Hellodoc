using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class EncounterVm
    {
        public int RequestId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage ="Please enter Email")]
        public string Email { get; set; }

        public string MedicalHistory { get; set; }

        public string Medications { get; set; }
        
        public string Allergies { get; set; }

        public string Temp {  get; set; }

        public string Hr {  get; set; }

        public string Rr {  get; set; }

        public string BloodPressureS {  get; set; }

        public string BloodPressureD {  get; set; }

        public string O2 {  get; set; }

        public string Pain {  get; set; }

        public string Heent {  get; set; }

        public string Cv {  get; set; }

        public string Chest {  get; set; }

        public string Abd {  get; set; }

        public string Extr {  get; set; }

        public string Skin {  get; set; }

        public string Neuro {  get; set; }

        public string Other {  get; set; }

        public string Diagnosis {  get; set; }

        public string TreatmentPlan {  get; set; }

        public string MedicationsDispensed {  get; set; }

        public string Procedures { get; set; }

        public string FollowUp { get; set; }


    }
}
