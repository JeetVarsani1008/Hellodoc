using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[PrimaryKey("UserId", "RoleId")]
public partial class AspNetUserRole
{
    [Key]
    public int UserId { get; set; }

    [Key]
    public int RoleId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AspNetUserRoles")]
    public virtual AspNetUser User { get; set; } = null!;
}
