using Application.DTOs.Kyc;
using Application.Interfaces.Ocr;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Grpc.Core; // <-- THÊM USING NÀY ĐỂ BẮT RPCEXCEPTION
using Microsoft.Extensions.Logging; // <-- THÊM USING NÀY ĐỂ GHI LOG
using System;
using System.IO;
using System.Text; // <-- THÊM USING NÀY
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Ocr
{
    public class GoogleVisionOcrRepository : IOcrRepository
    {
        private readonly ImageAnnotatorClient _client;
        private readonly ILogger<GoogleVisionOcrRepository> _logger; // <-- THÊM LOGGER
        private const string KeyFileName = "service-account-key.json";

        // === SỬA ĐỔI CONSTRUCTOR ĐỂ NHẬN LOGGER ===
        public GoogleVisionOcrRepository(ILogger<GoogleVisionOcrRepository> logger)
        {
            _logger = logger; // Gán logger
            try
            {
                var credential = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, KeyFileName));
                _client = new ImageAnnotatorClientBuilder
                {
                    Credential = credential
                }.Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể khởi tạo GoogleVisionOcrRepository. Vui lòng kiểm tra file key '{KeyFileName}'.", KeyFileName);
                throw; // Ném lại lỗi để hệ thống biết service không thể khởi tạo
            }
        }

        public async Task<KycDTO> ReadIdCardInfoAsync(string frontImageUrl, string backImageUrl)
        {
            var frontTextBuilder = new StringBuilder();
            var backTextBuilder = new StringBuilder();

            try
            {
                var frontImage = Image.FromUri(frontImageUrl);
                var backImage = Image.FromUri(backImageUrl);

                var frontTextAnnotationTask = _client.DetectDocumentTextAsync(frontImage);
                var backTextAnnotationTask = _client.DetectDocumentTextAsync(backImage);

                // === BỌC TRONG TRY-CATCH ĐỂ BẮT LỖI TỪ GOOGLE API ===
                try
                {
                    await Task.WhenAll(frontTextAnnotationTask, backTextAnnotationTask);
                }
                catch (RpcException ex)
                {
                    _logger.LogError(ex, "Lỗi RPC khi gọi Google Vision API. StatusCode: {StatusCode}, Details: {Details}", ex.StatusCode, ex.Status.Detail);
                    // Có thể URL ảnh không hợp lệ, hoặc không có quyền truy cập
                    return null; // Trả về null để báo hiệu xử lý thất bại
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi không xác định khi chờ tác vụ OCR từ Google.");
                    return null;
                }

                var frontTextAnnotation = await frontTextAnnotationTask;
                var backTextAnnotation = await backTextAnnotationTask;

                if (frontTextAnnotation != null)
                {
                    frontTextBuilder.Append(frontTextAnnotation.Text);
                }
                else
                {
                    _logger.LogWarning("Không nhận được kết quả OCR cho ảnh mặt trước: {frontImageUrl}", frontImageUrl);
                }

                if (backTextAnnotation != null)
                {
                    backTextBuilder.Append(backTextAnnotation.Text);
                }
                else
                {
                    _logger.LogWarning("Không nhận được kết quả OCR cho ảnh mặt sau: {backImageUrl}", backImageUrl);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng xảy ra trong quá trình ReadIdCardInfoAsync.");
                return null; // Trả về null nếu có lỗi
            }

            var fullText = $"{frontTextBuilder}\n{backTextBuilder}";

            if (string.IsNullOrWhiteSpace(fullText))
            {
                _logger.LogWarning("OCR không đọc được bất kỳ text nào từ cả hai ảnh.");
                return null;
            }

            // === CÁC MẪU REGEX VẪN GIỮ NGUYÊN ===
            var kycData = new KycDTO
            {
                NumberCard = ExtractValue(fullText, @"Số\s*[/:|Iil]\s*No\.\s*[:;]\s*(\d{12})"),
                FullName = ExtractValue(fullText, @"Họ và tên\s*[/:|Iil]\s*Full name\s*[:;]?\s*([A-ZÀ-Ỹ\s]+)"),
                Dob = ParseDate(ExtractValue(fullText, @"Ngày sinh\s*[/:|Iil]\s*Date of birth\s*[:;]\s*(\d{2}/\d{2}/\d{4})")),
                Gender = ExtractValue(fullText, @"Giới tính\s*[/:|Iil]\s*Sex\s*[:;]\s*(\p{L}+)"),
                PlaceOfOrigin = ExtractValue(fullText, @"Quê quán\s*[/:|Iil]\s*Place of origin\s*[:;]\s*([^\r\n]+(?:\r?\n[^\r\n]+)?)"),
                PlaceOfResidence = ExtractValue(fullText, @"Nơi thường trú\s*[/:|Iil]\s*Place of residence\s*[:;]\s*([^\r\n]+(?:\r?\n[^\r\n]+)?)"),
                ExpiryDate = ParseDateOffset(ExtractValue(fullText, @"Có giá trị đến\s*[/:|Iil]\s*Date of expiry\s*[:;]\s*(\d{2}/\d{2}/\d{4}|Không thời hạn)")),
                IssuedDate = ParseDateOffset(ExtractValue(fullText, @"Ngày,\s*tháng,\s*năm\s*[/:|Iil]\s*Date,\s*month,\s*year\s*(\d{2}/\d{2}/\d{4})")),
                IssuedBy = ExtractValue(fullText, @"(CỤC\s*TRƯỞNG\s*CỤC\s*CẢNH\s*SÁT[^\r\n]+)")
            };

            return kycData;
        }

        private string ExtractValue(string text, string pattern)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (match.Success)
            {
                return Regex.Replace(match.Groups[1].Value.Trim(), @"\s*\r?\n\s*", " ");
            }
            return null;
        }

        private DateTime? ParseDate(string dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }
            return null;
        }

        private DateTimeOffset? ParseDateOffset(string dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return new DateTimeOffset(date);
            }
            return null;
        }
    }
}

