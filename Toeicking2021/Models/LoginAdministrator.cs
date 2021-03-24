using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Models
{
    public class LoginAdministrator
    {
        [Required(ErrorMessage = "請輸入Email！")]
        [EmailAddress(ErrorMessage = "Email格式不正確！")]
        [StringLength(50, ErrorMessage = "最多輸入50個字元")]
        // 驗證Email是否有使用過，用[Remote]
        // [Remote]表利用AJAX執行動作方法進行檢查驗證，動作方法的傳入參數的"名稱"、"型別"必須與此屬性的"名稱"、"型別"相同
        // 該動作方法必須以JSON回傳true或錯誤訊息字串
        [Remote(action: "IsEmailExisting", controller: "Member", ErrorMessage = "Email錯誤或尚未註冊！")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入密碼！")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d).{6,12}$", ErrorMessage = "密碼需為6-12位小寫英文數字混合！")]
        public string PassWord { get; set; }
    }
}
