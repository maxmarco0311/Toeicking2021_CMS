using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toeicking2021.Models;
using Toeicking2021.Utilities;

namespace Toeicking2021.Controllers
{
    public class TestController : Controller
    {
        #region 私有唯讀欄位
        private readonly HttpClientHelper _client;
        #endregion

        #region 物件建構式
        public TestController(HttpClientHelper client)
        {
            _client = client;
        }
        #endregion

        #region 執行HttpPostJson
        public async Task<IActionResult> HttpPostJson()
        {
            Administrator administrator = new Administrator
            {
                Email = "maxmarco0311@gmail.com",
                Name = "Max",
                PassWord = "11111",
                Authcode = "sssss",
                IsAdmin = true,
                AdministratorId = 100
            };
            string url = "https://localhost:44371/Parse/ParseJson";
            Administrator responseAdministrator = await _client.HttpPostJson<Administrator>(url, administrator);
            return Content(responseAdministrator.Name);
        }
        #endregion

        #region 執行HttpPostFormData
        public async Task<IActionResult> HttpPostFormData()
        {
            Administrator administrator = new Administrator
            {
                Email = "maxmarco0311@gmail.com",
                Name = "Max",
                PassWord = "11111",
                Authcode = "sssss",
                IsAdmin = true,
                AdministratorId = 100
            };
            string url = "https://localhost:44371/Parse/ParseFormData";
            await _client.HttpPostFormData(url, administrator);
            return Content("done");
        }
        #endregion



    }
}
