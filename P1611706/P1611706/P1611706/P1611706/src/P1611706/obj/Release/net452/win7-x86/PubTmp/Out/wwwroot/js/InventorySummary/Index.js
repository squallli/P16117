var time = setInterval(removeStyle, 100);
var InventoryTable;
var InventoryDate;
var InventoryNo;

var PalletsDetailTable;
var BagTable;
var IsSummary = false;
Date.prototype.yyyymmdd = function () {
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();

    return [
        this.getFullYear(),
        (mm > 9 ? '' : '0') + mm,
        (dd > 9 ? '' : '0') + dd
    ].join('/');
};
$(document).ready(function () {
    $('#InventoryDate').datepicker({
        format: 'yyyy/mm/dd', //顯示的日期格式
        autoclose: true //選擇日期后自動關閉
    });
    $("#InventoryDate").on('change', function () {
        if ($('#InventoryDate').val() != "" ){
            GetInventoryNo();
        }
    });

    var invDate = new Date();
    $("#InventoryDate").val(invDate.yyyymmdd());
    if ($('#InventoryDate').val() != "") {
        GetInventoryNo();
    }
    $("#btnDate").on('click', function () {
        $('#InventoryDate').focus();
    });

    GetInventoryTable()
    GetPalletsDetailTable()
    // unblock when ajax activity stops 
    $(document).ajaxStop($.unblockUI); 
    $('#DivDetail').hide();
    $('#DivHead').show();
});
function GetInventoryTable() {
    InventoryTable = $('#Inventory-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "deferLoading": 0,
        "ajax": {
            "url": "api/InventorySummaryApi",
            "type": 'GET'
        },
        "columns": [
            { "data": "productName" },
            { "data": "palletsNo" },
            { "data": "qty" },
            { "data": "actualQty" }
        ],
        "columnDefs": [
            {

                "render": function (data, type, row) {
                    return '<button class="btn btn-primary" type="button" onclick="btnDetail(\'' + row.palletsNo + '\');"><i class="glyphicon glyphicon-list-alt"></i>Detail</button>';

                },
                "targets": 4
            }
        ]
    });

    $("#Inventory-table_filter").css("display", "none");
}
//移除table style屬性
function removeStyle() {
    var style = $('#Inventory-table').attr('style');

    if (ready == true)
        clearInterval(time);

    if (typeof style !== typeof undefined && style !== false) {
        ready = true;
    }

    $('#Inventory-table').removeAttr("style");
}
function GetInventoryNo() {

    var date = $('#InventoryDate').val().replace(/\//g, '');
    $.ajax({
        type: 'GET',
        url: 'api/InventorySummaryApi/' + date,
        cache: false,
        datatype: 'json',
        success: function (data) {
            $("#InventoryNo").empty();
            if (data.length > 0) {
                $("#InventoryNo").append($("<option></option>").attr("value", "").text("---請選擇---"));
                for (var i = 0; i < data.length; i++) {
                    $("#InventoryNo").append($("<option></option>").attr("value", data[i]).text(data[i]));
                }
            }
            else {
                $("#InventoryNo").append($("<option></option>").attr("value", "").text("--此日期無盤點底稿--"));
            }
            
        },
        error: function (response) {
            alert("系統發生錯誤");
        }
    });

}
function Search() {
    if ($('#InventoryNo').val() != "")
    {
        $.blockUI({ message: '<h1>查詢中，請稍後!</h1>' });
        InventoryDate = $('#InventoryDate').val().replace(/\//g, '');
        InventoryNo = $('#InventoryNo').val();
        InventoryTable.search(InventoryNo).draw();
    }
    else {
        alert("請選擇盤點底稿");
    }
    
}
function btnDetail(PalletsNo) {    

    $('#txtInventory_Date').val(InventoryDate);
    $('#txtInventory_No').val(InventoryNo);
    $('#txtPallets_No').val(PalletsNo);
    $('#DivHead').hide();
    $('#DivDetail').show();
    searchTable();
    GetIsSummary();
    $('#TitlePage').text(" > 棧板細項");
    //var form = $('<form></form>');
    //form.attr("method", "post");
    //form.attr("action", "InventorySummary/PalletsDetail");
    //var $InventoryDate = $('<input></input>');
    //$InventoryDate.attr("type", "hidden");
    //$InventoryDate.attr("name", "InventoryDate");
    //$InventoryDate.attr("value", InventoryDate);
    //form.append($InventoryDate);
    //var $InventoryNo = $('<input></input>');
    //$InventoryNo.attr("type", "hidden");
    //$InventoryNo.attr("name", "InventoryNo");
    //$InventoryNo.attr("value", InventoryNo);
    //form.append($InventoryNo);
    //var $PalletsNo = $('<input></input>');
    //$PalletsNo.attr("type", "hidden");
    //$PalletsNo.attr("name", "PalletsNo");
    //$PalletsNo.attr("value", PalletsNo);
    //form.append($PalletsNo);


    //$(document.body).append(form);
    //form.submit();

}


function Summary() {
    if ($('#InventoryNo').val() != "") {
        if (confirm("是否確認彙總-" + $('#InventoryNo').val() + ",彙總後無法再修改盤點內容。")) {
            $.blockUI({ message: '<h1>處理中，請稍後!</h1>' });
            $.ajax({
                type: 'POST',
                url: 'api/InventorySummaryApi',
                data: { InventoryDate: $('#InventoryDate').val().replace(/\//g, ''), InventoryNo: $('#InventoryNo').val() },
                dataType: "text",
                success: function (data) {
                    if (data == "success") {
                        alert("盤點底稿-" + $('#InventoryNo').val() + "彙總成功!");
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
    }
    else {
        alert("請選擇盤點底稿");
        return;
    }
}




function GetPalletsDetailTable() {
    PalletsDetailTable = $('#palletsLists-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "deferLoading": 0,
        "ajax": {
            "url": "api/InventoryPalletsDetailApi",
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
}
function searchTable() {
    $.blockUI({ message: '<h1>查詢中，請稍後!</h1>' });
    var data = "{ 'InventoryDate': '" + $('#txtInventory_Date').val() + "','InventoryNo': '" + $('#txtInventory_No').val() + "','PalletsNo': '" + $('#txtPallets_No').val() + "'}";
    PalletsDetailTable.search(data).draw();
};
$('#btnDetail').on('click', function () {
    CaseDetail();
})

function GetIsSummary() {
    $.ajax({
        type: 'POST',
        url: 'api/InventoryPalletsDetailApi',
        data: { InventoryNo: $('#txtInventory_No').val() },
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
        '<div class="form-group has-feedback has-clear">' +
        '<input type="text" class="form-control pull-right" id="txtBagNo">' +
        '</div>' +
        '<span class="input-group-btn">' +
        '<button Id="btnAddBag" class="btn btn-info btn-flat" >新增</button>' +
        '</span>' +
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
                        url: 'api/InventoryPalletsBagApi',
                        data: { InventoryDate: $('#txtInventory_Date').val(), InventoryNo: $('#txtInventory_No').val(), PalletsNo: $('#txtPallets_No').val(), CaseNo: CaseNo, BagNo: $('#txtBagNo').val() },
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
            "url": "api/InventoryPalletsBagApi",
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
            url: 'api/InventoryPalletsBagApi',
            data: { InventoryNo: $('#txtInventory_No').val(), CaseNo: CaseNo, BagNo: BagNo },
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
function Return() {
    $('#TitlePage').text("");
    $('#DivDetail').hide();
    $('#DivHead').show();
}