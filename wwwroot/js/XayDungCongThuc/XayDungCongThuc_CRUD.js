var Details = function (id) {
    var url = "/XayDungCongThuc/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết thành phẩm");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/XayDungCongThuc/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Sửa thông tin thành phẩm");
    }
    else {
        $('#titleExtraBigModal').html("Thêm");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmSanPham").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');

    $.ajax({
        type: "POST",
        url: "/XayDungCongThuc/AddEdit",
        data: _data(),
        processData: false,
        contentType: false,
        success: function (result) {
            var alertMessage = result.AlertMessage;
            if (result.IsSuccess) {
                Swal.fire({
                    title: alertMessage,
                    icon: "success",
                }).then(function () {
                    document.getElementById("btnClose").click();
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblThanhPham').DataTable().ajax.reload();
                });
            }
            else {
                Swal.fire({
                    title: alertMessage,
                    icon: "warning",
                }).then(function () {
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblThanhPham').DataTable().ajax.reload();
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
        title: 'Bạn chắc chắn muốn xoá ?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Lưu'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                url: "/XayDungCongThuc/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblThanhPham').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};

var _data = function () {
    var da = new FormData();

    da.append("Id", $("#Id").val());
    da.append("CreatedDate", $("#CreatedDate").val());
    da.append("CreatedBy", $("#CreatedBy").val());
    da.append("Name", $("#Name").val());
    da.append("Description", $("#Description").val());
    da.append("MaSP", $("#MaSP").val());

    AppThanhPham.$data.items.map(function (item, index) {
        da.append(`ListNguyenLieuInCongThuc[${index}][Id]`, item.Id);
        da.append(`ListNguyenLieuInCongThuc[${index}][MieuTa]`, item.MieuTa);
        da.append(`ListNguyenLieuInCongThuc[${index}][NguyenLieuId]`, item.NguyenLieuId);
        da.append(`ListNguyenLieuInCongThuc[${index}][SanPhamId]`, item.SanPhamId);
        da.append(`ListNguyenLieuInCongThuc[${index}][SoMetChi]`, item.SoMetChi);
        da.append(`ListNguyenLieuInCongThuc[${index}][Cancelled]`, item.Cancelled);
        da.append(`ListNguyenLieuInCongThuc[${index}][CachSuDung]`, item.CachSuDung);
        da.append(`ListNguyenLieuInCongThuc[${index}][DonVi]`, item.DonVi);
        da.append(`ListNguyenLieuInCongThuc[${index}][SoLuong]`, item.SoLuong);
        da.append(`ListNguyenLieuInCongThuc[${index}][DinhMuc]`, item.DinhMuc);
        da.append(`ListNguyenLieuInCongThuc[${index}][NhuCau]`, item.NhuCau);
        da.append(`ListNguyenLieuInCongThuc[${index}][SoLo]`, item.SoLo);
        da.append(`ListNguyenLieuInCongThuc[${index}][NgayVeVIT]`, item.NgayVeVIT);
        da.append(`ListNguyenLieuInCongThuc[${index}][ThucNhan]`, item.ThucNhan);
        da.append(`ListNguyenLieuInCongThuc[${index}][NgayVeVIT]`, item.NgayVeVIT);
        da.append(`ListNguyenLieuInCongThuc[${index}][ListSizeMau]`, item.ListSizeMau);
    });

    return da;
}