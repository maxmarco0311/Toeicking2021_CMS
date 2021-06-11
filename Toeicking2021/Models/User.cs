using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        [Required]
        public bool Valid { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string Rating { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string WordList { get; set; }
    }
}
