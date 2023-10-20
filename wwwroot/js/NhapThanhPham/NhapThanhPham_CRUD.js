var Details = function (id) {
    var url = "/NhapThanhPham/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/NhapThanhPham/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Thông tin nhập");
    }
    else {
        $('#titleExtraBigModal').html("Nhập mới");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmNhapThanhPham").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/NhapThanhPham/AddEdit",
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
                    $('#tblNhapThanhPham').DataTable().ajax.reload();
                });
            }
            else {
                Swal.fire({
                    title: alertMessage,
                    icon: "warning"
                }).then(function () {
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblNhapThanhPham').DataTable().ajax.reload();
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
                url: "/NhapThanhPham/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblNhapThanhPham').DataTable().ajax.reload();
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
    _FormData.append('Ma', $("#Ma").val())
    _FormData.append('DonViTienTe', $("#DonViTienTe").val())

    AppNhapThanhPham.$data.ThanhPhamNhapKho.map(function (item, index) {
        _FormData.append(`ThanhPhamNhapKhoList[${index}][ID]`, item.Id);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][ThanhPhamId]`, item.ThanhPhamId);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][SoLuong]`, item.SoLuong);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][DonGia]`, item.DonGia);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][IDPhieu]`, item.NhapKhoThanhPhamId);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][PO]`, item.PO);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][Size]`, item.Size);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][Mau]`, item.Mau);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][NgayNhap]`, item.NgayNhap);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][MaHang]`, item.MaHang);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][KhachHang]`, item.KhachHang);
        _FormData.append(`ThanhPhamNhapKhoList[${index}][Cancelled]`, item.Cancelled);
    });


    return _FormData;
}
function Print(id) {
    fetch('/NhapThanhPham/Print?id=' + id)
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