$(document).ready(function () {
    document.title = 'Asset Categories';

    $("#tblRelationshipTable").DataTable({
        paging: true,
        select: true,
        "order": [[0, "desc"]],
        dom: 'Bfrtip',


        buttons: [
            'pageLength',
        ],


        "processing": true,
        "serverSide": true,
        "filter": true, //Search Box
        "orderMulti": false,
        "stateSave": true,

        "ajax": {
            "url": "/RelationshipTable/GetDataTabelData",
            "type": "POST",
            "datatype": "json"
        },


        "columns": [
            {
                data: "Id", "name": "Id", render: function (data, type, row) {
                    return "<a href='#' class='fa fa-eye' onclick=Details('" + row.Id + "');>" + row.Id + "</a>";
                }
            },
            //{ "data": "Name", "name": "Name" },
            //{ "data": "Description", "name": "Description" },
            { "data": "TenBang", "name": "TenBang" },
{ "data": "MoTa", "name": "MoTa" },
            {
                "data": "CreatedDate",
                "name": "CreatedDate",
                "autoWidth": true,
                "render": function (data) {
                    var date = new Date(data);
                    var month = date.getMonth() + 1;
                    var formattedMonth = month < 10 ? '0' + month : month;
                    return date.getDate() + '/' + formattedMonth + '/' + date.getFullYear();                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-primary' onclick=AddEdit('" + row.Id + "');>Sửa</a>";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-danger' onclick=Delete('" + row.Id + "'); >Xóa</a>";
                }
            }
        ],

        'columnDefs': [{
            'targets': [4, 5],
            'orderable': false,
        }],

        "lengthMenu": [[20, 10, 15, 25, 50, 100, 200], [20, 10, 15, 25, 50, 100, 200]]
    });

});

