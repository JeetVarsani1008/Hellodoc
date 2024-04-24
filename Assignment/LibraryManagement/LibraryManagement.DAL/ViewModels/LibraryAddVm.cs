using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.DAL.ViewModels
{
    public class LibraryAddVm
    {
        public int BookId { get; set; }

        [Required(ErrorMessage ="Please Enter Book Name")]
        public string BookName { get; set; }

        [Required(ErrorMessage ="Please Enter Author Name")]
        public string Author { get; set; }

        [Required(ErrorMessage ="Please Enter Your Name")]
        public string BorrowerName { get; set; }

        [Required(ErrorMessage ="Provide Issue Date")]
        public DateTime DateOfIssue { get; set; }

        [Required(ErrorMessage ="Please Enter Your City")]
        public string City { get; set; }

        [Required(ErrorMessage ="Select One Tyepe")]
        public string Genere { get; set; }
    }
}
