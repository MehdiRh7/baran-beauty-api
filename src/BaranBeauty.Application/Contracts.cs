using BaranBeauty.Domain;

namespace BaranBeauty.Application;

public interface IRepository<T> where T : Entity
{
    Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default);
    Task<T?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Remove(T entity);
}

public interface IUnitOfWork { Task<int> SaveChangesAsync(CancellationToken ct = default); }
public interface IAppointmentQueries
{
    Task<bool> IsBlockedAsync(DateOnly date, CancellationToken ct = default);
    Task<bool> SlotExistsAsync(DateTime startsAt, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> ListDetailedAsync(CancellationToken ct = default);
}
public interface IAdminAuthentication
{
    Task<AuthenticatedAdmin?> AuthenticateAsync(string username, string password, CancellationToken ct = default);
}
public interface IFileStorage
{
    Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string url, CancellationToken ct = default);
}

public sealed record AuthenticatedAdmin(Guid Id, string Username, string DisplayName);
public sealed record LoginRequest(string Username, string Password);
public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string DisplayName);
public sealed record ServiceRequest(string Title, string Description, decimal StartingPrice, int DurationMinutes, string Icon, bool IsActive = true);
public sealed record ServiceDto(Guid Id, string Title, string Description, decimal StartingPrice, int DurationMinutes, string Icon, bool IsActive);
public sealed record AppointmentRequest(Guid ServiceId, string CustomerName, string Mobile, DateTime StartsAt, string? Notes);
public sealed record AppointmentDto(Guid Id, Guid ServiceId, string ServiceTitle, string CustomerName, string Mobile, DateTime StartsAt, string? Notes, string Status);
public sealed record BlockedDateRequest(DateOnly Date, string? Reason);
public sealed record StaffRequest(string Name, string Role, string Experience, string ImageUrl, bool IsActive = true);
public sealed record StaffDto(Guid Id, string Name, string Role, string Experience, string ImageUrl, bool IsActive);
public sealed record CertificateRequest(string Title, string Issuer, string Year, string Code, string Description, string Skills, string MediaUrl, string MediaType);
public sealed record CertificateDto(Guid Id, string Title, string Issuer, string Year, string Code, string Description, string[] Skills, string MediaUrl, string MediaType);
public sealed record MediaDto(Guid Id, string Name, string Url, string Type, string Section);
public sealed record SalonProfileRequest(string Name, string Phone, string Mobile, string Address, string About, TimeOnly OpensAt, TimeOnly ClosesAt);
public sealed record SalonProfileDto(Guid Id, string Name, string Phone, string Mobile, string Address, string About, TimeOnly OpensAt, TimeOnly ClosesAt);

public interface ISalonContentService
{
    Task<IReadOnlyList<ServiceDto>> GetServicesAsync(bool includeInactive, CancellationToken ct);
    Task<ServiceDto> SaveServiceAsync(Guid? id, ServiceRequest request, CancellationToken ct);
    Task DeleteServiceAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<StaffDto>> GetStaffAsync(bool includeInactive, CancellationToken ct);
    Task<StaffDto> SaveStaffAsync(Guid? id, StaffRequest request, CancellationToken ct);
    Task DeleteStaffAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<CertificateDto>> GetCertificatesAsync(CancellationToken ct);
    Task<CertificateDto> SaveCertificateAsync(Guid? id, CertificateRequest request, CancellationToken ct);
    Task DeleteCertificateAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<MediaDto>> GetMediaAsync(CancellationToken ct);
    Task<MediaDto> AddMediaAsync(string name, string url, string type, string section, CancellationToken ct);
    Task DeleteMediaAsync(Guid id, CancellationToken ct);
    Task<SalonProfileDto?> GetProfileAsync(CancellationToken ct);
    Task<SalonProfileDto> SaveProfileAsync(SalonProfileRequest request, CancellationToken ct);
}

public interface IBookingService
{
    Task<AppointmentDto> CreateAsync(AppointmentRequest request, CancellationToken ct);
    Task<IReadOnlyList<AppointmentDto>> ListAsync(CancellationToken ct);
    Task ChangeStatusAsync(Guid id, AppointmentStatus status, CancellationToken ct);
    Task<IReadOnlyList<BlockedDate>> GetBlockedDatesAsync(CancellationToken ct);
    Task SetBlockedDateAsync(BlockedDateRequest request, CancellationToken ct);
    Task RemoveBlockedDateAsync(Guid id, CancellationToken ct);
}
