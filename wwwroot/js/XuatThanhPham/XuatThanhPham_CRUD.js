var Details = function (id) {
    var url = "/XuatThanhPham/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/XuatThanhPham/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Thông tin");
    }
    else {
        $('#titleExtraBigModal').html("Thêm mới");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmXuatThanhPham").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/XuatThanhPham/AddEdit",
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
                    $('#tblXuatThanhPham').DataTable().ajax.reload();
                });
            }
            else {
                Swal.fire({
                    title: alertMessage,
                    icon: "warning"
                }).then(function () {
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblXuatThanhPham').DataTable().ajax.reload();
                });
            }
            
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.responseText, "warning");
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
                url: "/XuatThanhPham/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblXuatThanhPham').DataTable().ajax.reload();
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
    _FormData.append('NgayXuat', $("#NgayXuat").val())
    _FormData.append('DonViTienTe', $("#DonViTienTe").val())

    AppXuatThanhPham.$data.ThanhPhamXuatKho.map(function (item, index) {
        _FormData.append(`ThanhPhamXuatKhoList[${index}][Id]`, item.Id);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][ThanhPhamId]`, item.ThanhPhamId);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][SoLuong]`, item.SoLuong);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][DonGia]`, item.DonGia);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][XuatKhoThanhPhamId]`, item.XuatKhoThanhPhamId);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][PO]`, item.PO);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][Size]`, item.Size);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][Mau]`, item.Mau);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][Cancelled]`, item.Cancelled);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][NgayXuat]`, item.NgayNhap);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][MaHang]`, item.MaHang);
        _FormData.append(`ThanhPhamXuatKhoList[${index}][KhachHang]`, item.KhachHang);
    });

    return _FormData;
}
function Print(id) {
    fetch('/XuatThanhPham/Print?id=' + id)
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