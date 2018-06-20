
var ready;
var time = setInterval(removeStyle, 100);
var packingListsTable;
var productName = "";
var packingTime = "";
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

    $('#PackingTimeStart').datepicker({
        format: 'yyyy/mm/dd', //顯示的日期格式
        autoclose: true //選擇日期后自動關閉
    });
    $('#PackingTimeEnd').datepicker({
        format: 'yyyy/mm/dd', //顯示的日期格式
        autoclose: true //選擇日期后自動關閉
    });
    var packingTimeDate = new Date();
    $("#PackingTimeStart").val(packingTimeDate.yyyymmdd());
    var logDate = new Date();
    $("#PackingTimeEnd").val(packingTimeDate.yyyymmdd());
    $("#btnDateStart").on('click', function () {
        $('#PackingTimeStart').focus();
    });
    $("#btnDateEnd").on('click', function () {
        $('#PackingTimeEnd').focus();
    });

	packingListsTable = $('#packingLists-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
            "url": "api/PackingListsApi",
            "type": 'GET'
        },
        "columns": [
			{ "data": "companyCode" },
			{ "data": "packingTime" },
			{ "data": "productName" },
			{ "data": "palletsNo" },
			{ "data": "lot" },
			{ "data": "quantity" }
        ],
        "columnDefs": [
            {

                "render": function (data, type, row) {
					return '<button class="btn btn-primary" type="button" onclick="btnDetail(\'' + row.companyCode + '\', \'' + row.packingTime + '\', \'' + row.productNo + '\', \'' + row.lot + '\');"><i class="glyphicon glyphicon-list-alt"></i>Detail</button>';
                    
                },
                "targets": 6
            }
        ]
    });
    
	$("#packingLists-table_filter").css("display", "none");
    GetProductName();
    searchPackingLists();
});

//移除table style屬性
function removeStyle() {
	var style = $('#packingLists-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#packingLists-table').removeAttr("style");
}
function GetProductName() {
    $.ajax({
        type: 'POST',
        url: "api/PackingListsApi",
        datatype: 'json',
        success: function (data) {
            $("#ProductName").autocomplete({
                source: data
            });
        },
        error: function (response) {
            alert("系統發生錯誤");
        }
    })

}
function searchPackingLists() {
    if ($('#PackingTimeStart').val() != "" && $('#PackingTimeEnd').val() == "") {
        alert("請輸入搜尋時間(迄)");
        $('#PackingTimeEnd').focus();
        return;
    }
    if ($('#PackingTimeStart').val() == "" && $('#PackingTimeEnd').val() != "") {
        alert("請輸入搜尋時間(起)");
        $('#PackingTimeStart').focus();
        return;
    }
    if ($('#PackingTimeStart').val() != "" && $('#PackingTimeEnd').val() != "") {
        if ($('#PackingTimeStart').val().replace(/\//g, '') > $('#PackingTimeEnd').val().replace(/\//g, '')) {
            alert("結束時間不得大於起始時間");
            $('#PackingTimeEnd').focus();
            return;
        }
    }
	productName = $('#ProductName').val();
    packingTime = $('#PackingTimeStart').val() + $('#PackingTimeEnd').val();
    var data = "{ 'PackingTime': '" + packingTime + "', 'ProductName': '" + $('#ProductName').val() + "' }";
	packingListsTable.search(data).draw();

}

//棧號細項
function btnDetail(companyCode, packingTime, productNo, lot)
{
	if (companyCode == "null")
		companyCode = "";
	if (packingTime == "null")
		packingTime = "";
	if (productNo == "null")
		productNo = "";
	if (lot == "null")
		lot = "";
	
	location.href = "PackingLists/Pallets?CompanyCode=" + companyCode + "&PackingTime=" + packingTime + "&ProductName=" + productNo + "&Lot=" + lot;

}


