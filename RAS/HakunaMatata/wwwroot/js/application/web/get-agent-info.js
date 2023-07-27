(function ($) {
    'use strict';
    
    var getAgentInfo = function () {
        coreAjax(
            true
            , '/AdminArea/Agent/GetInfo'
            , null
            , 'GET'
            , function (res) {
                $('#userCoinElement').text('Coin: ' + res.coin);
                $('#userPackageElement').text('Gói hiện tại: ' + res.package + ' tháng');
            }
            , function () { }
        );
    }
    
    //Load functions
    $(document).ready(function () {
        getAgentInfo();
    });

})(jQuery);