
var ready;
var time = setInterval(removeStyle, 100);
var packingCaseTable;
var bagTable;

$(document).ready(function () {
	var url = location.href;
	
	var temp = url.split("?");

	//將值再度分開
	var vars = temp[1].split("&");

	$('#PalletsNo').val(vars[0].substring(10));
	$('#PackingTime').val(vars[1].substring(12));
	
	packingCaseTable = $('#packingCase-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
			"url": "../api/PackingCaseApi",
            "type": 'GET'
        },
        "columns": [
			{ "data": "item" },
			{ "data": "caseNo" },
			{ "data": "quantity" },
        ],
        "columnDefs": [
            {

                "render": function (data, type, row) {
					return '<button class="btn btn-primary" type="button" onclick="btnDetail(\'' + $('#PalletsNo').val() + '\', \'' + $('#PackingTime').val() + '\', \'' + row.caseNo + '\');"><i class="glyphicon glyphicon-list-alt"></i>Detail</button>';
                    
                },
                "targets": 3
            }
        ]
    });
    
	$("#packingCase-table_filter").css("display", "none");
	
	Search();
});

//移除table style屬性
function removeStyle() {
	var style = $('#packingCase-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#packingCase-table').removeAttr("style");
}

function Search() {
	var data = "{ 'PalletsNo': '" + $('#PalletsNo').val() + "', 'PackingTime': '" + $('#PackingTime').val() + "' }";
	packingCaseTable.search(data).draw();
}

function objectifyForm(formArray) {

    var returnArray = {};
    for (var i = 0; i < formArray.length; i++) {
        returnArray[formArray[i]['name']] = formArray[i]['value'];
        console.log(formArray[i]['name']);
    }
    return returnArray;
}

//箱號細項
function btnDetail(palletsNo, packingTime, caseNo)
{
	if (palletsNo == "null")
		palletsNo = "";
	if (packingTime == "null")
		packingTime = "";
	if (caseNo == "null")
		caseNo = "";
	
	BootstrapDialog.show({
		title: '箱號-' + caseNo,
		message: '<div class="input-group">' +
		'<span class="input-group-addon">箱號</span>' +
		'<input type="text" class="form-control" value="' + caseNo + '" disabled >' +
		'</div>' +
		'</br>' +
		'箱號細項' +
		'<table id="bag-table" class="table  table-bordered table-hover">' +
		'<thead>' +
		'<tr>' +
		'<th>項次</th>' +
		'<th>袋號</th>' +
		'</tr>' +
		'</thead>' +
		'<tbody>' +
		'</tbody>' +
		'</table>',
		onshown: function (dialog) {
			createProgram();
			$('#bag-table_paginate').css("display", "none");

			var data = "{ 'PalletsNo': '" + palletsNo + "', 'PackingTime': '" + packingTime + "', 'CaseNo': '" + caseNo + "' }";
			bagTable.search(data).draw();
		}
	});

}

function createProgram() {
	

	bagTable = $('#bag-table').DataTable({
		processing: true,
		serverSide: true,
		"searching": true,
		"ordering": false,
		"info": false,
		"lengthChange": false,
		"ajax": {
			"url": "../api/PackingBagApi",
			"type": 'GET'
		},
		"columns": [
			{ "data": "item" },
			{ "data": "bagNo" }
		]
	});

	$("#bag-table_filter").css("display", "none");
}
