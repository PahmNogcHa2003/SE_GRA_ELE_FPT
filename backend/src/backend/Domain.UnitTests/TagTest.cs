using Domain.UnitTests.Base;
using Xunit;
using System;
using Domain.Entities;
using FluentAssertions;

namespace Domain.UnitTests
{
    public class TagTests : BaseDomainTest
    {
        // Test Case 1: Tạo Tag thành công
        [Fact]
        public void CreateTag_ShouldSetValidName()
        {
            // Arrange
            var tagName = "ElectricBike";

            // Act
            // Giả định Factory gọi constructor/logic tạo hợp lệ
            var tag = Factory.CreateTag(name: tagName);

            // Assert
            tag.ShouldHaveName(tagName);
        }

        // Test Case 2: Cập nhật tên thẻ thành công
        [Fact]
        public void UpdateName_ShouldChangeTagName_WhenValid()
        {
            // Arrange
            var tag = Factory.CreateTag(name: "OldName");
            var newName = "NewCategory";

            // Act
            tag.UpdateName(newName);

            // Assert
            tag.ShouldHaveName(newName);
        }

        // Test Case 3: UpdateName phải ném lỗi khi tên thẻ là trống
        [Fact]
        public void UpdateName_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var tag = Factory.CreateTag(name: "ExistingTag");

            // Act & Assert
            ExpectException<ArgumentException>(
                () => tag.UpdateName(""),
                "Tag Name cannot be empty. (Parameter 'newName')"
            );

            // Đảm bảo tên thẻ không thay đổi
            tag.ShouldHaveName("ExistingTag");
        }

        // Test Case 4: UpdateName phải ném lỗi khi tên thẻ quá dài (trên 50 ký tự)
        [Fact]
        public void UpdateName_ShouldThrowException_WhenNameIsTooLong()
        {
            // Arrange
            var tag = Factory.CreateTag(name: "ExistingTag");
            var longName = new string('A', 51); // 51 ký tự

            // Act & Assert
            ExpectException<ArgumentException>(
                () => tag.UpdateName(longName),
                "Tag Name cannot exceed 50 characters. (Parameter 'newName')"
            );
        }
    }
}