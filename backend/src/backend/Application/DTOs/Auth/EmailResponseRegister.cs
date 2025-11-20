using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class EmailResponseRegister
    {
        public string HtmlContent { get; set; } = @"
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f3fbfd;padding:40px 0;font-family:Arial,Helvetica,sans-serif;"">
          <tr>
            <td align=""center"">
              <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff;border-radius:8px;box-shadow:0 6px 18px rgba(16,40,59,0.08);overflow:hidden;"">
                <tr>
                  <td style=""padding:28px 32px 8px 32px;text-align:left;"">
                    <h2 style=""margin:0;color:#153243;font-size:20px;font-weight:600;"">Chào mừng bạn đến với hệ thống!</h2>
                  </td>
                </tr>

                <tr>
                  <td style=""padding:22px 36px 28px 36px;text-align:center;"">
                    <div style=""max-width:460px;margin:0 auto;"">
                      <p style=""margin:0 0 18px 0;color:#516973;font-size:15px;line-height:1.5;"">
                        Xin chào <strong>{{UserName}}</strong>,<br/>
                        Cảm ơn bạn đã đăng ký tài khoản tại hệ thống của chúng tôi. Chúng tôi rất vui được chào đón bạn!
                      </p>

                      <p style=""margin:0 0 18px 0;color:#516973;font-size:15px;line-height:1.5;"">
                        Chúc bạn có trải nghiệm tuyệt vời và tận hưởng tất cả các tính năng mà hệ thống cung cấp.
                      </p>

                      <hr style=""border:none;border-top:1px solid #eef6f8;margin:22px 0 12px 0;"">

                      <p style=""margin:0;color:#9aaeb8;font-size:12px;"">
                        Một lần nữa, chào mừng bạn! Chúng tôi rất vui khi bạn trở thành thành viên của hệ thống.
                      </p>
                    </div>
                  </td>
                </tr>

                <tr>
                  <td style=""padding:14px 36px 24px 36px;text-align:center;background:#fbfeff;"">
                    <p style=""margin:0;color:#8da6ad;font-size:12px;"">&copy; Công ty của bạn. Bản quyền thuộc về chúng tôi.</p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>";
    }
}
