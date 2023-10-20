var Details = function (id) {
    var url = "/NhapKhoLyThuyet/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/NhapKhoLyThuyet/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Chi tiết");
    }
    else {
        $('#titleExtraBigModal').html("Thêm");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmNhapKhoLyThuyet").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/NhapKhoLyThuyet/AddEdit",
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
                $('#tblNhapKhoLyThuyet').DataTable().ajax.reload();
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
                url: "/NhapKhoLyThuyet/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblNhapKhoLyThuyet').DataTable().ajax.reload();
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
    _FormData.append('NgayNhap', $("#NgayNhap").val())
    _FormData.append('MoTa', $("#MoTa").val())
    _FormData.append('Status', $("#Status").val())
    _FormData.append('DuKienNgayVe', $("#DuKienNgayVe").val())
    _FormData.append('MaPhieu', $("#MaPhieu").val())
    _FormData.append('MaSoLo', $("#MaSoLo").val())
    _FormData.append('TenKhachHang', $("#TenKhachHang").val())
    _FormData.append('DonViTienTe', $("#DonViTienTe").val())

    AppNhapKhoLyThuyet.$data.DanhSachNguyenLieuNhap.map(function (item, index) {
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][Id]`, item.Id);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][NhapKhoLyThuyetId]`, item.NhapKhoId);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][MaHaiQuan]`, item.MaHaiQuan);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][MaNhaCungCap]`, item.MaNhaCungCap);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][NhaCungCap]`, item.NhaCungCap);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][NguyenLieuId]`, item.NguyenLieuId);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][SoLuongMua]`, item.SoLuongMua);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][DonGia]`, item.DonGia);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][GhiChu]`, item.GhiChu);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][Cancelled]`, item.Cancelled);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][ThanhPhan]`, item.ThanhPhan);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][DonViTinh]`, item.DonViTinh);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList[${index}][NgayNhap]`, item.NgayNhap);
    });

    return _FormData;
}


function Print(id) {
    fetch('/NhapKhoLyThuyet/Print?id=' + id)
        .then(response => response.text())
        .then(html => {
            const printWindow = window.open('', '_blank', 'width=800,height=600');
            printWindow.document.open();
            printWindow.document.write(html);
            printWindow.document.close();

            printWindow.onload = function () {
                printWindow.print();
            };
        });
}