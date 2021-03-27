using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Data;
using Toeicking2021.ViewModels;

namespace Toeicking2021.Services.SentenceDBService
{
    public class SentenceDBService:ISentenceDBService
    {
        private readonly DataContext _context;

        public SentenceDBService(DataContext context)
        {
            _context = context;
        }

        public async Task<string> AddSentenceGroup(SentenceInputVM data) 
        {
            string errorMesssage="成功";
            // 資料庫交易
            using (var transaction = _context.Database.BeginTransaction()) 
            {
                try
                {                    
                    // SentenceId(現有rows加1)要存在一個變數，與其它資料表共用(FK也是要填值存進DB)
                    int SentenceId = _context.Sentences.Count() + 1;
                    // 先存Sentence
                    data.Sentence.SentenceId = SentenceId;
                    // 寫入資料庫日期
                    data.Sentence.AddedDate = DateTime.Now;
                    await _context.Sentences.AddAsync(data.Sentence);
                    // 存Vocabulary
                    // 計算每個欄位的數量
                    int vocCount = data.Vocs.Select(v => v.Voc).ToList().Count;
                    int categoryCount = data.Vocs.Select(v => v.Category).ToList().Count;
                    int chineseCount = data.Vocs.Select(v => v.Chinese).ToList().Count;
                    // 每個欄位的數量要一樣才能存DB
                    if (vocCount==categoryCount&&categoryCount==chineseCount)
                    {
                        for (int i = 0; i < vocCount; i++)
                        {
                            // 將SentenceId存進每筆
                            data.Vocs[i].SentenceId = SentenceId;
                            await _context.Vocabularies.AddAsync(data.Vocs[i]);
                        }
                    }
                    else
                    {
                        errorMesssage = "字彙欄位有空白未填";
                    }
                    // 存GA
                    int GACount = data.GAs.Count;
                    // 有大於1筆才存DB
                    if (GACount>0)
                    {
                        for (int i = 0; i < GACount; i++)
                        {
                            data.GAs[i].SentenceId = SentenceId;
                            await _context.GAs.AddAsync(data.GAs[i]);
                        }
                    }
                    else
                    {
                        errorMesssage = "沒有填任何文法解析";
                    }
                    // 存VA
                    int VACount = data.VAs.Count;
                    if (VACount > 0)
                    {
                        for (int i = 0; i < VACount; i++)
                        {
                            data.VAs[i].SentenceId = SentenceId;
                            await _context.VAs.AddAsync(data.VAs[i]);
                        }
                    }
                    else
                    {
                        errorMesssage = "沒有填任何字彙解析";
                    }

                    // 到這裡都沒有exception error才會全部寫入資料庫
                    transaction.Commit();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    errorMesssage = "Sentence資料表出錯或其它未預期錯誤";
                }
            }
            return errorMesssage;


        }











    }


}
