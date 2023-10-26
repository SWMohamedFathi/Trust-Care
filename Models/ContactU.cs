using System;
using System.Collections.Generic;

namespace TrustCare.Models;

public partial class ContactU
{
    public decimal ContactId { get; set; }

    public string? Name { get; set; }

    public string? Subject { get; set; }

  
    public string? Email { get; set; }

    public string? Message { get; set; }
}
