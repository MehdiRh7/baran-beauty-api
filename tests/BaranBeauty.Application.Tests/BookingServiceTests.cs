using BaranBeauty.Application;
using BaranBeauty.Domain;
using NSubstitute;

namespace BaranBeauty.Application.Tests;

public sealed class BookingServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenDateIsBlocked_RejectsAppointment()
    {
        var appointments=Substitute.For<IRepository<Appointment>>();
        var services=Substitute.For<IRepository<BeautyService>>();
        var blocked=Substitute.For<IRepository<BlockedDate>>();
        var queries=Substitute.For<IAppointmentQueries>();
        var uow=Substitute.For<IUnitOfWork>();
        var service=new BeautyService("رنگ مو","توضیحات",100_000,60,"sparkles");
        services.GetAsync(service.Id,Arg.Any<CancellationToken>()).Returns(service);
        queries.IsBlockedAsync(Arg.Any<DateOnly>(),Arg.Any<CancellationToken>()).Returns(true);
        var sut=new BookingService(appointments,services,blocked,queries,uow);
        var request=new AppointmentRequest(service.Id,"کاربر","09120000000",DateTime.Now.AddDays(2),null);

        await Assert.ThrowsAsync<InvalidOperationException>(()=>sut.CreateAsync(request,CancellationToken.None));
        await appointments.DidNotReceive().AddAsync(Arg.Any<Appointment>(),Arg.Any<CancellationToken>());
    }

    [Fact]
    public void BeautyService_WithNegativePrice_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(()=>new BeautyService("خدمت","",-1,30,"star"));
}
