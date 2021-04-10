using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Toeicking2021.Data;
using Toeicking2021.Models;
using Toeicking2021.Services;
using Toeicking2021.Services.MembersDBService;
using Toeicking2021.Services.SentenceDBService;
using Toeicking2021.Utilities;
using Toeicking2021.ViewModels;

namespace Toeicking2021.Controllers
{
    [Authorize]
    public class SentenceController : Controller
    {
        #region 私有欄位
        private readonly HttpClientHelper _client;
        private readonly ISentenceDBService _sentenceDBService;
        private readonly IOptions<Encryption> _encryption;
        // 宣告Controller全域變數：加解密用的key
        string key;
        #endregion

        #region 類別建構式
        public SentenceController(HttpClientHelper client, ISentenceDBService sentenceDBService, IOptions<Encryption> encryption)
        {
            _client = client;
            _sentenceDBService = sentenceDBService;
            // 讀取appsettings中Encryption區段的值
            _encryption = encryption;
            // 從appsettings.Development.json中獲得key的值
            key = _encryption.Value.key;
        }
        #endregion

        #region Index
        public IActionResult Index(int? page)
        {
            return View();
        }
        #endregion

        #region 生儲存資料表單頁面
        public IActionResult Add(string result)
        {
            // 檢查是否從post導回來
            if (!string.IsNullOrEmpty(result))
            {
                result = EncryptionHelper.DecryptThenUrlDecode(key, result);
                // 把存DB的結果給ViewBag
                ViewBag.result = result;
            }
            ViewBag.ContextCategories = ControlListHelper.ContextCategories;
            ViewBag.Categories = ControlListHelper.Categories;
            ViewBag.Snippets = ControlListHelper.Snippets;
            return View();
        }
        #endregion

