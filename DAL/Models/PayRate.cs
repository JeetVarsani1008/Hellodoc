using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("PayRate")]
public partial class PayRate
{
    [Key]
    public int PayRateId { get; set; }

    public int PhysicianId { get; set; }

    public int? NightShiftWeekend { get; set; }

    public int? Shift { get; set; }

    public int? HouseCallNightWeekend { get; set; }

    public int? PhoneConsult { get; set; }

    public int? PhoneConsultNightWeekend { get; set; }

    public int? BatchTesting { get; set; }

    public int? HouseCall { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PayRates")]
    public virtual Physician Physician { get; set; } = null!;
}
