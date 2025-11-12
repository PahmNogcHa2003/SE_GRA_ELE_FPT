using Application.Services.Location;
using Application.DTOs.Location;
using Application.Interfaces.Location;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Features.Location
{
    public class GetProvincesQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnMappedProvinces_WhenRepositoryReturnsData()
        {
            // Arrange
            var fakeProvinces = new List<Province>
            {
                new() { Code = "01", Name = "Hà Nội" },
                new() { Code = "02", Name = "Hồ Chí Minh" }
            };

            var mockRepo = new Mock<ILocationRepository>();
            mockRepo.Setup(r => r.GetProvincesAsync())
                    .ReturnsAsync(fakeProvinces);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Province, LocationDTO>();
            });
            var mapper = mapperConfig.CreateMapper();

            var handler = new GetProvincesQueryHandler(mockRepo.Object, mapper);

            // Act
            var result = await handler.Handle(new GetProvincesQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "Hà Nội");
            result.Should().Contain(p => p.Name == "Hồ Chí Minh");

            mockRepo.Verify(r => r.GetProvincesAsync(), Times.Once);
        }
    }
}
