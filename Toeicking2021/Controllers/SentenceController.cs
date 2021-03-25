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
using Toeicking2021.Utilities;
using Toeicking2021.ViewModels;

namespace Toeicking2021.Controllers
{
    [Authorize]
    public class SentenceController : Controller
    {
        private readonly HttpClientHelper _client;

        public SentenceController(HttpClientHelper client)
        {
            _client = client;
        }
        // 第一次get時可能會沒有參數，所以要int?
        // 第二次開始get就從<a>傳page參數進來
        public IActionResult Index(int? page)
        {
            return View();
        }

        public IActionResult Add()
        {
            ViewBag.ContextCategories = new List<SelectListItem>
            {
                new SelectListItem{ Text="請選擇",Value=""},
                new SelectListItem{ Text="住宿交通",Value="1"},
                new SelectListItem{ Text="餐飲觀光",Value="2"},
                new SelectListItem{ Text="行銷與銷售",Value="3"},
                new SelectListItem{ Text="生產與製造",Value="4"},
                new SelectListItem{ Text="商務會議",Value="5"},
                new SelectListItem{ Text="辦公室溝通",Value="6"},
                new SelectListItem{ Text="人事招募",Value="7"},
                new SelectListItem{ Text="購物訂單",Value="8"},
                new SelectListItem{ Text="經營管理",Value="9"},
                new SelectListItem{ Text="設備與修繕",Value="10"},
                new SelectListItem{ Text="客戶溝通",Value="11"},
                new SelectListItem{ Text="典禮與活動",Value="12"}

            };
            ViewBag.Categories = new List<SelectListItem>
            {
                new SelectListItem{ Text="請選擇",Value=""},
                new SelectListItem{ Text="(n.)",Value="n"},
                new SelectListItem{ Text="(v.)",Value="v"},
                new SelectListItem{ Text="(adj.)",Value="adj"},
                new SelectListItem{ Text="(adv.)",Value="adv"},
                new SelectListItem{ Text="(prep.)",Value="prep"},
                new SelectListItem{ Text="(conj.)",Value="conj"},

            };

            return View();
        }

        [HttpPost]
        public IActionResult Add(SentenceInput input)
        {

            return View();
        }

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


        

    }

}
