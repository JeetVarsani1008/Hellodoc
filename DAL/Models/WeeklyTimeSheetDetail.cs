using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("WeeklyTimeSheetDetail")]
public partial class WeeklyTimeSheetDetail
{
    [Key]
    public int TimeSheetDetailId { get; set; }

    public DateOnly Date { get; set; }

    public int? NumberOfShifts { get; set; }

    public int? NightShiftWeekend { get; set; }

    public int? HouseCall { get; set; }

    public int? HouseCallNightWeekend { get; set; }

    public int? PhoneConsult { get; set; }

    public int? PhoneConsultNightWeekend { get; set; }

    public int? BatchTesting { get; set; }

    [StringLength(100)]
    public string? Item { get; set; }

    public int? Amount { get; set; }

    [StringLength(100)]
    public string? Bill { get; set; }

    public int? TotalAmount { get; set; }

    public int? BonusAmount { get; set; }

    public int? TimeSheetId { get; set; }

    public int? OnCallHours { get; set; }

    public int? TotalHours { get; set; }

    public bool? IsWeekendHoliday { get; set; }

    [ForeignKey("TimeSheetId")]
    [InverseProperty("WeeklyTimeSheetDetails")]
    public virtual WeeklyTimeSheet? TimeSheet { get; set; }
}
