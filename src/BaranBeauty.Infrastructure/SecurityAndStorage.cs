using BaranBeauty.Application;
using BaranBeauty.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaranBeauty.Infrastructure;

internal sealed class AdminAuthentication(BaranDbContext db, IPasswordHasher<AdminUser> hasher) : IAdminAuthentication
{
    public async Task<AuthenticatedAdmin?> AuthenticateAsync(string username,string password,CancellationToken ct=default)
    {
        var user=await db.AdminUsers.SingleOrDefaultAsync(x=>x.Username==username.Trim().ToLower(),ct);
        if(user is null)return null;
        return hasher.VerifyHashedPassword(user,user.PasswordHash,password)==PasswordVerificationResult.Failed?null:new(user.Id,user.Username,user.DisplayName);
    }
}

public sealed class LocalFileStorage(string rootPath, string publicBaseUrl) : IFileStorage
{
    private static readonly HashSet<string> Allowed=[".jpg",".jpeg",".png",".webp",".mp4",".mov"];
    public async Task<string> SaveAsync(Stream stream,string fileName,string contentType,CancellationToken ct=default)
    {
        var ext=Path.GetExtension(fileName).ToLowerInvariant();if(!Allowed.Contains(ext))throw new InvalidOperationException("نوع فایل مجاز نیست.");
        Directory.CreateDirectory(rootPath);var safeName=$"{Guid.NewGuid():N}{ext}";var path=Path.Combine(rootPath,safeName);
        await using var output=File.Create(path);await stream.CopyToAsync(output,ct);return $"{publicBaseUrl.TrimEnd('/')}/{safeName}";
    }
    public Task DeleteAsync(string url,CancellationToken ct=default){var path=Path.Combine(rootPath,Path.GetFileName(url));if(File.Exists(path))File.Delete(path);return Task.CompletedTask;}
}

public static class DatabaseSeeder
{
    public static async Task SeedAsync(BaranDbContext db,IPasswordHasher<AdminUser> hasher,CancellationToken ct=default)
    {
        await db.Database.EnsureCreatedAsync(ct);
        if(!await db.AdminUsers.AnyAsync(ct)){var placeholder=new AdminUser("admin","pending","مدیر سالن باران");var hash=hasher.HashPassword(placeholder,"Baran@1405");db.AdminUsers.Add(new AdminUser("admin",hash,"مدیر سالن باران"));}
        await db.SaveChangesAsync(ct);
    }
}
