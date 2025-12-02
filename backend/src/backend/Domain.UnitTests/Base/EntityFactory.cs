using Domain.Entities;
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

        // === User (AspNetUsers) ===
        public AspNetUser CreateUser(
            long id = 3,
            string email = "hatrung03022003@gmail.com",
            string phone = "0944362986")
        {
            return new AspNetUser
            {
                Id = id,
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                PhoneNumber = phone,
                EmailConfirmed = false,
                LockoutEnabled = true,
                CreatedDate = _now
            };
        }

        // === UserProfile ===
        public Domain.Entities.UserProfile CreateUserProfile(
            long userId = 3,
            string fullName = "Hà Hữu Trung",
            string numberCard = "025203000898")
        {
            return new Domain.Entities.UserProfile
            {
                Id = 1,
                UserId = userId,
                FullName = fullName,
                NumberCard = numberCard,
                Gender = "Nam",
                ProvinceName = "Hà Nội",
                CreatedAt = _now,
                UpdatedAt = _now
            };
        }

        // === VehicleCategory (CategoriesVehicle) ===
        public CategoriesVehicle CreateVehicleCategory(
            long id = 2,
            string name = "Xe đạp điện",
            string description = "Xe đạp có pin và động cơ điện hỗ trợ di chuyển.",
            bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name validation failed.", nameof(name));
            }

            // Giả định CategoriesVehicle có constructor CategoriesVehicle(string name)
            var category = new CategoriesVehicle(name)
            {
                Id = id,
                Description = description,
                IsActive = isActive
            };

            return category;
        }

        // === Station ===
        public Station CreateStation(
            string name = "Trạm Đại học FPT",
            decimal lat = 21.013m,
            decimal lng = 105.525m,
            bool isActive = true,
            int capacity = 25)
        {
            return new Station
            {
                Id = 1,
                Name = name,
                Lat = lat,
                Lng = lng,
                IsActive = isActive,
                Capacity = capacity
            };
        }

        // === Vehicle ===
        public Vehicle CreateVehicle(
            string status = "Available",
            string bikeCode = "EBIKE-104",
            long? stationId = 7,
            long? categoryId = 2,
            int batteryLevel = 70,
            bool chargingStatus = false)
        {
            return new Vehicle
            {
                Id = 10,
                Status = status,
                BikeCode = bikeCode,
                StationId = stationId,
                CategoryId = categoryId,
                BatteryLevel = batteryLevel,
                ChargingStatus = chargingStatus,
                CreatedAt = _now
            };
        }

        // === Rental ===
        public Rental CreateRental(long? userId = null, long? vehicleId = null, long? startStationId = null)
        {
            return new Rental
            {
                Id = 9,
                UserId = userId ?? 3,
                VehicleId = vehicleId ?? 9,
                StartStationId = startStationId ?? 5,
                StartTime = _now,
                CreatedAt = _now,
                Status = "Ongoing"
            };
        }

        // === TicketPlan ===
        public TicketPlan CreateTicketPlan(
            long id = 1,
            string code = "RIDE",
            string name = "Vé lượt",
            string type = "Lu?t")
        {
            return new TicketPlan
            {
                Id = id,
                Code = code,
                Name = name,
                Type = type,
                IsActive = true,
                CreatedAt = _now
            };
        }

        // === UserTicket ===
        public UserTicket CreateUserTicket(
            long id = 1,
            long userId = 3,
            long planPriceId = 1,
            string status = "Ready")
        {
            return new UserTicket
            {
                Id = id,
                UserId = userId,
                PlanPriceId = planPriceId,
                SerialCode = "TICKET-0001",
                PurchasedPrice = 5000.00m,
                Status = status,
                RemainingMinutes = 30,
                RemainingRides = 1,
                CreatedAt = _now
            };
        }

        // === BookingTicket (PHIÊN BẢN DUY NHẤT) ===
        public BookingTicket CreateBookingTicket(
            long id = 5,
            long rentalId = 8,
            long userTicketId = 2,
            decimal planPrice = 0.00m,
            string vehicleType = "Xe đạp điện",
            int usedMinutes = 0,
            DateTimeOffset? appliedAt = null)
        {
            if (rentalId <= 0 || userTicketId <= 0)
            {
                throw new ArgumentException("RentalId and UserTicketId must be positive.", rentalId <= 0 ? nameof(rentalId) : nameof(userTicketId));
            }

            var appliedTime = appliedAt ?? _now;

            // Gọi constructor BookingTicket(long, long, decimal, string, DateTimeOffset)
            var bookingTicket = new BookingTicket(
                rentalId,
                userTicketId,
                planPrice,
                vehicleType,
                appliedTime
            )
            {
                Id = id,
                UsedMinutes = usedMinutes
            };

            return bookingTicket;
        }

        // === Contact ===
        public Contact CreateContact(
            long id = 5,
            string email = "trunghhhe176079@fpt.edu.vn",
            string title = "Cần giải đáp thắc mắc",
            string content = "Vài thắc mắc",
            DateTimeOffset? createdAt = null,
            string status = ContactStatus.Open)
        {
            // Giả định Contact có constructor Contact(string title, string content, DateTimeOffset createdAt)
            var contact = new Contact(title, content, createdAt ?? _now)
            {
                Id = id,
                Email = email,
                PhoneNumber = "0944362986",
                IsReplySent = false,
            };

            // Gán trạng thái và các trường liên quan
            contact.Status = status;

            if (status == ContactStatus.Replied)
            {
                contact.ReplyById = 10;
                contact.ReplyContent = "Initial reply content.";
                contact.ReplyAt = createdAt ?? _now;
            }
            if (status == ContactStatus.Closed)
            {
                contact.ClosedAt = createdAt?.AddDays(1) ?? _now.AddDays(1);
            }

            return contact;
        }

        // === UserDevice ===
        public UserDevice CreateUserDevice(
            long id = 2,
            long userId = 3,
            string deviceId = "3FA85F64-5717-4562-B3FC-2C963F66AFA6")
        {
            return new UserDevice
            {
                Id = id,
                UserId = userId,
                DeviceId = deviceId,
                Platform = "ANDROID",
                PushToken = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                LastLoginAt = _now,
                CreatedAt = _now,
                UpdatedAt = _now
            };
        }

        // === Payment ===
        public Payment CreatePayment(
            long id = 1,
            long orderId = 1,
            string provider = "VNPAY",
            decimal amount = 100000.00m,
            string status = "Pending",
            string providerTxnRef = "REF-20251119-001")
        {
            return new Payment
            {
                Id = id,
                OrderId = orderId,
                Provider = provider,
                Amount = amount,
                Currency = "VND",
                Status = status,
                ProviderTxnRef = providerTxnRef,
                CreatedAt = _now
            };
        }

        // === News ===
        public News CreateNews(
            long id = 1,
            long userId = 3,
            string title = "Thông báo mới",
            string status = "Draft",
            DateTimeOffset? createdAt = null)
        {
            var created = createdAt ?? _now;
            var slug = title.ToLower().Replace(" ", "-") + $"-{id}";

            return new News
            {
                Id = id,
                UserId = userId,
                Title = title,
                Slug = slug,
                Status = status,
                CreatedAt = created
            };
        }

        // === TagNew ===
        public TagNew CreateTagNew(long newId = 5, long tagId = 8, long id = 1)
        {
            if (newId <= 0 || tagId <= 0)
            {
                string paramName = newId <= 0 ? "newId" : "tagId";
                throw new ArgumentException("NewId and TagId must be positive.", paramName);
            }

            // Giả định TagNew có constructor TagNew(long, long)
            var tagNew = new TagNew(newId, tagId)
            {
                Id = id
            };
            return tagNew;
        }

        // === Tag ===
        public Tag CreateTag(long id = 1, string name = "Xe đạp")
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Tag Name cannot be empty.", nameof(name));
            }

            // Giả định Tag có constructor Tag(string name)
            var tag = new Tag(name);
            tag.Id = id;

            return tag;
        }

        // === TicketPlanPrice ===
        public TicketPlanPrice CreateTicketPlanPrice(
            long id = 1,
            long planId = 1,
            string vehicleType = "Bike",
            decimal price = 5000.00m,
            PlanActivationMode activationMode = PlanActivationMode.IMMEDIATE, // <-- THÊM THAM SỐ NÀY
            int durationLimitMinutes = 30, // <-- ĐẶT GIÁ TRỊ MẶC ĐỊNH CHO CÁC RULE
            decimal overageFeePer15Min = 2000.00m, // <-- ĐẶT GIÁ TRỊ MẶC ĐỊNH CHO PHÍ VƯỢT GIỜ
            int validityDays = 1,
            int? activationWindowDays = null,
            bool isActive = true
        )
        {
            // Sử dụng constructor mới trong entity
            var pricePlan = new TicketPlanPrice(planId, price, activationMode)
            {
                Id = id,
                VehicleType = vehicleType,
                DurationLimitMinutes = durationLimitMinutes,
                OverageFeePer15Min = overageFeePer15Min,
                IsActive = isActive,
                ValidityDays = validityDays,
                ActivationWindowDays = activationWindowDays,
            };

            return pricePlan;
        }

    }
}