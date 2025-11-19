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
    public class GetWardsByProvinceQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnMappedWards_WhenRepositoryReturnsData()
        {
            // Arrange
            var provinceCode = "01";
            var fakeWards = new List<Ward>
            {
                new() { Code = "001", Name = "Phường Tràng Tiền" },
                new() { Code = "002", Name = "Phường Hàng Bài" }
            };

            var mockRepo = new Mock<ILocationRepository>();
            mockRepo.Setup(r => r.GetWardsByProvinceAsync(provinceCode))
                    .ReturnsAsync(fakeWards);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Ward, LocationDTO>();
            });
            var mapper = mapperConfig.CreateMapper();

            var handler = new GetWardsByProvinceQueryHandler(mockRepo.Object, mapper);

            // Act
            var result = await handler.Handle(new GetWardsByProvinceQuery(provinceCode), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(w => w.Name == "Phường Tràng Tiền");
            result.Should().Contain(w => w.Name == "Phường Hàng Bài");

            mockRepo.Verify(r => r.GetWardsByProvinceAsync(provinceCode), Times.Once);
        }
    }
}
