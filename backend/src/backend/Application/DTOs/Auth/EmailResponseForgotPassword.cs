using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class EmailResponseForgotPassword
    {
        public string HtmlContent { get; set; } = @"
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f3fbfd;padding:40px 0;font-family:Arial,Helvetica,sans-serif;"">
          <tr>
            <td align=""center"">
              <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff;border-radius:8px;box-shadow:0 6px 18px rgba(16,40,59,0.08);overflow:hidden;"">
                <tr>
                  <td style=""padding:28px 32px 8px 32px;text-align:left;"">
                    <h2 style=""margin:0;color:#153243;font-size:20px;font-weight:600;"">Reset your password</h2>
                  </td>
                </tr>

                <tr>
                  <td style=""padding:22px 36px 28px 36px;text-align:center;"">
                    <div style=""max-width:460px;margin:0 auto;"">
                      <p style=""margin:0 0 18px 0;color:#516973;font-size:15px;line-height:1.5;"">
                        Hello,<br/>
                        We received a request to reset your password. Click the button below to choose a new password.
                      </p>

                      <table cellpadding=""0"" cellspacing=""0"" style=""margin:18px auto;"">
                        <tr>
                          <td align=""center"" bgcolor=""#1e87f0"" style=""border-radius:6px;"">
                            <a href=""{{ResetLink}}"" target=""_blank"" 
                               style=""display:inline-block;padding:12px 26px;font-size:15px;color:#ffffff;text-decoration:none;font-weight:600;border-radius:6px;"">
                              Reset password
                            </a>
                          </td>
                        </tr>
                      </table>

                      <p style=""margin:16px 0 0 0;color:#9aaeb8;font-size:13px;line-height:1.4;"">
                        If the button doesn't work, copy and paste this link into your browser:
                        <br/>
                        <a href=""{{ResetLink}}"" target=""_blank"" style=""color:#1e87f0;word-break:break-all;"">{{ResetLink}}</a>
                      </p>

                      <hr style=""border:none;border-top:1px solid #eef6f8;margin:22px 0 12px 0;"">

                      <p style=""margin:0;color:#9aaeb8;font-size:12px;"">
                        If you didn't request a password reset, you can safely ignore this email.
                      </p>
                    </div>
                  </td>
                </tr>

                <tr>
                  <td style=""padding:14px 36px 24px 36px;text-align:center;background:#fbfeff;"">
                    <p style=""margin:0;color:#8da6ad;font-size:12px;"">&copy; YourCompany. All rights reserved.</p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>";
    }
}
