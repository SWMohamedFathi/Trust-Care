using System;
using System.Collections.Generic;

namespace TrustCare.Models;

public partial class Bank
{
    public decimal BankId { get; set; }

    public string? Owner { get; set; }

    public decimal? CardNumber { get; set; }

    public decimal? Cvv { get; set; }

    public decimal? Balance { get; set; }
}
