using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("PhysicianPayRate")]
public partial class PhysicianPayRate
{
    [Key]
    public int PhysicianPayRateId { get; set; }

    public int? PhysicianId { get; set; }

    public int? NightShift { get; set; }

    public int? Shift { get; set; }

    public int? HouseCallNight { get; set; }

    public int? HouseCall { get; set; }

    public int? PhoneConsultNight { get; set; }

    public int? PhoneConsults { get; set; }

    public int? BatchTesting { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianPayRates")]
    public virtual Physician? Physician { get; set; }
}
