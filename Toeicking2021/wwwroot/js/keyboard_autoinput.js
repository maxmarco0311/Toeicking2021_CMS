// 特定鍵盤自動輸入文字
// 先找有focus的文字框
$(".vocsupplement").focus(function () {
    // 先將focus的文字框物件(currentObj)存起來，要當參數傳進函式中
    var currentObj = $(this);
    // 在有focus的文字框(currentObj)keydown事件中檢查是否按下特定鍵盤按鍵(要用到e參數)
    currentObj.keydown(function (e) {
        var elem = (e.target) ? e.target : ((e.srcElement) ? e.srcElement : null);
        var charCode = (e.charCode) ? e.charCode : ((e.which) ? e.which : e.keyCode);
        switch (charCode) {
            // 111為數字鍵盤中的"/"
            case 111:
                var addedValue = "的同(近)義字有：";
                alert(addedValue);
                autoEnter(currentObj, e, addedValue);
                alert("函式跑完了");
                addedValue = "";
                break;
            // 106為數字鍵盤中的"*"
            case 106:
                var addedValue = "也可當名詞用，意思為「」";
                alert(addedValue);
                autoEnter(currentObj, e, addedValue);
                alert("函式跑完了");
                addedValue = "";
                break;
            // 109為數字鍵盤中的"-"
            case 109:
                var addedValue = "也可當動詞用，意思為「」";
                alert(addedValue);
                autoEnter(currentObj, e, addedValue);
                alert("函式跑完了");
                addedValue = "";
                break;
            default:
                break;

        }
        // 獨立成函式
        // 需傳入目前focus的文字框物件(currentObj)，keydown事件的e參數
        function autoEnter(currentObj, e, addedValue) {
            // 取得文字框目前的值
            var existingValue = currentObj.val();
            var newValue = existingValue + addedValue;
            // 需使用currentObj，不是用$(this)
            currentObj.val(newValue);
            //更改預設行為(不然會輸入原來的字元)
            e.preventDefault();
            //阻止事件向上層標籤傳播
            e.stopPropagation();

        }

    });

});