using ApplicationCore.Helper;
using ApplicationCore.Service;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore;

public static class AddDependencyInjectionServices
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<IDiaryService, DiaryService>();
        services.AddScoped<IBodyRecordsService, BodyRecordsService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMealService, MealService>();
        services.AddScoped<IFoodService, FoodService>();

        services.AddScoped<IAccessSessionHelper, AccessSessionHelper>();
        
        return services;
    }
}
