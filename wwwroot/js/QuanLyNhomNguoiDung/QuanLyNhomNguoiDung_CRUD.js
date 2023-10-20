var Details = function (id) {
    var url = "/QuanLyNhomNguoiDung/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết nhóm người dùng");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/QuanLyNhomNguoiDung/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Sửa thông tin nhóm người dùng");
    }
    else {
        $('#titleExtraBigModal').html("Thêm thông tin nhóm người dùng");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmQuanLyNhomNguoiDung").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/QuanLyNhomNguoiDung/AddEdit",
        data: PreparedFormObj(),
        processData: false,
        contentType: false,
        success: function (result) {
            Swal.fire({
                title: result,
                icon: "success"
            }).then(function () {
                document.getElementById("btnClose").click();
                $("#btnSave").val("Save");
                $('#btnSave').removeAttr('disabled');
                $('#tblQuanLyNhomNguoiDung').DataTable().ajax.reload();
            });
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.responseText, "warning");
            $("#btnSave").val("Save");
            $('#btnSave').removeAttr('disabled');
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
                url: "/QuanLyNhomNguoiDung/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    if (result.IsSuccess) {
                        Swal.fire({
                            title: message,
                            icon: 'info',
                            onAfterClose: () => {
                                $('#tblQuanLyNhomNguoiDung').DataTable().ajax.reload();
                            }
                        });
                    } else {
                        Swal.fire({
                            title: message,
                            icon: 'info',
                        });
                    }
                }
            });
        }
    });
};
var PreparedFormObj = function () {
    var _FormData = new FormData()


    _FormData.append('Id', $("#Id").val())
    _FormData.append('CreatedBy', $("#CreatedBy").val())
    _FormData.append('CreatedDate', $("#CreatedDate").val())
    _FormData.append('TenNhom', $("#TenNhom").val());
    _FormData.append('MoTa', $("#MoTa").val())

    return _FormData;
}