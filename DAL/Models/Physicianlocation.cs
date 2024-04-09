using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("PhysicianLocation")]
public partial class PhysicianLocation
{
    [Key]
    public int LocationId { get; set; }

    public int PhysicianId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    public string? PhysicianName { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianLocations")]
    public virtual Physician Physician { get; set; } = null!;
}
