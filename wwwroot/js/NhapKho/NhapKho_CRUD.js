var Details = function (id) {
    var url = "/NhapKho/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/NhapKho/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Thông tin");
    }
    else {
        $('#titleExtraBigModal').html("Thêm phiếu nhập kho mới");
    }
    loadExtraBigModal(url);
};

var Save = async function () {
    if (!$("#frmNhapKho").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    try {
        await $.ajax({
            type: "POST",
            url: "/NhapKho/AddEdit",
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
                        $('#tblNhapKho').DataTable().ajax.reload();
                    });
                }
                else {
                    Swal.fire({
                        title: alertMessage,
                        icon: "warning"
                    }).then(function () {
                        $("#btnSave").val("Lưu");
                        $('#btnSave').removeAttr('disabled');
                        $('#tblNhapKho').DataTable().ajax.reload();
                    });
                }
            },
            error: function (errormessage) {
                SwalSimpleAlert(errormessage.AlertMessage, "warning");
                $("#btnSave").val("Lưu");
                $('#btnSave').removeAttr('disabled');
            }
        });
    } catch (error) {
        SwalSimpleAlert(error.AlertMessage, "warning");
        $("#btnSave").val("Lưu");
        $('#btnSave').removeAttr('disabled');
    }

    try {

        await $.ajax({
            type: "POST",
            url: "/NhapKho/InsertCustomReportProduct",
            data: dataReport(),
            processData: false,
            contentType: false,
        });
    }
    catch (error) {
        console.log(error);
    }

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
                url: "/NhapKho/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblNhapKho').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};

var PreparedFormObj = function () {


    if (AppNhapKho.$data.IsNhapKhoLyThuyet == true) {
        fetch("/NhapKhoLyThuyet/ChuyenSangTrangThaiDaNhap?id=" + AppNhapKho.$data.NhapKhoLyThuyetId)
    }



    var _FormData = new FormData();
    _FormData.append('Id', $("#Id").val())
    _FormData.append('CreatedBy', $("#CreatedBy").val())
    _FormData.append('CreatedDate', $("#CreatedDate").val())
    _FormData.append('NgayNhap', $("#NgayNhap").val());
    _FormData.append('MoTa', $("#MoTa").val())
    _FormData.append('DonViTienTe', $("#DonViTienTe").val())

    AppNhapKho.$data.DanhSachNguyenLieuNhap.map(function (item, index) {
        _FormData.append(`ListNguyenLieuNhapKho[${index}][Id]`, item.Id);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][MaKho]`, item.MaKho);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][NguyenLieuId]`, item.NguyenLieuId);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][SoLuongNhap]`, item.SoLuongNhap);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][SoLuongNhapTrenChungTu]`, item.SoLuongNhapTrenChungTu);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][NhaCungCap]`, item.NhaCungCap);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][ChatLuong]`, item.ChatLuong);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][NhapKhoId]`, item.NhapKhoId);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][Cancelled]`, item.Cancelled);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][DonGia]`, item.DonGia);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][ChiTietCusTom]`, item.ChiTietCusTom);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][MaHaiQuan]`, item.MaHaiQuan);
        _FormData.append(`ListNguyenLieuNhapKho[${index}][Solo]`, item.Solo);
    });

    return _FormData;
}
var dataReport = function () {
    var _FormData = new FormData();

    AppNhapKho.$data.DanhSachNguyenLieuNhap.map(function (item, index) {
        _FormData.append(`vm[${index}][TenNguyenLieuId]`, item.NguyenLieuId);
        _FormData.append(`vm[${index}][ChatLuongDrop]`, item.ChatLuong);
    });
    return _FormData;


}
function Print(id) {
    fetch('/NhapKho/Print?id=' + id)
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