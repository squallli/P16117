
var ready;
var time = setInterval(removeStyle, 100);
var orderLogTable;
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

    $('#LogDateStart').datepicker({
        format: 'yyyy/mm/dd', //顯示的日期格式
        autoclose: true //選擇日期后自動關閉
    });
    $('#LogDateEnd').datepicker({
        format: 'yyyy/mm/dd', //顯示的日期格式
        autoclose: true //選擇日期后自動關閉
    });
    var logDate = new Date();
    $("#LogDateStart").val(logDate.yyyymmdd());
    var DoubleConfirmDate = new Date();
    $("#LogDateEnd").val(logDate.yyyymmdd());
    $("#btnDateStart").on('click', function () {
        $('#LogDateStart').focus();
    });
    $("#btnDateEnd").on('click', function () {
        $('#LogDateEnd').focus();
    });

	orderLogTable = $('#orderLog-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
            "url": "api/OrderLogApi",
            "type": 'GET'
        },
        "columns": [
			{ "data": "logDate" },
			{ "data": "logTime" },
			{ "data": "orderType" },
			{ "data": "orderNo" },
			{ "data": "workType" },
			{ "data": "productNo" },
			{ "data": "oriQty" },
			{ "data": "realQty" }
        ]
    });
    
	$("#orderLog-table_filter").css("display", "none");
	
	
});

//移除table style屬性
function removeStyle() {
	var style = $('#orderLog-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#orderLog-table').removeAttr("style");
}

function searchOrderLog() {
    if ($('#LogDateStart').val() != "" && $('#LogDateEnd').val() == "") {
        alert("請輸入搜尋時間(迄)");
        $('#LogDateEnd').focus();
        return;
    }
    if ($('#LogDateStart').val() == "" && $('#LogDateEnd').val() != "") {
        alert("請輸入搜尋時間(起)");
        $('#LogDateStart').focus();
        return;
    }
    if ($('#LogDateStart').val() != "" && $('#LogDateEnd').val() != "") {
        if ($('#LogDateStart').val().replace(/\//g, '') > $('#LogDateEnd').val().replace(/\//g, '')) {
            alert("結束時間不得大於起始時間");
            $('#LogDateEnd').focus();
            return;
        }
    }
    var data = "{ 'LogDate': '" + $('#LogDateStart').val() + $('#LogDateEnd').val() + "', 'OrderType': '" + $('#OrderType').val() + "', 'OrderNo': '" + $('#OrderNo').val() + "', 'ProductNo': '" + $('#ProductNo').val() + "' }";
	orderLogTable.search(data).draw();
    
}
