using System;
using System.IO;
using Google.Cloud.TextToSpeech.V1;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Toeicking2021.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Toeicking2021.Data;
using Microsoft.AspNetCore.Authorization;
using Toeicking2021.Services.MembersDBService;
using Toeicking2021.Utilities;
using Microsoft.AspNetCore.Hosting;
using Toeicking2021.Services.MailService;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Toeicking2021.Controllers
{
    public class MemberController : Controller
    {
        #region 私有唯讀欄位
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;
        private readonly IMembersDBService _membersDBService;
        private readonly IWebHostEnvironment _env;
        private readonly IMailService _mailService;
        private readonly IOptions<Encryption> _encryption;
        // 宣告Controller全域變數：加解密用的key
        string key;
        #endregion

        #region 類別建構式
        public MemberController(IMembersDBService membersDBService, DataContext context,
            IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, IMailService mailService,
            IOptions<Encryption> encryption)
        {
            // 注入IHttpContextAccessor
            _httpContextAccessor = httpContextAccessor;
            // 注入EF Core
            _context = context;
            // 注入membersDBService(還要在DI Container(Startup.cs)註冊)
            _membersDBService = membersDBService;
            // 注入IWebHostEnvironment(獲得檔案路徑)
            _env = env;
            // 注入mailService
            _mailService = mailService;
            // 讀取appsettings中Encryption區段的值
            _encryption = encryption;
            // 從appsettings.Development.json中獲得key的值
            key = _encryption.Value.key;

        }
        #endregion

        #region 導向Home的Index
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region 註冊(Get)
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                // 已驗證過的，導向首頁
                return RedirectToAction("Index", "Home");
            }
            // 沒驗證過導向註冊頁
            return View();
        }
        #endregion

        #region 註冊(Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Administrator administrator)
        {
            // 表單驗證通過
            if (ModelState.IsValid)
            {
                // Email要轉成小寫
                administrator.Email = administrator.Email.Trim().ToLower();
                administrator.PassWord = administrator.PassWord.Trim();
                administrator.Name = administrator.Name.Trim();
                // 透過Serviec產生10位數字"註冊"Email驗證碼後放進administrator物件中
                administrator.Authcode = _mailService.GetValidateCode(10);
                // 透過Serviec產生6位數字"忘記密碼"Email驗證碼後放進administrator物件中
                administrator.ResetPasswordCode = _mailService.GetValidateCode(6);
                #region 寄信流程
                // 取得專案根目錄路徑
                //string ContentRoot = _env.ContentRootPath;
                // 取得wwwroot根目錄路徑
                string WebRoot = _env.WebRootPath;
                string path = Path.Combine(WebRoot, "templates", "RegisterEmailTemplate.html");
                // 讀取驗證信範本的html字串
                string TempMail = System.IO.File.ReadAllText(path);
                // 獲得post進來的request url(也就是Get的url)(套件Microsoft.AspNetCore.Http.Extensions)
                var url = UriHelper.GetEncodedUrl(_httpContextAccessor.HttpContext.Request);
                // 宣告Email驗證用的Url物件(可製造帶參數導向動作方法的url物件)
                UriBuilder ValidateUrl = new UriBuilder(url)
                {

                    Path = Url.Action("RegisterEmailValidate", "Member"
                            , new
                            {
                                // 參數值先編碼再加密
                                e = EncryptionHelper.UrlEncodeThenEncrypt(key, administrator.Email),
                                c = EncryptionHelper.UrlEncodeThenEncrypt(key, administrator.Authcode)
                            }),
                    // 將url轉為https
                    Scheme = "https",
                    Port = 443

                };
                // 轉成url字串(處理編碼的部份)
                string ValidateUrlString = ValidateUrl.ToString().Replace("%3F", "?");
                // 取得最後欲寄出信件的body字串
                string body = _mailService.GetMailBody(TempMail, administrator.Name, ValidateUrlString);
                // 宣告寄信需要的物件參數
                MailRequest mailRequest = new MailRequest
                {
                    ToEmailAddress = administrator.Email,
                    Subject = "會員註冊驗證信",
                    Body = body,
                    Attachments = null

                };
                #endregion
                // 將表單資料存進DB(不要忘記await)
                string RegisterResult = await _membersDBService.Register(administrator);
                // 註冊失敗，導向註冊結果頁
                if (RegisterResult == "error")
                {
                    return RedirectToAction(nameof(RegisterResult),
                        new { result = EncryptionHelper.UrlEncodeThenEncrypt(key, "sad error") });
                }
                //註冊成功，寄出驗證信
                await _mailService.SendEmailAsync(mailRequest);
                //導向註冊結果頁
                return RedirectToAction(nameof(RegisterResult)
                    , new
                    {
                        result = EncryptionHelper.UrlEncodeThenEncrypt(key, "complete success"),
                        email = EncryptionHelper.UrlEncodeThenEncrypt(key, administrator.Email),
                        name = EncryptionHelper.UrlEncodeThenEncrypt(key, administrator.Name)
                    });

            }
            else
            {
                // 表單驗證不通過
                // 將兩個密碼文字框清空(賦值null)後，將傳入參數的整個物件回傳給view
                administrator.PassWord = null;
                return View(administrator);


            }

        }
        #endregion

        #region 註冊信箱驗證
        public async Task<IActionResult> RegisterEmailValidate(string e, string c)
        {
            // 將參數解密後再解碼即可還原其值
            string decryptedEmail = EncryptionHelper.DecryptThenUrlDecode(key, e);
            string decryptedAuthCode = EncryptionHelper.DecryptThenUrlDecode(key, c);
            // 信箱驗證
            string ValidateStr = await _membersDBService.RegisterEmailValidate(decryptedEmail, decryptedAuthCode);
            switch (ValidateStr)
            {
                case "ok":
                    // 因為要導向到不同的action
                    TempData["AlertHint"] = "successfully registered";
                    // 取出會員資料
                    Administrator administrator = _membersDBService.GetDataByEmail(decryptedEmail);
                    // 寫入cookie
                    await _membersDBService.SetCookie(administrator);
                    // 導回Index即是登入狀態
                    return RedirectToAction("Index", "Home");
                case "db error":
                    ViewBag.ValidationMessage = "系統出現異常，請再試一次";
                    break;
                case "authcode error":
                    ViewBag.ValidationMessage = "驗證碼錯誤，請重新確認或再註冊";
                    break;
                case "register error":
                    ViewBag.ValidationMessage = "傳送資料錯誤，請重新確認或再註冊";
                    break;
                case "already validated":
                    ViewBag.ValidationMessage = "信箱已驗證";
                    break;
                default:
                    break;
            }
            return View();

        }
        #endregion

        #region 檢查Email是否已使用過(給"註冊"的[remote]使用)
        [AcceptVerbs("Get", "Post")]
        public IActionResult IsEmailInUse(string Email)
        {
            Administrator administrator = _membersDBService.CheckEmail(Email.Trim().ToLower());
            if (administrator == null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
                //return Json($"{Email}已被使用過，請換一個Email帳號或確認是否已註冊");
            }


        }
        #endregion

        #region 檢查Email是否已存在(給"登入"及"忘記密碼"的[remote]使用)
        [AcceptVerbs("Get", "Post")]
        public IActionResult IsEmailExisting(string Email)
        {
            Administrator administrator = _membersDBService.CheckEmail(Email.Trim().ToLower());
            if (administrator != null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }


        }
        #endregion

        #region 檢查舊密碼是否正確(給"重設密碼"的OldPassword屬性[remote]使用)
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckOldPassword(string OldPassword)
        {
            // 因為[remote]對應的動作方法傳入參數只有該屬性值(OldPassword)，所以此處的Email要用User物件取得
            string Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Administrator administrator = _membersDBService.CheckEmail(Email);
            if (administrator != null)
            {
                bool result = _membersDBService.CheckPassword(administrator, OldPassword.Trim());
                if (result)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }

            }
            else
            {
                return Json(false);
            }


        }
        #endregion

        #region 註冊結果顯示
        public IActionResult RegisterResult(string result, string email, string name)
        {
            string decryptedResult = EncryptionHelper.DecryptThenUrlDecode(key, result);
            if (decryptedResult == "sad error")
            {
                ViewBag.Result = "error";
            }
            else if (decryptedResult == "complete success")
            {
                ViewBag.Result = "success";
                ViewBag.Email = EncryptionHelper.DecryptThenUrlDecode(key, email);
                ViewBag.Name = EncryptionHelper.DecryptThenUrlDecode(key, name);
            }
            else
            {
                ViewBag.Result = "";
            }
            return View();
        }
        #endregion

        #region 登入(Get)
        public IActionResult Login()
        {
            // 已驗證過的，導向首頁
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            // 忘記密碼重設成功後導向過來
            if (TempData["AlertHint"] != null)
            {
                if (TempData["AlertHint"].ToString() == "successfully reset")
                {
                    ViewBag.AlertHint = "success";
                    ViewBag.HintMessage = "重設密碼成功，請用新密碼登入";

                }

            }
            // 沒驗證過導向登入頁
            return View();
        }
        #endregion

        #region 登入(Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginAdministrator administrator, string ReturnUrl)
        {
            administrator.Email = administrator.Email.Trim().ToLower();
            administrator.PassWord = administrator.PassWord.Trim();
            // 檢查帳號密碼是否正確，正確回傳一個空字串
            string result = _membersDBService.LoginCheck(administrator.Email, administrator.PassWord);

            if (string.IsNullOrEmpty(result))
            {
                Administrator wholeAdministrator = _membersDBService.GetDataByEmail(administrator.Email);
                // 帳密正確，將使用者資訊寫入cookie
                await _membersDBService.SetCookie(wholeAdministrator);
                // 導回上一頁
                //return Redirect(Request.Headers["Referer"].ToString());
                ViewBag.AlertHint = "0";
                // 檢查是否在站內導向過來
                if (Url.IsLocalUrl(ReturnUrl))
                {
                    // 是的話導回之前的頁面
                    return Redirect(ReturnUrl);
                }
                else
                {
                    // 不是的話導向Index
                    return RedirectToAction("Index", "Home");
                }

            }
            else if (result == "password error")
            {
                ViewBag.AlertHint = "error";
                ViewBag.HintMessage = "密碼錯誤";
            }
            else if (result == "email not validated")
            {
                ViewBag.AlertHint = "error";
                ViewBag.HintMessage = "此Email尚未經過驗證，請去收信";

            }
            else if (result == "email error")
            {
                ViewBag.AlertHint = "error";
                ViewBag.HintMessage = "Email錯誤";
            }
            // 帳密不正確，將傳入參數回傳給View
            return View(administrator);

        }
        #endregion

        #region 忘記密碼(Get)
        public IActionResult ForgotPassword()
        {
            return View();
        }
        #endregion

        #region 忘記密碼(Post)(寄信)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            Administrator administrator = _membersDBService.GetDataByEmail(Email.Trim().ToLower());
            if (administrator != null)
            {
                // 寄重設密碼驗證信
                #region 寄信流程
                // 取得專案根目錄路徑
                //string ContentRoot = _env.ContentRootPath;
                // 取得wwwroot根目錄路徑
                string WebRoot = _env.WebRootPath;
                string path = Path.Combine(WebRoot, "templates", "ForgotPasswordEmailTemplate.html");
                // 讀取驗證信範本的html字串
                string TempMail = System.IO.File.ReadAllText(path);
                // 獲得post進來的request url(也就是Get的url)(套件Microsoft.AspNetCore.Http.Extensions)
                var url = UriHelper.GetEncodedUrl(_httpContextAccessor.HttpContext.Request);
                // 宣告Email驗證用的Url物件(可製造帶參數導向動作方法的url物件)
                UriBuilder ValidateUrl = new UriBuilder(url)
                {

                    // 信件url點按後要導向ForgotPasswordEmailValidate動作方法
                    Path = Url.Action("ForgotPasswordEmailValidate", "Member"
                            , new
                            {
                                // 參數值先編碼再加密
                                e = EncryptionHelper.UrlEncodeThenEncrypt(key, administrator.Email),
                                c = EncryptionHelper.UrlEncodeThenEncrypt(key, administrator.ResetPasswordCode)
                            }),
                    // 將url轉為https
                    Scheme = "https",
                    Port = 443

                };
                // 轉成url字串(處理編碼的部份)
                string ValidateUrlString = ValidateUrl.ToString().Replace("%3F", "?");
                // 取得最後欲寄出信件的body字串
                string body = _mailService.GetMailBody(TempMail, administrator.Name, ValidateUrlString);
                // 宣告寄信需要的物件參數
                MailRequest mailRequest = new MailRequest
                {
                    ToEmailAddress = administrator.Email,
                    Subject = "重設密碼驗證信",
                    Body = body,
                    Attachments = null

                };
                #endregion
                // 寄出郵件
                await _mailService.SendEmailAsync(mailRequest);
            }
            return RedirectToAction(nameof(ForgotPasswordResult)
                    , new
                    {
                        e = EncryptionHelper.UrlEncodeThenEncrypt(key, administrator.Email),
                        n = EncryptionHelper.UrlEncodeThenEncrypt(key, administrator.Name)
                    });
        }
        #endregion

        #region 忘記密碼寄信提示
        public IActionResult ForgotPasswordResult(string e, string n)
        {
            ViewBag.Email = EncryptionHelper.DecryptThenUrlDecode(key, e);
            ViewBag.Name = EncryptionHelper.DecryptThenUrlDecode(key, n);
            return View();
        }
        #endregion

        #region 忘記密碼信箱驗證
        public async Task<IActionResult> ForgotPasswordEmailValidate(string e, string c)
        {
            string decryptedEmail = EncryptionHelper.DecryptThenUrlDecode(key, e);
            string decryptedResetPasswordCode = EncryptionHelper.DecryptThenUrlDecode(key, c);
            string ValidateStr = await _membersDBService.ResetPasswordEmailValidate(decryptedEmail, decryptedResetPasswordCode);
            switch (ValidateStr)
            {
                // 驗證通過，導到ResetPassword動作方法去重設密碼
                case "ok":
                    return RedirectToAction(nameof(ResetPassword),
                        new { e = EncryptionHelper.UrlEncodeThenEncrypt(key, decryptedEmail) });
                case "already validated":
                    ViewBag.ValidationMessage = "此連結已驗證過";
                    break;
                case "no administrator found":
                    ViewBag.ValidationMessage = "無此Email註冊";
                    break;
                case "db error":
                    ViewBag.ValidationMessage = "出現不可預期的錯誤";
                    break;
                default:
                    break;

            }
            return View();
        }
        #endregion

        #region 密碼重設(Get)
        public IActionResult ResetPassword(string e)
        {
            if (e != null)
            {
                // 檢查如果e包含"@"代表已登入要重設密碼(從Layout傳過來)，反之是從email驗證過來的
                if (e.Contains("@"))
                {
                    ViewBag.userType = "logged in";
                    ViewBag.email = e;
                }
                else
                {
                    ViewBag.userType = "not logged in";
                    ViewBag.email = EncryptionHelper.DecryptThenUrlDecode(key, e);
                    Administrator administrator = _membersDBService.GetDataByEmail(ViewBag.email);
                    if (administrator == null)
                    {
                        ViewBag.result = "error";
                    }
                }
            }
            return View();

        }
        #endregion

        #region 密碼重設(Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordAdministrator administrator, string userType)
        {
            if (ModelState.IsValid)
            {
                // 表單驗證通過，更新DB
                bool result = await _membersDBService.ChangePassword(administrator.Email.Trim().ToLower()
                    , administrator.NewPassWord);
                // 傳遞資料到不同動作方法(Home/Index)，用在sweetalert
                TempData["AlertHint"] = "successfully reset";
                // 已登入的重設密碼
                if (userType == "logged in")
                {
                    // 存DB沒出錯
                    if (result)
                    {
                        // 導向Index
                        return RedirectToAction("Index", "Home");
                    }
                    // 存DB出錯
                    else
                    {
                        // 資料清空
                        administrator.OldPassWord = null;
                        administrator.NewPassWord = null;
                        administrator.NewPassWordCheck = null;
                        return View(administrator);
                    }

                }
                // 忘記密碼的重設密碼
                else
                {
                    // 存DB沒出錯
                    if (result)
                    {
                        // 導向Login
                        return RedirectToAction(nameof(Login));
                    }
                    // 存DB出錯
                    else
                    {
                        // 資料清空
                        administrator.OldPassWord = null;
                        administrator.NewPassWord = null;
                        administrator.NewPassWordCheck = null;
                        return View(administrator);
                    }
                }

            }
            // 表單驗證沒通過
            else
            {
                administrator.OldPassWord = null;
                administrator.NewPassWord = null;
                administrator.NewPassWordCheck = null;
                return View(administrator);
            }

        }
        #endregion

        #region 登出
        public async Task<IActionResult> Logout()
        {
            // 執行登出，只需傳入CookieAuthenticationDefaults.AuthenticationScheme
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // 登出後的view
            return RedirectToAction("Index", "Home");
        }
        #endregion


    }

}
