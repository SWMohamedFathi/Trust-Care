using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace TrustCare.Models;

public partial class Beneficiary
{
    [NotMapped]
    public IFormFile? Proof_Document { get; set; }
    public decimal BeneficiaryId { get; set; }

    public decimal? SubscriptionId { get; set; }

    public string? Relationship { get; set; }
    [Required]
    public string? ProofDocument { get; set; }

    public string? ApprovalStatus { get; set; }

    public virtual Subscription? Subscription { get; set; }
}
