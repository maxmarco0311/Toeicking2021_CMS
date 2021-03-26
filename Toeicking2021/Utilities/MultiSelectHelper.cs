using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Utilities
{
    public class MultiSelectHelper
    {
        // dictionary型別的物件屬性
        public static Dictionary<string, string> MultiSelectDic
        {
            // 使用getter將dictionary初始化
            get
            {
                return new Dictionary<string, string>
                {
                    { "現在簡單式","1"}, { "現在進行式","2"}, { "現在完成式","3"}, { "現在完成進行式","4"}, { "過去簡單式","5"},
                    { "過去進行式","6"}, { "過去完成式","7"}, { "過去完成進行式","8"}, { "未來簡單式","9"}, { "未來進行式","10"},
                    { "未來完成式","11"}, { "未來完成進行式","12"}, { "that開頭","13"}, { "if/whether開頭","14"}, { "wh-疑問詞開頭","15"},
                    { "修飾\"人\"","16"}, { "修飾\"事物\"","17"}, { "修飾\"時間\"","18"}, { "修飾\"地點\"","19"}, { "描述\"時間\"","20"},
                    { "描述\"原因\"","21"}, { "描述\"對比\"","22"}, { "描述\"條件\"","23"}, { "描述\"目的\"","24"}, { "假設語氣","25"},
                    { "其它副詞子句","26"}, { "名詞子句減化","27"}, { "形容詞子句減化","28"}, { "副詞子句減化","29"}, { "接續發展","30"},
                    { "單個介片","31"}, { "多個介片","32"}
                
                };
            }

        }

        // 靜態方法
        public static string TransferToValueFromDic(string keys) 
        {
            string[] temp = keys.Split(",");
            string result = "";
            foreach (var item in temp)
            {
                if (MultiSelectDic.ContainsKey(item))
                {
                    result += MultiSelectDic[item] + ",";
                } 

            }
            return result.TrimEnd(',');
        
        }



    }


}
