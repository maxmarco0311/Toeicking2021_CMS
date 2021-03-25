using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Models
{
    public class GA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AnalysisId { get; set; }

        // 導覽屬性
        public Sentence Sentence { get; set; }

        // FK(attribute optional!)
        [ForeignKey("Sentence")]
        public int SentenceId { get; set; }

        // 這裡不加[Required]，因為如果有空的控制項沒用到，到時繫結會出錯
        [Column(TypeName = "nvarchar(400)")]
        public string Analysis { get; set; }



    }
}
