using System.ComponentModel.DataAnnotations;
using DAL.Models;
using Microsoft.AspNetCore.Http;
namespace DAL.ViewModel
{
    public class PatientData
    {
        public List<Request> Requests { get; set; }
        public string? Symptoms { get; set; }

        public string? FirstName { get; set; }

        public int? Id { get; set; }


        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please Enter the Phone")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Phone Number should be between 8 and 12 characters")]
        public string? Phone { get; set; }

        public DateTime? BirthDate { get; set; }


        public string? PasswordHash { get; set; }


        public string? Email { get; set; }


        public string? Street { get; set; }


        public string? City { get; set; }


        public string? State { get; set; }


        public string? ZipCode { get; set; }

        public string? Room { get; set; }

        public string? Documents { get; set; }
        public string? Status { get; set; }
        public string? CreatedDate { get; set; }

        public IFormFile? Filepath { get; set; } = null;

        public int? Count { get; set; }
    }
}
