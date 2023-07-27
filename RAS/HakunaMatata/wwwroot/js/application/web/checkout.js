(function ($) {
    'use strict';

    var processCheckout = function () {
        $('#btn-submit-order').on('click', function (e) {
            e.preventDefault();
            var data = {
                PaymentMethod: $('input[name=Payment]:checked').val(),
                TotalAmount:$('#Coin').val()
            }

            if (parseInt(data.TotalAmount) >= 20000){
                coreAjax(
                    true
                    , '/AdminArea/Payment/ProcessCheckout'
                    , JSON.stringify(data)
                    , 'POST'
                    , function (res) {
                        localStorage.setItem('TotalAmount', JSON.stringify(data))
                        setTimeout(function () {
                            window.location.href = res;
                        }, 500)
                    }
                    , function () { }
                );
            }else{
                alert('Số tiền phải lớn hơn hoặc bằng 20.000 VND')
            }
            
        });
    }
    
    //Load functions
    $(document).ready(function () {
        processCheckout();
    });

})(jQuery);