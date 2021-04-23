using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toeicking2021.Utilities;

namespace Toeicking2021.Controllers
{
    public class VoiceController : Controller
    {

        #region 生成語音(Get)
        [Authorize]
        public IActionResult GenerateVoice()
        {
            ViewBag.result = "";
            return View();
        }
        #endregion

        #region 生成語音(Post)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateVoice(string text, string accent, string rate, string senNum)
        {
            // 存放音檔的網站資料夾名稱
            string webdir = "voice.toeicking.com";
            string result = GoogleTTS.GenerateVoice(text, accent, rate, senNum, webdir);
            ViewBag.result = result;
            return View();
        }
        #endregion


    }
}
