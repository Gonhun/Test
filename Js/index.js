$(document).ready(function () {
    loadGrid();
})

function loadGrid() {
    var grid = $("#gridUpload").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: "/Home/LoadData",
                    contentType: "application/json",
                    data: {param: $("#txtSearch").val()},
                    type: "POST",
                    cache: false
                },
                parameterMap: function (data, operation) {
                    return kendo.stringify(data)
                }
            },
            schema: {
                data: "Data",
                total: "Total",
                model: {
                    id: "ID",
                    fields: {
                        ID: { type: "string", editable: true },
                        CONTENT: { type: "string", editable: true }
                    }
                }
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,

        },
        editable: "inline",
        pageable: true,
        scrollable: true,
        resizable: true,
        pageable: {
            refresh: true,
            buttonCount: 10,
            input: true,
            pageSizes: [10, 20, 50, 100, 1000, 100000],
            info: true,
            messages: {
            }
        },
        columns: [
            {
                field: "ID", title: "ID", width: 50
            },
            {
                field: "CONTENT", title: "CONTENT", width: 100
            },
        ],
    });
}

$('#btnSearch').click(function () {
    loadGrid();
});

$('#UploadBtn').click(function () {

    var fileUpload = $("#Files").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();
    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }
    $.ajax({
        url: '/Home/UploadFiles',
        type: "POST",
        contentType: false,
        processData: false,
        data: fileData,
        async: false,
        success: function (result) {
            if (result != "") {
                alert(result.remarks);
            }
        },
        error: function (err) {
            alert(err.statusText);
        }
    });

});

