using Domain.UnitTests.Base;
using Xunit;
using System;
using Domain.Entities;

namespace Domain.UnitTests
{
    public class TagNewTests : BaseDomainTest
    {
        private const long SampleNewId = 5;
        private const long SampleTagId = 8;

        // Test Case 1: Tạo TagNew thành công với ID hợp lệ
        [Fact]
        public void CreateTagNew_ShouldSetNewIdAndTagId_WhenValid()
        {
            // Arrange
            // Giả định chúng ta dùng constructor hoặc Factory để tạo
            var tagNew = Factory.CreateTagNew(SampleNewId, SampleTagId);

            // Assert
            tagNew.ShouldBeLinkedTo(SampleNewId, SampleTagId);
        }

        // Test Case 2: Tạo TagNew phải ném ra lỗi nếu NewId không hợp lệ (Dựa trên Logic giả định ở bước 1)
        [Fact]
        public void CreateTagNew_ShouldThrowException_WhenNewIdIsZero()
        {
            // Arrange & Act & Assert
            ExpectException<ArgumentException>(
                () => Factory.CreateTagNew(newId: 0, tagId: SampleTagId),
                "NewId and TagId must be positive. (Parameter 'newId')"
            // Message tùy thuộc vào cách bạn implement constructor
            );
        }

        // Test Case 3: Tạo TagNew phải ném ra lỗi nếu TagId không hợp lệ (Dựa trên Logic giả định ở bước 1)
        [Fact]
        public void CreateTagNew_ShouldThrowException_WhenTagIdIsNegative()
        {
            // Arrange & Act & Assert
            ExpectException<ArgumentException>(
                () => Factory.CreateTagNew(newId: SampleNewId, tagId: -1),
                "NewId and TagId must be positive. (Parameter 'tagId')"
            // Message tùy thuộc vào cách bạn implement constructor
            );
        }
    }
}