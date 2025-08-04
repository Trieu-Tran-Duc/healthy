using Infrastructure.Entities.Shared;
using Infrastructure.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("Meal")]
public class Meal : IHasTimestamps
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int FoodId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime MealDate { get; set; }

    [Required]
    public MealType MealType { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Timestamp]
    public byte[]? Version { get; set; }

    public DateTime? CreatedAt { get; set; }
    public int? CreatedUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedUserId { get; set; }

    public User User { get; set; }
    public Food Food { get; set; }
}
