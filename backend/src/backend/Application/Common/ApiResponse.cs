using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        // Count sẽ chỉ xuất hiện trong JSON nếu nó có giá trị (tức là khi data là một danh sách)
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Count { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Errors { get; set; }

        public T? Data { get; set; }

        // --- CÁC PHƯƠNG THỨC TẠO RESPONSE ---

        /// <summary>
        /// Tạo một response thành công cho một đối tượng đơn lẻ.
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T? data, string message = "Operation successful.")
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };

            // Nếu data là PagedResult, nó đã có thông tin count bên trong rồi, không cần làm gì thêm.
            // Nếu data là một danh sách đơn giản (không phân trang), chúng ta sẽ thêm count.
            if (data is ICollection collection)
            {
                response.Count = collection.Count;
            }
            else if (data is IEnumerable enumerable && data is not string)
            {
                // Dùng Cast<object>() để đếm cho mọi loại IEnumerable
                response.Count = enumerable.Cast<object>().Count();
            }

            return response;
        }

        /// <summary>
        /// Tạo một response lỗi.
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message = "An error occurred.", IEnumerable<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}