using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Utilities
{
    public class JsonParser
    {
        // 想要轉成甚麼型別的物件，就傳該型別的泛型到方法中
        public static async Task<T> FromStream<T>(Stream stream) where T : new()
        {
            // T就是要回轉的C#物件(集合)型別
            T response = new T();
            // stream就是Controller裡的Request.Body
            using (var reader = new StreamReader(stream))
            {
                // 讀取body內容中json格式的大字串，這只是字串，C#還無法用
                var jsonBody = await reader.ReadToEndAsync();
                // 將大字串(jsonBody)反序列化成匹配的C#物件格式)，resonse即是C#物件
                // T可以是物件或物件集合
                response = JsonConvert.DeserializeObject<T>(jsonBody);
            }
            return response;
        }




    }
}
