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
            data.Sentence.Sen.Trim();
            data.Sentence.Chinesese.Trim();        
            foreach (var item in data.Vocs)
            {
                item.Voc.Trim();
                item.Chinese.Trim();              
            }
            foreach (var item in data.VAs)
            {
                item.Analysis.Trim();             
            }
            foreach (var item in data.GAs)
            {
                item.Analysis.Trim();    
            }

            return data;
        }
    }
}
