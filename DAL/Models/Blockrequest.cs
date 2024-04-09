using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class BlockRequest
{
    [Key]
    public int BlockRequestId { get; set; }

    [Column(TypeName = "character varying")]
    public string? PhoneNumber { get; set; }

    [Column(TypeName = "character varying")]
    public string? Email { get; set; }

    [Column(TypeName = "character varying")]
    public string? Reason { get; set; }

    [Column("IP", TypeName = "character varying")]
    public string? Ip { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public DateOnly? ModifiedDate { get; set; }

    public int RequestId { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("BlockRequests")]
    public virtual Request Request { get; set; } = null!;
}
