using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Models
{
    public class TableQueryFormData
    {
        public string SenNum { get; set; }
        public string Keyword { get; set; }
        public int? Page { get; set; }
        public int? AddedDate { get; set; }

        public int? CountDesc { get; set; }
        public int? CheckTimes { get; set; }

        public int? PageSize { get; set; }
    }
}
