var funAction = function (UserProfileId) {
    var _Action = $("#" + UserProfileId).val();
    if (_Action == 1)
        AddEditUserAccount(UserProfileId);
    else if (_Action == 2)
        ResetPasswordAdmin(UserProfileId);
    else if (_Action == 3)
        ManagePageAccessAdmin(UserProfileId);
    else if (_Action == 4)
        DeleteUserAccount(UserProfileId);
    $("#" + UserProfileId).prop('selectedIndex', 0);
};

var ViewUserDetails = function (Id) {
    var url = "/UserManagement/ViewUserDetails?Id=" + Id;
    $('#titleBigModal').html("Chi tiết người dùng");
    loadBigModal(url);
};

var ManagePageAccessAdmin = function (id) {
    $('#titleBigModal').html("<h4>Manage Page Access</h4>");
    var url = "/UserRole/ManageRoleAdmin?id=" + id;
    loadBigModal(url);
};

var ManagePageAccessGeneral = function (id) {
    $('#titleBigModal').html("<h4>Manage Page Access</h4>");
    var url = "/UserRole/ManageRoleGeneral?_ApplicationUserId=" + id;
    loadBigModal(url);
};

var UpdateRole = function () {
    $("#btnUpdateRole").val("Vui lòng chờ");
    $('#btnUpdateRole').attr('disabled', 'disabled');

    var _frmManageRole = $("#frmManageRole").serialize();
    $.ajax({
        type: "POST",
        url: "/UserRole/UpdateRole",
        data: _frmManageRole,
        success: function (result) {
            $("#btnUpdateRole").val("Save");
            $('#btnUpdateRole').removeAttr('disabled');

            Swal.fire({
                title: result.AlertMessage,
                icon: "success"
            }).then(function () {
                document.getElementById("btnClose").click();
                if (result.CurrentURL == "/UserRole/Index") {
                    setTimeout(function () {
                        $("#tblUserRole").load("/UserRole/Index #tblUserRole");
                    }, 1000);
                }
            });
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.responseText, "warning");
        }
    });
}


var ResetPasswordAdmin = function (id) {
    $('#titleMediumModal').html("<h4>Đặt lại mật khẩu</h4>");
    var url = "/UserManagement/ResetPasswordAdmin?id=" + id;
    loadMediumModal(url);
};

var ResetPasswordGeneral = function (ApplicationUserId) {
    $('#titleMediumModal').html("<h4>Đặt lại mật khẩu</h4>");
    var url = "/UserProfile/ResetPasswordGeneral?ApplicationUserId=" + ApplicationUserId;
    loadMediumModal(url);
};

var DeleteUserAccount = function (id) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa user?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "DELETE",
                url: "/UserManagement/DeleteUserAccount?id=" + id,
                success: function (result) {
                    var message = "User đã xóa thành công";
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblUserAccount').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};


var AddEditUserAccount = function (id) {
    var url = "/UserManagement/AddEditUserAccount?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Chỉnh sửa thông tin");
    }
    else {
        $('#titleExtraBigModal').html("Người dùng mới");
    }
    loadExtraBigModal(url);

    setTimeout(function () {
        $('#FirstName').focus();
    }, 200);
};

var SaveUser = function () {
    $('#ProfilePictureDetails').removeAttr('required');
    console.log($("#ProfilePictureDetails").val());
    if (!$("#ApplicationUserForm").valid()) {
        return;
    }

    if (!FieldValidation('#FirstName')) {
        FieldValidationAlert('#FirstName', 'First Name is Required.', "warning");
        return;
    }
    if (!FieldValidation('#LastName')) {
        FieldValidationAlert('#LastName', 'Last Name is Required.', "warning");
        return;
    }

    if (!$("#ApplicationUserForm").valid()) {
        FieldValidationAlert('#ConfirmPassword', 'Please fill up all input properly.', "warning");
        return;
    }

    var _UserProfileId = $("#UserProfileId").val();
    if (_UserProfileId > 0) {
        $("#btnSave").prop('value', 'Cập nhật thông tin');
    }
    else {
        $("#btnSave").prop('value', 'Thêm mới thông tin');
    }
    $('#btnSave').prop('disabled', true);

    $.ajax({
        type: "POST",
        url: "/UserManagement/AddEditUserAccount",
        data: PreparedFormObj(),
        processData: false,
        contentType: false,
        success: function (result) {
            console.log(result);
            $('#btnSave').prop('disabled', false);
            $("#btnAddProfile").prop('value', 'Save');
            if (result.IsSuccess) {
                Swal.fire({
                    title: result.AlertMessage,
                    icon: "success"
                }).then(function () {
                    document.getElementById("btnAddEditUserAccountClose").click();
                    if (result.CurrentURL == "/") {
                        setTimeout(function () {
                            $("#tblRecentRegisteredUser").load("#tblRecentRegisteredUser");
                        }, 1000);
                    }
                    else if (result.CurrentURL == "/UserProfile/Index") {
                        $("#divUserProfile").load("/UserProfile/Index #divUserProfile");
                    }
                    else {
                        document.getElementById("btnAddEditUserAccountClose").click();
                        $('#tblUserAccount').DataTable().ajax.reload();
                    }
                });
            }
            else {
                Swal.fire({
                    title: result.AlertMessage,
                    icon: "warning"
                }).then(function () {
                    $("#btnSave").prop('value', 'Save');
                    setTimeout(function () {
                        $('#Email').focus();
                    }, 400);
                });
            }
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.responseText, "warning");
        }
    });
}


var PreparedFormObj = function () {
    var _FormData = new FormData()
    _FormData.append('Id', $("#Id").val())
    _FormData.append('UserProfileId', $("#UserProfileId").val())
    _FormData.append('ApplicationUserId', $("#ApplicationUserId").val())
    _FormData.append('ProfilePictureDetails', $('#ProfilePictureDetails')[0].files[0])
    _FormData.append('GroupUserId', $('#GroupUserId').val())

    _FormData.append('FirstName', $("#FirstName").val())
    _FormData.append('LastName', $("#LastName").val())
    _FormData.append('PhoneNumber', $("#PhoneNumber").val())
    _FormData.append('Email', $("#Email").val())
    _FormData.append('PasswordHash', $("#PasswordHash").val())

    _FormData.append('ConfirmPassword', $("#ConfirmPassword").val())
    _FormData.append('Address', $("#Address").val())
    _FormData.append('Country', $("#Country").val())
    _FormData.append('CurrentURL', $("#CurrentURL").val())
    return _FormData;
}