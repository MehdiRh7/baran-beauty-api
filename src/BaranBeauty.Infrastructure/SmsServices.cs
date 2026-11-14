using System.Globalization;
using System.Net.Http.Json;
using BaranBeauty.Application;
using BaranBeauty.Domain;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaranBeauty.Infrastructure;

internal sealed class SmsSettingsService(BaranDbContext db, IDataProtectionProvider protection, IHttpClientFactory clients) : ISmsSettingsService
{
    private readonly IDataProtector protector = protection.CreateProtector("BaranBeauty.Sms.ApiKey.v1");
    public async Task<SmsSettingsDto> GetAsync(CancellationToken ct)
    {
        var x=await db.SmsSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        return x is null ? new("","",false,false,"نوبت شما برای {ServiceTitle} در تاریخ {Date} ساعت {Time} ثبت شد.","نوبت جدید: {CustomerName}، {Mobile}، {ServiceTitle}، {Date} ساعت {Time}") : Map(x);
    }
    public async Task<SmsSettingsDto> SaveAsync(SmsSettingsRequest r,CancellationToken ct)
    {
        var x=await db.SmsSettings.FirstOrDefaultAsync(ct);
        var protectedKey=!string.IsNullOrWhiteSpace(r.ApiKey)?protector.Protect(r.ApiKey.Trim()):x?.ProtectedApiKey??string.Empty;
        if(x is null){x=new SmsSettings(r.AdminMobile,protectedKey,r.Sender,r.IsEnabled,r.CustomerTemplate,r.ManagerTemplate);db.SmsSettings.Add(x);}
        else x.Update(r.AdminMobile,protectedKey,r.Sender,r.IsEnabled,r.CustomerTemplate,r.ManagerTemplate);
        await db.SaveChangesAsync(ct);return Map(x);
    }
    public async Task SendTestAsync(string receptor,CancellationToken ct)
    {
        var x=await db.SmsSettings.AsNoTracking().FirstOrDefaultAsync(ct)??throw new InvalidOperationException("تنظیمات پیامک ذخیره نشده است.");
        await SendAsync(x,receptor,"پیام آزمایشی سامانه نوبت‌دهی باران با موفقیت ارسال شد.",ct);
    }
    internal async Task SendAsync(SmsSettings x,string receptor,string message,CancellationToken ct)
    {
        if(string.IsNullOrWhiteSpace(x.ProtectedApiKey))throw new InvalidOperationException("کلید API پیامک وارد نشده است.");
        var key=protector.Unprotect(x.ProtectedApiKey);
        var form=new Dictionary<string,string>{{"receptor",receptor},{"message",message}};
        if(!string.IsNullOrWhiteSpace(x.Sender))form["sender"]=x.Sender;
        using var response=await clients.CreateClient("Kavenegar").PostAsync($"https://api.kavenegar.com/v1/{Uri.EscapeDataString(key)}/sms/send.json",new FormUrlEncodedContent(form),ct);
        if(!response.IsSuccessStatusCode)throw new InvalidOperationException("ارسال پیامک توسط سرویس‌دهنده ناموفق بود.");
    }
    private static SmsSettingsDto Map(SmsSettings x)=>new(x.AdminMobile,x.Sender,x.IsEnabled,!string.IsNullOrWhiteSpace(x.ProtectedApiKey),x.CustomerTemplate,x.ManagerTemplate);
}

internal sealed class KavenegarAppointmentNotifier(BaranDbContext db,SmsSettingsService sender,ILogger<KavenegarAppointmentNotifier> logger):IAppointmentNotifier
{
    public async Task NotifyCreatedAsync(Appointment appointment,BeautyService service,CancellationToken ct)
    {
        var settings=await db.SmsSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        if(settings is null||!settings.IsEnabled)return;
        var pc=new PersianCalendar();var local=appointment.StartsAt;
        var date=$"{pc.GetYear(local):0000}/{pc.GetMonth(local):00}/{pc.GetDayOfMonth(local):00}";var time=local.ToString("HH:mm",CultureInfo.InvariantCulture);
        string Format(string template)=>template.Replace("{CustomerName}",appointment.CustomerName).Replace("{Mobile}",appointment.Mobile).Replace("{ServiceTitle}",service.Title).Replace("{Date}",date).Replace("{Time}",time);
        try
        {
            await sender.SendAsync(settings,appointment.Mobile,Format(settings.CustomerTemplate),ct);
            if(!string.IsNullOrWhiteSpace(settings.AdminMobile))await sender.SendAsync(settings,settings.AdminMobile,Format(settings.ManagerTemplate),ct);
        }
        catch(Exception ex){logger.LogError(ex,"Appointment {AppointmentId} was saved but SMS delivery failed.",appointment.Id);}
    }
}
