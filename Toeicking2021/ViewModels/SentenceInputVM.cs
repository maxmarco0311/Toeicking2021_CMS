using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Models;

namespace Toeicking2021.ViewModels
{
    public class SentenceInputVM
    {
        // 一個Sentence物件
        public Sentence Sentence { get; set; }
        // Vocabulary集合(幾個字彙)
        public List<Vocabulary> Vocs { get; set; }
        // GA集合(幾個文法解析)
        public List<GA> GAs { get; set; }
        // VA集合(幾個字彙解析)
        public List<VA> VAs { get; set; }


    }
}
