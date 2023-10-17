using System;
using System.Collections.Generic;

namespace TrustCare.Models;

public partial class Footer
{
    public decimal FooterId { get; set; }

    public string? FooterLinks { get; set; }

    public string? CopyrightText { get; set; }

    public string? ContactDetails { get; set; }
}
