using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Utilities
{
    public class ControlListHelper
    {
        public static List<SelectListItem> Categories { get; } = new List<SelectListItem>
        {
            new SelectListItem{ Text="請選擇",Value=""},
            new SelectListItem{ Text="(n.)",Value="n"},
            new SelectListItem{ Text="(v.)",Value="v"},
            new SelectListItem{ Text="(adj.)",Value="adj"},
            new SelectListItem{ Text="(adv.)",Value="adv"},
            new SelectListItem{ Text="(prep.)",Value="prep"},
            new SelectListItem{ Text="(conj.)",Value="conj"},
        };

        public static List<SelectListItem> ContextCategories { get; } = new List<SelectListItem>
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

        public static List<string> Snippets { get; } = new List<string>
        { "本句主詞為", "是一個名詞子句", "開頭連接詞為", "動名詞" };


    }
}
