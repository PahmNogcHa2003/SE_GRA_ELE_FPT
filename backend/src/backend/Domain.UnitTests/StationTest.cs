using Domain.UnitTests.Base;
using Xunit;
using System;
using FluentAssertions;

namespace Domain.UnitTests
{
    public class StationTests : BaseDomainTest
    {
        // Test Case 1: Deactivate (Đóng trạm)
        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var station = Factory.CreateStation(isActive: true);

            // Act
            station.Deactivate();

            // Assert
            station.ShouldBeInactive();
        }

        // Test Case 2: Activate (Mở lại trạm)
        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var station = Factory.CreateStation(isActive: false);

            // Act
            station.Activate();

            // Assert
            station.ShouldBeActive();
        }

        // Test Case 3: UpdateCapacity (Cập nhật sức chứa thành công)
        [Fact]
        public void UpdateCapacity_ShouldSetNewCapacity_WhenValid()
        {
            // Arrange
            var station = Factory.CreateStation(capacity: 25);
            var newCapacity = 40;

            // Act
            station.UpdateCapacity(newCapacity);

            // Assert
            station.ShouldHaveCapacity(newCapacity);
        }

        // Test Case 4: UpdateCapacity (Ném ngoại lệ khi sức chứa không hợp lệ)
        [Fact]
        public void UpdateCapacity_ShouldThrowException_WhenCapacityIsZeroOrNegative()
        {
            // Arrange
            var station = Factory.CreateStation(capacity: 25);
            var invalidCapacity = 0;

            // Act & Assert
            ExpectException<ArgumentException>(
                () => station.UpdateCapacity(invalidCapacity),
                $"Capacity must be greater than zero. (Parameter 'newCapacity')"
            );

            // Đảm bảo sức chứa không thay đổi
            station.Capacity.Should().Be(25);
        }
    }
}