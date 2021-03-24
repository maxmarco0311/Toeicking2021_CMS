using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Models;

namespace Toeicking2021.Services.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest request);
        string GetValidateCode(int digit);
        string GetMailBody(string TempString, string UserName, string ValidateUrl);
    }
}
