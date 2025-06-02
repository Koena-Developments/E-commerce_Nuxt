using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace AuthApi.TFTEntities;

public partial class User
{
    [Key]
    public long Id { get; set; }

    public string? Username { get; set;}

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
}
