namespace Chirp.Core;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

/// <summary>
///Cheep represents the "tweets" in the Chirp application
/// </summary>
[Index(nameof(Timestamp))]
public class Cheep
{
    public long CheepId { get; set; }

    [StringLength(160)]
    [Required]
    public required string Text { get; set; }
    public DateTime Timestamp { get; set; }

    [Required]
    public required Author Author { get; set; }
    public int AuthorId { get; set; }
    public List<string> Likes { get; set; } = new List<string>();
    public int NrLikes { get; set; }
}
