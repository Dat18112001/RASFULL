var pagingOptions = {
    pageSize: 20,
    pageIndex: 1
};

var realEstateServices = {
    init: function () {
        realEstateServices.loadData(true);
        realEstateServices.registerEvent();
    },
    loadData: function (changePageSize) {
        var requestData = { pageIndex: pagingOptions.pageIndex, searchKey: $('#search-for-anything').val() };

        $.ajax({
            url: '/AdminArea/RealEstate/LoadData',
            type: 'POST',
            dataType: 'json',
            data: { pageIndex: pagingOptions.pageIndex, searchKey: $('#search-for-anything').val() },
            success: function (response) {
                var html = '';
                var data = response.data;
                var formData = $('#data-template').html();
                var currentPageIndex = Number($('#paging-current-index').val());
                $.each(data, function (i, item) {
                    html += Mustache.render(formData, {
                        Index: (currentPageIndex - 1) * 20 + Number(i) + 1,
                        Id: item.id,
                        Street: item.street,
                        Price: item.price,
                        PostDate: item.postDate,
                        ExpireTime: item.expireTime,
                        Agent: item.agent,
                        Type: item.type,
                        Status: item.status
                    });
                });

                $('#real-estate-list-data').html(html);
                realEstateServices.paging(response.totalRow, function () {
                    realEstateServices.loadData();
                }, changePageSize);
            },
            error: function (err) {
                console.log(err);
            }
        });
    },

    paging: function (totalRow, callback, changePageSize) {

        var totalPage = Math.ceil(totalRow / pagingOptions.pageSize);

        if ($('#pagination a').length === 0 || changePageSize === true) {
            $('#pagination').empty();
            $('#pagination').removeData("twbs-pagination");
            $('#pagination').unbind("page");
        }

        $('#pagination').twbsPagination({
            totalPages: totalPage,
            //startPage: pagingOptions.pageIndex > totalPage ? totalPage : pagingOptions.pageIndex,
            visiblePages: 5,
            onPageClick: function (event, page) {
                $('#paging-current-index').val(page);
                pagingOptions.pageIndex = page;
                setTimeout(callback, 200);
            }
        });
    },

    disableRealEstate: function (form) {
        swal({
            title: "Xác nhận",
            text: "Xác nhận khóa bài đăng này?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Xác nhận",
            cancelButtonText: "Hủy bỏ",
            closeOnConfirm: false,
            closeOnCancel: false
        },
            function (isConfirm) {
                if (isConfirm) {
                    try {
                        $.ajax({
                            url: '/AdminArea/RealEstate/Disable',
                            type: 'POST',
                            headers: {
                                RequestVerificationToken:
                                    $('input:hidden[name="__RequestVerificationToken"]').val()
                            },
                            dataType: 'json',
                            data: new FormData(form),
                            contentType: false,
                            processData: false,
                            success: function (response) {
                                if (response.isSuccess) {
                                    realEstateServices.loadData(true);
                                    setTimeout(function () {
                                        swal({
                                            title: "Thành công!",
                                            text: "Khóa thành công!",
                                            type: "success"
                                        });
                                    }, 200);
                                }
                                else {
                                    swal("Có lỗi!", "", "error");
                                }
                            },
                            error: function (err) {
                                alert(err);
                            }
                        });
                    } catch (ex) {
                        console.log(ex);
                    }
                } else {
                    swal("Hủy bỏ", "", "error");
                }
            });

        //prevent default form submit event
        return false;
    },

    deleteRealEstate: function (form) {
        swal({
            title: "Xác nhận",
            text: "Xác nhận xóa bài đăng này?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Xác nhận",
            cancelButtonText: "Hủy bỏ",
            closeOnConfirm: false,
            closeOnCancel: false
        },
            function (isConfirm) {
                if (isConfirm) {
                    try {
                        $.ajax({
                            url: '/AdminArea/RealEstate/Delete',
                            type: 'POST',
                            headers: {
                                RequestVerificationToken:
                                    $('input:hidden[name="__RequestVerificationToken"]').val()
                            },
                            dataType: 'json',
                            data: new FormData(form),
                            contentType: false,
                            processData: false,
                            success: function (response) {
                                if (response.isSuccess) {
                                    realEstateServices.loadData(true);
                                    setTimeout(function () {
                                        swal({
                                            title: "Thành công!",
                                            text: "Xóa thành công!",
                                            type: "success"
                                        });
                                    }, 200);
                                }
                                else {
                                    swal("Có lỗi!", res.message, "error");
                                }
                            },
                            error: function (err) {
                                alert(err);
                            }
                        });
                    } catch (ex) {
                        console.log(ex);
                    }
                } else {
                    swal("Hủy bỏ", "", "error");
                }
            });

        //prevent default form submit event
        return false;
    },

    gotoIndex: function () {

    },

    registerEvent: function () {
        $('#search-for-anything').keyup(function () {

            realEstateServices.loadData(true);
        });
    }
};

realEstateServices.init();