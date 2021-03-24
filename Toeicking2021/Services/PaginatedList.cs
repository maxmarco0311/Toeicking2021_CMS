using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Services
{
    // 繼承List<T>：1. 可使用AddRange()方法 2. 類別裡不用宣告List<T>，本身類別(物件)就會是個List<T>(也可有其他屬性，因為是繼承)
    // T型別就是查回的資料物件型別
    public class PaginatedList<T> : List<T>
    {
        #region 基本屬性
        // 目前頁數(可用來判斷並製造頁碼)
        public int PageIndex { get; private set; }
        // 總頁數(可用來判斷並製造頁碼)
        public int TotalPages { get; private set; }
        #endregion

        #region 物件建構式
        // 傳入參數去運算類別屬性的值，在該類別的靜態方法中實體化並回傳
        // items--> 查出的資料物件集合
        // count--> 資料總筆數
        // pageIndex--> 目前頁數(外部傳入，設定類別屬性PageIndex的值)
        // pageSize--> 每頁顯示幾筆(外部傳入，計算類別屬性TotalPages的值)
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            // 設定目前頁數
            PageIndex = pageIndex;
            // 計算出總頁數
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            // AddRange(items)後，類別物件就含有List<T>+四個屬性(PageIndex, TotalPages, HasPreviousPage, HasNextPage)
            // 在View中，Model跑迴圈時，Model就是List<T>，Model.類別屬性可取到屬性值
            this.AddRange(items);
        }
        #endregion

        #region getter
        // 是否有前一頁，透過getter回傳
        public bool HasPreviousPage { get { return (PageIndex > 1); } }
        // 是否有下一頁，透過getter回傳
        public bool HasNextPage { get { return (PageIndex < TotalPages); } }
        #endregion

        #region 類別靜態方法
        // 從外部傳入參數後運算取得此類別建構式的參數值，實體化此類別並回傳給View顯示頁面
        // source(IQueryable<T>)--> EF Core查詢物件，在方法裡執行資料庫查詢(可以是where條件式篩選過的IQueryable<T>，T型別不會變)
        // pageIndex--> 目前頁數
        // pageSize--> 每頁顯示幾筆
        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            // 計算資料總筆數
            //var count = await source.CountAsync();
            var count = source.Count();
            // 查出目前頁數的資料物件集合
            //var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            // 實體化此類別物件並回傳(給View)
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        #endregion


    }

}
