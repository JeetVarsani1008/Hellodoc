using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModel
{
    public class CreateAccount
    {
        [Required]
        [StringLength(256)]
        public string UserName { get; set; } = null!;

        [Required]
        [Column(TypeName = "character varying")]
        public string? PasswordHash { get; set; } = null!;

        [Required]
        [Compare("PasswordHash")]
        [StringLength(256)]
        public string ConfirmPassword { get; set; }

        public string? Email { get; set; } = null;

        public string? Token { get; set; } = null;

    }
}
