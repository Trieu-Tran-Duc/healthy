using Infrastructure.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("Food")]
public class Food
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string Name { get; set; }

    public string? PhotoUrl { get; set; }

    [Required]
    public MealType MealType { get; set; }

    [Timestamp]
    public byte[]? Version { get; set; }

    public ICollection<Meal> Meals { get; set; }
}
