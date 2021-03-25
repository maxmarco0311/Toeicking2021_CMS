using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Models;

namespace Toeicking2021.ViewModels
{
    public class SentenceInput
    {
        // 一個Sentence物件
        public Sentence Sentence { get; set; }
        // Vocabulary集合(幾個字彙)
        public List<Vocabulary> Vocs { get; set; }
        // GA集合(幾個文法解析)
        public List<GA> GAs { get; set; }
        // VA集合(幾個字彙解析)
        public List<VA> VAs { get; set; }

        // 下拉選單的items
        public List<SelectListItem> ContextCategories { get; set; } = new List<SelectListItem>
        {
            new SelectListItem{ Text="請選擇",Value=""},
            new SelectListItem{ Text="住宿交通",Value="1"},
            new SelectListItem{ Text="餐飲觀光",Value="2"},
            new SelectListItem{ Text="行銷與銷售",Value="3"},
            new SelectListItem{ Text="生產與製造",Value="4"},
            new SelectListItem{ Text="商務會議",Value="5"},
            new SelectListItem{ Text="辦公室溝通",Value="6"},
            new SelectListItem{ Text="人事招募",Value="7"},
            new SelectListItem{ Text="購物訂單",Value="8"},
            new SelectListItem{ Text="經營管理",Value="9"},
            new SelectListItem{ Text="設備與修繕",Value="10"},
            new SelectListItem{ Text="客戶溝通",Value="11"},
            new SelectListItem{ Text="典禮與活動",Value="12"}
            
        };

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>
        {
            new SelectListItem{ Text="請選擇",Value=""},
            new SelectListItem{ Text="(n.)",Value="n"},
            new SelectListItem{ Text="(v.)",Value="v"},
            new SelectListItem{ Text="(adj.)",Value="adj"},
            new SelectListItem{ Text="(adv.)",Value="adv"},
            new SelectListItem{ Text="(prep.)",Value="prep"},
            new SelectListItem{ Text="(conj.)",Value="conj"},
           
        };







    }
}
