var Details = function (id) {
    var url = "/QuanLyMenu/Details?id=" + id;
    $('#titleBigModal').html("Asset Categories Details");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/QuanLyMenu/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Chỉnh sửa menu");
    }
    else {
        $('#titleBigModal').html("Thêm một menu");
    }
    loadBigModal(url);
};

var Save = function () {
    if (!$("#frmQuanLyMenu").valid()) {
        return;
    }

    var _frmQuanLyMenu = $("#frmQuanLyMenu").serialize();
    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/QuanLyMenu/AddEdit",
        data: _frmQuanLyMenu,
        success: function (result) {
            Swal.fire({
                title: result,
                icon: "success"
            }).then(function () {
                document.getElementById("btnClose").click();
                $("#btnSave").val("Save");
                $('#btnSave').removeAttr('disabled');
                QuanLyMenu.Reload();
            });
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.responseText, "warning");
        }
    });
}

var Delete = function (id) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xoá?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                url: "/QuanLyMenu/Delete?id=" + id,
                success: function (result) {
                    var message = "Đã xoá thành công ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblQuanLyMenu').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};
var getViewConfig = function () {
    return;
    $.ajax({
        type: "POST",
        url: "/QuanLyMenu/Delete?id=" + id,
        success: function (result) {
            var message = "Đã xoá thành công ID: " + result.Id;
            Swal.fire({
                title: message,
                icon: 'info',
                onAfterClose: () => {
                    $('#tblQuanLyMenu').DataTable().ajax.reload();
                }
            });
        }
    });
}
