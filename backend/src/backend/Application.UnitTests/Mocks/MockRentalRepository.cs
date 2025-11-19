//using Application.Interfaces.User.Repository;
//using Domain.Entities;
//using Domain.Enums;
//using Moq;
//using System.Collections.Generic;

//namespace Application.UnitTests.Mocks
//{
//    public static class MockRentalRepository
//    {
//        // Hàm này tạo và "dạy" mock repo các hành vi cơ bản
//        public static Mock<IRentalsRepository> GetMockRentalRepository()
//        {
//            var mockRepo = new Mock<IRentalsRepository>();

//            // DỮ LIỆU GIẢ
//            var bookingTicket = new BookingTicket { Id = 1, RentalId = 1, UserTicketId = 10 };
//            var ongoingRental = new Rental
//            {
//                Id = 1,
//                UserId = 1, // User mặc định
//                VehicleId = 1,
//                Status = RentalStatus.Success,
//                StartTime = System.DateTimeOffset.UtcNow.AddMinutes(-30),
//                BookingTickets = new List<BookingTicket> { bookingTicket }
//            };

//            // "DẠY" MOCK
//            // 1. Dạy mock trả về cuốc xe 'ongoingRental' khi được gọi đúng user 1, rental 1
//            mockRepo.Setup(r => r.GetRentalForUserAsync(1, 1))
//                    .ReturnsAsync(ongoingRental);

//            // 2. Dạy mock trả về null nếu tìm không thấy
//            mockRepo.Setup(r => r.GetRentalForUserAsync(It.IsAny<long>(), 999))
//                    .ReturnsAsync((Rental)null);

//            // 3. Dạy mock hàm AddAsync (không cần làm gì phức tạp)
//            mockRepo.Setup(r => r.AddAsync(It.IsAny<Rental>()))
//                    .Returns(Task.CompletedTask);

//            return mockRepo;
//        }
//    }
//}