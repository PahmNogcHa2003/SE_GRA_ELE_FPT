//using Application.Interfaces.User.Repository;
//using Domain.Entities;
//using Domain.Enums;
//using Moq;
//using MockQueryable.Moq;
//namespace Application.UnitTests.Mocks
//{
//    public static class MockRentalRepository
//    {
//        public static Mock<IRentalsRepository> GetMock()
//        {
//            var mockRepo = new Mock<IRentalsRepository>();

//            // --- 1. Dữ liệu giả lập (phải là List) ---
//            var ongoingRental = new Rental
//            {
//                Id = 1,
//                UserId = 1,
//                VehicleId = 1,
//                StartStationId = 1,

//                // SỬA LỖI LOGIC: Phải khớp với chuỗi "Ongoing" trong Service
//                Status = "Ongoing", // <-- Sửa từ RentalStatus.Active

//                StartTime = DateTimeOffset.UtcNow.AddMinutes(-30),
//                BookingTickets = new List<BookingTicket>
//                {
//                    new BookingTicket
//                    {
//                        Id = 1,
//                        RentalId = 1,
//                        UserTicketId = 10,
//                        AppliedAt = DateTimeOffset.UtcNow
//                    }
//                }
//            };

//            var endedRental = new Rental
//            {
//                Id = 2,
//                UserId = 1,
//                VehicleId = 2,
//                StartStationId = 1,
//                EndStationId = 2,
//                Status = RentalStatus.End, // Giữ nguyên, vì nó không phải "Ongoing"
//                StartTime = DateTimeOffset.UtcNow.AddHours(-2),
//                EndTime = DateTimeOffset.UtcNow.AddHours(-1),
//                BookingTickets = new List<BookingTicket>()
//            };

//            // Tạo một List chứa dữ liệu
//            var rentals = new List<Rental>
//            {
//                ongoingRental,
//                endedRental
//            };

//            // --- 2. Setup các phương thức ---

//            // Biến List<Rental> thành một IQueryable "giả"
//            var mockRentalsQueryable = rentals.AsQueryable().BuildMock();

//            // Khi service gọi _rentalRepo.Query(), nó sẽ trả về cái mock IQueryable này
//            mockRepo.Setup(r => r.Query()).Returns(mockRentalsQueryable.Object);

//            // Setup cho AddAsync
//            mockRepo.Setup(r => r.AddAsync(It.IsAny<Rental>(), It.IsAny<CancellationToken>()))
//                    .Returns(Task.CompletedTask);

//            // Setup cho Update
//            mockRepo.Setup(r => r.Update(It.IsAny<Rental>()));

//            // Setup cho GetByIdAsync (vẫn nên giữ lại)
//            mockRepo.Setup(r => r.GetByIdAsync(1L, It.IsAny<CancellationToken>()))
//                    .ReturnsAsync(ongoingRental);
//            mockRepo.Setup(r => r.GetByIdAsync(2L, It.IsAny<CancellationToken>()))
//                    .ReturnsAsync(endedRental);

//            return mockRepo;
//        }
//    }
//}