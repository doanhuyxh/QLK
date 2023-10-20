var Details = function (id) {
    var url = "/KhoXuongThucTe/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết kho xưởng");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/KhoXuongThucTe/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Chỉnh sửa thông tin");
    }
    else {
        $('#titleBigModal').html("Thêm thông tin");
    }
    loadBigModal(url);
};

var Save = function () {
    if (!$("#frmKhoXuongThucTe").valid()) {
        return;
    }

    $("#btnSave").val("Please Wait");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/KhoXuongThucTe/AddEdit",
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
                $('#tblKhoXuongThucTe').DataTable().ajax.reload();
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
        title: 'Bạn có chắc chắn muốn xóa',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                url: "/KhoXuongThucTe/Delete?id=" + id,
                success: function (result) {
                    var message = result.AlertMessage;
                    if (result.IsSuccess) {
                        Swal.fire({
                            title: message,
                            icon: 'info',
                            onAfterClose: () => {
                                $('#tblKhoXuongThucTe').DataTable().ajax.reload();
                            }
                        });
                    }
                    else {
                        Swal.fire({
                            title: message,
                            icon: 'info',
                            onAfterClose: () => {
                                $('#tblKhoXuongThucTe').DataTable().ajax.reload();
                            }
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
    _FormData.append('TenKho', $("#TenKho").val());
    _FormData.append('ParentId', $("#ParentId").val());
    _FormData.append('GhiChu', $("#GhiChu").val())

    return _FormData;
}