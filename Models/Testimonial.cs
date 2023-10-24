using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustCare.Models;

public partial class Testimonial
{
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    public decimal TestimonialId { get; set; }

    public decimal? UserId { get; set; }

    public string? TestimonialText { get; set; }

    public string? ApprovalStatus { get; set; }

    public string? Imagepath { get; set; }

    public virtual User? User { get; set; }
}
