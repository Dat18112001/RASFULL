(function ($) {
    'use strict';

    var initFunc = function () {
        $('.cls-money').each(function () {
            var currentText = $(this).text();
            var moneyFormat = formatCurrencyVND(currentText);
            $(this).text(moneyFormat);
        })
    }

    var truncateText = function (length) {
        $('.truncate-text').each(function () {
            var currentText = $(this).text();
            
            if (currentText.length > length){
                currentText = currentText.substring(0, length) + '...'
            }
            
            $(this).text(currentText.trim());
        })
    }
    
    //Load functions
    $(document).ready(function () {
        setTimeout(function () {
            initFunc();
            truncateText(30);
        }, 500)
    });

})(jQuery);