using Domain.UnitTests.Base;
using Xunit;
using System;
using Domain.Entities;
using FluentAssertions;

namespace Domain.UnitTests
{
    public class BookingTicketTests : BaseDomainTest
    {
        private const long SampleRentalId = 100;
        private const long SampleUserTicketId = 200;
        private const decimal SamplePlanPrice = 5000.00m;
        private const string SampleVehicleType = "Xe đạp điện";

        // Test Case 1: Tạo mới BookingTicket thành công
        [Fact]
        public void BookingTicket_Creation_ShouldSetInitialValuesAndZeroMinutes()
        {
            // Act
            var bookingTicket = Factory.CreateBookingTicket(
                rentalId: SampleRentalId,
                userTicketId: SampleUserTicketId,
                planPrice: SamplePlanPrice,
                vehicleType: SampleVehicleType,
                appliedAt: Now
            );

            // Assert
            bookingTicket.ShouldBeApplied(SampleRentalId, SampleUserTicketId, 0);
            bookingTicket.PlanPrice.Should().Be(SamplePlanPrice);
        }

        // Test Case 2: Cập nhật thời gian sử dụng thành công
        [Fact]
        public void UpdateUsedMinutes_ShouldSetMinutes_WhenPositive()
        {
            // Arrange
            var bookingTicket = Factory.CreateBookingTicket(
                rentalId: SampleRentalId,
                userTicketId: SampleUserTicketId
            );
            var minutesUsed = 45;

            // Act
            bookingTicket.UpdateUsedMinutes(minutesUsed);

            // Assert
            bookingTicket.UsedMinutes.Should().Be(minutesUsed);
        }

        // Test Case 3: Cập nhật thời gian sử dụng phải ném lỗi khi là số âm
        [Fact]
        public void UpdateUsedMinutes_ShouldThrowException_WhenNegative()
        {
            // Arrange
            var bookingTicket = Factory.CreateBookingTicket(
                rentalId: SampleRentalId,
                userTicketId: SampleUserTicketId
            );
            var initialMinutes = bookingTicket.UsedMinutes; // 0

            // Act & Assert
            ExpectException<ArgumentException>(
                () => bookingTicket.UpdateUsedMinutes(-10),
                "UsedMinutes cannot be negative. (Parameter 'minutes')"
            );

            // Đảm bảo giá trị không thay đổi
            bookingTicket.UsedMinutes.Should().Be(initialMinutes);
        }

        // Test Case 4: Constructor phải ném lỗi nếu RentalId không hợp lệ (Dựa trên Logic giả định ở bước 1)
        [Fact]
        public void BookingTicket_Creation_ShouldThrowException_WhenRentalIdIsInvalid()
        {
            // Act & Assert
            ExpectException<ArgumentException>(
                () => Factory.CreateBookingTicket(rentalId: 0, userTicketId: 10),

                // CHỈNH SỬA: Thêm tên tham số 'rentalId' vào thông báo lỗi
                "RentalId and UserTicketId must be positive. (Parameter 'rentalId')"
            );
        }
    }
}