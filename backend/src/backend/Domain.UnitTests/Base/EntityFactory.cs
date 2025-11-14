using Domain.Entities; // Giả sử bạn có tất cả các class này
using Domain.Enums;
using System;

namespace Domain.UnitTests.Base
{
    public class EntityFactory
    {
        private readonly DateTimeOffset _now;

        public EntityFactory(DateTimeOffset now)
        {
            _now = now;
        }

        //// === User (AspNetUsers) ===
        //// Dựa trên UserId = 3 (hatrung03022003@gmail.com)
        //public AspNetUser CreateUser(
        //    long id = 3,
        //    string email = "hatrung03022003@gmail.com",
        //    string phone = "0944362986")
        //{
        //    return new AspNetUser
        //    {
        //        Id = id,
        //        UserName = email,
        //        NormalizedUserName = email.ToUpper(),
        //        Email = email,
        //        NormalizedEmail = email.ToUpper(),
        //        PhoneNumber = phone,
        //        EmailConfirmed = false,
        //        LockoutEnabled = true,
        //        CreatedDate = _now
        //    };
        //}

        //// === UserProfile ===
        //// Dựa trên UserProfile Id = 1 (của UserId = 3)
        //public UserProfile CreateUserProfile(
        //    long userId = 3,
        //    string fullName = "Hà Hữu Trung",
        //    string numberCard = "025203000898")
        //{
        //    return new UserProfile
        //    {
        //        Id = 1, // Id của UserProfile, không phải UserId
        //        UserId = userId,
        //        FullName = fullName,
        //        NumberCard = numberCard,
        //        Gender = "Nam",
        //        ProvinceName = "Hà Nội",
        //        CreatedAt = _now,
        //        UpdatedAt = _now
        //    };
        //}

        //// === VehicleCategory (CategoriesVehicle) ===
        //// Dựa trên CategoryId = 2 (Xe đạp điện)
        //public CategoriesVehicle CreateVehicleCategory(
        //    long id = 2,
        //    string name = "Xe đạp điện")
        //{
        //    return new CategoriesVehicle
        //    {
        //        Id = id,
        //        Name = name,
        //        Description = "Xe đạp có pin và động cơ điện hỗ trợ di chuyển.",
        //        IsActive = true
        //    };
        //}

        //// === Station ===
        //// Dựa trên StationId = 1 (Trạm Đại học FPT)
        //public Station CreateStation(
        //    string name = "Trạm Đại học FPT",
        //    decimal lat = 21.013m,
        //    decimal lng = 105.525m)
        //{
        //    return new Station
        //    {
        //        Id = 1,
        //        Name = name,
        //        Lat = lat,
        //        Lng = lng,
        //        IsActive = true,
        //        Capacity = 25
        //    };
        //}

        //// === Vehicle ===
        //// Dựa trên VehicleId = 10 (EBIKE-104)
        //public Vehicle CreateVehicle(
        //    string status = "Available",
        //    string bikeCode = "EBIKE-104",
        //    long? stationId = 7,
        //    long? categoryId = 2,
        //    int batteryLevel = 70,
        //    bool chargingStatus = false)
        //{
        //    return new Vehicle
        //    {
        //        Id = 10,
        //        Status = status,
        //        BikeCode = bikeCode,
        //        StationId = stationId,
        //        CategoryId = categoryId,
        //        BatteryLevel = batteryLevel,
        //        ChargingStatus = chargingStatus,
        //        CreatedAt = _now
        //    };
        //}

        //// === Rental ===
        //// Dựa trên RentalId = 9 (đang "Ongoing")
        //public Rental CreateRental(long? userId = null, long? vehicleId = null, long? startStationId = null)
        //{
        //    return new Rental
        //    {
        //        Id = 9,
        //        UserId = userId ?? 3,
        //        VehicleId = vehicleId ?? 9,
        //        StartStationId = startStationId ?? 5,
        //        StartTime = _now,
        //        CreatedAt = _now,
        //        Status = "Ongoing"
        //    };
        //}

