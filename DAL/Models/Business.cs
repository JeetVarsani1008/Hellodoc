﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("Business")]
public partial class Business
{
    [Key]
    public int BusinessId { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Address1 { get; set; }

    [StringLength(500)]
    public string? Address2 { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    public int? RegionId { get; set; }

    [StringLength(100)]
    public string? ZipCode { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(20)]
    public string? FaxNumber { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    public short? Status { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsRegistered { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("BusinessCreatedByNavigations")]
    public virtual AspNetUser? CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("BusinessModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("Businesses")]
    public virtual Region? Region { get; set; }

    [InverseProperty("Business")]
    public virtual ICollection<RequestBusiness> RequestBusinesses { get; set; } = new List<RequestBusiness>();
}
