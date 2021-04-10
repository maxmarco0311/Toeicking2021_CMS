using Microsoft.EntityFrameworkCore;
using System;
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
                    // 出現錯誤，資料庫回復之前的狀況
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

        #region 依句子編號取出文法解析
        public async Task<List<GA>> GetGrammarsBySentenceId(int sentenceId)
        {
            return await _context.GAs.Where(g => g.SentenceId == sentenceId).ToListAsync();
            //var test = _context.GAs.Where(g => g.SentenceId == sentenceId && g.Sentence.HasAudio==true).ToList();
        }
        #endregion

        #region 依句子編號取出字彙解析
        public async Task<List<VA>> GetVocAnalysesBySentenceId(int sentenceId)
        {
            return await _context.VAs.Where(va => va.SentenceId == sentenceId).ToListAsync();
        }
        #endregion

        #region 依句子編號取出字彙
        public async Task<List<Vocabulary>> GetVocabularyBySentenceId(int sentenceId)
        {
            return await _context.Vocabularies.Where(v => v.SentenceId == sentenceId).ToListAsync();
        }
        #endregion

        #region 更新句子
        public async Task<string> UpdateSentence(int sentenceId, string sen, string chinese)
        {
            string result = "";
            Sentence sentence = _context.Sentences.FirstOrDefault(s => s.SentenceId == sentenceId);
            if (sen!=null)
            {
                try
                {
                    sentence.Sen = sen;
                    sentence.Chinesese = chinese;
                    _context.Sentences.Update(sentence);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                result = "1";
            }
            else
            {
                result = "0";
            }
            return result;
        }
        #endregion

        #region 更新文法解析
        public async Task<string> UpdateGrammars(List<GA> grammars)
        {
            string result = "";
            using (var transaction = _context.Database.BeginTransaction()) 
            {
                try
                {
                    foreach (var item in grammars)
                    {
                        GA grammar = _context.GAs.FirstOrDefault(ga => ga.AnalysisId == item.AnalysisId);
                        if (grammar != null)
                        {
                            grammar.Analysis = item.Analysis;
                            _context.GAs.Update(grammar);
                            await _context.SaveChangesAsync(); 
                        }
                        else
                        {
                            result = "0";
                        }

                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result = ex.Message;
                }
                
            }     
            result = "1";
            return result;

        }
        #endregion

        #region 更新字彙解析
        public async Task<string> UpdateVocAnalysis(List<VA> vocAnalyses)
        {
            string result = "";
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in vocAnalyses)
                    {
                        VA vocAnalysis = _context.VAs.FirstOrDefault(va => va.AnalysisId == item.AnalysisId);
                        if (vocAnalysis != null)
                        {
                            vocAnalysis.Analysis = item.Analysis;
                            _context.VAs.Update(vocAnalysis);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            result = "0";
                        }

                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result = ex.Message;
                }

            }
            result = "1";
            return result;

        }
        #endregion

        #region 更新字彙
        public async Task<string> UpdateVoc(List<Vocabulary> vocabularies)
        {
            string result = "";
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in vocabularies)
                    {
                        Vocabulary vocabulary = _context.Vocabularies.FirstOrDefault(v => v.VocabularyId == item.VocabularyId);
                        if (vocabulary != null)
                        {
                            vocabulary.Voc = item.Voc;
                            vocabulary.Category = item.Category;
                            vocabulary.Chinese = item.Chinese;
                            _context.Vocabularies.Update(vocabulary);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            result = "0";
                        }

                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result = ex.Message;
                }

            }
            result = "1";
            return result;

        }
        #endregion

        #region 檢查次數加1
        public async Task<string> AddCheckTime(int sentenceId)
        {
            string result;
            try
            {
                Sentence sentence = await _context.Sentences.FindAsync(sentenceId);
                if (sentence!=null)
                {
                    sentence.CheckedTimes++;
                    _context.Sentences.Update(sentence);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    result = "0";
                }
                result = "1";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion





    }


}
