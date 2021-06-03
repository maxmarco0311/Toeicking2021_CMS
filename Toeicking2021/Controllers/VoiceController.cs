using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toeicking2021.Models;
using Toeicking2021.Services.SentenceDBService;
using Toeicking2021.Utilities;

namespace Toeicking2021.Controllers
{
    //[Authorize]
    public class VoiceController : Controller
    {
        #region 私有欄位
        private readonly ISentenceDBService _sentenceDBService;
        #endregion

        #region 類別建構式
        public VoiceController(ISentenceDBService sentenceDBService)
        {
            _sentenceDBService = sentenceDBService;
        }
        #endregion

        #region 生成語音資料搜尋頁
        public IActionResult GenerateVoice(string PageSize)
        {
            // 判斷是否為ajax get
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            // 沒有ajax get代表單純顯示表單
            if (!isAjax)
            {
                ViewBag.result = "";
                return View();
            }
            return RedirectToAction(nameof(Render), new { PageSize, Page=1 });

        }
        #endregion

        #region 生表格
        public async Task<IActionResult> Render(string PageSize, string Page)
        {
            ViewBag.result = "";
            ViewBag.PageSize = PageSize;
            int page = Convert.ToInt16(Page);
            int pageSize = Convert.ToInt16(PageSize);
            IQueryable<Sentence> source = _sentenceDBService.GetSentencesWithoutVoice();
            PaginatedList<Sentence> data = await PaginatedList<Sentence>.Create(source, page, 0, pageSize);
            return PartialView(data);
        }
        #endregion

        #region 生成語音檔(GET)
        public async Task<IActionResult> GenerateAudioFiles(string text, string senNum)
        {
            // 存放音檔的網站資料夾名稱
            string webdir = "voice.toeicking.com";
            //string result = GoogleTTS.GenerateVoice(text, accent, rate, senNum, webdir);
            string result = GoogleTTS.GenerateVoiceSet(text, senNum, webdir);
            if (result=="1")
            {
                // 將HasAudio改為True
                result = await _sentenceDBService.UpdateHasAudio(Convert.ToInt16(senNum));
            }
            ViewBag.result = result;
            return Content(result);
        }
        #endregion


    }
}
