using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.DAL.DataModels;

[Table("Book")]
public partial class Book
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string BookName { get; set; } = null!;

    [StringLength(30)]
    public string Author { get; set; } = null!;

    public int? BorrowerId { get; set; }

    [StringLength(50)]
    public string? BorrowerName { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime DateOfIssue { get; set; }

    [Column(TypeName = "character varying")]
    public string City { get; set; } = null!;

    [Column("genere")]
    [StringLength(50)]
    public string? Genere { get; set; }
}
