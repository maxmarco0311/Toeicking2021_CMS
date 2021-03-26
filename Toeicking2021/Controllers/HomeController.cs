using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Toeicking2021.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            if (TempData["AlertHint"] != null)
            {
                if (TempData["AlertHint"].ToString() == "successfully registered")
                {
                    ViewBag.AlertHint = "registered";
                    ViewBag.HintMessage = "信箱驗證成功，請開始使用";
                }
                else if (TempData["AlertHint"].ToString() == "successfully reset")
                {
                    ViewBag.AlertHint = "reset";
                    ViewBag.HintMessage = "重設密碼成功，請繼續使用";
                }
            }
            return View();
        }









    }
}
