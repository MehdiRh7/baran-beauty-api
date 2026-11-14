# Baran Beauty API

Backend سامانه سالن زیبایی باران با ASP.NET Core روی .NET 10 و معماری Clean.

## ساختار

- `BaranBeauty.Domain`: موجودیت‌ها، قوانین دامنه و enumها؛ بدون وابستگی به لایه‌های دیگر
- `BaranBeauty.Application`: قراردادها، DTOها و use caseهای مدیریت محتوا و رزرو
- `BaranBeauty.Infrastructure`: EF Core، SQLite، repositoryها، هش رمز عبور و ذخیره فایل
- `BaranBeauty.Api`: Controllerها، JWT، CORS و composition root
- `BaranBeauty.Application.Tests`: تست قوانین و use caseها

## اجرا

```powershell
dotnet restore
dotnet run --project src/BaranBeauty.Api --urls http://localhost:5077
```

دیتابیس SQLite و داده‌های اولیه در اولین اجرا ساخته می‌شوند.

## حساب مدیر اولیه

- Username: `admin`
- Password: `Baran@1405`

کلید JWT و رمز اولیه را پیش از استقرار production تغییر دهید.

## Endpointهای اصلی

- `POST /api/auth/login`
- `GET/POST/PUT/DELETE /api/services`
- `POST /api/appointments`
- `GET/PATCH /api/appointments` (مدیر)
- `GET/POST/DELETE /api/appointments/blocked-dates`
- `/api/content/staff`
- `/api/content/certificates`
- `/api/content/media`
- `/api/content/salon`

عملیات مدیریتی به Bearer JWT نیاز دارند.
