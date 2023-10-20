$(document).ready(function () {
    document.title = 'Asset Categories';
    $("#tblQuanLyMenu").DataTable({
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
            "url": "/QuanLyMenu/GetDataTabelData",
            "type": "POST",
            "datatype": "json"
        },


        "columns": [
            {
                data: "Id", "name": "Id", render: function (data, type, row) {
                    return "<a href='#' class='fa fa-eye' onclick=Details('" + row.Id + "');>" + row.Id + "</a>";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<i  class='fas " + row.Icon + "'></i> <a href='javascript:void(0)' class='btn btn-link btn-xs' onclick=AddEdit('" + row.Id + "');>" + row.Name +"</a>";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<i  class='fas " + row.Icon + "'></i> <a href='javascript:void(0)' class='btn btn-link btn-xs' onclick=AddEdit('" + row.Id + "');>" + row.Name + "</a>";
                }
            },
            { "data": "View", "name": "View" },
            { "data": "Parameter", "name": "Parameter" },
            { "data": "Description", "name": "Description" },

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
                    return "<a href='javascript:void(0)' class='btn btn-info btn-xs' onclick=AddEdit('" + row.Id + "');>Edit</a>";
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='javascript:void(0)' class='btn btn-danger btn-xs' onclick=Delete('" + row.Id + "'); >Delete</a>";
                }
            }
        ],

        'columnDefs': [{
            'targets': [1,4, 5],
            'orderable': false,
            'className': "nowrap",
        }],

        "lengthMenu": [[20, 10, 15, 25, 50, 100, 200], [20, 10, 15, 25, 50, 100, 200]],
        "initComplete": function (settings, json) {
            var info = json.info;
            // do whatever with info here...
            console.log("json", json);

            const myTimeout = setTimeout(function () {
                
            }, 5000);

            function myStopFunction() {
                clearTimeout(myTimeout);
            }
        }
    });

});

