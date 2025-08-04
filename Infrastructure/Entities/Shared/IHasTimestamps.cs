namespace Infrastructure.Entities.Shared;

internal interface IHasTimestamps
{
    DateTime? CreatedAt { get; set; }
    int? CreatedUserId { get; set; }
    DateTime? UpdatedAt { get; set; }
    int? UpdatedUserId { get; set; }
    DateTime? DeletedAt { get; set; }
    int? DeletedUserId { get; set; }
}
