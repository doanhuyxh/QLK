var Details = function (id) {
    var url = "/KiemKho/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/KiemKho/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Sửa phiếu");
    }
    else {
        $('#titleBigModal').html("Thêm phiếu");
    }
    loadBigModal(url);
};

var Save = function () {
    if (!$("#frmKiemKho").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/KiemKho/AddEdit",
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
                $('#tblKiemKho').DataTable().ajax.reload();
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
                url: "/KiemKho/Delete?id=" + id,
                success: function (result) {
                    var message = "Đã xoá thành công ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblKiemKho').DataTable().ajax.reload();
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
    _FormData.append('Name', $("#Name").val())
    _FormData.append('NgayKiem', $("#NgayKiem").val())
    _FormData.append('KhoId', $("#KhoId").val())
    _FormData.append('NhanVienId', $("#NhanVienId").val())
    AppKiemKho.$data.items.map(function (item, index) {
        _FormData.append(`NguyenLieuKiemKho[${index}][Id]`, item.Id);
        _FormData.append(`NguyenLieuKiemKho[${index}][NguyenLieuId]`, item.NguyenLieuId);
        _FormData.append(`NguyenLieuKiemKho[${index}][ChatLuong]`, item.ChatLuong);
        _FormData.append(`NguyenLieuKiemKho[${index}][SoLuongThucTe]`, item.SoLuongThucTe);
        _FormData.append(`NguyenLieuKiemKho[${index}][KiemKhoId]`, item.KiemKhoId);
    });

    return _FormData;
}