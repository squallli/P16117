var PalletsDetailTable;
var BagTable;
var IsSummary = false;
$(document).ready(function () {    
    PalletsDetailTable = $('#palletsLists-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "deferLoading": 0,
        "ajax": {
            "url": "../api/InventoryPalletsDetailApi",
            "type": 'GET'
        },
        "columns": [
            { "data": "caseNo" },
            { "data": "qty" },
            { "data": "actualQty" }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    
                    return '<button class="btn btn-primary" type="button" onclick="CaseDetail(\'' + row.caseNo + '\');"><i class="glyphicon glyphicon-list-alt"></i>Detail</button>';

                },
                "targets": 3
            }
        ]
    });
    $("#palletsLists-table_filter").css("display", "none");

    searchTable();
    GetIsSummary();
});

function searchTable() {
    var data = "{ 'InventoryDate': '" + $('#Inventory_Date').val() + "','InventoryNo': '" + $('#Inventory_No').val() + "','PalletsNo': '" + $('#Pallets_No').val() + "'}";
    PalletsDetailTable.search(data).draw();
};
$('#btnDetail').on('click', function () {
    CaseDetail();
})
function GetIsSummary() {
    $.ajax({
        type: 'POST',
        url: '../api/InventoryPalletsDetailApi',
        data: { InventoryNo: $('#Inventory_No').val() },
        success: function (data) {
            if (data == true) {
                IsSummary = true;                
            }
        },
        error: function (response) {
            alert("系統發生錯誤");
        }
    })
}
function CaseDetail(CaseNo) {

    BootstrapDialog.show({
        title: '箱號-' + CaseNo,
        message: '<div class="input-group">' +
        '<span class="input-group-addon">袋號</span>' +
        '<div class="form-group has-feedback has-clear">'+
        '<input type="text" class="form-control pull-right" id="txtBagNo">' +
        '</div>'+
        '<span class="input-group-btn">'+
        '<button Id="btnAddBag" class="btn btn-info btn-flat" >新增</button>'+
        '</span>'+
        '</div>' +
        '</br>' +
        '箱號細項' +
        '<table id="bag-table" class="table  table-bordered table-hover">' +
        '<thead>' +
        '<tr>' +
        '<th style="width:15%">項次</th>' +
        '<th style="width:70%">袋號</th>' +
        '<th style="width:15%">刪除</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>' +
        '</tbody>' +
        '</table>',
        onshown: function (dialog) {
            GetBagTable(CaseNo)
            BagTable.search(CaseNo).draw()

            $('#btnAddBag').on('click', function () {
                if (!IsSummary) {
                    alert("已匯總無法新增");
                    return;
                }
                if ($('#txtBagNo').val() == "") {
                    alert("請輸入袋號");
                    return;
                }
                if (confirm("是否確定新增?")) {
                    $.ajax({
                        type: 'POST',
                        url: '../api/InventoryPalletsBagApi',
                        data: { InventoryDate: $('#Inventory_Date').val(), InventoryNo: $('#Inventory_No').val(), PalletsNo: $('#Pallets_No').val(), CaseNo: CaseNo, BagNo: $('#txtBagNo').val() },
                        dataType: "text",
                        success: function (data) {
                            if (data == "success") {
                                alert("新增成功!");
                                $('#txtBagNo').val('');
                                BagTable.draw(false);
                                PalletsDetailTable.draw(false);
                            }
                            else {
                                alert(data);
                            }
                        },
                        error: function (response) {
                            alert("系統發生錯誤");
                        }
                    })
                }
            })
        }
    });
}
function GetBagTable(CaseNo) {
    BagTable = $('#bag-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "deferLoading": 0,
        "ajax": {
            "url": "../api/InventoryPalletsBagApi",
            "type": 'GET'
        },
        "columns": [
            { "data": "item" },
            { "data": "bag" }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {

                    return '<button class="btn btn-primary" type="button" onclick="DeleteBag(\'' + row.bag + '\',\'' + CaseNo + '\');"><i class="glyphicon glyphicon-remove"></i></button>';

                },
                "targets": 2
            }
        ]
    });
    $("#bag-table_filter").css("display", "none");
}
function DeleteBag(BagNo, CaseNo) {
    if (!IsSummary) {
        alert("已匯總無法刪除")
        return;
    }
    if (confirm("是否確定刪除?")) {
        $.ajax({
            type: 'DELETE',
            url: '../api/InventoryPalletsBagApi',
            data: { InventoryNo: $('#Inventory_No').val(), CaseNo: CaseNo, BagNo: BagNo },
            dataType: "text",
            success: function (data) {
                if (data == "success") {
                    alert("作廢成功!");
                    BagTable.draw(false);
                    PalletsDetailTable.draw(false);
                }
            },
            error: function (response) {
                alert("系統發生錯誤");
            }
        })
    }
    
}