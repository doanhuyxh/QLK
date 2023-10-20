var Details = function (id) {
    var url = "/QuanLyKhoXuong/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/QuanLyKhoXuong/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Sửa thông tin kho xưởng");
    }
    else {
        $('#titleBigModal').html("Thêm thông tin kho xưởng");
    }
    loadBigModal(url);
};

var Save = function () {
    if (!$("#frmQuanLyKhoXuong").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/QuanLyKhoXuong/AddEdit",
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
                $('#tblQuanLyKhoXuong').DataTable().ajax.reload();
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
                url: "/QuanLyKhoXuong/Delete?id=" + id,
                success: function (result) {
                    var message = "Đã xoá thành công ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblQuanLyKhoXuong').DataTable().ajax.reload();
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
    _FormData.append('TenKho', $("#TenKho").val());
    _FormData.append('GhiChu', $("#GhiChu").val());
    _FormData.append('ParentId', $("#ParentId").val())

    return _FormData;
}