        #region 存表單資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SentenceInputVM data)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                // 將空值和空字串踢出集合
                data = Filter.FilterNullOut(data);
                // 取GrammarCategory所需的value值
                data.Sentence.GrammarCategory = MultiSelectHelper.TransferGrammarCategories(data.Sentence.GrammarCategory);
                data.Sentence.Part = MultiSelectHelper.TransferPartCategories(data.Sentence.Part);
                // Trim全部字串
                data = TrimHelper.TrimAll(data);
                // 存DB，並將結果(字串)傳回前端
                result = await _sentenceDBService.AddSentenceGroup(data);
            }
            else
            {
                result = "模型繫結出錯";
            }
            // UI清單資料還要再存取一次(ViewBag的效力僅限當次request)
            ViewBag.ContextCategories = ControlListHelper.ContextCategories;
            ViewBag.Categories = ControlListHelper.Categories;
            ViewBag.Snippets = ControlListHelper.Snippets;
            // 導回GET動作方法避免重複SUBMIT(將result字串用路由參數傳回)
            return RedirectToAction(nameof(Add), new { result = EncryptionHelper.UrlEncodeThenEncrypt(key, result) });

        }
        #endregion

        #region 刪除句子
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.result = await _sentenceDBService.DeleteSenetnce(id);
            return View();
        }
        #endregion

        #region 動態生控制項(PartialView)
        public IActionResult AddControl(string number, string category)
        {
            ViewBag.number = number;
            ViewBag.category = category;
            ViewBag.Categories = ControlListHelper.Categories;
            return PartialView();
        }
        #endregion

        #region 顯示資料搜尋頁
        //public IActionResult Retrieve(TableQueryFormData FormData)
        //{
        //    // 判斷是否為ajax get
        //    var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        //    // 沒有ajax單純顯示表單
        //    if (!isAjax)
        //    {
        //        return View();
        //    }
        //    // 表單資料透過jQuery收集，用ajax get送到這個動作方法
        //    // 再將表單資料轉導到顯示表格的動作方法
        //    return RedirectToAction(nameof(RenderTable), new
        //    {
        //        FormData.SenNum,
        //        FormData.Keyword,
        //        FormData.AddedDate,
        //        FormData.CountDesc,
        //        FormData.CheckedTimes,
        //        FormData.PageSize
        //    });
        //}
        #endregion

        #region 顯示資料表(PartialView)
        //public IActionResult RenderTable(TableQueryFormData FormData)
        //{
        //    // 建立predicate變數，也就是where()的Lambda參數，New<T>的泛型是要查出的資料物件型別
        //    var predicate = PredicateBuilder.New<TestData>(true);
        //    // 將收到的表單資料放進ViewBag傳遞到前端
        //    ViewBag.SenNum = FormData.SenNum;
        //    // 表單資料的編號不為null，代表有加入這個篩選條件
        //    if (FormData.SenNum != null)
        //    {
        //        // 將篩選條件加入predicate(t就是要查詢的資料物件型別)
        //        predicate = predicate.And(t => t.Number == Convert.ToInt32(FormData.SenNum));
        //    }
        //    ViewBag.Keyword = FormData.Keyword;
        //    if (FormData.Keyword != null)
        //    {
        //        // StringComparison.InvariantCultureIgnoreCase表比較字串時"不區別文化特性也不區別大小寫"
        //        predicate = predicate.And(t => t.Description.Contains(FormData.Keyword.Trim(),
        //            StringComparison.InvariantCultureIgnoreCase));
        //    }
        //    ViewBag.AddedDate = FormData.AddedDate;
        //    ViewBag.CountDesc = FormData.CountDesc;
        //    ViewBag.CheckTimes = FormData.CheckedTimes;
        //    ViewBag.PageSize = FormData.PageSize;
        //    // 先將資料轉成IQueryable<T>-->AsQueryable()
        //    // 一般集合物件呼叫AsQueryable()後"無法"使用EF Core的非同步方法
        //    // DbSet物件呼叫AsQueryable()後"可以"使用EF Core的非同步方法
        //    // 操作資料庫時要把分頁的方法改成非同步(目前是同步！)
        //    IQueryable<TestData> source = new DataForTable().Data.AsQueryable();
        //    // predicate不為null代表有進行篩選
        //    if (predicate != null)
        //    {
        //        // 使用Where(predicate)篩選IQueryable<T>物件，送到分頁的方法裡查出那一頁的資料
        //        source = source.Where(predicate);
        //    }
        //    // 分頁方法回傳該頁資料List<T>
        //    List<TestData> data = PaginatedList<TestData>.Create(source, FormData.Page ?? 1, FormData.PageSize ?? 3);
        //    return PartialView(data);
        //}
        #endregion

        #region 顯示正式資料搜尋頁
        public IActionResult RetrieveData(TableQueryFormData FormData)
        {
            // 判斷是否為ajax get
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            // 沒有ajax get代表單純顯示表單
            if (!isAjax)
            {
                // 情境下拉選單
                ViewBag.ContextCategories = ControlListHelper.ContextCategories;
                return View();
            }
            // 表單資料透過jQuery收集，用ajax get送到這個動作方法，此時是撈第一頁
            // 再將表單資料轉導到顯示表格的動作方法
            return RedirectToAction(nameof(Render), new
            {
                FormData.SenNum,
                FormData.HasWordOrigin,
                FormData.HasSynonym,
                FormData.HasAudio,
                FormData.Context,
                FormData.GrammarCategories,
                FormData.Part,
                FormData.Keyword,
                FormData.AddedDate,
                FormData.CountDesc,
                FormData.CheckedTimes,
                FormData.PageSize,
                // 繫結類別有加新屬性，要記得來route value加進去，不然表格處理的傳入參數新屬性永遠是null
                FormData.BoolConditions,
                FormData.SkipOffset
            });

        }
        #endregion

        #region 顯示正式資料表(參數為查詢條件表單資料物件)
        public async Task<IActionResult> Render(TableQueryFormData FormData)
        {
            // 將收到的表單資料放進ViewBag傳遞到前端，放在頁碼按鈕的asp-route中，按下後又會將這些表單資料送回此動作方法
            // 當前頁碼(FormData.Page)要直接傳進分頁方法，前端再透過頁碼按鈕動態改變其值再傳回後端
            ViewBag.SenNum = FormData.SenNum;
            ViewBag.Keyword = FormData.Keyword;
            ViewBag.AddedDate = FormData.AddedDate;
            ViewBag.CheckedTimes = FormData.CheckedTimes;
            ViewBag.CountDesc = FormData.CountDesc;
            ViewBag.Part = FormData.Part;
            ViewBag.GrammarCategories = FormData.GrammarCategories;
            ViewBag.PageSize = FormData.PageSize;
            ViewBag.HasWordOrigin = FormData.HasWordOrigin;
            ViewBag.HasSynonym = FormData.HasSynonym;
            ViewBag.HasAudio = FormData.HasAudio;
            ViewBag.BoolConditions = FormData.BoolConditions;
            ViewBag.SkipOffset = FormData.SkipOffset;
            // 將要查詢資料表物件AsQueryable()
            var source = _sentenceDBService.TableAsQueryable();
            // 將表單資料處理成動態where條件式
            var predicate = DynamicPredicateHelper.SentenceDynamicPredicate(FormData);
            // predicate不為null代表有進行where條件篩選，反之沒有做任何篩選
            if (predicate != null)
            {
                // 使用Where(predicate)篩選IQueryable<T>物件，送到分頁的方法裡查出那一頁的資料
                source = source.Where(predicate);
            }
            // 篩選最近幾筆：一定要放最後，因為要等其它Where條件式都完成(Where(predicate))再OrderByDescending().Take()
            if (FormData.CountDesc != null)
            {
                // 此處source還是IQueryable<T>的型別，仍然可以當作分頁方法的參數
                // 呼叫GetValueOrDefault()可將int?轉型成int
                source = source.OrderByDescending(s => s.SentenceId).Take(FormData.CountDesc.GetValueOrDefault());
            }
            // 呼叫分頁方法，接收回傳值的型別必須是PaginatedList<T>，不可以是List<T>，否則取不到物件的另外四個屬性
            // FormData.Page?? 是因為撈第一頁的話，FormData.Page可以不用有值
            PaginatedList<Sentence> data = await PaginatedList<Sentence>.Create(source, FormData.Page ?? 1, FormData.SkipOffset?? 0, FormData.PageSize);
            if (data.Count == 0)
            {
                ViewBag.result = "查無資料";
            }
            return PartialView(data);

        }
        #endregion

        #region 依句子編號查出文法解析
        public async Task<IActionResult> ReviewGrammar(int sentenceId)
        {
            List<GA> result = await _sentenceDBService.GetGrammarsBySentenceId(sentenceId);
            return PartialView(result);
        }
        #endregion

        #region 依句子編號查出字彙解析
        public async Task<IActionResult> ReviewVocAnalysis(int sentenceId)
        {
            List<VA> result = await _sentenceDBService.GetVocAnalysesBySentenceId(sentenceId);
            return PartialView(result);
        }
        #endregion

        #region 依句子編號查出字彙
        public async Task<IActionResult> ReviewVocabulary(int sentenceId)
        {
            List<Vocabulary> result = await _sentenceDBService.GetVocabularyBySentenceId(sentenceId);
            return PartialView(result);
        }
        #endregion

        #region 更新句子
        // 若只是要回傳字串，回傳值型別也可用string
        [HttpPost]
        public async Task<IActionResult> UpdateSentence(int sentenceId, string sentence, string chinese)
        {
            string result = await _sentenceDBService.UpdateSentence(sentenceId, sentence, chinese);
            return Content(result);
        }
        #endregion

        #region 更新文法解析(集合)
        public async Task<IActionResult> UpdateGrammarPackage()
        {
            List<GA> response = await JsonParser.FromRequestBody<List<GA>>(Request.Body);
            string result = await _sentenceDBService.UpdateGrammars(response);
            return Content(result);
        }
        #endregion

        #region 更新字彙解析(集合)
        public async Task<IActionResult> UpdateVocAnalysisPackage()
        {
            List<VA> response = await JsonParser.FromRequestBody<List<VA>>(Request.Body);
            string result = await _sentenceDBService.UpdateVocAnalysis(response);
            return Content(result);
        }
        #endregion

        #region 更新字彙(集合)
        public async Task<IActionResult> UpdateVocabularyPackage()
        {
            List<Vocabulary> response = await JsonParser.FromRequestBody<List<Vocabulary>>(Request.Body);
            string result = await _sentenceDBService.UpdateVoc(response);
            return Content(result);
        }
        #endregion

        #region 檢查次數加1
        public async Task<IActionResult> AddCheckTime(int sentenceId) 
        {
            string result = await _sentenceDBService.AddCheckTime(sentenceId);
            return Content(result);
        }
        #endregion


    }

}
