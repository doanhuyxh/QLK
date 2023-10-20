$(document).ready(function () {
    document.title = 'Nhập kho lý thuyết';

    $("#tblNhapKhoLyThuyet2").DataTable({
        paging: true,
        select: true,
        "order": [[0, "desc"]],
        dom: 'Bfrtip',


        buttons: [
            'pageLength',
        ],


        "processing": true,
        "serverSide": true,
        "filter": true, //Search Box
        "orderMulti": false,
        "stateSave": true,

        "ajax": {
            "url": "/NhapKhoLyThuyet2/GetDataTabelData",
            "type": "POST",
            "datatype": "json"
        },


        "columns": [
            {
                data: "Id", "name": "Id", render: function (data, type, row) {
                    return "<a href='#' class='fa fa-eye' onclick=Details('" + row.Id + "');></a>";
                }
            },
            { "data": "MaPhieu", "name": "MaPhieu" },
            { "data": "MaSoLo", "name": "MaSoLo" },
            { "data": "SoToKhai", "name": "SoToKhai" },
            { "data": "TenKhachHang", "name": "TenKhachHang" },
            {
                "data": "NgayNhap",
                "name": "NgayNhap",
                "autoWidth": true,
                "render": function (data) {
                    const formattedDate = moment(data).format('DD/MM/YYYY');
                    return formattedDate;
                }
            }, {
                "data": "DuKienNgayVe",
                "name": "DuKienNgayVe",
                "autoWidth": true,
                "render": function (data) {
                    const formattedDate = moment(data).format('DD/MM/YYYY');
                    return formattedDate;
                }
            },
            {
                "data": "Status",
                "name": "Status",
                "render": function (data) {
                    if (data == true) {
                        return "Đã nhập kho thực tế"
                    }
                    return "Đang chờ nhập kho thực tế";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-primary' onclick=AddEdit('" + row.Id + "');>chi tiết</a>";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<button type='button' class='btn btn-success' style='margin:auto' onclick=Print('" + row.Id + "'); >" + '<svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 512 512"><!--! Font Awesome Free 6.4.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2023 Fonticons, Inc. --><path d="M128 0C92.7 0 64 28.7 64 64v96h64V64H354.7L384 93.3V160h64V93.3c0-17-6.7-33.3-18.7-45.3L400 18.7C388 6.7 371.7 0 354.7 0H128zM384 352v32 64H128V384 368 352H384zm64 32h32c17.7 0 32-14.3 32-32V256c0-35.3-28.7-64-64-64H64c-35.3 0-64 28.7-64 64v96c0 17.7 14.3 32 32 32H64v64c0 35.3 28.7 64 64 64H384c35.3 0 64-28.7 64-64V384zM432 248a24 24 0 1 1 0 48 24 24 0 1 1 0-48z"/></svg>' + "</button    `1>";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-danger' onclick=Delete('" + row.Id + "'); >Xóa</a>";
                }
            }
        ],

        'columnDefs': [{
            'targets': [-2, -1],
            'orderable': false,
        }],
        language: {
            "processing": "Đang xử lý...",
            "aria": {
                "sortAscending": ": Sắp xếp thứ tự tăng dần",
                "sortDescending": ": Sắp xếp thứ tự giảm dần"
            },
            "autoFill": {
                "cancel": "Hủy",
                "fill": "Điền tất cả ô với <i>%d<\/i>",
                "fillHorizontal": "Điền theo hàng ngang",
                "fillVertical": "Điền theo hàng dọc"
            },
            "buttons": {
                "collection": "Chọn lọc <span class=\"ui-button-icon-primary ui-icon ui-icon-triangle-1-s\"><\/span>",
                "colvis": "Hiển thị theo cột",
                "colvisRestore": "Khôi phục hiển thị",
                "copy": "Sao chép",
                "copyKeys": "Nhấn Ctrl hoặc u2318 + C để sao chép bảng dữ liệu vào clipboard.<br \/><br \/>Để hủy, click vào thông báo này hoặc nhấn ESC",
                "copySuccess": {
                    "1": "Đã sao chép 1 dòng dữ liệu vào clipboard",
                    "_": "Đã sao chép %d dòng vào clipboard"
                },
                "copyTitle": "Sao chép vào clipboard",
                "pageLength": {
                    "-1": "Xem tất cả các dòng",
                    "_": "Hiển thị %d dòng",
                    "1": "Hiển thị 1 dòng"
                },
                "print": "In ấn",
                "createState": "Tạo trạng thái",
                "csv": "CSV",
                "excel": "Excel",
                "pdf": "PDF",
                "removeAllStates": "Xóa hết trạng thái",
                "removeState": "Xóa",
                "renameState": "Đổi tên",
                "savedStates": "Trạng thái đã lưu",
                "stateRestore": "Trạng thái %d",
                "updateState": "Cập nhật"
            },
            "select": {
                "cells": {
                    "1": "1 ô đang được chọn",
                    "_": "%d ô đang được chọn"
                },
                "columns": {
                    "1": "1 cột đang được chọn",
                    "_": "%d cột đang được được chọn"
                },
                "rows": {
                    "1": "1 dòng đang được chọn",
                    "_": "%d dòng đang được chọn"
                }
            },
            "searchBuilder": {
                "title": {
                    "_": "Thiết lập tìm kiếm (%d)",
                    "0": "Thiết lập tìm kiếm"
                },
                "button": {
                    "0": "Thiết lập tìm kiếm",
                    "_": "Thiết lập tìm kiếm (%d)"
                },
                "value": "Giá trị",
                "clearAll": "Xóa hết",
                "condition": "Điều kiện",
                "conditions": {
                    "date": {
                        "after": "Sau",
                        "before": "Trước",
                        "between": "Nằm giữa",
                        "empty": "Rỗng",
                        "equals": "Bằng với",
                        "not": "Không phải",
                        "notBetween": "Không nằm giữa",
                        "notEmpty": "Không rỗng"
                    },
                    "number": {
                        "between": "Nằm giữa",
                        "empty": "Rỗng",
                        "equals": "Bằng với",
                        "gt": "Lớn hơn",
                        "gte": "Lớn hơn hoặc bằng",
                        "lt": "Nhỏ hơn",
                        "lte": "Nhỏ hơn hoặc bằng",
                        "not": "Không phải",
                        "notBetween": "Không nằm giữa",
                        "notEmpty": "Không rỗng"
                    },
                    "string": {
                        "contains": "Chứa",
                        "empty": "Rỗng",
                        "endsWith": "Kết thúc bằng",
                        "equals": "Bằng",
                        "not": "Không phải",
                        "notEmpty": "Không rỗng",
                        "startsWith": "Bắt đầu với",
                        "notContains": "Không chứa",
                        "notEndsWith": "Không kết thúc với",
                        "notStartsWith": "Không bắt đầu với"
                    },
                    "array": {
                        "equals": "Bằng",
                        "empty": "Trống",
                        "contains": "Chứa",
                        "not": "Không",
                        "notEmpty": "Không được rỗng",
                        "without": "không chứa"
                    }
                },
                "logicAnd": "Và",
                "logicOr": "Hoặc",
                "add": "Thêm điều kiện",
                "data": "Dữ liệu",
                "deleteTitle": "Xóa quy tắc lọc",
                "leftTitle": "Giảm thụt lề",
                "rightTitle": "Tăng thụt lề"
            },
            "searchPanes": {
                "countFiltered": "{shown} ({total})",
                "emptyPanes": "Không có phần tìm kiếm",
                "clearMessage": "Xóa hết",
                "loadMessage": "Đang load phần tìm kiếm",
                "collapse": {
                    "0": "Phần tìm kiếm",
                    "_": "Phần tìm kiếm (%d)"
                },
                "title": "Bộ lọc đang hoạt động - %d",
                "count": "{total}",
                "collapseMessage": "Thu gọn tất cả",
                "showMessage": "Hiện tất cả"
            },
            "datetime": {
                "hours": "Giờ",
                "minutes": "Phút",
                "next": "Sau",
                "previous": "Trước",
                "seconds": "Giây",
                "amPm": [
                    "am",
                    "pm"
                ],
                "unknown": "-",
                "weekdays": [
                    "Chủ nhật"
                ],
                "months": [
                    "Tháng Một",
                    "Tháng Hai",
                    "Tháng Ba",
                    "Tháng Tư",
                    "Tháng Năm",
                    "Tháng Sáu",
                    "Tháng Bảy",
                    "Tháng Tám",
                    "Tháng Chín",
                    "Tháng Mười",
                    "Tháng Mười Một",
                    "Tháng Mười Hai"
                ]
            },
            "emptyTable": "Không có dữ liệu",
            "info": "Hiển thị _START_ tới _END_ của _TOTAL_ dữ liệu",
            "infoEmpty": "Hiển thị 0 tới 0 của 0 dữ liệu",
            "lengthMenu": "Hiển thị _MENU_ dữ liệu",
            "loadingRecords": "Đang tải...",
            "paginate": {
                "first": "Đầu tiên",
                "last": "Cuối cùng",
                "next": "Sau",
                "previous": "Trước"
            },
            "search": "Tìm kiếm:",
            "zeroRecords": "Không tìm thấy kết quả",
            "decimal": ",",
            "editor": {
                "close": "Đóng",
                "create": {
                    "button": "Thêm",
                    "submit": "Thêm",
                    "title": "Thêm mục mới"
                },
                "edit": {
                    "button": "Sửa",
                    "submit": "Cập nhật",
                    "title": "Sửa mục"
                },
                "error": {
                    "system": "Đã xảy ra lỗi hệ thống (&lt;a target=\"\\\" rel=\"nofollow\" href=\"\\\"&gt;Thêm thông tin&lt;\/a&gt;)."
                },
                "multi": {
                    "info": "Các mục đã chọn chứa các giá trị khác nhau cho đầu vào này. Để chỉnh sửa và đặt tất cả các mục cho đầu vào này thành cùng một giá trị, hãy nhấp hoặc nhấn vào đây, nếu không chúng sẽ giữ lại các giá trị riêng lẻ của chúng.",
                    "noMulti": "Đầu vào này có thể được chỉnh sửa riêng lẻ, nhưng không phải là một phần của một nhóm.",
                    "restore": "Hoàn tác thay đổi",
                    "title": "Nhiều giá trị"
                },
                "remove": {
                    "button": "Xóa",
                    "confirm": {
                        "_": "Bạn có chắc chắn muốn xóa %d hàng không?",
                        "1": "Bạn có chắc chắn muốn xóa 1 hàng không?"
                    },
                    "submit": "Xóa",
                    "title": "Xóa"
                }
            },
            "infoFiltered": "(được lọc từ _MAX_ dữ liệu)",
            "searchPlaceholder": "Nhập tìm kiếm...",
            "stateRestore": {
                "creationModal": {
                    "button": "Thêm",
                    "columns": {
                        "search": "Tìm kiếm cột",
                        "visible": "Khả năng hiển thị cột"
                    },
                    "name": "Tên:",
                    "order": "Sắp xếp",
                    "paging": "Phân trang",
                    "scroller": "Cuộn vị trí",
                    "search": "Tìm kiếm",
                    "searchBuilder": "Trình tạo tìm kiếm",
                    "select": "Chọn",
                    "title": "Thêm trạng thái",
                    "toggleLabel": "Bao gồm:"
                },
                "duplicateError": "Trạng thái có tên này đã tồn tại.",
                "emptyError": "Tên không được để trống.",
                "emptyStates": "Không có trạng thái đã lưu",
                "removeConfirm": "Bạn có chắc chắn muốn xóa %s không?",
                "removeError": "Không xóa được trạng thái.",
                "removeJoiner": "và",
                "removeSubmit": "Xóa",
                "removeTitle": "Xóa trạng thái",
                "renameButton": "Đổi tên",
                "renameLabel": "Tên mới cho %s:",
                "renameTitle": "Đổi tên trạng thái"
            },
            "infoThousands": ".",
            "thousands": "."
        },

        "lengthMenu": [[20, 10, 15, 25, 50, 100, 200], [20, 10, 15, 25, 50, 100, 200]]
    });

});

