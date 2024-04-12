using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("SMSLog")]
public partial class Smslog
{
    [Key]
    [Column("SMSLogID")]
    public int SmslogId { get; set; }

    [Column("SMSTemplate", TypeName = "character varying")]
    public string Smstemplate { get; set; } = null!;

    [StringLength(50)]
    public string MobileNumber { get; set; } = null!;

    [StringLength(200)]
    public string? ConfirmationNumber { get; set; }

    public int? RoleId { get; set; }

    public int? AdminId { get; set; }

    public int? RequestId { get; set; }

    public int? PhysicianId { get; set; }

    public DateOnly CreateDate { get; set; }

    public DateOnly? SentDate { get; set; }

    public int SentTries { get; set; }

    public int? Action { get; set; }

    [Column("IsSMSSent")]
    public bool? IsSmssent { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Smslogs")]
    public virtual Request? Request { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Smslogs")]
    public virtual Role? Role { get; set; }
}
