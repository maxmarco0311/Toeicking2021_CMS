using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.ViewModels;

namespace Toeicking2021.Utilities
{
    public class TrimHelper
    {
        public static SentenceInputVM TrimAll(SentenceInputVM data) 
        {
            // 字串只有呼叫Trim()方法是不會改變此字串的值，必須在呼叫Trim()方法後再賦值給同字串才會改變其值
            data.Sentence.Sen = data.Sentence.Sen.Trim();
            data.Sentence.Chinesese = data.Sentence.Chinesese.Trim();   
            foreach (var item in data.Vocs)
            {
                item.Voc = item.Voc.Trim();
                item.Chinese = item.Chinese.Trim();              
            }
            foreach (var item in data.VAs)
            {
                item.Analysis = item.Analysis.Trim();             
            }
            foreach (var item in data.GAs)
            {
                item.Analysis = item.Analysis.Trim();    
            }

            return data;
        }
    }
}
