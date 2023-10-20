var Details = function (id) {
    var url = "/XuatKho/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};

var AddEdit = function (id) {
    var url = "/XuatKho/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Sửa thông tin đã xuất");
    }
    else {
        $('#titleExtraBigModal').html("Thêm phiếu xuất kho mới");
    }
    loadExtraBigModal(url);
};

var Save = function () {
    if (!$("#frmXuatKho").valid()) {
        return;
    }

    $("#btnSave").val("Please Wait");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/XuatKho/AddEdit",
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
                $('#tblXuatKho').DataTable().ajax.reload();
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
        title: 'Bạn có chắc chắn muốn xóa?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                url: "/XuatKho/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    if (result.IsSuccess) {
                        Swal.fire({
                            title: message,
                            icon: 'info',
                            onAfterClose: () => {
                                $('#tblXuatKho').DataTable().ajax.reload();
                            }
                        });
                    } else {
                        Swal.fire({
                            title: message,
                            icon: 'info'
                        });
                    }
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
    _FormData.append('TenPhieuXuatKho', $("#TenPhieuXuatKho").val());
    _FormData.append('DanhGia', $("#DanhGia").val())
    _FormData.append('NgayXuat', $("#NgayXuat").val())

    AppXuatKho.$data.DanhSachNguyenLieuXuat.map(function (item, index) {
        _FormData.append(`ListNguyenLieuXuatKho[${index}][Id]`, item.Id);
        _FormData.append(`ListNguyenLieuXuatKho[${index}][NguyenLieuId]`, item.NguyenLieuId);
        _FormData.append(`ListNguyenLieuXuatKho[${index}][ChiTietCusTom]`, item.ChiTietCusTom);
        if (item.SoLuongXuat > 0) {
            _FormData.append(`ListNguyenLieuXuatKho[${index}][SoLuongXuat]`, item.SoLuongXuat);

        }
        else {
            _FormData.append(`ListNguyenLieuXuatKho[${index}][SoLuongXuat]`, item.SoLuongCan);

        }

        _FormData.append(`ListNguyenLieuXuatKho[${index}][ChatLuong]`, item.ChatLuong);
        _FormData.append(`ListNguyenLieuXuatKho[${index}][XuatKhoId]`, item.XuatKhoId);
        if (!item.DonVi) {
            _FormData.append(`ListNguyenLieuXuatKho[${index}][DonVi]`, item.Unit);

        } else {
            _FormData.append(`ListNguyenLieuXuatKho[${index}][DonVi]`, item.DonVi);


        }
        _FormData.append(`ListNguyenLieuXuatKho[${index}][Cancelled]`, item.Cancelled);
    });


    return _FormData;
}


function Print(id) {
    fetch('/XuatKho/Print?id=' + id)
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