using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Toeicking2021.Models;

namespace Toeicking2021.Utilities
{
    public class HttpClientHelper
    {
        #region 私有唯讀欄位
        private readonly IHttpClientFactory _clientFactory;
        #endregion

        #region 類別建構式
        public HttpClientHelper(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        #endregion

        #region HttpPost Json
        // 泛型方法：傳入一個T型別的物件參數(HttpPostJson<T>)，回傳一個T型別物件(Task<T>)
        // 條件約束(where T：): T為類別型別(class)，可以new出T型別物件(new())
        public async Task<T> HttpPostJson<T>(string url, T ObjToJson) where T: class, new()
        {
            var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(ObjToJson), Encoding.UTF8,"application/json");
            using var httpResponse = await client.PostAsync(url, content);
            // post過去，對方處理回傳後再進入這一行
            httpResponse.EnsureSuccessStatusCode();
            T responseObj = new T();
            if (httpResponse.IsSuccessStatusCode)
            {
                using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                responseObj = await JsonSerializer.DeserializeAsync<T>(responseStream);
            }
            return responseObj;

        }
        #endregion

        #region HttpPost FormData
        public async Task HttpPostFormData(string url, Administrator administrator)
        {
            var client = _clientFactory.CreateClient();
            var formData = string.Format("email={0}&name={1}", Uri.EscapeDataString(administrator.Email), Uri.EscapeDataString(administrator.Name));
            var content = new StringContent(formData, Encoding.UTF8, "application/x-www-form-urlencoded");
            using var httpResponse = await client.PostAsync(url, content);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.IsSuccessStatusCode)
            {
                using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                Administrator responseAdministrator = await JsonSerializer.DeserializeAsync<Administrator>(responseStream);
            }

        }
        #endregion



    }



}
