using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Businesstype
{
    public int Businesstypeid { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Business> Businesses { get; set; } = new List<Business>();
}
