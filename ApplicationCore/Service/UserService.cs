using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Models.Api;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ApplicationCore.Service;

public interface IUserService
{
    Task<GetUserModelDto?> GetUserInfor(string email, string password);
    Task<GetUserModelDto?> RegisterUser(RegisterUserDto registerUser);
    Task<List<GetUserModelDto>?> GetAllUserActive();
}

/// <summary>
/// service for user operations
/// </summary>
public class UserService : IUserService
{
    private readonly HealtyDbContext _healtyDbContext;

    public UserService(HealtyDbContext healtyDbContext)
    {
        _healtyDbContext = healtyDbContext;
    }

    /// <summary>
    /// get user information by email and password
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<GetUserModelDto?> GetUserInfor(string email, string password)
    {
        var userExists = await _healtyDbContext.Users.FirstOrDefaultAsync(u => u.MailAddress == email);
        if (userExists == null)
        {
            return null;
        }

        if (!VerifyPassword(password, userExists.LoginPassword!))
        {
            return null;
        }
        
        return new GetUserModelDto
        {
            UserId = userExists.Id,
            FullName = userExists.FullName,
            MailAddress = userExists.MailAddress
        };
    }

    /// <summary>
    /// hash the input password using SHA256
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private static string HashPassword(string password)
    {
        using (var sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>
    /// verify the input password against the stored hashed password
    /// </summary>
    /// <param name="inputPassword"></param>
    /// <param name="storedHashed"></param>
    /// <returns></returns>
    private static bool VerifyPassword(string inputPassword, string storedHashed)
    {
        var hashedInput = HashPassword(inputPassword);
        return hashedInput == storedHashed;
    }

    /// <summary>
    /// register a new user
    /// </summary>
    /// <param name="registerUser"></param>
    /// <returns></returns>
    public async Task<GetUserModelDto?> RegisterUser(RegisterUserDto registerUser)
    {
        var userExists = await _healtyDbContext.Users.FirstOrDefaultAsync(u => u.MailAddress == registerUser.MailAddress);
        if (userExists != null)
        {
            throw new ArgumentException($"User with Email {registerUser.MailAddress} already exists.");
        }
        
        var newUser = new User
        {
            FirstName = registerUser.FirstName,
            LastName = registerUser.LastName,
            MailAddress = registerUser.MailAddress,
            LoginPassword = HashPassword(registerUser.LoginPassword ?? string.Empty)
        };
        
        _healtyDbContext.Users.Add(newUser);
        await _healtyDbContext.SaveChangesAsync();
        
        return new GetUserModelDto
        {
            UserId = newUser.Id,
            FullName = newUser.FullName,
            MailAddress = newUser.MailAddress
        };
    }

    /// <summary>
    /// get all active users in the system
    /// </summary>
    /// <returns></returns>
    public async Task<List<GetUserModelDto>?> GetAllUserActive()
    {
        return await _healtyDbContext.Users
            .Where(u => u.DeletedAt == null)
            .AsNoTracking()
            .Select(u => new GetUserModelDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                MailAddress = u.MailAddress
            })
            .OrderByDescending(u => u.UserId)
            .ToListAsync();
    }
}
