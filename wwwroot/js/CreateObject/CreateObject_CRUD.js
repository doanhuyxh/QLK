var Details = function (id) {
    var url = "/CreateObject/Details?id=" + id;
    $('#titleBigModal').html("Asset Categories Details");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/CreateObject/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Sửa view");
    }
    else {
        $('#titleExtraBigModal').html("Thêm view");
    }
    loadExtraBigModal(url);
};
var Save = function () {
    if (!$("#frmCreateObject").valid()) {
        SwalSimpleAlert("Có lỗi vui lòng kiểm tra", "warning");
        return;
    }
    var Id = $('#Id').val();
    var _frmCreateObject = new FormData();
    var form_data = $('#frmCreateObject').serializeArray();
    $.each(form_data, function (key, input) {
        _frmCreateObject.append(input.name, input.value);
    });
 
    $('.table-create-object *').removeClass("error");
    var flag_error = false;
    if (app_vujs.$data.items.length==0) {
        SwalSimpleAlert("Bạn vui lòng thêm các trường vào cho view", "warning");
        return;
    }
    let totalParentId = [];
    app_vujs.$data.items.forEach((a_item, i) => {
        if (a_item.KieuDuLieu == "ParentId") {
            totalParentId.push(a_item);
        }
    });
    if (totalParentId.length > 1) {
        SwalSimpleAlert("Đang tồn tại 2 trường kiểu Parent Id, trong một view chỉ được tồn tại một trường kiểu parent id", "warning");
        totalParentId.forEach((item, index) => {

            $(`.table-create-object tbody tr.tr-1`).eq(parseInt(item.indexInList)).find('td').addClass('error');
        });
        return;
    }
    let DuplicatesItems = [];
    app_vujs.$data.items.forEach((a_item, i) => {
        if (a_item.Label.toLowerCase().trim() === "") {
            $(`.table-create-object tbody tr.tr-1`).eq(parseInt(i)).find('td').addClass('error');
            SwalSimpleAlert("Nhãn cho các trường không được phép để rỗng", "warning");
            return;
        }
        

        a_item.indexInList = i;
        app_vujs.$data.items.forEach((b_item, j) => {
            if (i === j) return null;
            if (a_item.TenTruong.toLowerCase() === b_item.TenTruong.toLowerCase()) {
                if (!DuplicatesItems.includes(a_item)) {
                    DuplicatesItems.push(a_item);
                }
            }
        });
    });
    if (DuplicatesItems.length > 0) {
        console.log("DuplicatesItems", DuplicatesItems);
        SwalSimpleAlert("Có một số trường bị trùng lặp xin vui lòng kiểm tra lại", "warning");
        DuplicatesItems.forEach((item, index) => {

            $(`.table-create-object tbody tr.tr-1`).eq(parseInt(item.indexInList)).find('td').addClass('error');
        });
        return;
    }

    let DuplicatesItemsOfClass = [];
    app_vujs.$data.items.forEach((a_item, i) => {
       

        a_item.indexInList = i;
        app_vujs.$data.items.forEach((b_item, j) => {
            if (i === j) return null;
            if (a_item.TenTruong.toLowerCase() === app_vujs.$data.Name.toLowerCase()) {
                if (!DuplicatesItemsOfClass.includes(a_item)) {
                    DuplicatesItemsOfClass.push(a_item);
                }
            }
        });
    });
    if (DuplicatesItemsOfClass.length > 0) {
        console.log("DuplicatesItemsOfClass", DuplicatesItemsOfClass);
        SwalSimpleAlert("Tên các trường không được phép trùng với tên của view", "warning");
        DuplicatesItemsOfClass.forEach((item, index) => {

            $(`.table-create-object tbody tr.tr-1`).eq(parseInt(item.indexInList)).find('td').addClass('error');
        });
        return;
    }

    

    for (var i_index_curent = 0; i_index_curent < app_vujs.$data.items.length; i_index_curent++) {
        curent_item = app_vujs.$data.items[i_index_curent];
        if (curent_item.Label.toLowerCase().trim() == "") {
            SwalSimpleAlert("Có một số nhãn không được để trống", "warning");
            $(`.table-create-object tbody tr.tr-1:eq(${i_index_curent})`).find('td').addClass('error');
            return;
        }
        if (curent_item.Label.toLowerCase().trim().length<5) {
            SwalSimpleAlert("Có một số nhãn để quá ngắn việc này gây ra việc đọc code khó hiểu, số ký tự phải lớn hơn 5", "warning");
            $(`.table-create-object tbody tr.tr-1:eq(${i_index_curent})`).find('td').addClass('error');
            return;
        }
        if (curent_item.KieuDuLieu == "table" && curent_item.TenBang.trim() === "") {
            SwalSimpleAlert("Vui lòng nhập tên bảng", "warning");
            $(`.table-create-object tbody tr.tr-2-${i_index_curent}`).find('.table-name').addClass('error');
            return;
        }
        if (curent_item.KieuDuLieu == "table") {
            for (var j_index_curent = 0; j_index_curent < curent_item.sub_items.length; j_index_curent++) {

                if (curent_item.sub_items[j_index_curent].Label.toLowerCase().trim() == "") {
                    SwalSimpleAlert("Có một số nhãn con trong kiểu dữ liệu bảng không được để trống", "warning");
                    $(`.table-create-object tbody tr.tr-2-${i_index_curent} td .table-sub-create-object tbody tr:eq(${j_index_curent})`).find('td').addClass('error');
                    return;
                }
                if (curent_item.sub_items[j_index_curent].Label.toLowerCase().trim().length<5) {
                    SwalSimpleAlert("Có một số nhãn để quá ngắn việc này gây ra việc đọc code khó hiểu, số ký tự phải lớn hơn 5", "warning");
                    $(`.table-create-object tbody tr.tr-2-${i_index_curent} td .table-sub-create-object tbody tr:eq(${j_index_curent})`).find('td').addClass('error');
                    return;
                }
            }

        }
       

    }

    for (var i_index_curent = 0; i_index_curent < app_vujs.$data.items.length; i_index_curent++) {
        curent_item = app_vujs.$data.items[i_index_curent];

        if (curent_item.KieuDuLieu == "table") {
            let DuplicatesSubItems = [];
            curent_item.sub_items.forEach((a_item, i_index) => {
                a_item.indexInList = i_index;
                curent_item.sub_items.forEach((b_item, j_index) => {
                    if (i_index === j_index) return null;
                    if (a_item.TenTruong.toLowerCase() === b_item.TenTruong.toLowerCase()) {
                        if (!DuplicatesSubItems.includes(a_item)) {
                            DuplicatesSubItems.push(a_item);
                        }
                    }
                });
            });
            if (DuplicatesSubItems.length > 0) {
                console.log("DuplicatesSubItems", DuplicatesSubItems);
                SwalSimpleAlert("Có một số trường con trong kiểu dữ liệu bảng bị trùng lặp xin vui lòng kiểm tra lại", "warning");
                DuplicatesSubItems.forEach((item, index) => {
                    //TODTO: thong bao loi cu the
                    $(`.table-create-object tbody tr.tr-2-${index} td .table-sub-create-object tbody tr`).find('td').addClass('error');
                });
                return;
            }
        }

    }
    for (var i_index_curent = 0; i_index_curent < app_vujs.$data.items.length; i_index_curent++) {
        curent_item = app_vujs.$data.items[i_index_curent];
        if (curent_item.KieuDuLieu == "table" && curent_item.sub_items.length == 0) {
            SwalSimpleAlert("Bạn vui lòng thêm các trường con cho kiểu dữ liệu bảng", "warning");
            return;
        }
    }
    

   
    
    
    
    let ListNoSelectDropdown = [];
    for (var i_index_curent = 0; i_index_curent < app_vujs.$data.items.length; i_index_curent++) {
        a_item = app_vujs.$data.items[i_index_curent];
        if (a_item.KieuDuLieu == "dropdown") {
            console.log("a_item", a_item);
            a_item.indexInList = i_index_curent;
            no_select_ListFieds = false;
            a_item.ListFieds.forEach((field) => {
                if (field.Value) {
                    no_select_ListFieds = true;
                }
            });
            if (!no_select_ListFieds) {
                ListNoSelectDropdown.push(a_item);
            }
        }
        if (a_item.KieuDuLieu == "table") {
            let ListSubItemNoSelectDropdown = [];
            for (var i_index_sub_item = 0; i_index_sub_item < a_item.sub_items.length; i_index_sub_item++) {
                b_item = a_item.sub_items[i_index_sub_item];
                if (b_item.KieuDuLieu == "dropdown") {
                    console.log("b_item", b_item);
                    b_item.indexInList = i_index_sub_item;
                    no_select_ListFieds = false;
                    b_item.ListFieds.forEach((field) => {
                        if (field.Value) {
                            no_select_ListFieds = true;
                        }
                    });
                    if (!no_select_ListFieds) {
                        ListSubItemNoSelectDropdown.push(b_item);
                    }
                }
            }
            if (ListSubItemNoSelectDropdown.length > 0) {
                SwalSimpleAlert("Vui lòng chọn các trường cho dropdown trong kiểu table", "warning");
                ListSubItemNoSelectDropdown.forEach((item, index) => {
                    $(`.table-create-object tbody tr.tr-2-${i_index_sub_item} .table-sub-create-object tbody tr`).find('td').addClass('error');
                });
                return;
            }
        }
    }
    

    
    if (ListNoSelectDropdown.length > 0) {
        SwalSimpleAlert("Vui lòng chọn các trường cho dropdown", "warning");
        ListNoSelectDropdown.forEach((item, index) => {

            $(`.table-create-object tbody tr.tr-1`).eq(parseInt(item.indexInList)).find('td').addClass('error');
        });
        return;
    }
    let ListNoSelectParentId = [];
    app_vujs.$data.items.forEach((a_item, i) => {
        if (a_item.KieuDuLieu == "ParentId") {
            console.log("a_item", a_item);
            a_item.indexInList = i;
            no_select_ListFieds = false;
            a_item.ListFieds.forEach((field, i) => {
                if (field.Value) {
                    no_select_ListFieds = true;
                }
            });
            if (!no_select_ListFieds) {
                ListNoSelectParentId.push(a_item);
            }
        }
    });

    
    if (ListNoSelectParentId.length > 0) {
        SwalSimpleAlert("Vui lòng chọn các trường hiển thị cho trường ParentId", "warning");
        ListNoSelectParentId.forEach((item, index) => {

            $(`.table-create-object tbody tr.tr-1`).eq(parseInt(item.indexInList)).find('td').addClass('error');
        });
        return;
    } 
    
    let listFirstIsNumber = [];
    app_vujs.$data.items.forEach((a_item, i) => {
        a_item.indexInList = i;
        if (/^\d/.test(a_item.TenTruong)) {
            listFirstIsNumber.push(a_item);
        }
    });
   

    if (listFirstIsNumber.length > 0) {
        SwalSimpleAlert("Chữ cái đầu tiên của tên trường phải bắt đầu là ký tự", "warning");
        listFirstIsNumber.forEach((item, index) => {

            $(`.table-create-object tbody tr.tr-1`).eq(parseInt(item.indexInList)).find('td').addClass('error');
        });
        return;
    }

    for (var key in app_vujs.$data.items) {

        var item = app_vujs.$data.items[key];
        console.log("item", item);
        var $tr = $(`.table-create-object tbody tr.tr-1`).eq(parseInt(key));
        for (var key_item in item) {
            var valueOfItem = item[key_item];
            
            switch (key_item) {
                case "KieuDuLieu":
                    _frmCreateObject.append(`ObjectFields[${key}][${key_item}]`, valueOfItem);
                    if (valueOfItem.trim() === "table") {
                        _frmCreateObject.append(`ObjectFields[${key}][TenBang]`, item.TenBang);
                    }
                    break;
                case "Id":
                case "DoLon":
                case "RowIndex":
                case "ColumnIndex":
                case "HienThiTrongBang":
                case "BatBuocNhap":
                case "LabelTenBang":
                    _frmCreateObject.append(`ObjectFields[${key}][${key_item}]`, valueOfItem);
                    break;
                case "Label":
                case "TenTruong":
                    if (valueOfItem.trim() === "") {

                        $tr.find(`td input.input-Label`).addClass('error');
                        $tr.find(`td input.input-${key_item}`).addClass('error');
                        $tr.find(`td select.select-${key_item}`).addClass('error');
                        SwalSimpleAlert("Tên trường, hoặc kiểu dữ liệu không được trống", "warning");
                        flag_error = true;
                    }
                    _frmCreateObject.append(`ObjectFields[${key}][${key_item}]`, valueOfItem);
                    break;
                case "DoLon":
                    if ((item.KieuDuLieu == "string" || item.KieuDuLieu =="textarea") &&  (valueOfItem == "" || parseInt(valueOfItem) < 0)) {
                        $tr.find(`td input.input-${key_item}`).addClass('error');
                        $tr.find(`td input.input-Label`).addClass('error');
                        flag_error = true;
                    }
                    _frmCreateObject.append(`ObjectFields[${key}][${key_item}]`, valueOfItem);
                    break;
                case "ListFieds":
                    if (valueOfItem != null) {  
                        valueOfItem = valueOfItem.map(function (item) {
                            return {
                                Key: item.Key,
                                Value: item.Value,
                                }
                        });
                    } else {
                        valueOfItem = [];
                    }
                    if (valueOfItem != null) {
                        valueOfItem = valueOfItem.map(function (item, index) {
                            _frmCreateObject.append(`ObjectFields[${key}][${key_item}][${index}][TenTruong]`, item.Key);
                            _frmCreateObject.append(`ObjectFields[${key}][${key_item}][${index}][Selected]`, item.Value);
                        });
                    } 
                    break;
                // code block
                case "subItemsNeedDelete":
                    console.log("valueOfItem", valueOfItem);
                    for (var indexValueOfItem = 0; indexValueOfItem < valueOfItem.length; indexValueOfItem++) {
                        _frmCreateObject.append(`ObjectFields[${key}][${key_item}][${indexValueOfItem}]`, valueOfItem[indexValueOfItem]);
                    }
                    
                    break;
                case "sub_items":
                    for (var i_sub_item = 0; i_sub_item < valueOfItem.length; i_sub_item++) {
                        var sub_item_flag_error = false;
                        var $tr_sub_item = $(`.table-create-object tbody tr.tr-2-${key} .table-sub-create-object tbody tr`);
                        var sub_item = valueOfItem[i_sub_item];
                        
                        for (var key_sub_item in sub_item) {
                            var valueOfSubItem = sub_item[key_sub_item];
                            switch (key_sub_item) {
                                case "Label":
                                case "TenTruong":
                                case "KieuDuLieu":
                                    if (valueOfSubItem.trim() === "") {
                                        $tr_sub_item.find(`td input.input-Label`).addClass('error');
                                        $tr_sub_item.find(`td input.input-${key_sub_item}`).addClass('error');
                                        $tr_sub_item.find(`td select.select-${key_sub_item}`).addClass('error');
                                        SwalSimpleAlert("Tên trường, hoặc kiểu dữ liệu trong bản con không được trống", "warning");
                                        sub_item_flag_error = true;
                                    }
                                    _frmCreateObject.append(`ObjectFields[${key}][sub_items][${i_sub_item}][${key_sub_item}]`, valueOfSubItem);
                                    break;
                                case "Id":
                                case "DoLon":
                                case "HienThiTrongBang":
                                case "BatBuocNhap":
                                    _frmCreateObject.append(`ObjectFields[${key}][sub_items][${i_sub_item}][${key_sub_item}]`, valueOfSubItem);
                                    break;
                                case "DoLon":
                                    if ((sub_item.KieuDuLieu == "string" || sub_item.KieuDuLieu == "textarea") && (valueOfSubItem == "" || parseInt(valueOfSubItem) < 0)) {
                                        $tr_sub_item.find(`td input.input-${key_sub_item}`).addClass('error');
                                        $tr_sub_item.find(`td input.input-Label`).addClass('error');
                                        SwalSimpleAlert("Vui lòng nhập độ lớn cho các trường con trong bản", "warning");
                                        sub_item_flag_error = true;
                                    }
                                    _frmCreateObject.append(`ObjectFields[${key}][sub_items][${i_sub_item}][${key_sub_item}]`, valueOfSubItem);
                                    break;
                                case "ListFieds":
                                    if (valueOfSubItem != null) {
                                        valueOfSubItem = valueOfSubItem.map(function (item) {
                                            return {
                                                Key: item.Key,
                                                Value: item.Value,
                                            }
                                        });
                                    } else {
                                        valueOfSubItem = [];
                                    }
                                    if (valueOfSubItem != null) {
                                        valueOfSubItem = valueOfSubItem.map(function (item, index) {
                                            _frmCreateObject.append(`ObjectFields[${key}][sub_items][${i_sub_item}][${key_sub_item}][${index}][TenTruong]`, item.Key);
                                            _frmCreateObject.append(`ObjectFields[${key}][sub_items][${i_sub_item}][${key_sub_item}][${index}][Selected]`, item.Value);
                                        });
                                    }
                                    break;
                               
                            }

                        }
                        if (sub_item_flag_error) {
                            return false;
                        }
                    }
                   
                    break;
               
                // code block
            }
            
           
        }
    }
    if (flag_error) {
        return false;
    }
    if (app_vujs.GetListFieldsOfPending().length > 0) {
        SwalSimpleAlert("Bạn cần phải xây dưng form trước khi lưu", "warning");
        return;
    }
    console.log("_frmCreateObject", _frmCreateObject);
    if (app_vujs.$data.itemsNeedDelete.length > 0) {
        console.log("app_vujs.$data.itemsNeedDelete", app_vujs.$data.itemsNeedDelete);
        for (var key in app_vujs.$data.itemsNeedDelete) {
            _frmCreateObject.append(`itemsNeedDelete[${key}]`, app_vujs.$data.itemsNeedDelete[key]);
        }
    }
    
    Swal.fire({
        title: 'Bạn có chắc chắn muốn lưu?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $("#btnSave").val("Vui lòng đợi");
            $('#btnSave').attr('disabled', 'disabled');
            $.ajax({
                type: "POST",
                processData: false,
                contentType: false,
                enctype: 'multipart/form-data',
                url: "/CreateObject/AddEdit",
                data: _frmCreateObject,
                success: function (result) {
                    if (result.IsSuccess == true) {
                        Swal.fire({
                            title: result.AlertMessage,
                            icon: "success"
                        }).then(function () {
                            document.getElementById("btnClose").click();
                            $("#btnSave").val("Save");
                            $('#btnSave').removeAttr('disabled');
                            $('#tblCreateObject').DataTable().ajax.reload();
                        });
                    } else {
                        $("#btnSave").val("Save");
                        $('#btnSave').removeAttr('disabled');
                        SwalSimpleAlert(result.AlertMessage, "warning");
                    }
                },
                error: function (errormessage) {
                    SwalSimpleAlert(errormessage.responseText, "warning");
                    $("#btnSave").val("Save");
                    $('#btnSave').removeAttr('disabled');
                }
            });
        }
    });


}


var Delete = function (id) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xoá, vì nếu xoá thì hệ thống sẽ xoá cải thư mục?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "POST",
                url: "/CreateObject/Delete?id=" + id,
                success: function (result) {
                    var message = "Đã xoá thành công ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblCreateObject').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};
