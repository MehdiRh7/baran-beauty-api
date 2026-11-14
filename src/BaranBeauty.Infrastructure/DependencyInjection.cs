using BaranBeauty.Application;
using BaranBeauty.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaranBeauty.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)=>services
        .AddScoped<ISalonContentService,SalonContentService>().AddScoped<IBookingService,BookingService>();
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration config,string contentRoot)
    {
        var cs=config.GetConnectionString("Sqlite")??throw new InvalidOperationException("Connection string missing.");
        services.AddDbContext<BaranDbContext>(o=>o.UseSqlite(cs));
        services.AddScoped(typeof(IRepository<>),typeof(EfRepository<>)).AddScoped<IUnitOfWork>(x=>x.GetRequiredService<BaranDbContext>())
            .AddScoped<IAppointmentQueries,AppointmentQueries>().AddScoped<IAdminAuthentication,AdminAuthentication>()
            .AddSingleton<IPasswordHasher<AdminUser>,PasswordHasher<AdminUser>>()
            .AddSingleton<IFileStorage>(new LocalFileStorage(Path.Combine(contentRoot,"wwwroot","uploads"),"/uploads"));
        return services;
    }
}