        //// === TicketPlan ===
        //// Dựa trên PlanId = 1 (Vé lượt)
        //public TicketPlan CreateTicketPlan(
        //    long id = 1,
        //    string code = "RIDE",
        //    string name = "Vé lượt",
        //    string type = "Lu?t") // Chú ý: "Lu?t" có thể là lỗi encoding, nên là "Lượt"
        //{
        //    return new TicketPlan
        //    {
        //        Id = id,
        //        Code = code,
        //        Name = name,
        //        Type = type,
        //        IsActive = true,
        //        CreatedAt = _now
        //    };
        //}

        //// === TicketPlanPrice ===
        //// Dựa trên PriceId = 1 (Vé lượt - Bike)
        //public TicketPlanPrice CreateTicketPlanPrice(
        //    long id = 1,
        //    long planId = 1,
        //    string vehicleType = "Bike",
        //    decimal price = 5000.00m)
        //{
        //    return new TicketPlanPrice
        //    {
        //        Id = id,
        //        PlanId = planId,
        //        VehicleType = vehicleType,
        //        Price = price,
        //        DurationLimitMinutes = 30,
        //        OverageFeePer15Min = 2000.00m,
        //        IsActive = true,
        //        ActivationMode = 0, // 0 = Immediate (Giả định)
        //        ValidityDays = 1
        //    };
        //}

        //// === UserTicket ===
        //// Dựa trên UserTicketId = 1 (vé "Ready" của UserId = 3)
        //public UserTicket CreateUserTicket(
        //    long id = 1,
        //    long userId = 3,
        //    long planPriceId = 1,
        //    string status = "Ready")
        //{
        //    return new UserTicket
        //    {
        //        Id = id,
        //        UserId = userId,
        //        PlanPriceId = planPriceId,
        //        SerialCode = "TICKET-0001",
        //        PurchasedPrice = 5000.00m,
        //        Status = status, // "Ready", "Active", "Expired"
        //        RemainingMinutes = 30,
        //        RemainingRides = 1,
        //        CreatedAt = _now
        //    };
        //}

        //// === BookingTicket ===
        //// Dựa trên BookingTicketId = 5
        //public BookingTicket CreateBookingTicket(
        //    long id = 5,
        //    long rentalId = 8,
        //    long userTicketId = 2)
        //{
        //    return new BookingTicket
        //    {
        //        Id = id,
        //        RentalId = rentalId,
        //        UserTicketId = userTicketId,
        //        PlanPrice = 0.00m,
        //        VehicleType = "Xe đạp điện", // Lấy từ SQL
        //        UsedMinutes = 1,
        //        AppliedAt = _now
        //    };
        //}

        //// === Contact ===
        //// Dựa trên ContactId = 5 (Trạng thái "Open")
        //public Contact CreateContact(
        //    long id = 5,
        //    string email = "trunghhhe176079@fpt.edu.vn",
        //    string status = ContactStatus.Open)
        //{
        //    return new Contact
        //    {
        //        Id = id,
        //        Email = email,
        //        PhoneNumber = "0944362986",
        //        Title = "Cần giải đáp thắc mắc",
        //        Content = "Vài thắc mắc",
        //        Status = status, // "Open", "Replied"
        //        CreatedAt = _now,
        //        IsReplySent = false
        //    };
        //}

        //// === UserDevice ===
        //// Dựa trên UserDeviceId = 2 (của UserId = 3)
        //public UserDevice CreateUserDevice(
        //    long id = 2,
        //    long userId = 3,
        //    string deviceId = "3FA85F64-5717-4562-B3FC-2C963F66AFA6")
        //{
        //    return new UserDevice
        //    {
        //        Id = id,
        //        UserId = userId,
        //        DeviceId = deviceId,
        //        Platform = "ANDROID",
        //        PushToken = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        //        LastLoginAt = _now,
        //        CreatedAt = _now,
        //        UpdatedAt = _now
        //    };
        //}
    }
}