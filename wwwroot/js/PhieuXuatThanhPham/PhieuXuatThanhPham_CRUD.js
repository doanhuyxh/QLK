var Details = function (id) {
    var url = "/PhieuXuatThanhPham/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/PhieuXuatThanhPham/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Sửa");
    }
    else {
        $('#titleExtraBigModal').html("Thêm");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmPhieuXuatThanhPham").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/PhieuXuatThanhPham/AddEdit",
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
                    $('#tblPhieuXuatThanhPham').DataTable().ajax.reload();
                });
            }
            else {
                Swal.fire({
                    title: alertMessage,
                    icon: "warning"
                }).then(function () {
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblPhieuXuatThanhPham').DataTable().ajax.reload();
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
                url: "/PhieuXuatThanhPham/Delete?id=" + id,
                success: function (result) {
                    var message = "Đã xoá thành công ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblPhieuXuatThanhPham').DataTable().ajax.reload();
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
    _FormData.append('MaPhieuXuat', $("#MaPhieuXuat").val());
    _FormData.append('NgayDuKienXuat', $("#NgayDuKienXuat").val())
    _FormData.append('DonViTienTe', $("#DonViTienTe").val())

     
    AppPhieuXuatThanhPham.$data.ThanhPhamNhapKho.map(function (item, index) {
        _FormData.append(`ThanhPhamNhapKhoList[${index}][Id]`, item.Id);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][IDThanhPham]`, item.IDThanhPham);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][SoLuong]`, item.SoLuong);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][DonGia]`, item.DonGia);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][IDPhieu]`, item.IDPhieu);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][PO]`, item.PO);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][Size]`, item.Size);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][Mau]`, item.Mau);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][Cancelled]`, item.Cancelled);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][NgayNhap]`, item.NgayNhap);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][MaHang]`, item.MaHang);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][KhachHang]`, item.KhachHang);
    });
    return _FormData;
}
function Print(id) {
    fetch('/PhieuXuatThanhPham/Print?id=' + id)
        .then(response => response.text())
        .then(html => {
            const printWindow = window.open('', '_blank', 'width=1200,height=600');
            printWindow.document.open();
            printWindow.document.write(html);
            printWindow.document.close();

            printWindow.onload = function () {
                printWindow.print();
            };
        });
}