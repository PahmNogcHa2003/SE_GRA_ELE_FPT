using Domain.Entities;
using FluentAssertions;
using System;

namespace Domain.UnitTests.Base
{
    public static class CustomAssertions
    {
        // --- Rental Assertions ---
        public static void ShouldBeCompleted(this Rental rental, DateTimeOffset expectedEndTime, long expectedEndStation)
        {
            rental.Status.Should().Be("End");
            rental.EndTime.Should().Be(expectedEndTime);
            rental.EndStationId.Should().Be(expectedEndStation);
        }

        public static void ShouldBeOngoing(this Rental rental)
        {
            rental.Status.Should().Be("Ongoing");
            rental.EndTime.Should().BeNull();
        }

        // --- Payment Assertions ---
        public static void ShouldBeSuccessful(this Payment payment, string expectedGatewayTxnId, DateTimeOffset expectedPaidAt)
        {
            payment.Status.Should().Be("Success");
            payment.GatewayTxnId.Should().Be(expectedGatewayTxnId);
            payment.PaidAt.Should().Be(expectedPaidAt);
            payment.FailureReason.Should().BeNull();
        }

        public static void ShouldBeFailed(this Payment payment, string expectedReason)
        {
            payment.Status.Should().Be("Failed");
            payment.FailureReason.Should().Contain(expectedReason);
            payment.PaidAt.Should().BeNull();
            payment.GatewayTxnId.Should().BeNull();
        }

        public static void ShouldBePending(this Payment payment)
        {
            payment.Status.Should().Be("Pending");
            payment.PaidAt.Should().BeNull();
            payment.GatewayTxnId.Should().BeNull();
            payment.FailureReason.Should().BeNull();
        }

        // --- Station Assertions ---
        public static void ShouldBeActive(this Station station)
        {
            station.IsActive.Should().BeTrue();
        }

        public static void ShouldBeInactive(this Station station)
        {
            station.IsActive.Should().BeFalse();
        }

        public static void ShouldHaveCapacity(this Station station, int expectedCapacity)
        {
            station.Capacity.Should().Be(expectedCapacity);
        }

        // --- News Assertions ---
        public static void ShouldBeDraft(this News news)
        {
            news.Status.Should().Be("Draft");
            news.PublishedAt.Should().BeNull();
            news.ScheduledAt.Should().BeNull();
        }

        public static void ShouldBeScheduled(this News news, DateTimeOffset expectedScheduleTime, long expectedPublisherId)
        {
            news.Status.Should().Be("Scheduled");
            news.ScheduledAt.Should().Be(expectedScheduleTime);
            news.PublishedBy.Should().Be(expectedPublisherId);
            news.PublishedAt.Should().BeNull();
        }

        public static void ShouldBePublished(this News news, DateTimeOffset expectedPublishTime, long expectedPublisherId)
        {
            news.Status.Should().Be("Published");
            news.PublishedAt.Should().Be(expectedPublishTime);
            news.PublishedBy.Should().Be(expectedPublisherId);
            news.ScheduledAt.Should().BeNull();
        }

        // --- BookingTicket Assertions ---
        public static void ShouldBeApplied(this BookingTicket bookingTicket, long expectedRentalId, long expectedTicketId, int expectedUsedMinutes)
        {
            bookingTicket.RentalId.Should().Be(expectedRentalId);
            bookingTicket.UserTicketId.Should().Be(expectedTicketId);
            bookingTicket.AppliedAt.Should().NotBeNull();
            bookingTicket.UsedMinutes.Should().Be(expectedUsedMinutes);
        }

        // --- Tag Assertions ---
        public static void ShouldHaveName(this Tag tag, string expectedName)
        {
            tag.Name.Should().Be(expectedName);
        }

        // --- TagNew Assertions ---
        public static void ShouldBeLinkedTo(this TagNew tagNew, long expectedNewId, long expectedTagId)
        {
            tagNew.NewId.Should().Be(expectedNewId);
            tagNew.TagId.Should().Be(expectedTagId);
        }

        // --- CategoriesVehicle Assertions ---
        public static void ShouldBeActive(this CategoriesVehicle category)
        {
            category.IsActive.Should().BeTrue();
        }

        public static void ShouldBeInactive(this CategoriesVehicle category)
        {
            category.IsActive.Should().BeFalse();
        }

        public static void ShouldHaveName(this CategoriesVehicle category, string expectedName)
        {
            category.Name.Should().Be(expectedName);
        }

        // --- Contact Assertions ---
        public static void ShouldBeOpen(this Contact contact)
        {
            contact.Status.Should().Be("Open");
            contact.ReplyById.Should().BeNull();
            contact.ReplyContent.Should().BeNull();
            contact.IsReplySent.Should().BeFalse();
            contact.ClosedAt.Should().BeNull();
        }

        public static void ShouldBeReplied(this Contact contact, long expectedReplyId, string expectedContent, DateTimeOffset expectedReplyAt)
        {
            contact.Status.Should().Be("Replied");
            contact.ReplyById.Should().Be(expectedReplyId);
            contact.ReplyContent.Should().Contain(expectedContent);
            contact.ReplyAt.Should().Be(expectedReplyAt);
            contact.IsReplySent.Should().BeFalse();
        }

        public static void ShouldBeClosed(this Contact contact, DateTimeOffset expectedClosedAt)
        {
            contact.Status.Should().Be("Closed");
            contact.ClosedAt.Should().Be(expectedClosedAt);
        }

        // --- TicketPlanPrice Assertions ---
        public static void ShouldBeActive(this TicketPlanPrice pricePlan)
        {
            pricePlan.IsActive.Should().BeTrue();
        }

        public static void ShouldBeInactive(this TicketPlanPrice pricePlan)
        {
            pricePlan.IsActive.Should().BeFalse();
        }

        public static void ShouldHavePricing(this TicketPlanPrice pricePlan, decimal expectedPrice, decimal? expectedOverageFee)
        {
            pricePlan.Price.Should().Be(expectedPrice);
            pricePlan.OverageFeePer15Min.Should().Be(expectedOverageFee);
        }

        public static void ShouldHaveDurationRules(this TicketPlanPrice pricePlan, int? durationLimitMinutes, int? validityDays, int? activationWindowDays)
        {
            pricePlan.DurationLimitMinutes.Should().Be(durationLimitMinutes);
            pricePlan.ValidityDays.Should().Be(validityDays);
            pricePlan.ActivationWindowDays.Should().Be(activationWindowDays);
        }
    }
}