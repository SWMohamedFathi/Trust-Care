using System;
using System.Collections.Generic;

namespace TrustCare.Models;

public partial class Role
{
    public decimal RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
