﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Data;
using Toeicking2021.Models;
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

        #region 儲存句子
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
                    // 1.先存Sentence
                    // 計算SentenceId
                    data.Sentence.SentenceId = SentenceId;
                    // 寫入資料庫日期(DB資料型別為date，就只會存年月日)
                    data.Sentence.AddedDate = DateTime.Now;
                    // 尚未生成語音檔
                    data.Sentence.HasAudio = false;
                    await _context.Sentences.AddAsync(data.Sentence);
                    await _context.SaveChangesAsync();
                    // 2.存Vocabulary
                    // 計算每個欄位的數量
                    int vocCount = data.Vocs.Select(v => v.Voc).ToList().Count;
                    int categoryCount = data.Vocs.Select(v => v.Category).ToList().Count;
                    int chineseCount = data.Vocs.Select(v => v.Chinese).ToList().Count;
                    // 每個欄位的數量要一樣才能存DB
                    if (vocCount == categoryCount && categoryCount == chineseCount)
                    {
                        for (int i = 0; i < vocCount; i++)
                        {
                            // 將SentenceId存進每筆
                            data.Vocs[i].SentenceId = SentenceId;
                            // 自身資料表的PK也要生出來存
                            data.Vocs[i].VocabularyId = _context.Vocabularies.Count() + 1;
                            await _context.Vocabularies.AddAsync(data.Vocs[i]);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        errorMesssage = "字彙欄位有空白未填";
                    }
                    // 3.存GA
                    int GACount = data.GAs.Count;
                    // 有大於1筆才存DB
                    if (GACount>0)
                    {
                        for (int i = 0; i < GACount; i++)
                        {
                            data.GAs[i].SentenceId = SentenceId;
                            data.GAs[i].AnalysisId = _context.GAs.Count() + 1;
                            await _context.GAs.AddAsync(data.GAs[i]);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        errorMesssage = "沒有填任何文法解析";
                    }
                    // 4.存VA
                    int VACount = data.VAs.Count;
                    if (VACount > 0)
                    {
                        for (int i = 0; i < VACount; i++)
                        {
                            data.VAs[i].SentenceId = SentenceId;
                            data.VAs[i].AnalysisId = _context.VAs.Count() + 1;
                            await _context.VAs.AddAsync(data.VAs[i]);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        errorMesssage = "沒有填任何字彙解析";
                    }

                    // 到這裡都沒有exception error才會全部寫入資料庫
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    // 出現資料庫，資料庫回復之前的狀況
                    transaction.Rollback();
                    errorMesssage = ex.Message;
                }
            }
            return errorMesssage;


        }
        #endregion

        #region 刪除句子
        public async Task<string> DeleteSenetnce(int id) 
        {
            string resultMessage = "";
            try
            {
                Sentence sentenceToDelte= await _context.Sentences.FindAsync(id);
                _context.Sentences.Remove(sentenceToDelte);
                await _context.SaveChangesAsync();
                resultMessage = "刪除成功";
            }
            catch (Exception)
            {
                resultMessage = "刪除錯誤";
            }
            return resultMessage;
        }
        #endregion

        #region 回傳Sentence資料表的IQueryable<>物件
        public IQueryable<Sentence> TableAsQueryable() 
        {
            return _context.Sentences.AsQueryable();
        }
        #endregion




    }


}
