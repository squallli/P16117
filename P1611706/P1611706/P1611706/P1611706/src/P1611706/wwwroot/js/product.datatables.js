
var ready;
var time = setInterval(removeStyle, 100);
var productTable;
$(document).ready(function () {
    productTable = $('#product-table').DataTable({
        processing: true,
        serverSide: true,
        "searching": true,
        "ordering": false,
        "info": false,
        "lengthChange": false,
        "ajax": {
            "url": "api/ProductApi",
            "type": 'GET'
        },
        "columns": [
           { "data": "productNo" },
           { "data": "productName" },
           { "data": "spec" },
           { "data": "unit" },
		   { "data": "capcity" },
		   { "data": "effectiveMonth" },
		   { "data": "effectiveDay" },
		   { "data": "barcode" }
        ],
        "columnDefs": [
            {

                "render": function (data, type, row) {
					return '<button class="btn btn-primary" type="button" onclick="btnEdit(\'' + row.productNo + '\', \'' + row.productName + '\', \'' + row.spec + '\', \'' + row.unit + '\', \'' + row.capcity + '\', \'' + row.effectiveMonth + '\', \'' + row.effectiveDay + '\', \'' + row.barcode + '\');"><i class="glyphicon glyphicon-pencil"></i>Edit</button>' +
						'<button class="btn btn-danger" type="button" name="remove_levels" value="delete" onclick="btnDelete(\'' + row.productNo + '\', \'' + row.productName + '\', \'' + row.spec + '\', \'' + row.unit + '\', \'' + row.capcity + '\', \'' + row.effectiveMonth + '\', \'' + row.effectiveDay + '\', \'' + row.barcode + '\');"><span class="fa fa-times"></span>Delete</button>';

                },
                "targets": 8
            }
        ]
    });
    
    $("#product-table_filter").css("display", "none");

});


//移除table style屬性
function removeStyle() {
	var style = $('#product-table').attr('style');

	if (ready == true)
		clearInterval(time);

	if (typeof style !== typeof undefined && style !== false) {
		ready = true;
	}

	$('#product-table').removeAttr("style");
}

function searchProduct() {

    //var data = JSON.stringify(objectifyForm($(searchForm).serializeArray()));
    var data = "{ 'ProductNo': '" + $('#ProductNo').val() + "', 'ProductName': '" + $('#ProductName').val() + "' }";
    productTable.search(data).draw();
    
}

