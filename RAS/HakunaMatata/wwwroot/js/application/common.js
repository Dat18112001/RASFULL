var openModalCheckOrderWeb = function () {
    $('#OrderCode').val('');
    $('#modal-popup-order-detail-check').modal('show');
}

var formatMoney = function (number) {
    return number.toLocaleString('en-US', { style: 'currency', currency: 'USD' }).replaceAll('$', '').split('.')[0];
}

var getDataCart = function () {
    if (!localStorage.getItem('cart')) {
        return [];
    } else {
        return JSON.parse(localStorage.getItem('cart'));
    }
}

var setCartLocal = function (value, productId) {
    var tempCart = JSON.parse(localStorage.getItem('cart'));

    for (var element of tempCart) {
        if (productId === element.ProductId) {
            element.Quantity = value;
            break;
        }
    }

    tempCart = tempCart.filter(function (e) {
        return e.Quantity > 0;
    })

    localStorage.setItem('cart', JSON.stringify(tempCart));

    setTimeout(function () {
        coreAjaxPostPartialView(true, '/Cart/GetCart', localStorage.getItem('cart'), function (response) {
            $("#your-cart").html(response);
        }, function () {

        });
    }, 500);
}

var countValue = function (value, array) {
    var count = 0;
    $.each(array, function (i, v) {
        if (v.toLowerCase() == value.toLowerCase()) count++;
    });
    return count;
}

var validationForm = function (formId) {
    var isValid = true;

    $('#' + formId).find('.required').each(function () {
        var val = $(this).find('input').val()
        if (!val) {
            $(this).find('span').show();
            isValid = false;
        } else {
            $(this).find('span').hide();
        }
    });

    return isValid;
}

var onClickLink = function (isNewTab, href) {
    if (isNewTab)
        window.open(
            href,
            '_blank'
        );
    else
        window.location.href = href
}

var hiddenTextRequired = function (formId) {
    $('#' + formId).find('.required').each(function () {
        $(this).find('span').hide();
    })
}

var getFormDataJson = function (formId) {
    var unindexed_array = $('#' + formId).serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

var signInOrSignUp = function (isLogin) {
    if (isLogin) {
        hiddenTextRequired('frmLogin');
        $('#modal-sign-in').modal('show');
    } else {
        hiddenTextRequired('frmSubmit');
        $('#modal-sign-up').modal('show');
    }
}

var toastMessage = function (code, text) {
    var header, bgColor, icon, textColor;

    switch (code) {
        case 'success':
            header = 'Success';
            bgColor = '#3c763d';
            icon = 'success';
            textColor = '#f1f1f1';
            break;
        case 'error':
            header = 'Error';
            bgColor = '#F73F52';
            icon = 'error';
            textColor = '#f1f1f1';
            break;
        case 'warning':
            header = 'warning';
            bgColor = '#fcf8e3';
            icon = 'warning';
            textColor = '#8a6d3b';
            break;
        case 'info':
            header = 'Infomation';
            bgColor = '#d9edf7';
            icon = 'info';
            textColor = '#31708f';
            break;
    }

    $.toast({
        text: `${text}`,
        position: 'top-right',
        heading: `${header}`,
        bgColor: `${bgColor}`,
        icon: `${icon}`,
        textColor: `${textColor}`,
        hideAfter: 6000
    });
}

var getParamFromUrl = function (name, url) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(url);

    if (results == null) {
        return null;
    }
    return decodeURI(results[1]) || 0;
};

var coreAjaxGetPartialView = function (isValid, url, data, successCallBack, errorCallback) {
    if (isValid) {
        $.ajax({
            headers: {
                'AuthorizationAPI': 'Bearer ' + localStorage.getItem('token')
            },
            cache: false,
            url: url,
            type: 'GET',
            data: data,
            success: function (res) {
                if (successCallBack) {
                    successCallBack(res);
                }
            },
            error: function (err) {

                toastMessage('error', 'Somethings went wrong. Please try agian!');

                if (errorCallback) {
                    errorCallback();
                }
            }
        });
    }
}

var coreAjaxPostPartialView = function (isValid, url, data, successCallBack, errorCallback) {
    if (isValid) {
        $.ajax({
            headers: {
                'AuthorizationAPI': 'Bearer ' + localStorage.getItem('token')
            },
            cache: false,
            url: url,
            type: 'POST',
            data: data,
            contentType: 'application/json',
            success: function (res) {
                if (successCallBack) {
                    successCallBack(res);
                }
            },
            error: function (err) {

                toastMessage('error', 'Somethings went wrong. Please try agian!');

                if (errorCallback) {
                    errorCallback();
                }
            }
        });
    }
}

var coreAjax = function (isValid, url, data, method, successCallBack, errorCallback) {
    console.log(data)
    if (isValid) {
        $.ajax({
            cache: false,
            url: url,
            type: method,
            data: data,
            contentType: 'application/json',
            success: function (res) {
                if (successCallBack) {
                    successCallBack(res);
                }
            },
            error: function (err) {
                if (err.responseText) {
                    alert(err.responseText)
                } else {
                    alert('Lỗi')
                }
                if (errorCallback) {
                    errorCallback();
                }
            }
        });
    }
}

var coreAjaxWithFormData = function (isValid, url, data, method, successCallBack, errorCallback) {
    if (isValid) {
        $.ajax({
            cache: false,
            url: url,
            type: method,
            data: data,
            contentType: false,
            processData: false,
            success: function (res) {
                if (successCallBack) {
                    successCallBack(res);
                }
            },
            error: function (err) {
                if (err.responseText) {
                    toastMessage('error', err.responseText)
                } else {
                    toastMessage('error', 'Something went wrong!')
                }
                if (errorCallback) {
                    errorCallback();
                }
            }
        });
    }
}

var sweetAlertDevareConfirmation = function (url, id, title, text, callBack) {
    var data = {
        Id: id
    };

    Swal.fire({
        title: '<h2 class="custom-font">' + title + '</h2>',
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            coreAjax(true, url, JSON.stringify(data), 'POST', function (res) {
                toastMessage('success', 'Devared successfully');
                callBack();
            }, function () {
            });
        }
    })
}

var formatCurrencyVND = function (amount) {
    return amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' VND';
}

var formatCurrencyDollar = function (amount) {
    return '$' + amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

var formatNumber = function (amount) {
    return amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

var renderChart = function (chartId, type, dataPoints, uffix, indexLabel, toolTipContent, backgroundColor, titleChart, axisYTitle, axisXTitle, isReload) {
    var options = {
        title: {
            text: titleChart
        },
        exportEnabled: true,
        backgroundColor: backgroundColor,
        animationEnabled: true,
        interactivityEnabled: true,
        axisY: {
            title: axisYTitle,
            uffix: uffix
        },
        axisX: {
            title: axisXTitle
        },
        data: [
            {
                type: type, // Change type to doughnut, line, splineArea, etc, column, area, bar, pie, etc
                indexLabel: indexLabel, //"{y} " + suffixText,
                toolTipContent: toolTipContent, //"<b>{label}</b>: {y} " + suffixTextEnd,
                dataPoints: dataPoints
            }
        ]
    };

    (new CanvasJS.Chart(chartId, options)).render();
}