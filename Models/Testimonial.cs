using System;
using System.Collections.Generic;

namespace TrustCare.Models;

public partial class Testimonial
{
    public decimal TestimonialId { get; set; }

    public decimal? UserId { get; set; }

    public string? TestimonialText { get; set; }

    public string? ApprovalStatus { get; set; }

    public virtual User? User { get; set; }
}
