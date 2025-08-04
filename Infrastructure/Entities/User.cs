using Infrastructure.Entities.Shared;
using Infrastructure.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("User")]
public class User : IHasTimestamps
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(64)]
    public string? FirstName { get; set; }
    
    [Required]
    [StringLength(64)]
    public string? LastName { get; set; }
    
    [Required]
    [StringLength(256)]
    [MailAddress]
    public string? MailAddress { get; set; }


    [StringLength(128)]
    public string? LoginPassword { get; set; }
    
    [Timestamp]
    public byte[]? Version { get; set; }
    
    [NotMapped]
    [Display(Name = "Full Name")]
    public string FullName
    {
        get
        {
            return (LastName ?? string.Empty) + (FirstName ?? string.Empty);
        }
    }

    public DateTime? CreatedAt { get; set; }
    public int? CreatedUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedUserId { get; set; }

    public ICollection<Diaries>? Diaries { get; set; }
    public ICollection<Meal>? Meals { get; set; }
    public ICollection<Exercise>? Exercises { get; set; }
    public ICollection<BodyRecords>? BodyRecords { get; set; }
}
