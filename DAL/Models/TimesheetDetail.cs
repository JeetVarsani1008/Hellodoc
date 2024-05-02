using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("TimesheetDetail")]
public partial class TimesheetDetail
{
    [Key]
    public int TimesheetDetailId { get; set; }

    public int TimesheetId { get; set; }

    public DateOnly Date { get; set; }

    public int? OnCallHours { get; set; }

    public int? TotalHours { get; set; }

    public int? HouseCall { get; set; }

    public int? PhoneConsult { get; set; }

    [Column(TypeName = "character varying")]
    public string? Item { get; set; }

    public int? Amount { get; set; }

    [Column(TypeName = "character varying")]
    public string? Bill { get; set; }

    public bool? IsHoliday { get; set; }

    public int? NumberOfShifts { get; set; }

    public int? NightShiftWeekend { get; set; }

    public int? PhoneConsultNightWeekend { get; set; }

    public int? HouseCallNightWeekend { get; set; }

    public int? BatchTesting { get; set; }

    public int? TotalAmount { get; set; }

    public int? BonusAmount { get; set; }

    [ForeignKey("TimesheetId")]
    [InverseProperty("TimesheetDetails")]
    public virtual WeeklyTimeSheet Timesheet { get; set; } = null!;
}
