using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class AspNetRole
{
    [StringLength(256)]
    public string Name { get; set; } = null!;

    [Key]
    public int Id { get; set; }
}
