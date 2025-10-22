using Application.DTOs.Kyc;
using Application.Interfaces.Ocr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Infrastructure.Repositories.Ocr
{
    public class OcrSpaceRepository : IOcrRepository
    {
        private readonly ILogger<OcrSpaceRepository> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // Lớp helper để deserialize JSON trả về từ OCR.space
        private class OcrSpaceResponse
        {
            public List<ParsedResult> ParsedResults { get; set; }
            public int OCRExitCode { get; set; }
            public bool IsErroredOnProcessing { get; set; }
            public JsonElement ErrorMessage { get; set; }
        }

        private class ParsedResult
        {
            public string ParsedText { get; set; }
        }

        public OcrSpaceRepository(ILogger<OcrSpaceRepository> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["OcrSpaceSettings:ApiKey"];

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("API Key for OCR.space is not configured in appsettings.json");
            }
        }

        public async Task<KycDTO> ReadIdCardInfoAsync(string frontImageUrl, string backImageUrl)
        {
            try
            {
                var frontTextTask = GetTextFromImageUrlAsync(frontImageUrl);
                var backTextTask = GetTextFromImageUrlAsync(backImageUrl);

                await Task.WhenAll(frontTextTask, backTextTask);

                var frontText = await frontTextTask;
                var backText = await backTextTask;

                if (string.IsNullOrWhiteSpace(frontText) && string.IsNullOrWhiteSpace(backText))
                {
                    _logger.LogWarning("OCR.space không đọc được text từ cả hai ảnh.");
                    return null;
                }

                var fullText = $"{frontText}\n{backText}";

                // === LOGGING: Ghi lại toàn bộ text đã đọc được ===
                _logger.LogInformation("Toàn bộ text đọc được từ OCR:\n{FullText}", fullText);

                var kycData = new KycDTO
                {
                    NumberCard = ExtractValue(fullText, "NumberCard", @"Số\s*[/:|Iil]\s*No\.\s*[:;]\s*(\d{12})"),
                    FullName = ExtractValue(fullText, "FullName", @"Họ và tên\s*[/:|Iil]\s*Full name\s*[:;]?\s*([A-ZÀ-Ỹ\s]+)"),
                    Dob = ParseDate(ExtractValue(fullText, "Dob", @"Ngày sinh\s*[/:|Iil]\s*Date of birth\s*[:;]\s*(\d{2}/\d{2}/\d{4})")),
                    Gender = ExtractValue(fullText, "Gender", @"Giới tính\s*[/:|Iil]\s*Sex\s*[:;]\s*(\p{L}+)"),
                    PlaceOfOrigin = ExtractValue(fullText, "PlaceOfOrigin", @"Quê quán\s*[/:|Iil]\s*Place of origin\s*[:;]\s*([^\r\n]+(?:\r?\n[^\r\n]+)?)"),
                    PlaceOfResidence = ExtractValue(fullText, "PlaceOfResidence", @"Nơi thường trú\s*[/:|Iil]\s*Place of residence\s*[:;]\s*([^\r\n]+(?:\r?\n[^\r\n]+)?)"),
                    ExpiryDate = ParseDateOffset(ExtractValue(fullText, "ExpiryDate", @"Có giá trị đến\s*[/:|Iil]\s*Date of expiry\s*[:;]\s*(\d{2}/\d{2}/\d{4}|Không thời hạn)")),
                    IssuedDate = ParseDateOffset(ExtractValue(fullText, "IssuedDate", @"Ngày,\s*tháng,\s*năm\s*[/:|Iil]\s*Date,\s*month,\s*year\s*(\d{2}/\d{2}/\d{4})")),
                    IssuedBy = ExtractValue(fullText, "IssuedBy", @"(CỤC\s*TRƯỞNG\s*CỤC\s*CẢNH\s*SÁT[^\r\n]+)")
                };

                return kycData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng trong quá trình xử lý OCR với OCR.space.");
                return null;
            }
        }

        private async Task<string> GetTextFromImageUrlAsync(string imageUrl)
        {
            async Task<string> SendOcrRequestAsync(string language)
            {
                using var formData = new MultipartFormDataContent
        {
            { new StringContent(_apiKey), "apikey" },
            { new StringContent(language), "language" },
            { new StringContent("1"), "ocrengine" },
            { new StringContent(imageUrl), "url" }
        };

                var response = await _httpClient.PostAsync("https://api.ocr.space/parse/image", formData);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Lỗi khi gọi API OCR.space. Status: {StatusCode}, URL: {ImageUrl}", response.StatusCode, imageUrl);
                    return string.Empty;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("JSON response từ OCR.space (lang={Lang}) cho URL {ImageUrl}: {JsonString}", language, imageUrl, jsonString);

                var ocrResponse = JsonSerializer.Deserialize<OcrSpaceResponse>(jsonString);

                if (ocrResponse == null)
                {
                    _logger.LogError("Phản hồi từ OCR.space null hoặc không hợp lệ (lang={Lang})", language);
                    return string.Empty;
                }

                if (ocrResponse.IsErroredOnProcessing || ocrResponse.ParsedResults == null || ocrResponse.ParsedResults.Count == 0)
                {
                    var errorMessage = ocrResponse.ErrorMessage.ToString();
                    _logger.LogWarning("OCR.space lỗi hoặc không có kết quả (lang={Lang}): {ErrorMessage}", language, errorMessage);
                    return string.Empty;
                }

                return ocrResponse.ParsedResults[0].ParsedText;
            }

            // --- Thử OCR tiếng Việt trước ---
            var text = await SendOcrRequestAsync("vie");

            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("OCR tiếng Việt thất bại, thử lại với tiếng Anh (eng).");
                text = await SendOcrRequestAsync("eng");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("OCR.space không đọc được text từ ảnh URL: {ImageUrl}", imageUrl);
            }

            return text;
        }


        // Cập nhật hàm ExtractValue để ghi log
        private string ExtractValue(string text, string fieldName, string pattern)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (match.Success)
            {
                var extractedValue = Regex.Replace(match.Groups[1].Value.Trim(), @"\s*\r?\n\s*", " ");
                _logger.LogInformation("[Regex Success] Field: {FieldName}, Value: '{ExtractedValue}'", fieldName, extractedValue);
                return extractedValue;
            }

            _logger.LogWarning("[Regex Fail] Field: {FieldName} không tìm thấy.", fieldName);
            return null;
        }

        private DateTime? ParseDate(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return null;
            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }
            return null;
        }

        private DateTimeOffset? ParseDateOffset(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return null;
            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return new DateTimeOffset(date);
            }
            return null;
        }
    }
}

