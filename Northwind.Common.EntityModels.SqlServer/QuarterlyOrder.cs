﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Mdk.Shared;

[Keyless]
public partial class QuarterlyOrder
{
    [Column("CustomerID")]
    [StringLength(5)]
    public string? CustomerId { get; set; }

    [StringLength(40)]
    public string? CompanyName { get; set; }

    [StringLength(15)]
    public string? City { get; set; }

    [StringLength(15)]
    public string? Country { get; set; }
}
