using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

/// <summary>
/// Author represents a user in the chirp application
/// Author inherits from IdentityUser where an int is used as the key*
/// </summary>
public class Author : IdentityUser<int>
{
    [StringLength(100)]
    [RegularExpression(@"^[^\/]*$")]
    [Required]
    public required string Name { get; set; }

    [StringLength(100)]
    [Required]
    public new required string Email { get; set; }
}
