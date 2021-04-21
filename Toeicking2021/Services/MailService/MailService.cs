using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Models;
using System.IO;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using System.Net;

namespace Toeicking2021.Services.MailService
{
    public class MailService: IMailService
    {
        #region 私有唯讀欄位
        private readonly MailSettings _mailSettings;
        #endregion

        #region 類別建構式
        // 要用DI取得appSettings內某區段的值(寄信帳號相關資訊)，這裡已經對應到MailSettings物件
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        #endregion

        #region 產生驗證碼方法
        public string GetValidateCode(int digit)
        {
            //設定驗證碼字元的陣列
            string[] Code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K"
        , "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y"
            , "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b"
                , "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n"
                    , "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            //宣告初始為空的驗證碼字串
            string ValidateCode = string.Empty;
            //宣告可產生隨機數值的物件
            Random rd = new Random();
            //使用迴圈產生出驗證碼
            for (int i = 0; i < digit; i++)
            {
                ValidateCode += Code[rd.Next(Code.Count())];
            }
            //回傳驗證碼
            return ValidateCode;
        }
        #endregion

        #region 將使用者資料填入驗證信範本中
        public string GetMailBody(string TempString, string UserName, string ValidateUrl)
        {
            //將使用者資料填入
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);
            //回傳最後結果
            return TempString;
        }
        #endregion

        #region 寄信
        public async Task SendEmailAsync(MailRequest mailRequest) 
        {
            // 宣告mail物件
            var email = new MimeMessage();
            // 要這樣寫才可加入寄件者名稱
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            // 收件者地址(若為多人則使用迴圈)
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmailAddress));
            // 信件主題
            email.Subject = mailRequest.Subject;
            // 信件body
            var builder = new BodyBuilder();
            // 表單上傳的附件處理
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            // 信件內容的html
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            // 此處的SmtpClient()命名空間為套件MailKit.Net.Smtp，不是內建的
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

        }
        #endregion




    }
}
