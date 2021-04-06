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
            // 有可能只會回傳其中一個predicate，所以全部predicate變數都是宣告全域
            // 最外層的predicate，可能會合併其他predicate
            var OuterPredicate = PredicateBuilder.New<Sentence>();
            // 巢狀的predicate，通常內部條件是Or，然後再用And的方式併入OuterPredicate，也有可能不會合併(OuterPredicate沒有加入條件)
            var InnerPredicate_Grammar = PredicateBuilder.New<Sentence>();
            var InnerPredicate_Part = PredicateBuilder.New<Sentence>();
            // 處理布林值的predicate
            var InnerPredicate_Booleans = PredicateBuilder.New<Sentence>();
            // 1. 篩選編號
            if (FormData.SenNum != null)
            {
                // 檢查OuterPredicate是否還沒加入條件
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
            // 2. 篩選關鍵字
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
            // 3. 篩選日期
            if (FormData.AddedDate != null)
            {
                int dateSpan = Convert.ToInt16(FormData.AddedDate);
                // 查當天存入
                if (dateSpan==0)
                {
                    if (OuterPredicate.IsStarted == false)
                    {
                        // 查詢時間一定要有"區間"，"當天"是"DateTime.Now.AddDays(-1)"到"DateTime.Now"
                        // 時間區間的開頭(就是DateTime.Now.AddDays()的參數)應為N天前的值-1，也就是N-1(通常是負數)
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
            // 4. 篩選檢查次數(int?)
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
            // 5. 篩選情境
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
            // 6. 篩選文法分類(巢狀predicate)
            if (FormData.GrammarCategories != null)
            {
                // 轉成value值陣列
                string[] GrammarCategories = MultiSelectHelper.TransferGrammarCategories(FormData.GrammarCategories).Split(",");
                // predicate中的lambda條件若使用陣列或集合索引取值GrammarCategories[i]，最終會報錯，使用foreach迴圈不會！
                foreach (var item in GrammarCategories)
                {
                    // 檢查InnerPredicateGrammar是否還沒加入條件
                    if (InnerPredicate_Grammar.IsStarted==false)
                    {
                        
                        InnerPredicate_Grammar = InnerPredicate_Grammar.Start(t => t.GrammarCategory.Contains(item));
                    }
                    else
                    {
                        // 要眾多條件符合其中一個就撈出，要用Or()，用And()是全符合才撈出
                        InnerPredicate_Grammar = InnerPredicate_Grammar.Or(t => t.GrammarCategory.Contains(item));
                    }
                    
                }

            }
            // 7. 篩選大題(巢狀predicate)
            if (FormData.Part != null)
            {
                // 轉成value值陣列
                string[] PartCategories = MultiSelectHelper.TransferPartCategories(FormData.Part).Split(",");
                foreach (var item in PartCategories)
                {
                    // 檢查InnerPredicateGrammar是否還沒加入條件
                    if (InnerPredicate_Part.IsStarted==false)
                    {
                        InnerPredicate_Part = InnerPredicate_Part.Start(t => t.Part.Contains(item));
                    }
                    else
                    {
                        InnerPredicate_Part = InnerPredicate_Part.Or(t => t.Part.Contains(item));
                    }
                    
                }
             
            }
            // 8. 篩選布林值(表單值不等於disabled才代表要加入布林值條件篩選)
            if (FormData.BoolConditions != "disabled")
            {
                // 聯集篩選
                if (FormData.BoolConditions == "union")
                {
                    // 加在自己的predicate後(用Or加條件式)
                    InnerPredicate_Booleans = InnerPredicate_Booleans.Start(t => t.WordOrigin == FormData.HasWordOrigin);
                    InnerPredicate_Booleans = InnerPredicate_Booleans.Or(t => t.Synonym == FormData.HasSynonym);
                    InnerPredicate_Booleans = InnerPredicate_Booleans.Or(t => t.HasAudio == FormData.HasAudio);

                }
                // 交集篩選(用And加條件式)
                else if (FormData.BoolConditions == "intersection")
                {
                    // 直接加在OuterPredicate，檢查是否已有條件式
                    if (OuterPredicate.IsStarted==false)
                    {
                        OuterPredicate = OuterPredicate.Start(t => t.WordOrigin == FormData.HasWordOrigin);
                    }
                    else
                    {
                        OuterPredicate = OuterPredicate.And(t => t.WordOrigin == FormData.HasWordOrigin);

                    }
                    OuterPredicate = OuterPredicate.And(t => t.Synonym == FormData.HasSynonym);
                    OuterPredicate = OuterPredicate.And(t => t.HasAudio == FormData.HasAudio);

                }

            }
            // 所有predicate合併判斷(放在最後面，不然會有預期外的錯誤)
            // 判斷原則：要往OuterPredicate(最上層的predicate)合併，若OuterPredicate沒有條件式，再往下一層(InnerPredicate_Grammar)合併，以此類推
            // 若OuterPredicate已有條件式
            if (OuterPredicate.IsStarted==true)
            {
                // 且InnerPredicate_Grammar已有條件式
                if (InnerPredicate_Grammar.IsStarted==true)
                {
                    // 將InnerPredicate_Grammar合併進OuterPredicate
                    OuterPredicate = OuterPredicate.And(InnerPredicate_Grammar);
                }
                // 且InnerPredicate_Part已有條件式
                if (InnerPredicate_Part.IsStarted==true)
                {
                    // 將InnerPredicate_Part合併進OuterPredicate
                    OuterPredicate = OuterPredicate.And(InnerPredicate_Part);

                }
                // 且InnerPredicate_Booleans已有條件式
                if (InnerPredicate_Booleans.IsStarted==true)
                {
                    // 將InnerPredicate_Booleans合併進OuterPredicate
                    OuterPredicate = OuterPredicate.And(InnerPredicate_Booleans);

                }

            }
            // 若OuterPredicate沒有條件式
            else
            {
                // 並且InnerPredicate_Grammar已有條件式
                if (InnerPredicate_Grammar.IsStarted==true)
                {
                    // 並且InnerPredicate_Part已有條件式
                    if (InnerPredicate_Part.IsStarted==true)
                    {
                        // 將InnerPredicate_Part合併進InnerPredicate_Grammar
                        InnerPredicate_Grammar = InnerPredicate_Grammar.And(InnerPredicate_Part);
                    }
                    // 並且InnerPredicate_Booleans已有條件式
                    if (InnerPredicate_Booleans.IsStarted==true)
                    {
                        // 將InnerPredicate_Booleans合併進InnerPredicate_Grammar
                        InnerPredicate_Grammar = InnerPredicate_Grammar.And(InnerPredicate_Booleans);
                    }
                }
                // 並且InnerPredicate_Grammar也沒有條件式
                else
                {
                    // 並且InnerPredicate_Part已有條件式
                    if (InnerPredicate_Part.IsStarted==true)
                    {
                        // 並且InnerPredicate_Booleans已有條件式
                        if (InnerPredicate_Booleans.IsStarted==true)
                        {
                            // 將InnerPredicate_Booleans合併進InnerPredicate_Part
                            InnerPredicate_Part = InnerPredicate_Part.And(InnerPredicate_Booleans);
                        }
                    }
                }
            }
            // 最後回傳判斷
            // 若OuterPredicate還沒加入條件
            if (OuterPredicate.IsStarted == false)
            {
                // 且InnerPredicateGrammar也還沒加入條件
                if (InnerPredicate_Grammar.IsStarted == false)
                {
                    // InnerPredicatePart也沒加入條件
                    if (InnerPredicate_Part.IsStarted==false)
                    {
                        if (InnerPredicate_Booleans.IsStarted==false)
                        {
                            // 沒有進行任何篩選回傳null
                            return null;
                        }
                        return InnerPredicate_Booleans;
                        
                    }
                    // InnerPredicatePart已加入條件則回傳InnerPredicatePart
                    return InnerPredicate_Part;
                }
                // 若OuterPredicate還沒加入條件，但InnerPredicateGrammar已加入條件
                else
                {
                    // 則回傳InnerPredicateGrammar
                    return InnerPredicate_Grammar;
                }
            }
            // 若OuterPredicate已加入條件則回傳OuterPredicate
            return OuterPredicate;


        }
        #endregion



    }
}
