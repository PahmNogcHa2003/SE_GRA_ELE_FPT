using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs;
using Application.DTOs.Station;
using FluentAssertions;
using Xunit;

namespace AdminLayer.IntegrationTests.Controllers.Staff
{
    // Sử dụng CustomWebApplicationFactory thay vì WebApplicationFactory mặc định
    public class StationsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public StationsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        #region GET Tests

        [Fact]
        public async Task GetStationById_KhiStationTonTai_TraVe200OK()
        {
            // Act
            var response = await _client.GetAsync("/api/stations/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<StationDTO>>();
            result.Data.Id.Should().Be(1);
            result.Data.Name.Should().Be("Ga Sài Gòn");
        }

        [Fact]
        public async Task GetStationById_KhiStationKhongTonTai_TraVe404NotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/stations/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetStations_TraVeDanhSachPhanTrang()
        {
            // Act
            var response = await _client.GetAsync("/api/stations?page=1&pageSize=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<StationDTO>>>();
            result.Data.Items.Should().HaveCount(1);
            result.Data.TotalCount.Should().Be(2);
        }

        #endregion

        #region POST Tests

        [Fact]
        public async Task CreateStation_VoiDuLieuHopLe_TraVe201Created()
        {
            // Arrange
            // ⭐ SỬA LẠI: Dùng 'Location' và thêm các trường khác cho khớp với model
            var newStation = new StationDTO
            {
                Name = "Ga Đà Nẵng",
                Location = "200 Hải Phòng, Đà Nẵng", // <-- SỬA TỪ Address
                Lat = 16.0712m,
                Lng = 108.2142m,
                IsActive = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/stations", newStation);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<StationDTO>>();
            result.Data.Name.Should().Be("Ga Đà Nẵng");
            result.Data.Location.Should().Be("200 Hải Phòng, Đà Nẵng");
            result.Data.Id.Should().NotBe(0);
            response.Headers.Location.ToString().Should().Contain($"/api/stations/{result.Data.Id}");
        }

        [Fact]
        public async Task CreateStation_VoiDuLieuKhongHopLe_TraVe400BadRequest()
        {
            // Arrange
            // ⭐ SỬA LẠI: Dùng 'Location'
            var newStation = new StationDTO { Name = "", Location = "" }; // Tên và địa chỉ rỗng -> không hợp lệ

            // Act
            var response = await _client.PostAsJsonAsync("/api/stations", newStation);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region PUT Tests

        [Fact]
        public async Task UpdateStation_VoiDuLieuHopLe_TraVe200OK()
        {
            // Arrange
            // ⭐ SỬA LẠI: Dùng 'Location'
            var updatedStation = new StationDTO { Name = "Ga Sài Gòn Cập Nhật", Location = "Địa chỉ mới" };

            // Act
            var response = await _client.PutAsJsonAsync("/api/stations/1", updatedStation);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // (Optional) Verify the update was successful
            var verifyResponse = await _client.GetAsync("/api/stations/1");
            var result = await verifyResponse.Content.ReadFromJsonAsync<ApiResponse<StationDTO>>();
            result.Data.Name.Should().Be("Ga Sài Gòn Cập Nhật");
            result.Data.Location.Should().Be("Địa chỉ mới");
        }

        [Fact]
        public async Task UpdateStation_KhiStationKhongTonTai_TraVe404NotFound()
        {
            // Arrange
            var updatedStation = new StationDTO { Name = "Tên không quan trọng" };

            // Act
            var response = await _client.PutAsJsonAsync("/api/stations/999", updatedStation);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region DELETE Tests

        [Fact]
        public async Task DeleteStation_KhiStationTonTai_TraVe200OK()
        {
            // Act
            var response = await _client.DeleteAsync("/api/stations/2");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // (Optional) Verify it was deleted
            var verifyResponse = await _client.GetAsync("/api/stations/2");
            verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteStation_KhiStationKhongTonTai_TraVe404NotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/stations/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}