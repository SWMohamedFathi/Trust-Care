using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustCare.Models;

public partial class User
{
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    public decimal UserId { get; set; }

    public decimal? RoleId { get; set; }

    public string? ProfileImage { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public decimal? Phone { get; set; }

    public DateTime? Dateofbirth { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
}
