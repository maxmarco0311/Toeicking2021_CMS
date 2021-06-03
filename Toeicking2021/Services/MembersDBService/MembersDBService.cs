using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Toeicking2021.Data;
using Toeicking2021.Models;
using Toeicking2021.Services;
using Toeicking2021.Services.MailService;

namespace Toeicking2021.Services.MembersDBService
{
    // 繼承ControllerBase，讓類別內可使用其成員(如Url.Action())
    public class MembersDBService : ControllerBase, IMembersDBService
    {
        #region 私有唯讀欄位
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;
        private readonly IMailService _mailService;
        #endregion

        #region 類別建構式
        public MembersDBService(IHttpContextAccessor httpContextAccessor, DataContext context, IMailService mailService)
        {
            // 一般類別使用httpContextAccessor才能獲得HttpContext
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mailService = mailService;
        }
        #endregion

        #region 密碼加密
        public string HashPassword(string Password)
        {

            //宣告Hash時所添加的無意義亂數值
            string saltkey = "3g2w3e4r5t8p7u1ue9o0po1tyz";
            //將剛剛宣告的字串與密碼結合
            string saltAndPassword = String.Concat(Password, saltkey);
            //定義SHA256的HASH物件
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            //取得密碼轉換成byte資料
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            //取得Hash後byte資料
            byte[] HashDate = sha256Hasher.ComputeHash(PasswordData);
            //將Hash後byte資料轉換成string
            string Hashresult = Convert.ToBase64String(HashDate);
            //回傳Hash後結果(字串)
            return Hashresult;
        }
        #endregion

        #region 註冊(寫入資料庫)
        // 不要寫async void，會報錯
        public async Task<string> Register(Administrator NewAdministrator)
        {
            // Hash密碼
            NewAdministrator.PassWord = HashPassword(NewAdministrator.PassWord);
            string RegisterResult = "";
            // 存入資料庫
            try
            {
                await _context.Administrators.AddAsync(NewAdministrator);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                RegisterResult = "error";
            }
            return RegisterResult;

        }
        #endregion

        #region 檢查Email是否已存在
        public Administrator CheckEmail(string Email)
        {
            Administrator administrator = _context.Administrators.FirstOrDefault(a => a.Email == Email);
            return administrator;
        }
        #endregion

        #region 用email找一筆資料
        public Administrator GetDataByEmail(string email)
        {
            Administrator administrator = _context.Administrators.FirstOrDefault(a => a.Email == email);
            return administrator;
        }
        #endregion

        #region 註冊信箱驗證
        public async Task<string> RegisterEmailValidate(string Email, string AuthCode)
        {
            // 根據Email取得該administrator
            Administrator administrator = GetDataByEmail(Email);
            //宣告驗證結果的訊息字串
            string ValidateStr = string.Empty;
            // 檢查所查到的administrator是否為空
            if (administrator != null)
            {
                // 檢查是否已驗證
                if (administrator.Authcode != "1")
                {
                    // 確認尚未驗證後去檢查驗證碼參數是否和DB內一樣
                    if (administrator.Authcode == AuthCode)
                    {
                        // 驗證通過，將Authcode屬性給"20150719"
                        administrator.Authcode = "1";
                        try
                        {
                            // 更新DB
                            _context.Administrators.Update(administrator);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            ValidateStr = "db error";
                        }

                        ValidateStr = "ok";

                    }
                    // 驗證碼和DB內不一樣
                    else
                    {
                        ValidateStr = "authcode error";
                    }
                }
                // 已驗證過
                else
                {
                    ValidateStr = "already validated";
                }


            }
            // 查無此administrator
            else
            {
                ValidateStr = "register error";
            }
            return ValidateStr;

        }
        #endregion

        #region 登入確認
        public string LoginCheck(string email, string password)
        {
            // 用email去查administrator
            Administrator LoginAdministrator = GetDataByEmail(email);
            // 檢查是否有該位administrator
            if (LoginAdministrator != null)
            {
                // 再檢查Email是否驗證過
                if (LoginAdministrator.Authcode == "1")
                {
                    // 最後檢查密碼是否正確
                    if (CheckPassword(LoginAdministrator, password))
                    {
                        // 全部正確，回傳空字串，可以登入
                        return "";
                    }
                    else
                    {
                        return "password error";
                    }
                }
                else
                {
                    return "email not validated";
                }

            }
            return "email error";

        }
        #endregion

        #region 檢查密碼是否正確
        public bool CheckPassword(Administrator administrator, string password)
        {
            bool result = administrator.PassWord.Equals(HashPassword(password));
            return result;
        }
        #endregion

        #region 變更密碼
        public async Task<bool> ChangePassword(string Email, string newPassword)
        {
            // 取得傳入帳號的會員資料
            Administrator administrator = GetDataByEmail(Email);
            // 將新密碼Hash後寫入資料庫中
            administrator.PassWord = HashPassword(newPassword);
            // 更新DB
            _context.Administrators.Update(administrator);
            await _context.SaveChangesAsync();
            return true;

        }
        #endregion

        #region 寫入cookie
        public async Task SetCookie(Administrator NewAdministrator)
        {
            // 建立Claim物件集合，放id(email)和role即可
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, NewAdministrator.Email),
                        new Claim(ClaimTypes.Name, NewAdministrator.Name),
                        new Claim(ClaimTypes.Role, "1")
                    };
            // 建立ClaimsIdentity物件，參數1為Claim物件集合，參數2為CookieAuthentication scheme的列舉值
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            claimsIdentity.GetHashCode();
            // 建立AuthenticationProperties物件，設置cookie的屬性
            var authProperties = new AuthenticationProperties
            {
                //是否可以被刷新
                AllowRefresh = false,
                // 設置了一個1天有效期的持久化cookie
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };
            // 執行登入，傳入CookieAuthentication scheme的列舉值、ClaimsPrincipal物件、AuthenticationProperties物件
            // 至此，Controller和View可用User物件取得已成功登入的使用者資料(一般類別要用IHttepContextAccessor)
            // SignInAsync()寫在一般類別(非controller)時，要用_httpContextAccessor.HttpContext.SignInAsync()
            // 直接寫HttpContext.SignInAsync()會報錯
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                    new ClaimsPrincipal(claimsIdentity),
                                    authProperties);

        }
        #endregion

        #region 重設密碼信箱驗證
        public async Task<string> ResetPasswordEmailValidate(string Email, string ResetPasswordCode)
        {
            // 根據Email取得該administrator
            Administrator administrator = GetDataByEmail(Email);
            //宣告驗證結果的訊息字串
            string ValidateStr = string.Empty;
            // 檢查所查到的administrator是否為空
            if (administrator != null)
            {
                // 檢查連結是否已驗證過或惡意進入
                if (administrator.ResetPasswordCode != ResetPasswordCode)
                {
                    // 連結已驗證過(所以參數ResetPasswordCode與DB的ResetPasswordCode不一樣)或惡意登入
                    ValidateStr = "already validated";
                }
                else
                {
                    // 驗證通過，將Authcode屬性再隨機給6個字元
                    administrator.ResetPasswordCode = _mailService.GetValidateCode(6);
                    try
                    {
                        // 更新DB
                        _context.Administrators.Update(administrator);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        ValidateStr = "db error";
                    }

                    ValidateStr = "ok";
                    
                }

            }
            // 查無此administrator
            else
            {
                ValidateStr = "no administrator found";
            }
            return ValidateStr;

        }
        #endregion


    }





}
