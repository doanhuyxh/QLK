var Details = function (id) {
    var url = "/NguyenLieu/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/NguyenLieu/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Sửa thông tin nguyên liệu");
    }
    else {
        $('#titleExtraBigModal').html("Thêm nguyên liệu mới");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmNguyenLieu").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/NguyenLieu/AddEdit",
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
                $('#tblNguyenLieu').DataTable().ajax.reload();
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
                url: "/NguyenLieu/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblNguyenLieu').DataTable().ajax.reload();
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
    _FormData.append('MaNguyenLieu', $("#MaNguyenLieu").val());
    _FormData.append('TenNguyenLieu', $("#TenNguyenLieu").val());
    _FormData.append('NgayNhap', $("#NgayNhap").val());
    _FormData.append('SoLuong', $("#SoLuong").val());
    _FormData.append('SoLuongLyThuyet', $("#SoLuongLyThuyet").val());
    _FormData.append('DonViTinh', $("#DonViTinh").val());
    _FormData.append('KhoId', $("#KhoId").val());
    AppNguyenLieu.$data.TotalKey.map(function (item, index) {
        const listCustom = JSON.stringify(item.ListCustom);
        _FormData.append(`ListCustom[${index}][Id]`, item.ID);
        _FormData.append(`ListCustom[${index}][NguyenLieuId]`, item.NguyenLieuID);
        _FormData.append(`ListCustom[${index}][ListCustom]`, listCustom);
        _FormData.append(`ListCustom[${index}][QuantityProduct]`, item.QuantityProduct);
        _FormData.append(`ListCustom[${index}][Cancel]`, item.Cancel);
    });

    return _FormData;
}