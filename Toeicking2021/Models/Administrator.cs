using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Models
{
    public class Administrator
    {
        // 主鍵
        public int AdministratorId { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Required(ErrorMessage = "請輸入Email！")]
        [EmailAddress(ErrorMessage = "Email格式不正確！")]
        [StringLength(50, ErrorMessage = "最多輸入50個字元")]
        // 驗證Email是否有使用過，用[Remote]
        // [Remote]表利用AJAX執行動作方法進行檢查驗證，動作方法的傳入參數的"名稱"、"型別"必須與此屬性的"名稱"、"型別"相同
        // 該動作方法必須以JSON回傳true或錯誤訊息字串
        [Remote(action: "IsEmailInUse", controller: "Member", ErrorMessage = "此Email已被註冊過！")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "請輸入暱稱！")]
        [StringLength(50, ErrorMessage = "最多輸入25個字元")]
        public string Name { get; set; }

        [Required(ErrorMessage = "請輸入密碼！")]
        [Column(TypeName = "varchar(max)")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d).{6,12}$", ErrorMessage = "密碼需為6-12位小寫英文數字混合！")]
        public string PassWord { get; set; }

        // 要用nvarchar，不然重新填值，不到10個字元會被填空白字元，會出錯
        [Column(TypeName = "nvarchar(20)")]
        public string Authcode { get; set; }

        public bool? IsAdmin { get; set; }

        // 重設密碼驗證碼(6個字元)
        [Column(TypeName = "nvarchar(12)")]
        public string ResetPasswordCode { get; set; }

    }
}
