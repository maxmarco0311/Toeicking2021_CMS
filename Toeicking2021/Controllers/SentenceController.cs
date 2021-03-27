using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly HttpClientHelper _client;
        private readonly ISentenceDBService _sentenceDBService;

        public SentenceController(HttpClientHelper client, ISentenceDBService sentenceDBService)
        {
            _client = client;
            _sentenceDBService = sentenceDBService;
           
        }

        #region Index
        public IActionResult Index(int? page)
        {
            return View();
        }
        #endregion

        #region 生表單頁面
        public IActionResult Add()
        {
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
            if (ModelState.IsValid)
            {
                // 將空值和空字串踢出集合
                data = Filter.FilterNullOut(data);
                // 取GrammarCategory所需的value值
                data.Sentence.GrammarCategory = MultiSelectHelper.TransferToValueFromDic(data.Sentence.GrammarCategory);
                // 存DB，並將結果(字串)傳回前端
                ViewBag.result = await _sentenceDBService.AddSentenceGroup(data);
            }
            else
            {
                ViewBag.result = "模型繫結出錯";   
            }
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
        public IActionResult Retrieve(TableFormData FormData)
        {
            // 判斷是否為ajax get
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            // 沒有ajax單純顯示表單
            if (!isAjax)
            {
                return View();
            }
            // 表單資料透過jQuery收集，用ajax get送到這個動作方法
            // 再將表單資料轉導到顯示表格的動作方法
            return RedirectToAction(nameof(RenderTable), new { 
                FormData.SenNum, FormData.Keyword, FormData.AddedDate, FormData.CountDesc, FormData.CheckTimes, FormData.PageSize });
        }
        #endregion

        #region 顯示資料表(PartialView)
        public IActionResult RenderTable(TableFormData FormData)
        {
            // 建立predicate變數，也就是where()的Lambda參數，New<T>的泛型是要查出的資料物件型別
            var predicate = PredicateBuilder.New<TestData>(true);
            // 將收到的表單資料放進ViewBag傳遞到前端
            ViewBag.SenNum = FormData.SenNum;
            // 表單資料的編號不為null，代表有加入這個篩選條件
            if (FormData.SenNum!=null)
            {
                // 將篩選條件加入predicate(t就是要查詢的資料物件型別)
                predicate = predicate.And(t => t.Number == Convert.ToInt32(FormData.SenNum));
            }
            ViewBag.Keyword = FormData.Keyword;
            if (FormData.Keyword!=null)
            {
                // StringComparison.InvariantCultureIgnoreCase表比較字串時"不區別文化特性也不區別大小寫"
                predicate = predicate.And(t => t.Description.Contains(FormData.Keyword.Trim(),
                    StringComparison.InvariantCultureIgnoreCase));
            }
            ViewBag.AddedDate = FormData.AddedDate;
            ViewBag.CountDesc = FormData.CountDesc;
            ViewBag.CheckTimes = FormData.CheckTimes;
            ViewBag.PageSize = FormData.PageSize;
            // 先將資料轉成IQueryable<T>-->AsQueryable()
            // 一般集合物件呼叫AsQueryable()後"無法"使用EF Core的非同步方法
            // DbSet物件呼叫AsQueryable()後"可以"使用EF Core的非同步方法
            // 操作資料庫時要把分頁的方法改成非同步(目前是同步！)
            IQueryable<TestData> source = new DataForTable().Data.AsQueryable();
            // predicate不為null代表有進行篩選
            if (predicate!=null)
            {
                // 使用Where(predicate)篩選IQueryable<T>物件，送到分頁的方法裡查出那一頁的資料
                source = source.Where(predicate);
            }
            // 分頁方法回傳該頁資料List<T>
            List<TestData> data = PaginatedList<TestData>.Create(source, FormData.Page ?? 1, FormData.PageSize ?? 3);
            return PartialView(data);
        }
        #endregion




    }

}
