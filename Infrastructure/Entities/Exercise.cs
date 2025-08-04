using Infrastructure.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("Exercise")]
public class Exercise : IHasTimestamps
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ExerciseDate { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    public int DurationMinutes { get; set; }

    public int? Calories { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; } = false;

    [Timestamp]
    public byte[]? Version { get; set; }

    public DateTime? CreatedAt { get; set; }
    public int? CreatedUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedUserId { get; set; }

    public User User { get; set; }
}
