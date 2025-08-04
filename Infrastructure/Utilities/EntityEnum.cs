using System.ComponentModel;

namespace Infrastructure.Utilities;

public enum MealType
{
    [Description("Morning")]
    Morning,
    [Description("Lunch")]
    Lunch,
    [Description("Dinner")]
    Dinner,
    [Description("Snack")]
    Snack
}
