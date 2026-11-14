using BaranBeauty.Application;
using BaranBeauty.Domain;
using Microsoft.EntityFrameworkCore;

namespace BaranBeauty.Infrastructure;

public sealed class BaranDbContext(DbContextOptions<BaranDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<BeautyService> Services => Set<BeautyService>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<BlockedDate> BlockedDates => Set<BlockedDate>();
    public DbSet<StaffMember> Staff => Set<StaffMember>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<MediaAsset> Media => Set<MediaAsset>();
    public DbSet<SalonProfile> SalonProfiles => Set<SalonProfile>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<SmsSettings> SmsSettings => Set<SmsSettings>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<BeautyService>().Property(x=>x.StartingPrice).HasPrecision(18,2);
        b.Entity<Appointment>().HasOne(x=>x.Service).WithMany().HasForeignKey(x=>x.ServiceId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Appointment>().HasIndex(x=>x.StartsAt).IsUnique();
        b.Entity<BlockedDate>().HasIndex(x=>x.Date).IsUnique();
        b.Entity<AdminUser>().HasIndex(x=>x.Username).IsUnique();
        b.Entity<SalonProfile>().Property(x=>x.About).HasMaxLength(2000);
        b.Entity<Certificate>().Property(x=>x.Description).HasMaxLength(3000);
    }
}

internal sealed class EfRepository<T>(BaranDbContext db) : IRepository<T> where T : Entity
{
    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct=default)=>await db.Set<T>().AsNoTracking().ToListAsync(ct);
    public Task<T?> GetAsync(Guid id,CancellationToken ct=default)=>db.Set<T>().FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task AddAsync(T entity,CancellationToken ct=default)=>db.Set<T>().AddAsync(entity,ct).AsTask();
    public void Remove(T entity)=>db.Set<T>().Remove(entity);
}

internal sealed class AppointmentQueries(BaranDbContext db) : IAppointmentQueries
{
    public Task<bool> IsBlockedAsync(DateOnly date,CancellationToken ct=default)=>db.BlockedDates.AnyAsync(x=>x.Date==date,ct);
    public Task<bool> SlotExistsAsync(DateTime startsAt,CancellationToken ct=default)=>db.Appointments.AnyAsync(x=>x.StartsAt==startsAt&&x.Status!=AppointmentStatus.Cancelled,ct);
    public async Task<IReadOnlyList<Appointment>> ListDetailedAsync(CancellationToken ct=default)=>await db.Appointments.AsNoTracking().Include(x=>x.Service).OrderByDescending(x=>x.StartsAt).ToListAsync(ct);
}
