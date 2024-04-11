using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("EmailLog")]
public partial class EmailLog
{
    [Key]
    [Column("EmailLogID")]
    public int EmailLogId { get; set; }

    [Column(TypeName = "character varying")]
    public string EmailTemplate { get; set; } = null!;

    [StringLength(200)]
    public string SubjectName { get; set; } = null!;

    [Column("EmailID")]
    [StringLength(200)]
    public string EmailId { get; set; } = null!;

    [StringLength(200)]
    public string? ConfirmationNumber { get; set; }

    [Column(TypeName = "character varying")]
    public string? FilePath { get; set; }

    public int? RoleId { get; set; }

    public int? RequestId { get; set; }

    public int? AdminId { get; set; }

    public int? PhysicianId { get; set; }

    public DateOnly CreateDate { get; set; }

    public DateOnly? SentDate { get; set; }

    public int? SentTries { get; set; }

    public int? Action { get; set; }

    public bool? IsEmailSent { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("EmailLogs")]
    public virtual Request? Request { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("EmailLogs")]
    public virtual Role? Role { get; set; }
}
