using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Models
{
    public class TestData
    {
        [Display(Name="編號")]
        public int Number  { get; set; }
        [Display(Name = "多益必考金句")]
        public string Name  { get; set; }
        [Display(Name = "翻譯")]
        public string Description { get; set; }
    }
}
