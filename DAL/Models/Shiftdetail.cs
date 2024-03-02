using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("ShiftDetail")]
public partial class ShiftDetail
{
    [Key]
    public int ShiftDetailId { get; set; }

    public int ShiftId { get; set; }

    public DateTime ShiftDate { get; set; }

    public int? RegionId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public short Status { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray IsDeleted { get; set; } = null!;

    public DateTime? ModifiedDate { get; set; }

    public DateTime? LastRunningDate { get; set; }

    [StringLength(100)]
    public string? EventId { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsSync { get; set; }

    public int? Modifiedby { get; set; }

    [ForeignKey("Modifiedby")]
    [InverseProperty("ShiftDetails")]
    public virtual AspNetUser? ModifiedbyNavigation { get; set; }

    [ForeignKey("ShiftId")]
    [InverseProperty("ShiftDetails")]
    public virtual Shift Shift { get; set; } = null!;

    [InverseProperty("ShiftDetail")]
    public virtual ICollection<ShiftDetailRegion> ShiftDetailRegions { get; set; } = new List<ShiftDetailRegion>();
}
