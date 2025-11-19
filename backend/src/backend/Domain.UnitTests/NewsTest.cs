using Domain.UnitTests.Base;
using Xunit;
using System;
using FluentAssertions;
using Domain.Entities;

namespace Domain.UnitTests
{
    public class NewsTests : BaseDomainTest
    {
        private const long AdminUserId = 10;

        // Test Case 1: Công bố bài viết từ trạng thái Draft
        [Fact]
        public void Publish_ShouldSetStatusToPublished_WhenDraft()
        {
            // Arrange
            var news = Factory.CreateNews(status: News.StatusDraft, userId: DefaultUserId);
            var publishTime = Now.AddHours(1);

            // Act
            news.Publish(publishTime, AdminUserId);

            // Assert
            news.ShouldBePublished(publishTime, AdminUserId);
        }

        // Test Case 2: Đặt lịch công bố bài viết
        [Fact]
        public void Schedule_ShouldSetStatusToScheduled()
        {
            // Arrange
            var news = Factory.CreateNews(status: News.StatusDraft, userId: DefaultUserId);
            var scheduleTime = Now.AddDays(5);

            // Act
            news.Schedule(scheduleTime, AdminUserId);

            // Assert
            news.ShouldBeScheduled(scheduleTime, AdminUserId);
        }

        // Test Case 3: Chuyển Scheduled sang Published
        [Fact]
        public void Publish_ShouldOverrideSchedule_WhenScheduled()
        {
            // Arrange
            var news = Factory.CreateNews(status: News.StatusScheduled, userId: DefaultUserId);
            var publishTime = Now.AddHours(2);

            // Act
            news.Publish(publishTime, AdminUserId);

            // Assert
            news.ShouldBePublished(publishTime, AdminUserId);
        }

        // Test Case 4: Không cho phép Draft bài viết đã Published
        [Fact]
        public void Draft_ShouldThrowException_WhenPublished()
        {
            // Arrange
            var news = Factory.CreateNews(status: News.StatusPublished, userId: DefaultUserId);

            // Act & Assert
            ExpectException<InvalidOperationException>(
                () => news.Draft(),
                "Cannot draft a published news article."
            );

            // Đảm bảo trạng thái không thay đổi
            news.Status.Should().Be(News.StatusPublished);
        }

        // Test Case 5: Đặt lịch phải sau thời gian tạo
        [Fact]
        public void Schedule_ShouldThrowException_WhenScheduleTimeBeforeCreation()
        {
            // Arrange
            var news = Factory.CreateNews(status: News.StatusDraft, userId: DefaultUserId);
            var invalidScheduleTime = Now.AddHours(-1);

            // Act & Assert
            ExpectException<ArgumentException>(
                () => news.Schedule(invalidScheduleTime, AdminUserId),
                $"Schedule time must be after creation time. (Parameter 'scheduleTime')"
            );
        }
    }
}