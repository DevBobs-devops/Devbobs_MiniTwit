using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Core;

/// <summary>
/// Used to keep track of who Follows who.
/// Follower - Is the Author who follows by another Author
/// Followed - Is the Author who is followed by another Author
/// </summary>
[PrimaryKey(nameof(Follower), nameof(Followed))]
public class Follow
{
    [Required]
    [StringLength(100)]
    public required string Follower { get; set; }

    [Required]
    [StringLength(100)]
    public required string Followed { get; set; }
}
