using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.DAL.DataModels;

[Table("Borrower")]
public partial class Borrower
{
    [Key]
    public int BorrowerId { get; set; }

    [Column(TypeName = "character varying")]
    public string City { get; set; } = null!;

    [ForeignKey("BorrowerId")]
    [InverseProperty("InverseBorrowerNavigation")]
    public virtual Borrower BorrowerNavigation { get; set; } = null!;

    [InverseProperty("BorrowerNavigation")]
    public virtual Borrower? InverseBorrowerNavigation { get; set; }
}
