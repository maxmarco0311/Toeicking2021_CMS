using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Models;

namespace Toeicking2021.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
        // 資料表類別檔屬性
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Sentence> Sentences { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<GA> GAs { get; set; }
        public DbSet<VA> VAs { get; set; }

    }
}
