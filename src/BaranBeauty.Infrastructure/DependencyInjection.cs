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
        var cs=config.GetConnectionString("SqlServer")??throw new InvalidOperationException("ConnectionStrings:SqlServer is missing.");
        services.AddDbContext<BaranDbContext>(o=>o.UseSqlServer(cs,sql=>sql.EnableRetryOnFailure()));
        services.AddScoped(typeof(IRepository<>),typeof(EfRepository<>)).AddScoped<IUnitOfWork>(x=>x.GetRequiredService<BaranDbContext>())
            .AddScoped<IAppointmentQueries,AppointmentQueries>().AddScoped<IAdminAuthentication,AdminAuthentication>()
            .AddScoped<SmsSettingsService>().AddScoped<ISmsSettingsService>(x=>x.GetRequiredService<SmsSettingsService>()).AddScoped<IAppointmentNotifier,KavenegarAppointmentNotifier>()
            .AddSingleton<IPasswordHasher<AdminUser>,PasswordHasher<AdminUser>>()
            .AddSingleton<IFileStorage>(new LocalFileStorage(Path.Combine(contentRoot,"wwwroot","uploads"),"/uploads"));
        services.AddDataProtection();
        services.AddHttpClient("Kavenegar",c=>c.Timeout=TimeSpan.FromSeconds(20));
        return services;
    }
}
