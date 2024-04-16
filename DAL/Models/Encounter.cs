using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("Encounter")]
public partial class Encounter
{
    [Key]
    public int EncounterId { get; set; }

    public int RequestId { get; set; }

    [StringLength(500)]
    public string? MedicalHistory { get; set; }

    [StringLength(500)]
    public string? Medications { get; set; }

    [StringLength(500)]
    public string? Allergies { get; set; }

    [StringLength(200)]
    public string? Temp { get; set; }

    [Column("HR")]
    [StringLength(200)]
    public string? Hr { get; set; }

    [Column("RR")]
    [StringLength(200)]
    public string? Rr { get; set; }

    [Column("Blood Pressure(S)")]
    [StringLength(100)]
    public string? BloodPressureS { get; set; }

    [Column("Blood Pressure(D)")]
    [StringLength(100)]
    public string? BloodPressureD { get; set; }

    [StringLength(100)]
    public string? O2 { get; set; }

    [StringLength(100)]
    public string? Pain { get; set; }

    [StringLength(200)]
    public string? Heent { get; set; }

    [Column("CV")]
    [StringLength(200)]
    public string? Cv { get; set; }

    [StringLength(200)]
    public string? Chest { get; set; }

    [Column("ABD")]
    [StringLength(200)]
    public string? Abd { get; set; }

    [StringLength(200)]
    public string? Extr { get; set; }

    [StringLength(200)]
    public string? Skin { get; set; }

    [StringLength(200)]
    public string? Neuro { get; set; }

    [StringLength(200)]
    public string? Other { get; set; }

    [StringLength(200)]
    public string? Diagnosis { get; set; }

    [Column("Treatment Plan")]
    [StringLength(200)]
    public string? TreatmentPlan { get; set; }

    [Column("Medications Dispensed")]
    [StringLength(200)]
    public string? MedicationsDispensed { get; set; }

    [StringLength(200)]
    public string? Procedures { get; set; }

    [StringLength(200)]
    public string? Followup { get; set; }

    [Column("location")]
    [StringLength(200)]
    public string? Location { get; set; }

    [Column("History Of Present Illness Or Injury")]
    [StringLength(200)]
    public string? HistoryOfPresentIllnessOrInjury { get; set; }

    public bool? IsFinalize { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Encounters")]
    public virtual Request Request { get; set; } = null!;
}
