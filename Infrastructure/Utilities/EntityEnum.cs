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

public enum TimeMetrics
{
    [Description("日")]
    Daily,
    [Description("週")]
    Week,
    [Description("月")]
    Month,
    [Description("年")]
    Year
}