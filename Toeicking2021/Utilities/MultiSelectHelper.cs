using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Utilities
{
    public class MultiSelectHelper
    {
        // dictionary型別的靜態物件屬性，直接初始設定
        public static Dictionary<string, string> GrammaryCategoriesDic { get; } = new Dictionary<string, string>
        {
            { "現在簡單式","1"}, { "現在進行式","2"}, { "現在完成式","3"}, { "現在完成進行式","4"}, { "過去簡單式","5"},
            { "過去進行式","6"}, { "過去完成式","7"}, { "過去完成進行式","8"}, { "未來簡單式","9"}, { "未來進行式","10"},
            { "未來完成式","11"}, { "未來完成進行式","12"}, { "that開頭","13"}, { "if/whether開頭","14"}, { "wh-疑問詞開頭","15"},
            { "修飾\"人\"","16"}, { "修飾\"事物\"","17"}, { "修飾\"時間\"","18"}, { "修飾\"地點\"","19"}, { "whose","20"},
            { "描述\"時間\"","21"}, { "描述\"原因\"","22"}, { "描述\"對比\"","23"}, { "描述\"條件\"","24"}, { "描述\"目的\"","25"},
            { "假設語氣","26"}, { "其它副詞子句","27"}, { "名詞子句減化","28"}, { "形容詞子句減化","29"}, { "副詞子句減化","30"},
            { "接續發展","31"}, { "背景說明","32"}, { "單個介片","33"}, { "多個介片","34"},{ "受詞為動名詞","35"},{ "受詞為不定詞","36"},
            { "Yes/No問句","37"},{ "Wh-疑問句","38"}
        };
        public static Dictionary<string, string> PartCategoriesDic { get; } = new Dictionary<string, string>
        {
            { "Part 1","1"}, { "Part 2","2"}, { "Part 3","3"}, { "Part 4","4"}, { "Part 5","5"},
            { "Part 6","6"}, { "Part 7","7"}
        };

        // 文法分類
        public static string TransferGrammarCategories(string keys)
        {
            // 靜態屬性可直接用
            return TransferToValueFromDic(keys, GrammaryCategoriesDic);
        }

        // 大題分類
        public static string TransferPartCategories(string keys)
        {
            return TransferToValueFromDic(keys, PartCategoriesDic);
        }

        // 基礎共用方法：將text字串(傳入參數)換成value字串回傳
        public static string TransferToValueFromDic(string keys, Dictionary<string, string> dic)
        {
            string[] temp = keys.Split(",");
            string result = "";
            foreach (var item in temp)
            {

                if (dic.ContainsKey(item))
                {
                    result += dic[item] + ",";
                }

            }
            return result.TrimEnd(',');
        }



    }


}
