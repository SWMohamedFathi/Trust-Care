using System;
using System.Collections.Generic;

namespace TrustCare.Models;

public partial class Subscription
{
    public decimal SubscriptionId { get; set; }

    public decimal? UserId { get; set; }

    public DateTime? SubscriptionDate { get; set; }

    public decimal? SubscriptionAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

    public virtual User? User { get; set; }
}
