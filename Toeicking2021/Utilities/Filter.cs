using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.ViewModels;

namespace Toeicking2021.Utilities
{
    public class Filter
    {
        // 將空值和空字串踢出集合回傳給controller
        public static SentenceInputVM FilterNullOut(SentenceInputVM data)
        {
            data.GAs = data.GAs.Where(g => !string.IsNullOrEmpty(g.Analysis)).ToList();
            data.VAs = data.VAs.Where(v => !string.IsNullOrEmpty(v.Analysis)).ToList();
            data.Vocs = data.Vocs.Where(v => !string.IsNullOrEmpty(v.Voc) && !string.IsNullOrEmpty(v.Category) 
                                             && !string.IsNullOrEmpty(v.Chinese)).ToList();
            return data;

        }


    }



}
