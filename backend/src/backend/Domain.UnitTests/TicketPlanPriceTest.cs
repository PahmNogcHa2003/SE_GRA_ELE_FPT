using Domain.UnitTests.Base;
using Xunit;
using System;
using Domain.Entities;
using FluentAssertions;

namespace Domain.UnitTests
{
    // Kiểm tra logic giá và quy tắc kích hoạt/thời hạn
    public class TicketPlanPriceTests : BaseDomainTest
    {
        private const long SamplePlanId = 1;
        private const decimal InitialPrice = 15000.00m;
        private const decimal InitialOverageFee = 5000.00m;

        // Test Case 1: Tạo mới thành công với chế độ IMMEDIATE
        [Fact]
        public void Creation_ShouldSucceed_WithValidRequiredFields()
        {
            // Act
            var pricePlan = Factory.CreateTicketPlanPrice(
                planId: SamplePlanId,
                price: InitialPrice,
                activationMode: PlanActivationMode.IMMEDIATE
            );

            // Assert
            pricePlan.PlanId.Should().Be(SamplePlanId);
            pricePlan.Price.Should().Be(InitialPrice);
            pricePlan.ActivationMode.Should().Be(PlanActivationMode.IMMEDIATE);
            pricePlan.IsActive.Should().BeTrue();
        }

        // Test Case 2: UpdatePriceAndFees thành công
        [Fact]
        public void UpdatePriceAndFees_ShouldUpdateValues_WhenValid()
        {
            // Arrange
            var pricePlan = Factory.CreateTicketPlanPrice(
                price: InitialPrice,
                overageFeePer15Min: InitialOverageFee
            );
            var newPrice = 20000.00m;
            var newOverageFee = 7500.00m;

            // Act
            pricePlan.UpdatePriceAndFees(newPrice, newOverageFee);

            // Assert
            pricePlan.Price.Should().Be(newPrice);
            pricePlan.OverageFeePer15Min.Should().Be(newOverageFee);
        }

        // Test Case 3: UpdatePriceAndFees ném lỗi khi Price là số âm
        [Fact]
        public void UpdatePriceAndFees_ShouldThrowException_WhenPriceIsZero()
        {
            // Arrange
            var pricePlan = Factory.CreateTicketPlanPrice(price: InitialPrice);

            // Act & Assert
            ExpectException<ArgumentException>(
                () => pricePlan.UpdatePriceAndFees(0, null),
                "Price must be greater than zero. (Parameter 'newPrice')"
            );
            pricePlan.Price.Should().Be(InitialPrice);
        }

        // Test Case 4: UpdateDurationRules thành công
        [Fact]
        public void UpdateDurationRules_ShouldUpdateValues_WhenValid()
        {
            // Arrange
            var pricePlan = Factory.CreateTicketPlanPrice();

            // Act
            pricePlan.UpdateDurationRules(
                durationLimitMinutes: 60,
                validityDays: 365,
                activationWindowDays: 90
            );

            // Assert
            pricePlan.DurationLimitMinutes.Should().Be(60);
            pricePlan.ValidityDays.Should().Be(365);
            pricePlan.ActivationWindowDays.Should().Be(90);
        }

        // Test Case 5: UpdateDurationRules ném lỗi khi ValidityDays không hợp lệ
        [Fact]
        public void UpdateDurationRules_ShouldThrowException_WhenValidityDaysIsInvalid()
        {
            // Arrange
            var pricePlan = Factory.CreateTicketPlanPrice();
            var initialValidity = pricePlan.ValidityDays; // 1 (từ Factory)

            // Act & Assert
            ExpectException<ArgumentException>(
                () => pricePlan.UpdateDurationRules(30, 0, null),
                "Validity days must be positive. (Parameter 'validityDays')"
            );
            pricePlan.ValidityDays.Should().Be(initialValidity);
        }
    }
}