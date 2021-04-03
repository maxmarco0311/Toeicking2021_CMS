using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toeicking2021.Data;
using Toeicking2021.Models;
using Toeicking2021.Services.SentenceDBService;

namespace Toeicking2021.Utilities
{
    public class DynamicPredicateHelper
    {
       
        public DynamicPredicateHelper(){}

        #region 回傳動態條件式predicate
        public static ExpressionStarter<Sentence> SentenceDynamicPredicate(TableQueryFormData FormData)
        {
            // 建立predicate變數，也就是where()的Lambda參數，New<T>的泛型是要查出的資料物件型別
            var OuterPredicate = PredicateBuilder.New<Sentence>();
            // 巢狀的predicate，通常內部條件是Or，然後再用And的方式併入OuterPredicate
            var InnerPredicateGrammar = PredicateBuilder.New<Sentence>();
            var InnerPredicatePart = PredicateBuilder.New<Sentence>();
            // 篩選編號
            if (FormData.SenNum != null)
            {
                // 如果OuterPredicate還沒開始
                if (OuterPredicate.IsStarted == false)
                {
                    // 用Start()將第一個條件加入OuterPredicate，才可合併其它InnerPredicate(t就是要查詢的資料物件型別)
                    OuterPredicate = OuterPredicate.Start(t => t.SentenceId == Convert.ToInt32(FormData.SenNum));
                }
                else
                {
                    // 若OuterPredicate已開始，則要使用And()
                    OuterPredicate = OuterPredicate.And(t => t.SentenceId == Convert.ToInt32(FormData.SenNum));
                }
                
            }
            // 篩選關鍵字
            if (FormData.Keyword != null)
            {
                if (OuterPredicate.IsStarted==false)
                {
                    // StringComparison.InvariantCultureIgnoreCase表比較字串時"不區別文化特性也不區別大小寫"
                    // 但目前使用的EF Core不支援，只能用ToLower()
                    OuterPredicate = OuterPredicate.Start(t => t.Sen.ToLower().Contains(FormData.Keyword.ToLower().Trim()));
                }
                else
                {
                    OuterPredicate = OuterPredicate.And(t => t.Sen.ToLower().Contains(FormData.Keyword.ToLower().Trim()));
                }  
                
            }
            // 篩選日期
            if (FormData.AddedDate != null)
            {
                int dateSpan = Convert.ToInt16(FormData.AddedDate);
                // 查當天存入
                if (dateSpan==0)
                {
                    if (OuterPredicate.IsStarted == false)
                    {
                        OuterPredicate = OuterPredicate.Start(t => t.AddedDate >= DateTime.Now.AddDays(dateSpan - 1)
                                               && t.AddedDate <= DateTime.Now);
                    }
                    else
                    {
                        OuterPredicate = OuterPredicate.And(t => t.AddedDate >= DateTime.Now.AddDays(dateSpan - 1)
                                               && t.AddedDate <= DateTime.Now);
                    }
                    
                }
                // 前3天存入
                else if (dateSpan==-3)
                {
                    if (OuterPredicate.IsStarted == false)
                    {
                        OuterPredicate = OuterPredicate.Start(t => t.AddedDate >= DateTime.Now.AddDays(dateSpan - 1)
                                               && t.AddedDate <= DateTime.Now.AddDays(dateSpan));
                    }
                    else
                    {
                        OuterPredicate = OuterPredicate.And(t => t.AddedDate >= DateTime.Now.AddDays(dateSpan - 1)
                                                                       && t.AddedDate <= DateTime.Now.AddDays(dateSpan));
                    }
                    
                }
                // 幾天前存入
                else
                {
                    if (OuterPredicate.IsStarted == false)
                    {
                        OuterPredicate = OuterPredicate.Start(t => t.AddedDate >= DateTime.Now.AddDays(dateSpan)
                                               && t.AddedDate <= DateTime.Now);
                    }
                    else
                    {
                        OuterPredicate = OuterPredicate.And(t => t.AddedDate >= DateTime.Now.AddDays(dateSpan)
                                                                       && t.AddedDate <= DateTime.Now);
                    }
                    
                }
                
            }
            // 篩選檢查次數(int?)
            if (FormData.CheckedTimes != null)
            {
                if (OuterPredicate.IsStarted == false)
                {
                    OuterPredicate = OuterPredicate.Start(t => t.CheckedTimes == FormData.CheckedTimes);
                }
                else
                {
                    OuterPredicate = OuterPredicate.And(t => t.CheckedTimes == FormData.CheckedTimes);
                }
                
            }
            // 篩選情境
            if (FormData.Context != null)
            {
                if (OuterPredicate.IsStarted == false)
                {
                    OuterPredicate = OuterPredicate.Start(t => t.Context == FormData.Context);
                }
                else
                {
                    OuterPredicate = OuterPredicate.And(t => t.Context == FormData.Context);
                }
                
            }
            //// 篩選字首字根(布林值不可為null)
            //predicate = predicate.And(t => t.WordOrigin == FormData.HasWordOrigin);
            //// 篩選同義字(布林值不可為null)
            //predicate = predicate.And(t => t.Synonym == FormData.HasSynonym);
            //// 篩選音檔(布林值不可為null)
            //predicate = predicate.And(t => t.HasAudio == FormData.HasAudio);
            // 篩選文法分類
            if (FormData.GrammarCategories != null)
            {
                // 轉成value值陣列
                string[] GrammarCategories = MultiSelectHelper.TransferGrammarCategories(FormData.GrammarCategories).Split(",");
                
                //// for迴圈將條件加入InnerPredicateGrammar
                //for (int i = 0; i < GrammarCategories.Length; i++)
                //{
                //    if (i == 0)
                //    {
                //        // 用Start()將第一個條件加入InnerPredicateGrammar
                //        InnerPredicateGrammar = InnerPredicateGrammar.Start(t => t.GrammarCategory.Contains(GrammarCategories[i]));
                //    }
                //    else
                //    {
                //        // 是Or條件(符合其中一項就列出)
                //        InnerPredicateGrammar = InnerPredicateGrammar.Or(t => t.GrammarCategory.Contains(GrammarCategories[i]));
                //    }
                //}
                foreach (var item in GrammarCategories)
                {
                    if (InnerPredicateGrammar.IsStarted==false)
                    {
                        // 要眾多條件符合其中一個就撈出，要用Or()，用And()是全符合才撈出
                        InnerPredicateGrammar = InnerPredicateGrammar.Start(t => t.GrammarCategory.Contains(item));
                    }
                    else
                    {
                        InnerPredicateGrammar = InnerPredicateGrammar.Or(t => t.GrammarCategory.Contains(item));
                    }
                    
                }
                if (OuterPredicate.IsStarted==true)
                {
                    // 最後要用And()的參數與OuterPredicate合併
                    OuterPredicate = OuterPredicate.And(InnerPredicateGrammar);
                }
                
            }
            // 篩選大題
            if (FormData.Part != null)
            {
                // 轉成value值陣列
                string[] PartCategories = MultiSelectHelper.TransferPartCategories(FormData.Part).Split(",");
                
                //for (int i = 0; i < PartCategories.Length; i++)
                //{
                //    if (i == 0)
                //    {
                //        // 用Start()將第一個條件加入InnerPredicatePart
                //        InnerPredicatePart = InnerPredicatePart.Start(t => t.GrammarCategory.Contains(PartCategories[i]));
                //    }
                //    else
                //    {
                //        // 是Or條件(符合其中一項就列出)
                //        InnerPredicatePart = InnerPredicatePart.Or(t => t.GrammarCategory.Contains(PartCategories[i]));
                //    }
                //}
                foreach (var item in PartCategories)
                {
                    if (InnerPredicatePart.IsStarted==false)
                    {
                        InnerPredicatePart = InnerPredicatePart.Start(t => t.Part.Contains(item));
                    }
                    else
                    {
                        InnerPredicatePart = InnerPredicatePart.Or(t => t.Part.Contains(item));
                    }
                    
                }
                if (OuterPredicate.IsStarted==true)
                {
                    // 最後要用And()的參數與OuterPredicate合併
                    OuterPredicate = OuterPredicate.And(InnerPredicatePart);
                }
                
            }
            if (OuterPredicate.IsStarted == false)
            {
                if (InnerPredicateGrammar.IsStarted==true)
                {
                    return InnerPredicateGrammar;
                }
                else if (InnerPredicatePart.IsStarted==true)
                {
                    return InnerPredicatePart;
                }
            }
            return OuterPredicate;


        }
        #endregion



    }
}
