using System;
using System.Collections.Generic;

namespace TrustCare.Models;

public partial class About
{
    public decimal AboutId { get; set; }

    public string? HeadingOne { get; set; }

    public string? Content { get; set; }

    public string? ImagePath { get; set; }
}
