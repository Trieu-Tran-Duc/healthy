using Infrastructure.Models.Api;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ApplicationCore.Helper;

public interface IAccessSessionHelper
{
    Task<GetUserModelDto> GetUserContextAsync();
}

/// <summary>
/// get information about access session
/// </summary>
public class AccessSessionHelper : IAccessSessionHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccessSessionHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// get user information from session
    /// </summary>
    /// <returns></returns>
    public async Task<GetUserModelDto?> GetUserContextAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.Session == null) return null;

        return await GetAsync(context.Session, nameof(GetUserModelDto));
    }

    /// <summary>
    /// get user information from session
    /// </summary>
    /// <param name="session"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private static async Task<GetUserModelDto?> GetAsync(ISession session, string key)
    {
        if (!session.IsAvailable)
            await session.LoadAsync();

        if (session.TryGetValue(key, out var result))
        {
            return JsonSerializer.Deserialize<GetUserModelDto>(result);
        }

        return default;
    }
}
