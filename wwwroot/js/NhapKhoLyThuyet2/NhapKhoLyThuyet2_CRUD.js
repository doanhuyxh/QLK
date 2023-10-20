var Details = function (id) {
    var url = "/NhapKhoLyThuyet2/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/NhapKhoLyThuyet2/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Chi tiết");
    }
    else {
        $('#titleExtraBigModal').html("Thêm");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmNhapKhoLyThuyet2").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/NhapKhoLyThuyet2/AddEdit",
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
                $('#tblNhapKhoLyThuyet2').DataTable().ajax.reload();
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
                url: "/NhapKhoLyThuyet2/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblNhapKhoLyThuyet2').DataTable().ajax.reload();
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
    _FormData.append('SoToKhai', $("#SoToKhai").val())

    AppNhapKhoLyThuyet2.$data.DanhSachNguyenLieuNhap2.map(function (item, index) {
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][Id]`, item.Id);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][NhapKhoLyThuyetId]`, item.NhapKhoId);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][MaHaiQuan]`, item.MaHaiQuan);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][MaNhaCungCap]`, item.MaNhaCungCap);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][NhaCungCap]`, item.NhaCungCap);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][NguyenLieuId]`, item.NguyenLieuId);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][SoLuongMua]`, item.SoLuongMua);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][DonGia]`, item.DonGia);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][GhiChu]`, item.GhiChu);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][Cancelled]`, item.Cancelled);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][ThanhPhan]`, item.ThanhPhan);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][DonViTinh]`, item.DonViTinh);
        _FormData.append(`NguyenLieuNhapKhoLyThuyetList2[${index}][NgayNhap]`, item.NgayNhap);
    });

    return _FormData;
}


function Print(id) {
    fetch('/NhapKhoLyThuyet2/Print?id=' + id)
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