function btnEdit(productNo, productName, spec, unit, capcity, effectiveMonth, effectiveDay, barcode)
{
    if (productName == "null")
        productName = "";
    if (spec == "null")
        spec = "";
    if (unit == "null")
        unit = "";
    if (capcity == "null")
		capcity = "";
	if (effectiveMonth == "null")
		effectiveMonth = "";
	if (effectiveDay == "null")
		effectiveDay = "";
	if (barcode == "null")
		barcode = "";
    BootstrapDialog.show({
        title: '編輯-' + productNo,
        message:'<div class="input-group">' +
                    '<span class="input-group-addon">產品編號</span>' +
                    '<input type="text" class="form-control" value="' + productNo + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">產品名稱</span>' +
                    '<input type="text" class="form-control" value="' + productName + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">規格</span>' +
                    '<input type="text" class="form-control" value="' + spec + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">單位</span>' +
                    '<input type="text" class="form-control" value="' + unit + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">容量</span>' +
                    '<input type="text" id="txtCapacity" class="form-control" value="' + capcity + '" onkeyup="return ValidateNumber($(this),value)" >' +
		'</div>' +
		'</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">效期(月)</span>' +
		'<input type="text" id="txtEffectiveMonth" class="form-control" value="' + effectiveMonth + '"  onkeyup="return ValidateNumber($(this),value)" >' +
		'</div>' +
		'</br>' +
		'<div class="input-group">' +
		'<span class="input-group-addon">效期(日)</span>' +
		'<input type="text" id="txtEffectiveDay" class="form-control" value="' + effectiveDay + '"  onkeyup="return ValidateNumber($(this),value)" >' +
		'</div>' +
		'</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">條碼</span>' +
					'<input type="text" id="txtBarcode" class="form-control" value="' + barcode + '" disabled >' +
                '</div>',
        buttons: [{
            label: '確認',
            cssClass: 'btn-primary',
            action: function (dialog) {
                
				var productInfoes = { productNo: productNo, productName: productName, spec: spec, unit: unit, capcity: $('#txtCapacity').val(), effectiveMonth: $('#txtEffectiveMonth').val(), effectiveDay: $('#txtEffectiveDay').val(), barcode: barcode };
                $.ajax({
					type: 'Put',
					url: 'api/ProductApi',
                    data: JSON.stringify(productInfoes),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'text',
					success: function (data) {
						if (data == "success") {
							//alert("編輯成功!!");
							dialog.close();
							productTable.draw(false);

						}
						else {
							alert("刪除失敗!!" + data);
						}
                    },
                    error: function (data) {
                        alert("編輯失敗!!" + data);
                    }
                });
            }
        }, {
            label: '取消',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });

}

function btnDelete(productNo, productName, spec, unit, capcity, effectiveMonth, effectiveDay, barcode) {
	if (productName == "null")
		productName = "";
	if (spec == "null")
		spec = "";
	if (unit == "null")
		unit = "";
	if (capcity == "null")
		capcity = "";
	if (effectiveMonth == "null")
		effectiveMonth = "";
	if (effectiveDay == "null")
		effectiveDay = "";
	if (barcode == "null")
		barcode = "";

    BootstrapDialog.show({
        title: '刪除-' + productNo,
        message: '<div class="input-group">' +
                    '<span class="input-group-addon">產品編號</span>' +
                    '<input type="text" class="form-control" value="' + productNo + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">產品名稱</span>' +
                    '<input type="text" class="form-control" value="' + productName + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">規格</span>' +
                    '<input type="text" class="form-control" value="' + spec + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">單位</span>' +
                    '<input type="text" class="form-control" value="' + unit + '" disabled >' +
                '</div>' +
                '</br>' +
                '<div class="input-group">' +
                    '<span class="input-group-addon">容量</span>' +
                    '<input type="text" class="form-control" value="' + capcity + '" disabled >' +
                '</div>' +
				'</br>' +
				'<div class="input-group">' +
                    '<span class="input-group-addon">效期(月)</span>' +
		'<input type="text" class="form-control" value="' + effectiveMonth + '" disabled >' +
                '</div>' +
		'</br>' +
		'<div class="input-group">' +
		'<span class="input-group-addon">效期(日)</span>' +
		'<input type="text" class="form-control" value="' + effectiveDay + '" disabled >' +
		'</div>' +
		'</br>' +
				'<div class="input-group">' +
					'<span class="input-group-addon">條碼</span>' +
					'<input type="text" class="form-control" value="' + barcode + '" disabled >' +
				'</div>' +
				'</br>' +
                '<h3>確認刪除此項目?</h3>',
        type: BootstrapDialog.TYPE_DANGER,
        buttons: [{
            label: '確認',
            cssClass: 'btn-danger',
            action: function (dialog) {
                var dataJSON = productNo;
                $.ajax({
                    type: 'DELETE',
                    url: 'api/ProductApi',
                    data: JSON.stringify(dataJSON),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'text',
					success: function (data) {
						if (data == "success") {
							//alert("刪除成功!!");
							dialog.close();
							productTable.draw(false);

						}
						else {
							alert("刪除失敗!!" + data);
						}
                    },
                    error: function (data) {
                        alert("刪除失敗!!" + data);
                    }
                });
                    
            }
        }, {
            label: '取消',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });
}

function ValidateNumber(e, pnumber) {
    if (pnumber == "")
        return false;
    if (!/^\d+$/.test(pnumber)) {
        alert("只能輸入數字!!");
        $(e).val(/^\d+/.exec($(e).val()));
    }
    return false;
}

