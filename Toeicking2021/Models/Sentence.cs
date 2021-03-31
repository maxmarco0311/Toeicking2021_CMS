using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Models
{
    public class Sentence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SentenceId { get; set; }

        [Column(TypeName = "nvarchar(600)")]
        [Required(ErrorMessage = "請輸入句子內容")]
        public string Sen { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "請輸入中文翻譯")]
        public string Chinesese  { get; set; }

        [Column(TypeName = "varchar(2)")]
        [Required(ErrorMessage = "請選擇情境")]
        public string Context  { get; set; }

        // bool屬性搭配checkbox用在asp-for當中不可為null，解決辦法是設預設值false
        public bool WordOrigin { get; set; } = false;
        public bool Synonym { get; set; } = false;

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "請選擇文法類型")]
        public string GrammarCategory { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        [Required(ErrorMessage = "請選擇大題")]
        public string Part { get; set; }

        [Column(TypeName = "date")]
        public DateTime AddedDate { get; set; }

        public bool HasAudio { get; set; }

        [Required]
        [Column(TypeName = "tinyint")]
        public int CheckedTimes { get; set; } = 0;

        // 用在View當中，跟DB無關
        [NotMapped]
        public string AudioFileName { get; set; }

        // 導覽屬性
        public ICollection<Vocabulary> Vocabularies { get; set; }
        public ICollection<GA> GAs { get; set; }
        public ICollection<VA> Vas { get; set; }


    }
}
