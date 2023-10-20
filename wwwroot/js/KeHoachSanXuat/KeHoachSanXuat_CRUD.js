var Details = function (id) {
    var url = "/KeHoachSanXuat/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadExtraBigModal(url);
};


var AddEdit = function (id) {
    var url = "/KeHoachSanXuat/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Sửa thông tin kế hoạch");
    }
    else {
        $('#titleExtraBigModal').html("Thêm ");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmKeHoachSanXuat").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/KeHoachSanXuat/AddEdit",
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
                    $("#btnSave").val("Save");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblKeHoachSanXuat').DataTable().ajax.reload();
                });
            }
            else {
                Swal.fire({
                    title: alertMessage,
                    icon: "warning"
                }).then(function () {
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblKeHoachSanXuat').DataTable().ajax.reload();
                });
            }

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
                url: "/KeHoachSanXuat/Delete?id=" + id,
                success: function (result) {
                    var message = "Đã Xóa thành Công";
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblKeHoachSanXuat').DataTable().ajax.reload();
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
    _FormData.append('NgayDuKienHoan', $("#NgayDuKienHoan").val())
    if (AppKeHoachSanXuat.$data.CongThuc == false) {

        _FormData.append('SoLuongThanhPham', 0);
    }
    else {
    _FormData.append('SoLuongThanhPham', $("#SoLuongThanhPham").val())

    }

    AppKeHoachSanXuat.$data.items.map(function (item, index) {
        _FormData.append(`ListNguyenLieuKeHoach[${index}][Id]`, item.Id);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][NguyenLieuId]`, item.NguyenLieuId);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][KeHoachSanXuatId]`, item.KeHoachSanXuatId);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][SanPhamId]`, item.SanPhamId);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][SoMetChi]`, item.SoMetChi);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][CachSuDung]`, item.CachSuDung);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][ListSizeMau]`, item.ListSizeMau);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][DonVi]`, item.DonVi);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][SoLuong]`, item.SoLuong);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][DinhMuc]`, item.DinhMuc);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][NhuCau]`, Number(item.SoLuong) * Number(item.DinhMuc));
        _FormData.append(`ListNguyenLieuKeHoach[${index}][SoLo]`, item.SoLo);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][NgayVeVIT]`, item.NgayVeVIT);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][ThucNhan]`, item.ThucNhan);
        _FormData.append(`ListNguyenLieuKeHoach[${index}][MieuTa]`, item.MieuTa);
    });


    return _FormData;
}