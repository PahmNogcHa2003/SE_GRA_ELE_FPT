using Domain.UnitTests.Base;
using Xunit;
using System;
using Domain.Entities;
using FluentAssertions;

namespace Domain.UnitTests
{
    public class CategoriesVehicleTests : BaseDomainTest
    {
        private const string InitialName = "Xe Máy Điện";

        // Test Case 1: Tạo Category thành công
        [Fact]
        public void CreateCategory_ShouldSetValidNameAndBeActiveByDefault()
        {
            // Act
            var category = Factory.CreateVehicleCategory(name: InitialName);

            // Assert
            category.ShouldHaveName(InitialName);
            category.ShouldBeActive();
        }

        // Test Case 2: Kích hoạt Category
        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var category = Factory.CreateVehicleCategory(name: InitialName);
            category.IsActive = false; // Đặt trạng thái ban đầu là false

            // Act
            category.Activate();

            // Assert
            category.ShouldBeActive();
        }

        // Test Case 3: Vô hiệu hóa Category
        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var category = Factory.CreateVehicleCategory(name: InitialName); // Mặc định là Active

            // Act
            category.Deactivate();

            // Assert
            category.ShouldBeInactive();
        }

        // Test Case 4: Cập nhật tên thành công
        [Fact]
        public void UpdateName_ShouldChangeCategoryName_WhenValid()
        {
            // Arrange
            var category = Factory.CreateVehicleCategory(name: "OldName");
            var newName = "Xe Đạp Thường";

            // Act
            category.UpdateName(newName);

            // Assert
            category.ShouldHaveName(newName);
        }

        // Test Case 5: UpdateName phải ném lỗi khi tên thẻ là trống
        [Fact]
        public void UpdateName_ShouldThrowException_WhenNameIsWhitespace()
        {
            // Arrange
            var category = Factory.CreateVehicleCategory(name: InitialName);

            // Act & Assert
            ExpectException<ArgumentException>(
                () => category.UpdateName(" "),
                "Category Name cannot be empty. (Parameter 'newName')"
            );

            // Đảm bảo tên không thay đổi
            category.ShouldHaveName(InitialName);
        }
    }
}