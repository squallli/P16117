
var ready;
var time = setInterval(removeStyle, 100);
var packingPalletsListsTable;
var companyCode = "";
var packingTime = "";
var productName = "";
var lot = "";
$(document).ready(function () {
	var url = location.href;

	var temp = url.split("?");

	//將值再度分開
	var vars = temp[1].split("&");

	companyCode = vars[0].substring(12);
	packingTime = vars[1].substring(12);
	productName = vars[2].substring(12);
	lot = vars[3].substring(4);

	packingPalletsListsTable = $('#packingpalletsLists-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "deferLoading": 0,
        "ajax": {
			"url": "../api/PackingPalletsListsApi",
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
					return '<button class="btn btn-primary" type="button" onclick="btnDetail(\'' + row.palletsNo + '\', \'' + row.packingTime + '\');"><i class="glyphicon glyphicon-list-alt"></i>Detail</button>';
                    
                },
                "targets": 6
            }
        ]
    });
    
	$("#packingpalletsLists-table_filter").css("display", "none");
	searchPackingLists();
});

//移除table style屬性
function removeStyle() {
	var style = $('#packingpalletsLists-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#packingpalletsLists-table').removeAttr("style");
}

function searchPackingLists() {
	$('#packingpalletsLists-table').removeAttr("style");
	var data = "{ 'CompanyCode': '" + companyCode + "', 'PackingTime': '" + packingTime + "', 'ProductName': '" + productName + "', 'Lot': '" + lot + "' }";
	packingPalletsListsTable.search(data).draw();

}

//棧號細項
function btnDetail(palletsNo, packingTime)
{
    var url = location.href;
	if (palletsNo == "null")
		palletsNo = "";
	if (packingTime == "null")
		packingTime = "";


    if (url.indexOf("PackingListsE") >= 0)
        location.href = "../PackingListsE/Case?PalletsNo=" + palletsNo + "&PackingTime=" + packingTime + "";
    else
        location.href = "../PackingLists/Case?PalletsNo=" + palletsNo + "&PackingTime=" + packingTime + "";
}


