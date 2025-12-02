using Domain.UnitTests.Base;
using Xunit;
using System;
using FluentAssertions;

namespace Domain.UnitTests
{
    public class PaymentTests : BaseDomainTest
    {

        // Test Case 1: Đánh dấu thanh toán thành công
        [Fact]
        public void MarkAsPaid_ShouldSetStatusToSuccess_WhenPending()
        {
            // Arrange
            var payment = Factory.CreatePayment(status: "Pending");
            var paidAt = Now.AddSeconds(15);
            var gatewayTxnId = "VNPay_TXN_12345";

            // Act
            payment.MarkAsPaid(gatewayTxnId, paidAt);

            // Assert
            payment.ShouldBeSuccessful(gatewayTxnId, paidAt);
        }

        // Test Case 2: Đánh dấu thanh toán thất bại
        [Fact]
        public void MarkAsFailed_ShouldSetStatusToFailed()
        {
            // Arrange
            var payment = Factory.CreatePayment(status: "Pending");
            var reason = "Insufficient funds";

            // Act
            payment.MarkAsFailed(reason);

            // Assert
            payment.ShouldBeFailed(reason);
        }

        // Test Case 3: Không cho phép đánh dấu thành công nếu đã thất bại
        [Fact]
        public void MarkAsPaid_ShouldThrow_WhenStatusIsFailed()
        {
            // Arrange
            var payment = Factory.CreatePayment(status: "Failed");
            var paidAt = Now.AddSeconds(15);
            var gatewayTxnId = "VNPay_TXN_12345";

            // Act & Assert
            ExpectException<InvalidOperationException>(
                () => payment.MarkAsPaid(gatewayTxnId, paidAt),
                "Cannot mark a failed or cancelled payment as paid."
            );

            // Đảm bảo trạng thái không thay đổi
            payment.Status.Should().Be("Failed");
        }

        // Test Case 4: Không cho phép đánh dấu thất bại nếu đã thành công
        [Fact]
        public void MarkAsFailed_ShouldThrow_WhenStatusIsSuccess()
        {
            // Arrange
            var payment = Factory.CreatePayment(status: "Success");

            // Act & Assert
            ExpectException<InvalidOperationException>(
                () => payment.MarkAsFailed("System Error"),
                "Cannot mark a successfully paid payment as failed."
            );

            // Đảm bảo trạng thái không thay đổi
            payment.Status.Should().Be("Success");
        }
    }
}