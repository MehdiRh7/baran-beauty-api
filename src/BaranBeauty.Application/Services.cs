using BaranBeauty.Domain;

namespace BaranBeauty.Application;

public sealed class SalonContentService(
    IRepository<BeautyService> services, IRepository<StaffMember> staff, IRepository<Certificate> certificates,
    IRepository<MediaAsset> media, IRepository<SalonProfile> profiles, IUnitOfWork uow) : ISalonContentService
{
    public async Task<IReadOnlyList<ServiceDto>> GetServicesAsync(bool includeInactive, CancellationToken ct) =>
        (await services.ListAsync(ct)).Where(x=>includeInactive||x.IsActive).Select(Map).ToList();
    public async Task<ServiceDto> SaveServiceAsync(Guid? id, ServiceRequest r, CancellationToken ct)
    { var e=id is null?new BeautyService(r.Title,r.Description,r.StartingPrice,r.DurationMinutes,r.Icon):await services.GetAsync(id.Value,ct)??throw new KeyNotFoundException(); if(id is not null)e.Update(r.Title,r.Description,r.StartingPrice,r.DurationMinutes,r.Icon,r.IsActive);else await services.AddAsync(e,ct); await uow.SaveChangesAsync(ct); return Map(e); }
    public async Task DeleteServiceAsync(Guid id,CancellationToken ct){var e=await services.GetAsync(id,ct)??throw new KeyNotFoundException();services.Remove(e);await uow.SaveChangesAsync(ct);}
    public async Task<IReadOnlyList<StaffDto>> GetStaffAsync(bool includeInactive,CancellationToken ct)=>(await staff.ListAsync(ct)).Where(x=>includeInactive||x.IsActive).Select(Map).ToList();
    public async Task<StaffDto> SaveStaffAsync(Guid? id,StaffRequest r,CancellationToken ct){var e=id is null?new StaffMember(r.Name,r.Role,r.Experience,r.ImageUrl):await staff.GetAsync(id.Value,ct)??throw new KeyNotFoundException();if(id is not null)e.Update(r.Name,r.Role,r.Experience,r.ImageUrl,r.IsActive);else await staff.AddAsync(e,ct);await uow.SaveChangesAsync(ct);return Map(e);}
    public async Task DeleteStaffAsync(Guid id,CancellationToken ct){var e=await staff.GetAsync(id,ct)??throw new KeyNotFoundException();staff.Remove(e);await uow.SaveChangesAsync(ct);}
    public async Task<IReadOnlyList<CertificateDto>> GetCertificatesAsync(CancellationToken ct)=>(await certificates.ListAsync(ct)).Select(Map).ToList();
    public async Task<CertificateDto> SaveCertificateAsync(Guid? id,CertificateRequest r,CancellationToken ct){var e=id is null?new Certificate(r.Title,r.Issuer,r.Year,r.Code,r.Description,r.Skills,r.MediaUrl,r.MediaType):await certificates.GetAsync(id.Value,ct)??throw new KeyNotFoundException();if(id is not null)e.Update(r.Title,r.Issuer,r.Year,r.Code,r.Description,r.Skills,r.MediaUrl,r.MediaType);else await certificates.AddAsync(e,ct);await uow.SaveChangesAsync(ct);return Map(e);}
    public async Task DeleteCertificateAsync(Guid id,CancellationToken ct){var e=await certificates.GetAsync(id,ct)??throw new KeyNotFoundException();certificates.Remove(e);await uow.SaveChangesAsync(ct);}
    public async Task<IReadOnlyList<MediaDto>> GetMediaAsync(CancellationToken ct)=>(await media.ListAsync(ct)).Select(Map).ToList();
    public async Task<MediaDto> AddMediaAsync(string name,string url,string type,string section,CancellationToken ct){var e=new MediaAsset(name,url,type,section);await media.AddAsync(e,ct);await uow.SaveChangesAsync(ct);return Map(e);}
    public async Task DeleteMediaAsync(Guid id,CancellationToken ct){var e=await media.GetAsync(id,ct)??throw new KeyNotFoundException();media.Remove(e);await uow.SaveChangesAsync(ct);}
    public async Task<SalonProfileDto?> GetProfileAsync(CancellationToken ct)=>(await profiles.ListAsync(ct)).Select(Map).FirstOrDefault();
    public async Task<SalonProfileDto> SaveProfileAsync(SalonProfileRequest r,CancellationToken ct){var e=(await profiles.ListAsync(ct)).FirstOrDefault();if(e is null){e=new SalonProfile(r.Name,r.Phone,r.Mobile,r.Address,r.About,r.OpensAt,r.ClosesAt);await profiles.AddAsync(e,ct);}else e.Update(r.Name,r.Phone,r.Mobile,r.Address,r.About,r.OpensAt,r.ClosesAt);await uow.SaveChangesAsync(ct);return Map(e);}
    private static ServiceDto Map(BeautyService x)=>new(x.Id,x.Title,x.Description,x.StartingPrice,x.DurationMinutes,x.Icon,x.IsActive);
    private static StaffDto Map(StaffMember x)=>new(x.Id,x.Name,x.Role,x.Experience,x.ImageUrl,x.IsActive);
    private static CertificateDto Map(Certificate x)=>new(x.Id,x.Title,x.Issuer,x.Year,x.Code,x.Description,x.Skills.Split('،',StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries),x.MediaUrl,x.MediaType);
    private static MediaDto Map(MediaAsset x)=>new(x.Id,x.Name,x.Url,x.Type,x.Section);
    private static SalonProfileDto Map(SalonProfile x)=>new(x.Id,x.Name,x.Phone,x.Mobile,x.Address,x.About,x.OpensAt,x.ClosesAt);
}

