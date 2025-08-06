using Infrastructure.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("Recommend")]
public class Recommend : IHasTimestamps
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Content { get; set; }

    [StringLength(2000)]
    public string? HashTags { get; set; }

    public DateTime? CreatedAt { get; set; }
    public int? CreatedUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedUserId { get; set; }
}
