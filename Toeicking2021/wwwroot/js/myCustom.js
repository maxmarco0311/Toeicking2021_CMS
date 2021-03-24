
$(function () {
    // 使用文字切換
    //var showPassword = false;
    //$(".field-icon").on("click", function () {
    //    if (!showPassword) {
    //        showPassword = !showPassword;
    //        $(this).text("隱藏");
    //        $(".password").attr("type", "text");
    //    } else if (showPassword) {
    //        showPassword = !showPassword;
    //        $(this).text("顯示");
    //        $(".password").attr("type", "password");
    //    }

    //});

    // 使用icon切換
    $(".password-eye").mousedown(function () {
        $(this).removeClass("fa-eye").addClass("fa-eye-slash");
        $(this).prev().find(".password").attr("type", "text");
    }).mouseup(function () {
        $(this).removeClass("fa-eye-slash").addClass("fa-eye");
        $(this).prev().find(".password").attr("type", "password");
    }).mouseout(function () {
        $(this).removeClass("fa-eye-slash").addClass("fa-eye");
        $(this).prev().find(".password").attr("type", "password");
    });

});