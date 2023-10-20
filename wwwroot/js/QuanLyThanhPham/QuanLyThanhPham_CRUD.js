var Details = function (id) {
    var url = "/QuanLyThanhPham/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/QuanLyThanhPham/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Sửa thông tin");
    }
    else {
        $('#titleBigModal').html("Thêm mới");
    }
    loadBigModal(url);
};

var Save = function () {
    if (!$("#frmQuanLyThanhPham").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/QuanLyThanhPham/AddEdit",
        data: PreparedFormObj(),
        processData: false,
        contentType: false,
        success: function (result) {
            var alertMessage = result.AlertMessage;
            if (result.IsSuccess) {
                Swal.fire({
                    title: alertMessage,
                    icon: "success"
                }).then(function () {
                    document.getElementById("btnClose").click();
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblQuanLyThanhPham').DataTable().ajax.reload();
                });
            }
            else {
                Swal.fire({
                    title: alertMessage,
                    icon: "warning"
                }).then(function () {
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblQuanLyThanhPham').DataTable().ajax.reload();
                });
            }
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.AlertMessage, "warning");
            $("#btnSave").val("Lưu");
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
                url: "/QuanLyThanhPham/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblQuanLyThanhPham').DataTable().ajax.reload();
                        }
                    });
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
    _FormData.append('TenThanhPham', $("#TenThanhPham").val());
    _FormData.append('ChatLuong', $("#ChatLuong").val());
    _FormData.append('GhiChu', $("#GhiChu").val());
    _FormData.append('KhachHang', $("#KhachHang").val())
    _FormData.append('Mau', $("#Mau").val().toLowerCase())
    _FormData.append('Size', $("#Size").val())
    _FormData.append('PO', $("#PO").val())

    return _FormData;
}