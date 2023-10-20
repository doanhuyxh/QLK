var Details = function (id) {
    var url = "/ObjectField/Details?id=" + id;
    $('#titleBigModal').html("Asset Categories Details");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/ObjectField/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleBigModal').html("Edit Asset Categories");
    }
    else {
        $('#titleBigModal').html("Add Asset Categories");
    }
    loadBigModal(url);
};

var Save = function () {
    if (!$("#frmObjectField").valid()) {
        return;
    }

    var _frmObjectField = $("#frmObjectField").serialize();
    $("#btnSave").val("Vui lòng chờ");
    $('#btnSave').attr('disabled', 'disabled');
    $.ajax({
        type: "POST",
        url: "/ObjectField/AddEdit",
        data: _frmObjectField,
        success: function (result) {
            Swal.fire({
                title: result,
                icon: "success"
            }).then(function () {
                document.getElementById("btnClose").click();
                $("#btnSave").val("Save");
                $('#btnSave').removeAttr('disabled');
                $('#tblObjectField').DataTable().ajax.reload();
            });
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.responseText, "warning");
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
                url: "/ObjectField/Delete?id=" + id,
                success: function (result) {
                    var message = "Đã xoá thành công ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblObjectField').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};
