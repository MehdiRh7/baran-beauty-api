namespace BaranBeauty.Domain;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; protected set; }
    protected void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}

public sealed class BeautyService : Entity
{
    private BeautyService() { }
    public BeautyService(string title, string description, decimal startingPrice, int durationMinutes, string icon)
        => Update(title, description, startingPrice, durationMinutes, icon, true);
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal StartingPrice { get; private set; }
    public int DurationMinutes { get; private set; }
    public string Icon { get; private set; } = "sparkles";
    public bool IsActive { get; private set; } = true;
    public void Update(string title, string description, decimal startingPrice, int durationMinutes, string icon, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("عنوان خدمت الزامی است.");
        if (startingPrice < 0 || durationMinutes <= 0) throw new ArgumentOutOfRangeException(nameof(startingPrice));
        Title = title.Trim(); Description = description.Trim(); StartingPrice = startingPrice;
        DurationMinutes = durationMinutes; Icon = icon.Trim(); IsActive = isActive; Touch();
    }
}

public enum AppointmentStatus { Pending, Confirmed, Completed, Cancelled }
public sealed class Appointment : Entity
{
    private Appointment() { }
    public Appointment(Guid serviceId, string customerName, string mobile, DateTime startsAt, string? notes)
    {
        if (startsAt <= DateTime.Now) throw new ArgumentException("زمان نوبت باید در آینده باشد.");
        ServiceId = serviceId; CustomerName = customerName.Trim(); Mobile = mobile.Trim(); StartsAt = startsAt;
        Notes = notes?.Trim(); Status = AppointmentStatus.Pending;
    }
    public Guid ServiceId { get; private set; }
    public BeautyService? Service { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string Mobile { get; private set; } = string.Empty;
    public DateTime StartsAt { get; private set; }
    public string? Notes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public void ChangeStatus(AppointmentStatus status) { Status = status; Touch(); }
}

public sealed class BlockedDate : Entity
{
    private BlockedDate() { }
    public BlockedDate(DateOnly date, string? reason) { Date = date; Reason = reason?.Trim(); }
    public DateOnly Date { get; private set; }
    public string? Reason { get; private set; }
}

public sealed class StaffMember : Entity
{
    private StaffMember() { }
    public StaffMember(string name, string role, string experience, string imageUrl) => Update(name, role, experience, imageUrl, true);
    public string Name { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public string Experience { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public void Update(string name, string role, string experience, string imageUrl, bool active)
    { Name = name.Trim(); Role = role.Trim(); Experience = experience.Trim(); ImageUrl = imageUrl.Trim(); IsActive = active; Touch(); }
}

public sealed class Certificate : Entity
{
    private Certificate() { }
    public Certificate(string title, string issuer, string year, string code, string description, string skills, string mediaUrl, string mediaType)
        => Update(title, issuer, year, code, description, skills, mediaUrl, mediaType);
    public string Title { get; private set; } = string.Empty;
    public string Issuer { get; private set; } = string.Empty;
    public string Year { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Skills { get; private set; } = string.Empty;
    public string MediaUrl { get; private set; } = string.Empty;
    public string MediaType { get; private set; } = "image";
    public void Update(string title, string issuer, string year, string code, string description, string skills, string mediaUrl, string mediaType)
    { Title=title.Trim(); Issuer=issuer.Trim(); Year=year.Trim(); Code=code.Trim(); Description=description.Trim(); Skills=skills.Trim(); MediaUrl=mediaUrl.Trim(); MediaType=mediaType.Trim(); Touch(); }
}

public sealed class MediaAsset : Entity
{
    private MediaAsset() { }
    public MediaAsset(string name, string url, string type, string section) { Name=name.Trim(); Url=url.Trim(); Type=type.Trim(); Section=section.Trim(); }
    public string Name { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string Type { get; private set; } = "image";
    public string Section { get; private set; } = "gallery";
}

public sealed class SalonProfile : Entity
{
    private SalonProfile() { }
    public SalonProfile(string name, string phone, string mobile, string address, string about, TimeOnly opensAt, TimeOnly closesAt)
        => Update(name, phone, mobile, address, about, opensAt, closesAt);
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Mobile { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string About { get; private set; } = string.Empty;
    public TimeOnly OpensAt { get; private set; }
    public TimeOnly ClosesAt { get; private set; }
    public void Update(string name, string phone, string mobile, string address, string about, TimeOnly opensAt, TimeOnly closesAt)
    { Name=name.Trim(); Phone=phone.Trim(); Mobile=mobile.Trim(); Address=address.Trim(); About=about.Trim(); OpensAt=opensAt; ClosesAt=closesAt; Touch(); }
}

public sealed class AdminUser : Entity
{
    private AdminUser() { }
    public AdminUser(string username, string passwordHash, string displayName) { Username=username.Trim().ToLowerInvariant(); PasswordHash=passwordHash; DisplayName=displayName.Trim(); }
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
}
