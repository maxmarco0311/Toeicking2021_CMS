using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Toeicking2021.Models;

namespace Toeicking2021.Controllers
{
    public class ParseController : Controller
    {

        #region ParseJson
        [HttpPost]
        public async Task<IActionResult> ParseJson()
        {
            string container = "";
            // Request.Body獲得request body的stream
            using (var reader = new StreamReader(Request.Body))
            {
                // 讀取body內容中json格式的大字串，這只是字串，C#還無法用
                var jsonBody = await reader.ReadToEndAsync();
                // 將大字串(jsonBody)反序列化成匹配的C#物件(User)格式，resonse即是C#物件
                // 若是傳來物件集合，反序列化的泛型型別要記得換成List<User>
                var response = JsonConvert.DeserializeObject<Administrator>(jsonBody);
                // 若要再將C#物件回傳給對方，需將物件再序列化成json字串
                container = JsonConvert.SerializeObject(response);
            }
            // 用Content()回傳json字串
            return Content(container);

        }
        #endregion

        #region ParseFormData
        [HttpPost]
        public IActionResult ParseFormData(Administrator administrator)
        {
            string email = administrator.Email;
            return Content(email);

        }
        #endregion


    }
}
