using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustCare.Models;

public partial class About
{
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    public decimal AboutId { get; set; }

    public string? HeadingOne { get; set; }

    public string? Content { get; set; }

    public string? ImagePath { get; set; }
}
