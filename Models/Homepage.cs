﻿using System;
using System.Collections.Generic;
namespace TrustCare.Models;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Homepage
{

    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    [NotMapped]
    public IFormFile? ImageLogo { get; set; }
    public decimal HomeId { get; set; }

    public string? Logo { get; set; }

    public string? SectionName { get; set; }

    public string? ContentText { get; set; }

    public string? SlideImageImage { get; set; }

    public string? HeadingOne { get; set; }

    public string? HeadingThree { get; set; }
}
