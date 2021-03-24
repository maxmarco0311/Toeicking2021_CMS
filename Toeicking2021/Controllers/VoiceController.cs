using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toeicking2021.Utilities;

namespace Toeicking2021.Controllers
{
    public class VoiceController : Controller
    {

        #region 生成語音(Get)
        //[Authorize]
        public IActionResult GenerateVoice()
        {
            return View();
        }
        #endregion

        #region 生成語音(Post)
        //[Authorize]
        [HttpPost]
        public IActionResult GenerateVoice(string text, string accent, string rate)
        {
            GoogleTTS.GenerateVoice(text, accent, rate);
            string script = "alert('done');";
            return Content(script, "application/javascript");
            //return Json(script);
        }
        #endregion


    }
}
