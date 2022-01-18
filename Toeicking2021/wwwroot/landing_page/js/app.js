$(function () {
    // 啟用flexslider
    $('.flexslider').flexslider({
        animation: "fade",
        touch: true,
        directionNav: false
    });
    //啟用wow
    var wow = new WOW({
        boxClass: 'max',
        animateClass: 'animated',
        offset: 0,
        mobile: true,
        live: true
    });
    wow.init();

    // 滑過jumbotron導覽列出現
    $(window).scroll(function (e) {
        var scrollTop = $(window).scrollTop();
        var headerHeight = $("#header").height();
        if (scrollTop > headerHeight)
            $(".appear").css("opacity", "1");
        else if (scrollTop < headerHeight)
            $(".appear").css("opacity", "0");

    });

    // 文字顯示動畫
    var titleAnimation = function() {

        document.querySelector(".normal-color").textContent = "";
        var heading = "克服多國腔調，多益聽力練習神器!!!";
        var i = 0;
        //每次進入typing函式就會執行一次setTimeout()
        var typing = function() {
            if (i < heading.length) {
                document.querySelector(".normal-color").textContent += heading.charAt(i);
                i++;
                // 若setTimeout()的位置就在其傳入的函式參數中，setTimeout()會一直不停呼叫，需設中止條件
                setTimeout(typing, 300);
            }
        };
        typing();

    };
    titleAnimation();
    setInterval(titleAnimation, 8000);


    // scroll效果
    // 監聽所有href以#開頭的a標籤的click事件
    $(document).on('click', 'a[href^="#"]', function (e) {
        // 漢堡選單裡a點選時選單會消失
        e.preventDefault();
        $('.navbar-toggler').addClass('collapsed');
        $('#navbarSupportedContent').removeClass('show');
        // scroll到id區塊時要扣掉navbar的高度。 
        var navHeight = $(".navbar").height();
        $('html, body').animate({
            scrollTop: $($.attr(this, 'href')).offset().top - (navHeight+1)
        }, 1000);

    });

});

//手機hover功能
//$("#header .container .header-wrapper a").on("touchstart", function () {
//    $(this).addClass("header-a-cellhover");
//});
//$("#header .container .header-wrapper a").on("touchend", function () {
//    $(this).removeClass("header-a-cellhover");
//});

$(".features-left .feature-single, .features-right .feature-single").on("touchstart", function () {

    $(".features-left .feature-single .feature-icon span, .features-right .feature-single .feature-icon span").addClass("feature-icon-span-cellhover");
    $(".features-left .feature-single .feature-icon span:before, .features-right .feature-single .feature-icon span:before").addClass(".feature-icon-span-before-cellhover");


});
$(".features-left .feature-single, .features-right .feature-single").on("touchend", function () {

    $(".features-left .feature-single .feature-icon span, .features-right .feature-single .feature-icon span").removeClass("feature-icon-span-cellhover");
    $(".features-left .feature-single .feature-icon span:before, .features-right .feature-single .feature-icon span:before").removeClass(".feature-icon-span-before-cellhover");


});

