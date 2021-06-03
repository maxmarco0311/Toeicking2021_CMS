using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Utilities
{
    public class MultiSelectHelper
    {
        // 多選前端只能取出text值，所以需要到後端進行處理才能取出value值
        // 設定多選的Dictionary<text, value>，才能轉換
        // dictionary型別的靜態物件屬性，直接初始設定
        #region 文法分類Dic
        public static Dictionary<string, string> GrammaryCategoriesDic { get; } = new Dictionary<string, string>
        {
            { "現在簡單式","1"}, { "現在進行式","2"}, { "現在完成式","3"}, { "現在完成進行式","4"}, { "過去簡單式","5"},
            { "過去進行式","6"}, { "過去完成式","7"}, { "過去完成進行式","8"}, { "未來簡單式","9"}, { "未來進行式","10"},
            { "未來完成式","11"}, { "未來完成進行式","12"}, { "被動語態","13"}, { "that開頭","14"}, { "if/whether開頭","15"},
            { "wh-疑問詞開頭","16"}, { "修飾\"人\"","17"}, { "修飾\"事物\"","18"}, { "修飾\"時間\"","19"}, { "修飾\"地點\"","20"},
            { "whose","21"}, { "描述\"時間\"","22"}, { "描述\"原因\"","23"}, { "描述\"對比\"","24"}, { "描述\"條件\"","25"},
            { "描述\"目的\"","26"}, { "假設語氣","27"}, { "其它副詞子句","28"}, { "名詞子句減化","29"}, { "形容詞子句減化","30"},
            { "副詞子句減化","31"}, { "接續發展","32"}, { "背景說明","33"}, { "單個介片","34"}, { "多個介片","35"},{ "動名詞","36"},
            { "不定詞","37"}, { "Yes/No問句","38"},{ "Wh-疑問句","39"}, { "轉折詞","40"}, { "對等連接詞","41"}
        };
        #endregion

        #region 大題分類Dic
        public static Dictionary<string, string> PartCategoriesDic { get; } = new Dictionary<string, string>
        {
            { "Part 1","1"}, { "Part 2","2"}, { "Part 3","3"}, { "Part 4","4"}, { "Part 5","5"},
            { "Part 6","6"}, { "Part 7","7"}
        };
        #endregion

        #region 文法分類取value
        public static string TransferGrammarCategories(string keys)
        {
            // 靜態屬性可直接用
            return TransferToValueFromDic(keys, GrammaryCategoriesDic);
        }
        #endregion

        #region 大題分類取value
        public static string TransferPartCategories(string keys)
        {
            return TransferToValueFromDic(keys, PartCategoriesDic);
        }
        #endregion

        # region 基礎共用方法：將以逗號連接各個text的字串(傳入參數)換成value字串回傳
        public static string TransferToValueFromDic(string keys, Dictionary<string, string> dic)
        {
            // 組成text字串陣列
            string[] temp = keys.Split(",");
            string result = "";
            foreach (var item in temp)
            {
                // 將每個text去配對Dic的key(也就是text)
                if (dic.ContainsKey(item))
                {
                    // 利用dic[item]取出value
                    result += dic[item] + ",";
                }

            }
            return result.TrimEnd(',');
        }
        #endregion


    }


}
