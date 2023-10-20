var Details = function (id) {
    var url = "/QuanLyNguoiDung/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/QuanLyNguoiDung/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Sửa thông tin người dùng");
    }
    else {
        $('#titleBigModal').html("Thêm thông tin người dùng");
    }
    loadBigModal(url);
};

var Save = function () {
    if (!$("#frmQuanLyNguoiDung").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/QuanLyNguoiDung/AddEdit",
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
                $('#tblQuanLyNguoiDung').DataTable().ajax.reload();
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
                url: "/QuanLyNguoiDung/Delete?id=" + id,
                success: function (result) {
                    var message = "Đã xoá thành công ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblQuanLyNguoiDung').DataTable().ajax.reload();
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
    _FormData.append('HoVaTen', $("#HoVaTen").val());
    _FormData.append('AnhDaiDien', $('input[name="AnhDaiDien"]')[0].files[0])
    _FormData.append('SoDienThoai', $("#SoDienThoai").val());
    _FormData.append('TaiKhoan', $("#TaiKhoan").val());
    _FormData.append('QuanLyNhomNguoiDungId', $("#QuanLyNhomNguoiDungId").val());
    _FormData.append('NgayThem', $("#NgayThem").val())
    _FormData.append('ChucVu', $("#ChucVu").val())

    return _FormData;
}