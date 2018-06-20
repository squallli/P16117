
var ready;
var time = setInterval(removeStyle, 100);
var vendorTable;
var vendorProductTable;
var productVendorTable;

$(document).ready(function () {
    vendorTable = $('#vendor-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
            "url": "api/VendorApi",
            "type": 'GET'
        },
        "columns": [
           { "data": "vendorNo" }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
					return '<button class="btn btn-primary" type="button" onclick="btnDetail(\'' + row.vendorNo + '\');"><i class="glyphicon glyphicon-pencil"></i>Detail</button>';

                },
                "targets": 1
            },
            {
                "render": function (data, type, row) {
                    return '<button class="btn btn-danger" type="button" onclick="btnDelete(\'' + row.vendorNo + '\');"><i class="fa fa-times"></i>Delete</button>';

                },
                "targets": 2
            }
        ]
    });
    $("#vendor-table_filter").css("display", "none");
});

//移除table style屬性
function removeStyle() {
	var style = $('#vendor-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#vendor-table').removeAttr("style");
}


function searchVendor() {

    var data = "{ 'VendorNo': '" + $('#VendorNo').val() + "' }";
    vendorTable.search(data).draw();

}
function AddVendor() {
    BootstrapDialog.show({
        id: 'dialog-detail',
        title: '新增代工廠',
        type: BootstrapDialog.TYPE_INFO,
        message: '<div>' +
        '</br>' +
        '<div class="row">' +
        '<div class="col-md-4">' +
        '<div class="input-group">' +
        '<span class="input-group-addon">代工廠編號</span>' +
        '<input type="text" id="txtAddVendorNo" class="form-control" value=""  >' +
        '</div>' +
        '</div>' +
        '<div class="col-md-8 text-right">' +
        '<button id="btnProductVendorConfirm" class="edit btn btn-primary" onclick="AddVendorProduct()"><i class="fa fa-fw fa-save"></i>確認</button>' +
        '</div>' +
        '</div>' +
        '</br>' +
        '<table id="productvendor-table" class="table table-bordered table-hover" style="width:100%">' +
        '<thead>' +
        '<tr>' +
        '<th ></th>' +
        '<th > 產品編號</th>' +
        '<th >產品名稱</th>' +
        '<th > 規格</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>' +
        '</tbody>' +
        '</table>' +
        '</div>',
        onshown: function (dialog) {
            CreateProductVendorTable()
        },
        buttons: [{
            label: '返回',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });
}
function btnDetail(vendorNo) {
	BootstrapDialog.show({
		id: 'dialog-detail',
		title: '代工廠-' + vendorNo,
		message: '<div id="exTab2" >' +
		'<ul class="nav nav-tabs" >' +
		'<li class="active">' +
		'<a href="#1" data-toggle="tab">查詢</a>' +
		'</li>' +
        '<li>' +
        '<a href="#2" data-toggle="tab">設定</a>' +
		'</li>' +
		'</ul>' +
		'<div class="tab-content ">' +


		//tab1
		'<div class="tab-pane active" id="1">' +
		'</br>' +
		'<div class="row">' +
		'<div class="col-md-4">' +
		'<div class="input-group">' +
		'<span class="input-group-addon">代工廠編號</span>' +
		'<input type="text" id="txtTab1VendorNo" class="form-control" disabled value="' + vendorNo + '"  >' +
		'</div>' +
		'</div>' +
		'</div>' +
		'</br>' +
		'<div class="row">' +
		'<div class="col-md-4">' +
		'<div class="input-group">' +
		'<span class="input-group-addon">產品編號</span>' +
		'<div class="form-group has-feedback has-clear">' +
		'<input type="text" class="form-control" id="txtTab1ProductNo" name="txtTab1ProductNo">' +
		'<span class="form-control-clear glyphicon glyphicon-remove form-control-feedback hidden"></span>' +
		'</div>' +
		'</div>' +
		'</div>' +
		'<div class="col-md-4">' +
		'<div class="input-group">' +
		'<span class="input-group-addon">產品名稱</span>' +
		'<div class="form-group has-feedback has-clear">' +
		'<input type="text" class="form-control" id="txtTab1ProductName" name="txtTab1ProductName">' +
		'<span class="form-control-clear glyphicon glyphicon-remove form-control-feedback hidden"></span>' +
		'</div>' +
		'</div>' +
		'</div>' +
		'<div class="col-md-4">' +
		'<button id="btnVendorProductSearch" class="edit btn btn-success" onclick="searchVendorProduct()"><i class="fa fa-fw fa-search"></i>搜尋</button>' +
		'</div>' +
		'</div>' +
		'<br />' +
		'<table id="vendorproduct-table" class="table  table-bordered table-hover">' +
		'<thead>' +
		'<tr>' +
		'<th style="width:35%" > 產品編號</th>' +
		'<th style="width:35%">產品名稱</th>' +
		'<th style="width:30%" > 規格</th>' +
		'</tr>' +
		'</thead>' +
		'<tbody>' +
		'</tbody>' +
		'</table>' +
		'</div>' +


		//tab2
		'<div class="tab-pane" id="2">' +
		'</br>' +
		'<div class="row">' +
		'<div class="col-md-4">' +
		'<div class="input-group">' +
		'<span class="input-group-addon">代工廠編號</span>' +
		'<input type="text" id="txtTab2VendorNo" class="form-control" disabled value="' + vendorNo + '"  >' +
		'</div>' +
		'</div>' +
		'<div class="col-md-8 text-right">' +
		'<button id="btnProductVendorConfirm" class="edit btn btn-primary" onclick="ConfirmVendorProduct()"><i class="fa fa-fw fa-save"></i>確認</button>' +
		'</div>' +
		'</div>' +
		'</br>' +
		'<table id="productvendor-table" class="table table-bordered table-hover" style="width:100%">' +
		'<thead>' +
		'<tr>' +
		'<th ></th>' +
		'<th > 產品編號</th>' +
		'<th >產品名稱</th>' +
		'<th > 規格</th>' +
		'</tr>' +
		'</thead>' +
		'<tbody>' +
		'</tbody>' +
		'</table>' +
		'</div>' +
		

		'</div>' +
		'</div>',
		onshown: function (dialog) {
			CreateVendorProductTable();
			CreateProductVendorTable();

			//$("#vendor-table_filter").css("display", "none");
			//$("#productvendor-table_filter").css("display", "none");
			$('#productvendor-table_paginate').css("display", "none");

			searchVendorProduct();
			searchProductVendor();
		},
		buttons: [{
			label: '返回',
			action: function (dialog) {
				dialog.close();
			}
		}]
	});

}


//代工廠產品載入
function CreateVendorProductTable() {

	vendorProductTable = $('#vendorproduct-table').DataTable({
		processing: true,
		serverSide: true,
		"searching": true,
		"ordering": false,
		"info": false,
		"lengthChange": false,
		"ajax": {
			"url": "api/VendorProductApi",
			"type": 'GET'
		},
		"columns": [
			{ "data": "productNo" },
			{ "data": "productName" },
			{ "data": "spec" }
		]
	});

	$("#vendorproduct-table_filter").css("display", "none");
}

//產品載入
function CreateProductVendorTable() {

	productVendorTable = $('#productvendor-table').DataTable({
		processing: true,
		serverSide: true,
		"searching": true,
		"ordering": false,
		"info": false,
		"lengthChange": false,
		"ajax": {
			"url": "api/ProductVendorApi",
			"type": 'GET'
		},
		"columns": [
			{ "data": "" },
			{ "data": "productNo" },
			{ "data": "productName" },
			{ "data": "spec" }
		],
		"columnDefs": [
			{

				"render": function (data, type, row) {
					var thHTML;

					if (!row.manufacture) {
						thHTML = '<th class="text-center"><div class="btn-group" data-toggle="buttons"><label id="' + row.productNo + '" class="btn btn-primary " >' + /*onclick="changeShowTips(this)"*/
							'<input type="checkbox" autocomplete="off"   >' +
							'<span class="glyphicon glyphicon-ok"></span>' +
							'</label></div></th>';
						//thHTML = '<th class="text-center"><input type="checkbox" id="' + row.power + '" onclick="changeShowTips(this)" ></th>';
					}
					else {
						thHTML = '<th class="text-center"><div class="btn-group" data-toggle="buttons"><label id="' + row.productNo + '" class="btn btn-primary active" >' + /*onclick="changeShowTips(this)"*/
							'<input type="checkbox" autocomplete="off"   >' +
							'<span class="glyphicon glyphicon-ok"></span>' +
							'</label></div></th>';
						//thHTML = '<th class="text-center"><input type="checkbox" id="' + row.power + '" onclick="changeShowTips(this)" checked=true ></th>';
					}
					return thHTML;

				},
				"targets": 0
			}
		]
	});

	$("#productvendor-table_filter").css("display", "none");
}

function searchVendorProduct() {
	
	var data = "{ 'VendorNo': '" + $('#txtTab1VendorNo').val() + "', 'ProductNo': '" + $('#txtTab1ProductNo').val() + "', 'ProductName': '" + $('#txtTab1ProductName').val() + "' }";
	vendorProductTable.search(data).draw();

}

function searchProductVendor() {

	var data = "{ 'VendorNo': '" + $('#txtTab2VendorNo').val() + "' }";
	productVendorTable.search(data).draw();

}


function ConfirmVendorProduct() {
	var arrProductVendor = [];

	$('#productvendor-table tbody').find('tr').each(function () {
		var VendorNo = $('#txtTab2VendorNo').val();
		var ProductNo = $(this).find('td').eq(0).find('div label').attr("id");
		var ProductName = $(this).find('td').eq(2).html();
		var Spec = $(this).find('td').eq(3).html();



		if ($(this).find('td').eq(0).find('div label').hasClass("active"))
			arrProductVendor.push({ VendorNo: VendorNo, ProductNo: ProductNo, ProductName: ProductName, Spec: Spec, Manufacture: "1"});
		else
			arrProductVendor.push({ VendorNo: VendorNo, ProductNo: ProductNo, ProductName: ProductName, Spec: Spec, Manufacture: "0" });
	});

	var data = { productVendor: arrProductVendor };

	$.ajax({
		type: 'Post',
		url: 'api/ProductVendorApi',
		data: JSON.stringify(arrProductVendor),
		contentType: 'application/json; charset=utf-8',
		dataType: 'text',
		success: function (data) {
			if (data == "success") {
				//alert("設定成功!!");
				$("#dialog-detail .close").click()
				productVendorTable.draw(false);

			}
			else {
				alert("設定失敗!!" + data);
				productVendorTable.draw(false);
			}
		},
		error: function (data) {
			alert("設定失敗!!" + data);
			productVendorTable.draw(false);
		}
	});
}
function AddVendorProduct() {
    if ($('#txtAddVendorNo').val() == "") {
        alert("請輸入代工廠代號");
        return;
    }
    var arrProductVendor = [];

    $('#productvendor-table tbody').find('tr').each(function () {
        var VendorNo = $('#txtAddVendorNo').val();
        var ProductNo = $(this).find('td').eq(0).find('div label').attr("id");
        var ProductName = $(this).find('td').eq(2).html();
        var Spec = $(this).find('td').eq(3).html();



        if ($(this).find('td').eq(0).find('div label').hasClass("active"))
            arrProductVendor.push({ VendorNo: VendorNo, ProductNo: ProductNo, ProductName: ProductName, Spec: Spec, Manufacture: "1" });
        else
            arrProductVendor.push({ VendorNo: VendorNo, ProductNo: ProductNo, ProductName: ProductName, Spec: Spec, Manufacture: "0" });
    });

    var data = { productVendor: arrProductVendor };

    $.ajax({
        type: 'Post',
        url: 'api/VendorApi',
        data: JSON.stringify(arrProductVendor),
        contentType: 'application/json; charset=utf-8',
        dataType: 'text',
        success: function (data) {
            if (data == "success") {
                alert("新增成功!!");
                $("#dialog-detail .close").click()
                vendorTable.draw(false);

            }
            else {
                alert("新增失敗!!" + data);
                vendorTable.draw(false);
            }
        },
        error: function (data) {
            alert("新增失敗!!" + data);
            vendorTable.draw(false);
        }
    });
}
function btnDelete(vendorNo) {
    if (confirm("是否刪除代工廠-" + vendorNo + "?")) {
        $.ajax({
            type: 'DELETE',
            url: 'api/VendorApi',
            data: { VendorNo: vendorNo },
            dataType: 'text',
            success: function (data) {
                if (data == "success") {
                    alert("刪除成功!!");
                    vendorTable.draw(false);

                }
                else {
                    alert("刪除失敗!!" + data);
                    vendorTable.draw(false);
                }
            },
            error: function (data) {
                alert("刪除失敗!!" + data);
                vendorTable.draw(false);
            }
        });
    }
}