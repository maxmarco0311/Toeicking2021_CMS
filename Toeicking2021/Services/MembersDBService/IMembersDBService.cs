using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Models;

namespace Toeicking2021.Services.MembersDBService
{
    public interface IMembersDBService
    {
        Task<string> Register(Administrator NewAdministrator);
        Administrator CheckEmail(string Email);
        string LoginCheck (string email, string password);
        Task SetCookie(Administrator NewAdministrator);
        Task<string> RegisterEmailValidate(string Email, string AuthCode);
        Administrator GetDataByEmail(string email);
        Task<bool> ChangePassword(string Email, string newPassword);
        Task<string> ResetPasswordEmailValidate(string Email, string ResetPasswordCode);
        bool CheckPassword(Administrator administrator, string password);

    }
}
