using Domain.UnitTests.Base;
using Xunit;
using System;
using Domain.Entities;
using FluentAssertions;

namespace Domain.UnitTests
{
    public class ContactTests : BaseDomainTest
    {
        private const long AdminUserId = 99;
        private const string InitialTitle = "Vấn đề về thanh toán";
        private const string InitialContent = "Tôi đã bị trừ tiền hai lần.";
        private const string ReplyText = "Chúng tôi đã kiểm tra và hoàn tiền thành công.";

        // Test Case 1: Tạo mới Contact phải có trạng thái Open
        [Fact]
        public void Contact_Creation_ShouldBeOpenAndSetInitialValues()
        {
            // Act
            var contact = Factory.CreateContact(
                title: InitialTitle,
                content: InitialContent,
                createdAt: Now,
                status: "Open" // Factory mặc định là Open
            );

            // Assert
            contact.Title.Should().Be(InitialTitle);
            contact.Content.Should().Be(InitialContent);
            contact.ShouldBeOpen();
        }

        // Test Case 2: SubmitReply thành công, chuyển sang trạng thái Replied
        [Fact]
        public void SubmitReply_ShouldSetStatusToRepliedAndRecordAdminInfo()
        {
            // Arrange
            var contact = Factory.CreateContact(status: "Open");
            var replyTime = Now.AddMinutes(5);

            // Act
            contact.SubmitReply(AdminUserId, ReplyText, replyTime);

            // Assert
            contact.ShouldBeReplied(AdminUserId, ReplyText, replyTime);
        }

        // Test Case 3: MarkAsSent thành công, sau khi đã có Reply
        [Fact]
        public void MarkAsSent_ShouldSetIsReplySentToTrue_WhenReplied()
        {
            // Arrange
            // Cần giả lập Contact đã được Reply (vì IsReplySent mặc định là false)
            var contact = Factory.CreateContact(status: "Replied");

            // Act
            contact.MarkAsSent();

            // Assert
            contact.IsReplySent.Should().BeTrue();
            contact.Status.Should().Be("Replied");
        }

        // Test Case 4: Đóng liên hệ thành công
        [Fact]
        public void Close_ShouldSetStatusToClosedAndRecordClosedAt()
        {
            // Arrange
            var contact = Factory.CreateContact(status: "Replied");
            var closedTime = Now.AddHours(1);

            // Act
            contact.Close(closedTime);

            // Assert
            contact.ShouldBeClosed(closedTime);
        }

        // Test Case 5: Không cho phép Reply khi đã Closed
        [Fact]
        public void SubmitReply_ShouldThrowException_WhenContactIsClosed()
        {
            // Arrange
            var contact = Factory.CreateContact(status: "Closed");

            // Act & Assert
            ExpectException<InvalidOperationException>(
                () => contact.SubmitReply(AdminUserId, ReplyText, Now),
                "Cannot reply to a closed contact."
            );
            contact.Status.Should().Be("Closed");
        }
    }
}