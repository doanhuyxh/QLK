var Details = function (id) {
    var url = "/TheoDoiChatLuong/Details?id=" + id;
    $('#titleBigModal').html("Chi tiết");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/TheoDoiChatLuong/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Sửa thông tin theo dõi chất lượng");
    }
    else {
        $('#titleBigModal').html("Thêm thông tin theo dõi chất lượng");
    }
    loadBigModal(url);
};

var Save = function () {
    if (!$("#frmTheoDoiChatLuong").valid()) {
        return;
    }

    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/TheoDoiChatLuong/AddEdit",
        data: PreparedFormObj(),
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
                    $('#tblTheoDoiChatLuong').DataTable().ajax.reload();
                    window.location.reload();
                });
            }
            else {
                Swal.fire({
                    title: alertMessage,
                    icon: "warning",
                }).then(function () {
                    $("#btnSave").val("Lưu");
                    $('#btnSave').removeAttr('disabled');
                    $('#tblTheoDoiChatLuong').DataTable().ajax.reload();
                    window.location.reload();

                });
            }
            
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.responseText, "warning");
            $("#btnSave").val("Save");
            $('#btnSave').removeAttr('disabled');
            ThongBao.init();
        }
    });
}
var Delete = async function (id) {
    const result = await Swal.fire({
        title: 'Bạn có chắc chắn muốn xoá?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    });

    if (result.value) {
        try {
            const ajaxResult = await $.ajax({
                type: "POST",
                url: "/TheoDoiChatLuong/Delete?id=" + id
            });

            var message = "Đã xoá thành công ID: " + ajaxResult.Id;

            await Swal.fire({
                title: message,
                icon: 'info',
                onAfterClose: () => {
                    $('#tblTheoDoiChatLuong').DataTable().ajax.reload();
                }
            }).then(() => {
                window.location.reload();
            });

            ThongBao.init();
        } catch (error) {
            console.log(error);
        }
    }
};

var PreparedFormObj = function () {
    var _FormData = new FormData()


    _FormData.append('Id', $("#Id").val())
    _FormData.append('CreatedBy', $("#CreatedBy").val())
    _FormData.append('CreatedDate', $("#CreatedDate").val())
    _FormData.append('GhiChuVeChatLuong', $("#GhiChuVeChatLuong").val())
    _FormData.append('TenNguyenLieuId', $("#TenNguyenLieuId").val());
    _FormData.append('ChatLuongDrop', $("#ChatLuongDrop").val());

    return _FormData;
}