public sealed class BookingService(IRepository<Appointment> appointments,IRepository<BeautyService> services,IRepository<BlockedDate> blocked,IAppointmentQueries queries,IUnitOfWork uow):IBookingService
{
    public async Task<AppointmentDto> CreateAsync(AppointmentRequest r,CancellationToken ct)
    { var service=await services.GetAsync(r.ServiceId,ct)??throw new KeyNotFoundException("خدمت یافت نشد.");var date=DateOnly.FromDateTime(r.StartsAt);if(await queries.IsBlockedAsync(date,ct)||r.StartsAt.DayOfWeek==DayOfWeek.Friday)throw new InvalidOperationException("این روز قابل رزرو نیست.");if(await queries.SlotExistsAsync(r.StartsAt,ct))throw new InvalidOperationException("این ساعت قبلاً رزرو شده است.");var e=new Appointment(r.ServiceId,r.CustomerName,r.Mobile,r.StartsAt,r.Notes);await appointments.AddAsync(e,ct);await uow.SaveChangesAsync(ct);return Map(e,service.Title);}
    public async Task<IReadOnlyList<AppointmentDto>> ListAsync(CancellationToken ct)=>(await queries.ListDetailedAsync(ct)).Select(x=>Map(x,x.Service?.Title??string.Empty)).ToList();
    public async Task ChangeStatusAsync(Guid id,AppointmentStatus status,CancellationToken ct){var e=await appointments.GetAsync(id,ct)??throw new KeyNotFoundException();e.ChangeStatus(status);await uow.SaveChangesAsync(ct);}
    public Task<IReadOnlyList<BlockedDate>> GetBlockedDatesAsync(CancellationToken ct)=>blocked.ListAsync(ct);
    public async Task SetBlockedDateAsync(BlockedDateRequest r,CancellationToken ct){if(!await queries.IsBlockedAsync(r.Date,ct)){await blocked.AddAsync(new BlockedDate(r.Date,r.Reason),ct);await uow.SaveChangesAsync(ct);}}
    public async Task RemoveBlockedDateAsync(Guid id,CancellationToken ct){var e=await blocked.GetAsync(id,ct)??throw new KeyNotFoundException();blocked.Remove(e);await uow.SaveChangesAsync(ct);}
    private static AppointmentDto Map(Appointment x,string title)=>new(x.Id,x.ServiceId,title,x.CustomerName,x.Mobile,x.StartsAt,x.Notes,x.Status.ToString());
}
